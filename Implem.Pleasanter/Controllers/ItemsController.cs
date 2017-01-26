using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    [RefleshSiteInfo]
    public class ItemsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index(long id = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = new ItemModel(id).Index();
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = new ItemModel(id).IndexJson();
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Gantt(long id = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = new ItemModel(id).Gantt();
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = new ItemModel(id).GanttJson();
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult BurnDown(long id = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = new ItemModel(id).BurnDown();
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = new ItemModel(id).BurnDownJson();
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult TimeSeries(long id = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = new ItemModel(id).TimeSeries();
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = new ItemModel(id).TimeSeriesJson();
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Kamban(long id = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = new ItemModel(id).Kamban();
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = new ItemModel(id).KambanJson();
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult New(long id = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = new ItemModel(id).New();
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = new ItemModel(id).NewJson();
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Edit(long id)
        {
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = new ItemModel(id).Editor();
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = new ItemModel(id).EditorJson();
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [HttpPost]
        public string Import(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).Import();
            log.Finish(json.Length);
            return json;
        }

        [HttpGet]
        public ActionResult Export(long id)
        {
            var log = new SysLogModel();
            var responseFile = new ItemModel(id).Export();
            log.Finish(responseFile.Length);
            return responseFile.ToFile();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Search()
        {
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = SearchIndexUtilities.Search();
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = SearchIndexUtilities.SearchJson();
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [HttpPost]
        public ActionResult SearchDropDown(long id = 0)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).SearchDropDown();
            log.Finish(json.Length);
            return Content(json);
        }

        [HttpPost]
        public ActionResult SelectSearchDropDown(long id = 0)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).SelectSearchDropDown();
            log.Finish(json.Length);
            return Content(json);
        }

        [HttpPost]
        public string GridRows(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).GridRows();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Create(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).Create();
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Update(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).Update();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Copy(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).Copy();
            log.Finish(json.Length);
            return json;
        }

        [HttpGet]
        public string MoveTargets(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).MoveTargets();
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Move(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).Move();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string MoveSiteMenu(long id)
        {
            var log = new SysLogModel();
            var json = SiteUtilities.MoveSiteMenu(id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string SortSiteMenu(long id)
        {
            var log = new SysLogModel();
            var json = SiteUtilities.SortSiteMenu(id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string BulkMove(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).BulkMove();
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).Delete();
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string BulkDelete(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).BulkDelete();
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).DeleteComment();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Restore(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel().Restore(id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string EditSeparateSettings(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).EditSeparateSettings();
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Separate(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).Separate();
            log.Finish(json.Length);
            return json;
        }

        [AcceptVerbs(HttpVerbs.Put | HttpVerbs.Post | HttpVerbs.Delete)]
        public string SetSiteSettings(long id)
        {
            var log = new SysLogModel();
            var json = new SiteModel(id).SetSiteSettings();
            log.Finish(json.Length);
            return json;
        }

        [HttpGet]
        public string Histories(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).Histories();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string History(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).History();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string BurnDownRecordDetails(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).BurnDownRecordDetailsJson();
            log.Finish(json.Length);
            return json;
        }

        public string UpdateByKamban(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).UpdateByKamban();
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string SynchronizeSummaries(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).SynchronizeSummaries();
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string SynchronizeFormulas(long id)
        {
            var log = new SysLogModel();
            var json = new ItemModel(id).SynchronizeFormulas();
            log.Finish(json.Length);
            return json;
        }
    }
}
