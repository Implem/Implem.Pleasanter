using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Initializers;
using Implem.Pleasanter.Libraries.Migrators;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Web.Optimization;
using System.Web.Routing;
namespace Implem.Pleasanter
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var context = new Context()
            {
                Controller = "Global.asax",
                Action = "Application_Start",
                Id = 0
            };
            Application["StartTime"] = DateTime.Now;
            Application["LastAccessTime"] = Application["StartTime"];
            Initialize();
            var log = new SysLogModel(context: context);
            UsersInitializer.Initialize(context: context);
            ItemsInitializer.Initialize(context: context);
            StatusesMigrator.Migrate(context: context);
            SiteSettingsMigrator.Migrate(context: context);
            StatusesInitializer.Initialize(context: context);
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
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

        protected void Application_End()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            Performances.PerformanceCollection.Save(Directories.Logs());
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
            var context = new Context()
            {
                Controller = "Global.asax",
                Action = "Session_Start",
                Id = 0
            };
            Session["StartTime"] = DateTime.Now;
            Session["LastAccessTime"] = Session["StartTime"];
            Session["SessionGuid"] = Strings.NewGuid();
            if (context.Authenticated)
            {
                if (Authentications.Windows())
                {
                    Ldap.UpdateOrInsert(
                        context: context,
                        loginId: context.LoginId);
                }
                var userModel = GetUser(context: context);
                userModel.SetContext(context: context);
                StatusesInitializer.Initialize(context: context);
                if (userModel.AccessStatus == Databases.AccessStatuses.Selected &&
                    !userModel.Disabled)
                {
                    userModel.SetSession();
                }
                else
                {
                    Authentications.SignOut();
                    SetAnonymousSession();
                    Response.Redirect(HttpContext.Current.Request.Url.ToString());
                }
            }
            else
            {
                SetAnonymousSession();
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

        private static UserModel GetUser(Context context)
        {
            return new UserModel(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                dataRow: Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersDefaultColumns(),
                        join: Rds.UsersJoinDefault(),
                        where: Rds.UsersWhere().LoginId(context.LoginId)))
                            .AsEnumerable()
                            .FirstOrDefault());
        }

        private void SetAnonymousSession()
        {
            var userModel = new UserModel();
            Session["Language"] = Parameters.Service.DefaultLanguage ?? userModel.Language;
            Session["RdsUser"] = userModel.RdsUser();
            Session["Developer"] = userModel.Developer;
        }
    }
}