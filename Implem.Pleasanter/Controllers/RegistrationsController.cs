using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class RegistrationsController : Controller
    {
        public string Index(Context context)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = RegistrationUtilities.Index(
                    context: context,
                    ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = RegistrationUtilities.IndexJson(
                    context: context,
                    ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string New(Context context, long id = 0)
        {
            var log = new SysLogModel(context: context);
            var html = RegistrationUtilities.EditorNew(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
            log.Finish(context: context, responseSize: html.Length);
            return html;
        }

        public string Edit(Context context, int id)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = RegistrationUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                    registrationId: id,
                    clearSessions: true);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = RegistrationUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                    registrationId: id);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string GridRows(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.GridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Create(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Create(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Update(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Delete(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Delete(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string DeleteComment(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Histories(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Histories(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string History(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.History(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string BulkDelete(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.BulkDelete(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Login(Context context)
        {
            var log = new SysLogModel(context: context);
            var html = RegistrationUtilities.Login(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
            log.Finish(context: context);
            log.Finish(context: context, responseSize: html.Length);
            return html;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ApprovalRequest(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.ApprovalRequest(context: context,
            ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
            registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Approval(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Approval(context: context,
            ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
            registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
