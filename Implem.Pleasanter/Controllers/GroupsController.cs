using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class GroupsController : Controller
    {
        public string Index(Context context)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = GroupUtilities.Index(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = GroupUtilities.IndexJson(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string New(Context context, long id = 0)
        {
            var log = new SysLogModel(context: context);
            var html = GroupUtilities.EditorNew(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
            log.Finish(context: context, responseSize: html.Length);
            return html;
        }

        public string Edit(Context context, int id)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = GroupUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                    groupId: id,
                    clearSessions: true);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = GroupUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                    groupId: id);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string GridRows(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.GridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Create(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Create(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Update(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Delete(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Delete(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string DeleteComment(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Histories(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Histories(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string History(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.History(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SelectableMembers(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.SelectableMembersJson(context: context);
            return json;
        }
    }
}
