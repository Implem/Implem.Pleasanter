﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Initializers;
using Implem.Pleasanter.Libraries.Migrators;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
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
            Context context = new Context(tenantId: 0)
            {
                Controller = "Global.asax",
                Action = "Application_Start",
            };
            var log = new SysLogModel(context: context);
            ExtensionInitializer.Initialize(context: context);
            UsersInitializer.Initialize(context: context);
            StatusesMigrator.Migrate(context: context);
            StatusesInitializer.Initialize(context: context);
            NotificationInitializer.Initialize();
            SetConfigrations();
            SiteInfo.Reflesh(context: context);
            log.Finish(context: context);
        }

        private void Initialize()
        {
            Initializer.Initialize(
                path: Server.MapPath("./"),
                assemblyVersion: Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        private void SetConfigrations()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(ApiRouteConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            Saml.RegisterSamlConfiguration();
        }

        protected void Application_End()
        {
            var context = new Context(tenantId: 0)
            {
                Controller = "Global.asax",
                Action = "Application_End",
            };
            var log = new SysLogModel(context: context);
            log.Finish(context: context);
        }

        protected void Application_Error()
        {
            var context = new Context();
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
            var context = new Context()
            {
                Controller = "Global.asax",
                Action = "Session_Start",
            };
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

        private static bool WindowsAuthenticated(Context context)
        {
            return Authentications.Windows()
                && !context.LoginId.IsNullOrEmpty()
                && (!Parameters.Authentication.RejectUnregisteredUser
                || context.Authenticated);
        }
    }
}