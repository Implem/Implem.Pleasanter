using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Initializers;
using Implem.Pleasanter.Libraries.Migrators;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
namespace Implem.Pleasanter
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Initialize();
            ContextImplement context = ApplicationStartContext();
            var log = new SysLogModel(context: context);
            UsersInitializer.Initialize(context: context);
            StatusesMigrator.Migrate(context: context);
            StatusesInitializer.Initialize(context: context);
            NotificationInitializer.Initialize();
            SetConfigrations(context: context);
            SiteInfo.Reflesh(context: context);
            log.Finish(context: context);
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

        private void Initialize()
        {
            ContextImplement.Init();
            Initializer.Initialize(
                path: Server.MapPath("./"),
                assemblyVersion: Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        private void SetConfigrations(Context context)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(ApiRouteConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            Saml.RegisterSamlConfiguration(context: context);
        }

        protected void Application_End()
        {
            var context = new ContextImplement();
            var log = new SysLogModel(context: context);
            log.Finish(context: context);
        }

        protected void Application_Error()
        {
            var context = new ContextImplement();
            if (Server != null)
            {
                var error = Server.GetLastError();
                if (error != null)
                {
                    try
                    {
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
                }
            }
        }

        protected void Session_Start()
        {
            Session["Enabled"] = true;
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
            switch (Request.AppRelativeCurrentExecutionFilePath.ToLower())
            {
                case "~/backgroundtasks/do":
                case "~/reminderschedules/remind":
                    break;
                default:
                    new SysLogModel(context: context).Finish(context: context);
                    break;
            }
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
}