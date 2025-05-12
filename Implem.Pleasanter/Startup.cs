using Azure.Identity;
using Azure.Storage.Blobs;
using HealthChecks.UI.Client;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.BackgroundServices;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Initializers;
using Implem.Pleasanter.Libraries.Migrators;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.TrialLicenses;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Implem.Pleasanter.NetCore
{
    public class Startup
    {
        IConfiguration configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Applications.StartTime = DateTime.Now;
            Applications.LastAccessTime = Applications.StartTime;
            Context.Init();
            var exceptions = Initializer.Initialize(
                path: env.ContentRootPath,
                assemblyVersion: Assembly.GetExecutingAssembly().GetName().Version.ToString());
            var context = InitializeContext();
            // テンポラリ削除などで発生したエラーをSysLogsに記録する
            if (exceptions.Any())
            {
                exceptions.ForEach(e =>
                    new SysLogModel(
                        context: context,
                        e: e));
            }
            TrialLicenseUtilities.Initialize();
            LogManager.Setup()
                .LoadConfigurationFromAppSettings(environment: env.EnvironmentName)
                .SetupSerialization(ss => ss.RegisterObjectTransformation<SysLogModel>(s => SysLogModel.ToLogModel(context: context, sysLogModel: s)));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            if (Parameters.Security.AccessControlAllowOrigin?.Any() == true)
            {
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        builder => builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins(Parameters.Security.AccessControlAllowOrigin.ToArray()));
                });
            }
            services.AddControllersWithViews();
            services.AddDistributedMemoryCache();
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(Parameters.Session.RetentionPeriod);
            });
            var mvcBuilder = services.AddMvc(
                options =>
                {
                    options.Filters.Add(new HandleErrorExAttribute());
                    options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
                    options.Filters.Add(new CheckContextAttributes());
                    if (Parameters.Service.RequireHttps)
                    {
                        options.Filters.Add(new Microsoft.AspNetCore.Mvc.RequireHttpsAttribute());
                    }
                });
            if (Authentications.SAML())
            {
                services
                    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(o =>
                    {
                        o.LoginPath = new PathString("/users/login");
                        o.ExpireTimeSpan = TimeSpan.FromMinutes(Parameters.Session.RetentionPeriod);
                    })
                    .AddSaml2(options =>
                    {
                        Saml.SetSPOptions(options: options);
                        Saml.RegisterSamlConfiguration(
                            context: context,
                            options: options);
                    });
            }
            else
            {
                services
                    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(o =>
                    {
                        o.LoginPath = new PathString("/users/login");
                        o.ExpireTimeSpan = TimeSpan.FromMinutes(Parameters.Session.RetentionPeriod);
                    });
            }
            services.AddSingleton<ITicketStore, AuthenticationTicketStore>();
            services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<ITicketStore>((options, store) => options.SessionStore = store);
            if (Parameters.Security.SecureCookies)
            {
                services.Configure<CookiePolicyOptions>(options =>
                {
                    options.Secure = CookieSecurePolicy.Always;
                });
            }
            foreach (var path in GetExtendedLibraryPaths())
            {
                if (Directory.Exists(path))
                {
                    foreach (var assembly in Directory.GetFiles(path, "*.dll").Select(dll => Assembly.LoadFrom(dll)).ToArray())
                    {
                        mvcBuilder.AddApplicationPart(assembly);
                        // DLL内にImplem.Pleasanter.NetCore.ExtendedLibrary.ExtendedLibraryクラスInitialize()staticメソッドがあった場合は呼び出す
                        // 拡張DLL内でbackgrondのworkerスレッドを起動したい場合に使用
                        assembly.GetType("Implem.Pleasanter.NetCore.ExtendedLibrary.ExtendedLibrary")?
                            .GetMethod("Initialize")?
                            .Invoke(null, null);
                    }
                }
            }
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = int.MaxValue;
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.MaxRequestBodySize = Parameters.Service.MaxRequestBodySize;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.Limits.MaxRequestBodySize = Parameters.Service.MaxRequestBodySize;
            })
            .Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"));
            if (Parameters.Security.HealthCheck.Enabled)
            {
                services
                    .AddHealthChecks()
                    .AddDatabaseHealthCheck(
                        enableDatabaseCheck: Parameters.Security.HealthCheck.EnableDatabaseCheck,
                        dbms: Parameters.Rds.Dbms,
                        conStr: Parameters.Rds.UserConnectionString,
                        healthQuery: Parameters.Security.HealthCheck.HealthQuery ?? "select 1;");
            }
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            services.Configure<HostOptions>(options =>
            {
                // BackgroundServiceで例外発生してもWebアプリケーション自体は終了させない設定
                options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });
            services.AddHostedService<CustomQuartzHostedService>();
            new TimerBackground().Init();
            BackgroundServerScriptUtilities.InitSchedule();
            var blobContainerUri = Parameters.Security.AspNetCoreDataProtection?.BlobContainerUri;
            var keyIdentifier = Parameters.Security.AspNetCoreDataProtection?.KeyIdentifier;
            if (!blobContainerUri.IsNullOrEmpty()
                && !keyIdentifier.IsNullOrEmpty())
            {
                var blobContainer = new BlobContainerClient(new Uri(blobContainerUri), new DefaultAzureCredential());
                blobContainer.CreateIfNotExists();
                var blobClient = blobContainer.GetBlobClient(Parameters.Security.AspNetCoreDataProtection?.KeyFileName ?? "keys.xml");
                services
                    .AddDataProtection()
                    .PersistKeysToAzureBlobStorage(blobClient)
                    .ProtectKeysWithAzureKeyVault(new Uri(keyIdentifier), new DefaultAzureCredential());
            }
            else
            {
                services
                    .AddOptions<KeyManagementOptions>()
                    .Configure<IServiceScopeFactory>((options, factory) =>
                    {
                        options.XmlRepository = new AspNetCoreKeyManagementXmlRepository();
                        options.XmlEncryptor = new AspNetCoreKeyManagementXmlEncryptor();
                    });
            }
            if (Parameters.Security.HttpStrictTransportSecurity?.Enabled == true)
            {
                services.AddHsts(options =>
                {
                    options.Preload = Parameters.Security.HttpStrictTransportSecurity.Preload;
                    options.IncludeSubDomains = Parameters.Security.HttpStrictTransportSecurity.IncludeSubDomains;
                    options.MaxAge = Parameters.Security.HttpStrictTransportSecurity.MaxAge;
                    if (Parameters.Security.HttpStrictTransportSecurity.ExcludeHosts != null)
                    {
                        foreach (var host in Parameters.Security.HttpStrictTransportSecurity.ExcludeHosts)
                        {
                            options.ExcludedHosts.Add(host);
                        }
                    }
                });
            }
            services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder => builder.NoCache());
                options.AddPolicy("imageCache", builder => builder.Expire(System.TimeSpan.FromSeconds(Parameters.OutputCache.OutputCacheControl.OutputCacheDuration)));
            });
        }

        // 拡張DLLの探索をExtendedLibrariesディレクトリ内の一段下のディレクトリも対象をする。
        private IEnumerable<string> GetExtendedLibraryPaths()
        {
            var list = new List<string>();
            var basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ExtendedLibraries");
            if (Directory.Exists(basePath))
            {
                list.Add(basePath);
                list.AddRange(Directory.GetDirectories(basePath));
            }
            return list;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            app.UseCurrentRequestContext();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/errors/internalservererror");
            }
            app.UseHsts();
            app.UseSecurityHeadersMiddleware();
            app.Use(async (context, next) => await Invoke(context, next));
            app.UseStatusCodePages(context =>
            {
                var statusCode = context.HttpContext.Response.StatusCode;
                if (statusCode == 400) context.HttpContext.Response.Redirect("/errors/badrequest");
                else if (statusCode == 404) context.HttpContext.Response.Redirect("/errors/notfound");
                else if (statusCode == 405) context.HttpContext.Response.Redirect("/errors/badrequest");
                else if (statusCode == 500) context.HttpContext.Response.Redirect("/errors/internalservererror");
                else if (statusCode == 401
                    && !context.HttpContext.User.Identity.IsAuthenticated
                    && context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.HttpContext.Response.StatusCode = 403;
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                    {
                        Message = Libraries.Responses.Displays.Unauthorized(context: new Context())
                    }));
                }
                else context.HttpContext.Response.Redirect("/errors/internalservererror");
                return Task.CompletedTask;
            });
            app.UsePathBase(configuration["pathBase"]);
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseCors();

            if (Parameters.OutputCache.OutputCacheControl != null && !Parameters.OutputCache.OutputCacheControl.NoOutputCache)
            {
                app.UseOutputCache();
            }
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSessionMiddleware();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                if (Parameters.Security.HealthCheck.Enabled)
                {
                    if (Parameters.Security.HealthCheck.EnableDetailedResponse)
                    {
                        endpoints
                            .MapHealthChecks("/healthz", new HealthCheckOptions()
                            {
                                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                            })
                            .RequireHost(Parameters.Security.HealthCheck.RequireHosts ?? Array.Empty<string>());
                    }
                    else
                    {
                        endpoints
                            .MapHealthChecks("/healthz")
                            .RequireHost(Parameters.Security.HealthCheck.RequireHosts ?? Array.Empty<string>());
                    }
                }
                endpoints.MapControllerRoute(
                    name: "Default",
                    pattern: "{controller}/{action}",
                    defaults: new
                    {
                        Controller = "Items",
                        Action = "Index"
                    },
                    constraints: new
                    {
                        Controller = "[A-Za-z][A-Za-z0-9_]*",
                        Action = "[A-Za-z][A-Za-z0-9_]*"
                    }
                );
                endpoints.MapControllerRoute(
                    name: "Others",
                    pattern: "{reference}/{id}/{controller}/{action}",
                    defaults: new
                    {
                        Action = "Index"
                    },
                    constraints: new
                    {
                        Reference = "[A-Za-z][A-Za-z0-9_]*",
                        Id = "[0-9]+",
                        Controller = "Binaries|PublishBinaries|OutgoingMails",
                        Action = "[A-Za-z][A-Za-z0-9_]*"
                    }
                );
                endpoints.MapControllerRoute(
                    name: "Item",
                    pattern: "{controller}/{id}/{action}",
                    defaults: new
                    {
                        Controller = "Items",
                        Action = "Edit"
                    },
                    constraints: new
                    {
                        Id = "[0-9]+",
                        Action = "[A-Za-z][A-Za-z0-9_]*"
                    }
                );
                endpoints.MapControllerRoute(
                    name: "Binaries",
                    pattern: "{controller}/{guid}/{action}",
                    defaults: new
                    {
                        Controller = "Binaries"
                    },
                    constraints: new
                    {
                        Guid = "[A-Za-z0-9]+",
                        Action = "[A-Za-z][A-Za-z0-9_]*"
                    }
                );
                endpoints.MapControllerRoute(
                    name: "BinariesUpload",
                    pattern: "binaries/upload",
                    defaults: new
                    {
                        Controller = "Binaries",
                        Action = "Upload"
                    },
                    constraints: new
                    {
                        Guid = "[A-Za-z0-9]+",
                        Action = "[A-Za-z][A-Za-z0-9_]*"
                    }
                );
            });
        }

        private static Context InitializeContext()
        {
            return new Context(
                tenantId: 0,
                request: false)
            {
                Controller = "Startup.cs",
                Action = "Initialize",
                Id = 0
            };
        }

        private static Context ApplicationStartContext()
        {
            return new Context(tenantId: 0)
            {
                Controller = "Startup.cs",
                Action = "Application_Start",
                Id = 0
            };
        }

        private static bool isFirst = true;
        public async Task Invoke(HttpContext httpContext, Func<Task> next)
        {
            if (isFirst)
            {
                isFirst = false;
                Initialize();
            }
            try
            {
                await next.Invoke();
            }
            catch (Exception error)
            {
                try
                {
                    var context = new Context();
                    var log = new SysLogModel(context: context);
                    log.SysLogType = SysLogModel.SysLogTypes.Exception;
                    log.ErrMessage = error.Message;
                    log.ErrStackTrace = error.StackTrace;
                    log.Finish(context: context);
                }
                catch
                {
                    throw;
                }
                throw;
            }
        }

        private void OnShutdown()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            log.Finish(context: context);
        }

        private void Initialize()
        {
            Context context = ApplicationStartContext();
            var log = new SysLogModel(
                context: context,
                method: null,
                message: Parameters.GetLicenseInfo().ToJson());
            TenantInitializer.Initialize();
            ExtensionInitializer.Initialize(context: context);
            UsersInitializer.Initialize(context: context);
            ItemsInitializer.Initialize(context: context);
            StatusesMigrator.Migrate(context: context);
            SiteSettingsMigrator.Migrate(context: context);
            StatusesInitializer.Initialize(context: context);
            NotificationInitializer.Initialize();
            SiteInfo.Refresh(context: context);
            log.Finish(context: context);
        }
    }

    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            const string enabled = "Enabled";
            if (!httpContext.Session.Keys.Any(key => key == enabled))
            {
                AspNetCoreCurrentRequestContext.AspNetCoreHttpContext.Current.Session.Set("SessionGuid", System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(Strings.NewGuid())));
                SetClientId();
                httpContext.Session.Set(enabled, new byte[] { 1 });
                var context = SessionStartContext();
                SessionUtilities.SetStartTime(context: context);
                if (WindowsAuthenticated(context))
                {
                    Ldap.UpdateOrInsert(
                        context: context,
                        loginId: context.LoginId);
                    context.Set();
                }
                if (context.Authenticated)
                {
                    StatusesInitializer.Initialize(context: context);
                }
                switch (httpContext.Request.Path.Value.ToLower())
                {
                    case "~/backgroundtasks/do":
                    case "~/reminderschedules/remind":
                        break;
                    default:
                        break;
                }
            }
            await _next.Invoke(httpContext);
        }

        private static Context SessionStartContext()
        {
            return new Context()
            {
                Controller = "Startup.cs",
                Action = "Session_Start",
                Id = 0
            };
        }

        private static void SetClientId()
        {
            if (Parameters.SysLog.ClientId &&
                AspNetCoreCurrentRequestContext.AspNetCoreHttpContext.Current?.Request.Cookies["Pleasanter_ClientId"] == null)
            {
                // Google ChromeにおけるCookie有効期限の上限400日を設定する
                AspNetCoreCurrentRequestContext.AspNetCoreHttpContext.Current?.Response.Cookies.Append(
                    "Pleasanter_ClientId",
                    Strings.NewGuid(),
                    new CookieOptions()
                    {
                        Expires = DateTime.UtcNow.AddDays(400),
                        Secure = true
                    });
            }
        }

        private static bool WindowsAuthenticated(Context context)
        {
            return Authentications.Windows(context: context)
                && !context.LoginId.IsNullOrEmpty()
                && (!Parameters.Authentication.RejectUnregisteredUser
                || context.Authenticated);
        }
    }

    public static class SessionMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SessionMiddleware>();
        }
    }

    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public Task Invoke(HttpContext context)
        {
            context.Response.Headers.Append("X-Frame-Options", new StringValues("SAMEORIGIN"));
            context.Response.Headers.Append("X-Xss-Protection", new StringValues("1; mode=block"));
            context.Response.Headers.Append("X-Content-Type-Options", new StringValues("nosniff"));
            if (Parameters.Security.SecureCacheControl != null)
            {
                if (Parameters.Security.SecureCacheControl.NoCache
                    || Parameters.Security.SecureCacheControl.NoStore
                    || Parameters.Security.SecureCacheControl.Private
                    || Parameters.Security.SecureCacheControl.MustRevalidate)
                {
                    context.Response.GetTypedHeaders().CacheControl =
                        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                        {
                            NoCache = Parameters.Security.SecureCacheControl.NoCache,
                            NoStore = Parameters.Security.SecureCacheControl.NoStore,
                            Private = Parameters.Security.SecureCacheControl.Private,
                            MustRevalidate = Parameters.Security.SecureCacheControl.MustRevalidate
                        };
                }
                if (Parameters.Security.SecureCacheControl.PragmaNoCache)
                {
                    context.Response.Headers.Append("Pragma", new StringValues("no-cache"));
                }
            }
            return _next(context);
        }
    }

    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeadersMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }

    public static class HealthCheckMiddlewareExtensions
    {
        public static IHealthChecksBuilder AddDatabaseHealthCheck(
            this IHealthChecksBuilder services,
            bool enableDatabaseCheck,
            string dbms,
            string conStr,
            string healthQuery)
        {
            if (!enableDatabaseCheck) { return services; }
            switch (dbms)
            {
                case "SQLServer":
                    return services.AddSqlServer(
                        connectionString: conStr,
                        healthQuery: healthQuery);
                case "PostgreSQL":
                    return services.AddNpgSql(
                        connectionString: conStr,
                        healthQuery: healthQuery);
                case "MySQL":
                    return services.AddMySql(
                        connectionString: conStr,
                        healthQuery: healthQuery);
                default:
                    return services;
            }
        }
    }
}
