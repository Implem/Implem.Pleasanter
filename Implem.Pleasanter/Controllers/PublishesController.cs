using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    [PublishesAttributes]
    public class PublishesController : Controller
    {
        public ActionResult Index(long id = 0)
        {
            var context = new Context();
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(
                    context: context,
                    referenceId: id)
                        .Index(context: context);
                ViewBag.HtmlBody = html;
                log.Finish(
                    context: context,
                    responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(
                    context: context,
                    referenceId: id)
                        .IndexJson(context: context);
                log.Finish(
                    context: context,
                    responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpPost]
        public string OpenSetNumericRangeDialog(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).OpenSetNumericRangeDialog(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string OpenSetDateRangeDialog(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).OpenSetDateRangeDialog(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public ActionResult SearchDropDown(long id = 0)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).SearchDropDown(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return Content(json);
        }

        [HttpPost]
        public ActionResult SelectSearchDropDown(long id = 0)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).SelectSearchDropDown(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return Content(json);
        }

        [HttpPost]
        public string GridRows(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).GridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Edit(long id)
        {
            var context = new Context();
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).Editor(context: context);
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).EditorJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }
    }
}