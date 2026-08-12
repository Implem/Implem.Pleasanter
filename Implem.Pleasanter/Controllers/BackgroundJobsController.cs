using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class BackgroundJobsController : Controller
    {
        [HttpGet]
        [HttpPost]
        public ActionResult Index()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                log.Finish(context: context, responseSize: 0);
                return StatusCode(403);
            }
            if (context.Ajax)
            {
                var json = HtmlBackgroundJobs.BackgroundJobsIndexJson(
                    context: context);
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
            else
            {
                var html = new HtmlBuilder().BackgroundJobsIndex(
                    context: context);
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
        }

        [HttpPost]
        public string BulkCancel()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                log.Finish(context: context, responseSize: 0);
                return new ResponseCollection(context: context)
                    .Message(new Message(
                        id: "HasNotPermission",
                        text: Displays.HasNotPermission(context: context),
                        css: "error"))
                    .ToJson();
            }
            var json = BackgroundJobQueue.BulkCancel(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string BulkDelete()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                log.Finish(context: context, responseSize: 0);
                return new ResponseCollection(context: context)
                    .Message(new Message(
                        id: "HasNotPermission",
                        text: Displays.HasNotPermission(context: context),
                        css: "error"))
                    .ToJson();
            }
            var json = BackgroundJobQueue.BulkDelete(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public async Task<string> BulkNextJob()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                log.Finish(context: context, responseSize: 0);
                return new ResponseCollection(context: context)
                    .Message(new Message(
                        id: "HasNotPermission",
                        text: Displays.HasNotPermission(context: context),
                        css: "error"))
                    .ToJson();
            }
            var json = await BackgroundJobQueue.BulkNextJob(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpGet]
        public IActionResult Download(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                log.Finish(context: context, responseSize: 0);
                return StatusCode(403);
            }
            var download = BackgroundJobQueue.PrepareDownload(
                context: context,
                backgroundJobId: id);
            switch (download.Status)
            {
                case BackgroundJobDownloadStatus.NotFound:
                    log.Finish(context: context, responseSize: 0);
                    return NotFound();
                case BackgroundJobDownloadStatus.Conflict:
                    log.Finish(context: context, responseSize: 0);
                    return StatusCode(409);
            }
            log.Finish(
                context: context,
                responseSize: (int)(download.FileInfo?.Length ?? 0));
            HttpContext.Response.OnCompleted(() =>
            {
                download.ReleaseLock();
                return Task.CompletedTask;
            });
            return PhysicalFile(
                physicalPath: download.FileInfo.FullName,
                contentType: download.ContentType,
                fileDownloadName: download.FileInfo.Name);
        }

        [HttpPost]
        public string IndexJson()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                return new ResponseCollection(context: context)
                    .Message(new Message(
                        id: "HasNotPermission",
                        text: Displays.HasNotPermission(context: context),
                        css: "error"))
                    .ToJson();
            }
            var json = HtmlBackgroundJobs.BackgroundJobsIndexJson(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string OpenSetDateRangeDialog(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                log.Finish(context: context, responseSize: 0);
                return Messages.ResponseNotFound(context: context).ToJson();
            }
            var ss = SiteSettingsUtilities.BackgroundJobsSiteSettings(
                context: context);
            var controlId = context.Forms.ControlId();
            var columnName = controlId
                .Substring(controlId.IndexOf("__") + 2)
                .Replace("_DateRange", string.Empty);
            var column = ss.GetColumn(
                context: context,
                columnName: columnName)
                    ?? ss.GetColumn(
                        context: context,
                        columnName: "JobFinishedTime");
            var json = new ResponseCollection(context: context)
                .Html(
                    "#SetDateRangeDialog",
                    new HtmlBuilder().SetDateRangeDialog(
                        context: context,
                        ss: ss,
                        column: column,
                        itemfilter: true))
                .ToJson();
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string GridRows()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                return new ResponseCollection(context: context)
                    .Message(new Message(
                        id: "HasNotPermission",
                        text: Displays.HasNotPermission(context: context),
                        css: "error"))
                    .ToJson();
            }
            var json = HtmlBackgroundJobs.BackgroundJobsGridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Edit(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                log.Finish(context: context, responseSize: 0);
                return StatusCode(403);
            }
            if (context.Ajax)
            {
                var json = HtmlBackgroundJobs.BackgroundJobsEditorJson(
                    context: context,
                    backgroundJobId: id);
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
            else
            {
                var html = HtmlBackgroundJobs.BackgroundJobsEditor(
                    context: context,
                    backgroundJobId: id);
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
        }

        [HttpPost]
        public string Cancel(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                log.Finish(context: context, responseSize: 0);
                return new ResponseCollection(context: context)
                    .Message(new Message(
                        id: "HasNotPermission",
                        text: Displays.HasNotPermission(context: context),
                        css: "error"))
                    .ToJson();
            }
            var json = BackgroundJobQueue.CancelJson(
                context: context,
                backgroundJobId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Delete(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                log.Finish(context: context, responseSize: 0);
                return new ResponseCollection(context: context)
                    .Message(new Message(
                        id: "HasNotPermission",
                        text: Displays.HasNotPermission(context: context),
                        css: "error"))
                    .ToJson();
            }
            var json = BackgroundJobQueue.DeleteJson(
                context: context,
                backgroundJobId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public async Task<string> NextJob(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                log.Finish(context: context, responseSize: 0);
                return new ResponseCollection(context: context)
                    .Message(new Message(
                        id: "HasNotPermission",
                        text: Displays.HasNotPermission(context: context),
                        css: "error"))
                    .ToJson();
            }
            var json = await BackgroundJobQueue.NextJobJson(
                context: context,
                backgroundJobId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
