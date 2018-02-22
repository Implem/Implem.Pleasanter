using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Initializers;
using Implem.Pleasanter.Libraries.Migrators;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
namespace Implem.Pleasanter
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Application["StartTime"] = DateTime.Now;
            Application["LastAccessTime"] = Application["StartTime"];
            Initialize();
            var log = new SysLogModel();
            SiteInfo.TenantCaches.Add(0, new TenantCache(0));
            SiteInfo.Reflesh();
            UsersInitializer.Initialize();
            ItemsInitializer.Initialize();
            StatusesMigrator.Migrate();
            SiteSettingsMigrator.Migrate();
            StatusesInitializer.Initialize();
            SetConfigrations();
            log.Finish();
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
            var log = new SysLogModel();
            Performances.PerformanceCollection.Save(Directories.Logs());
            log.Finish();
        }

        protected void Application_Error()
        {
            if (Server != null)
            {
                var error = Server.GetLastError();
                if (error != null)
                {
                    try
                    {
                        var log = new SysLogModel();
                        log.SysLogType = SysLogModel.SysLogTypes.Execption;
                        log.ErrMessage = error.Message;
                        log.ErrStackTrace = error.StackTrace;
                        log.Finish();
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
            Session["StartTime"] = DateTime.Now;
            Session["LastAccessTime"] = Session["StartTime"];
            Session["SessionGuid"] = Strings.NewGuid();
            if (Sessions.LoggedIn())
            {
                if (Authentications.Windows())
                {
                    Ldap.UpdateOrInsert(HttpContext.Current.User.Identity.Name);
                }
                var userId = Sessions.UserId();
                var tenantId = Rds.ExecuteScalar_int(statements:
                    Rds.SelectUsers(
                        column: Rds.UsersColumn().TenantId(),
                        where: Rds.UsersWhere().UserId(userId)));
                Sessions.SetTenantId(tenantId);
                StatusesInitializer.Initialize(tenantId);
                var userModel = new UserModel(
                    SiteSettingsUtilities.UsersSiteSettings(),
                    userId);
                if (userModel.AccessStatus == Databases.AccessStatuses.Selected &&
                    !userModel.Disabled)
                {
                    userModel.SetSession();
                }
                else
                {
                    Authentications.SignOut();
                    SetAnonymouseSession();
                    Response.Redirect(HttpContext.Current.Request.Url.ToString());
                }
            }
            else
            {
                SetAnonymouseSession();
            }
            switch (Request.AppRelativeCurrentExecutionFilePath.ToLower())
            {
                case "~/backgroundtasks/do":
                case "~/reminderschedules/remind":
                    break;
                default:
                    new SysLogModel().Finish();
                    break;
            }
        }

        private void SetAnonymouseSession()
        {
            var userModel = new UserModel();
            Session["Language"] = userModel.Language;
            Session["RdsUser"] = userModel.RdsUser();
            Session["Developer"] = userModel.Developer;
        }
    }
}