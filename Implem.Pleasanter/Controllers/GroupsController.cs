using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Index()
        {
            var context = new Context();
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = GroupUtilities.Index(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = GroupUtilities.IndexJson(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var html = GroupUtilities.EditorNew(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
            ViewBag.HtmlBody = html;
            log.Finish(context: context, responseSize: html.Length);
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Edit(int id)
        {
            var context = new Context();
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = GroupUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                    groupId: id,
                    clearSessions: true);
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = GroupUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                    groupId: id);
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpPost]
        public string GridRows()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.GridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Create(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Delete(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Histories(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Histories(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string History(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.History(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public ActionResult SearchDropDown()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = Libraries.Models.DropDowns.SearchDropDown(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return Content(json);
        }

        [HttpPost]
        public ActionResult SelectSearchDropDown()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = Libraries.Models.DropDowns.SelectSearchDropDown(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return Content(json);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string SelectableMembers(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.SelectableMembersJson(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string SelectableChildren(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.SelectableChildrenJson(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string CurrentMembers(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.CurrentMembersJson(context: context, groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string CurrentChildren(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.CurrentChildrenJson(context: context, groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string Import(long id, ICollection<IFormFile> file)
        {
            var context = new Context(files: file);
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Import(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string OpenExportSelectorDialog()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.OpenExportSelectorDialog(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public ActionResult Export()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var responseFile = GroupUtilities.Export(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
            if (responseFile != null)
            {
                log.Finish(context: context, responseSize: responseFile.Length);
                return responseFile.ToFile();
            }
            else
            {
                log.Finish(context: context, responseSize: 0);
                return null;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpDelete]
        public string BulkDelete(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.BulkDelete(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult TrashBox()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (!context.Ajax)
            {
                var html = GroupUtilities.TrashBox(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(
                        context: context,
                        tableTypes: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Deleted));
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return context.RedirectData.Url.IsNullOrEmpty()
                    ? View()
                    : Redirect(context.RedirectData.Url);
            }
            else
            {
                var json = GroupUtilities.TrashBoxJson(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(
                        context: context,
                        tableTypes: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Deleted));
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string TrashBoxGridRows(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.GridRows(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(
                    context: context,
                    tableTypes: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Deleted),
                offset: context.Forms.Int("GridOffset"),
                action: "TrashBoxGridRows");
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string Restore(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Restore(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(
                    context: context,
                    tableTypes: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Deleted));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpDelete]
        public string PhysicalDelete(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.PhysicalBulkDelete(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(
                    context: context,
                    tableTypes: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Deleted));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
