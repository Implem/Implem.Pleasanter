using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Initializers;
using Implem.Pleasanter.Libraries.Migrators;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.NetCore.Filters;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Implem.Pleasanter.NetCore
{
    public class Startup
    {
        IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Applications.StartTime = DateTime.Now;
            Applications.LastAccessTime = Applications.StartTime;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession();

            services.AddMvc(
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o => o.LoginPath = new PathString("/users/login"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCurrentRequestContext();

            ContextImplement.Init();
            Initializer.Initialize(path: env.ContentRootPath, assemblyVersion: System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            app.UseHttpsRedirection();
            app.UsePathBase(configuration["pathBase"]);
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();

            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            lifetime.ApplicationStopping.Register(OnShutdown);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/errors/internalservererror");
            }
            app.UseStatusCodePages(context =>
            {
                var statusCode = context.HttpContext.Response.StatusCode;
                if (statusCode == 400) context.HttpContext.Response.Redirect("/errors/badrequest");
                else if (statusCode == 404) context.HttpContext.Response.Redirect("/errors/notfound");
                else if (statusCode == 500) context.HttpContext.Response.Redirect("/errors/internalservererror");
                else context.HttpContext.Response.Redirect("/errors/internalservererror");
                return Task.CompletedTask;
            });

            app.Use(async (context, next) => await Invoke(context, next));
            app.UseSessionMiddleware();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}",
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
                routes.MapRoute(
                    name: "Others",
                    template: "{reference}/{id}/{controller}/{action}",
                    defaults: new
                    {
                        Action = "Index"
                    },
                    constraints: new
                    {
                        Reference = "[A-Za-z][A-Za-z0-9_]*",
                        Id = "[0-9]+",
                        Controller = "Binaries|OutgoingMails",
                        Action = "[A-Za-z][A-Za-z0-9_]*"
                    }
                );
                routes.MapRoute(
                    name: "Item",
                    template: "{controller}/{id}/{action}",
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
                routes.MapRoute(
                    name: "Binaries",
                    template: "binaries/{guid}/{action}",
                    defaults: new
                    {
                        Controller = "Binaries"
                    },
                    constraints: new
                    {
                        Guid = "[A-Z0-9]+",
                        Action = "[A-Za-z][A-Za-z0-9_]*"
                    }
                );

            });
        }

        private static ContextImplement ApplicationStartContext()
        {
            return new ContextImplement(tenantId: 0)
            {
                Controller = "Global.asax",
                Action = "Application_Start",
                Id = 0
            };
        }

        private void SetConfigrations(Context context)
        {
            Saml.RegisterSamlConfiguration(context: context);
        }

        private static bool isFirst = true;
        public async Task Invoke(HttpContext httpContext, Func<Task> next)
        {
            if (isFirst)
            {
                InitializeLog();
                isFirst = false;
            }
            try
            {
                await next.Invoke();
            }
            catch (Exception error)
            {
                try
                {
                    var context = new ContextImplement();
                    var log = new SysLogModel(context: context);
                    log.SysLogType = SysLogModel.SysLogTypes.Execption;
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
            var context = new ContextImplement();
            var log = new SysLogModel(context: context);
            Performances.PerformanceCollection.Save(Directories.Logs());
            log.Finish(context: context);
        }

        private void InitializeLog()
        {
            ContextImplement context = ApplicationStartContext();
            var log = new SysLogModel(context: context);
            UsersInitializer.Initialize(context: context);
            ItemsInitializer.Initialize(context: context);
            StatusesMigrator.Migrate(context: context);
            SiteSettingsMigrator.Migrate(context: context);
            StatusesInitializer.Initialize(context: context);
            SetConfigrations(context: context);
            SiteInfo.Reflesh(context: context);
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
            if (!httpContext.Session.Keys.Any(key => key == enabled)) {
                AspNetCoreCurrentRequestContext.AspNetCoreHttpContext.Current.Session.Set("SessionGuid", System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(Strings.NewGuid())));
                httpContext.Session.Set(enabled, new byte[]{ 1 });
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

        private static ContextImplement SessionStartContext()
        {
            return new ContextImplement()
            {
                Controller = "Global.asax",
                Action = "Session_Start",
                Id = 0
            };
        }

        private static bool WindowsAuthenticated(ContextImplement context)
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
}
