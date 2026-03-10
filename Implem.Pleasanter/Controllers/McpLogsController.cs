using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class McpLogsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Index()
        {
            var context = new Context();
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = McpLogUtilities.Index(
                    context: context,
                    ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context));
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = McpLogUtilities.IndexJson(
                    context: context,
                    ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context));
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Edit(int id)
        {
            var context = new Context();
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = McpLogUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context),
                    mcpLogId: id,
                    clearSessions: true);
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = McpLogUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context),
                    mcpLogId: id);
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpPost]
        public string GridRows()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = McpLogUtilities.GridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string OpenSetNumericRangeDialog(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = ResponseFilterDialogs.OpenSetNumericRangeDialog(
                context: context,
                ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string OpenSetDateRangeDialog(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = ResponseFilterDialogs.OpenSetDateRangeDialog(
                context: context,
                ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context));
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
                ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context));
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
                ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return Content(json);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string OpenExportSelectorDialog()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = McpLogUtilities.OpenExportSelectorDialog(
                context: context,
                ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public ActionResult Export()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var responseFile = McpLogUtilities.Export(
                context: context,
                ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context));
            if (responseFile != null)
            {
                log.Finish(context: context, responseSize: responseFile.Length);
                return responseFile.ToFile().ToFileContentResult();
            }
            else
            {
                return null;
            }
        }
    }
}
