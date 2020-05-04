using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class ItemsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.Index(
                context: context,
                id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult TrashBox(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.TrashBox(
                context: context,
                id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Calendar(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.Calendar(
                context: context,
                id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Crosstab(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.Crosstab(
                context: context,
                id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Gantt(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.Gantt(
                context: context,
                id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult BurnDown(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.BurnDown(
                context: context,
                id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult TimeSeries(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.TimeSeries(
                context: context,
                id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Kamban(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.Kamban(
                context: context,
                id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ImageLib(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.ImageLib(
                context: context,
                id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult New(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.New(
                context: context,
                id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [HttpPost]
        public ActionResult NewOnGrid(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.NewOnGrid(
                context: context,
                id: id);
            return Content(json);
        }

        [HttpPost]
        public ActionResult CancelNewRow(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.CancelNewRow(
                context: context,
                id: id);
            return Content(json);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Edit(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.Edit(
                context: context,
                id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [HttpPost]
        public string LinkTable(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.LinkTable(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string Import(long id, HttpPostedFileBase[] file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.Import(
                context: context,
                id: id,
                file: Libraries.Requests.HttpPostedFile.Create(file));
            return json;
        }

        [HttpPost]
        public string OpenExportSelectorDialog(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.OpenExportSelectorDialog(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string OpenBulkUpdateSelectorDialog(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.OpenBulkUpdateSelectorDialog(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string BulkUpdateSelectChanged(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.BulkUpdateSelectChanged(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string OpenSetNumericRangeDialog(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.OpenSetNumericRangeDialog(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string OpenSetDateRangeDialog(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.OpenSetDateRangeDialog(
                context: context,
                id: id);
            return json;
        }

        [HttpGet]
        public ActionResult Export(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var responseFile = controller.Export(
                context: context,
                id: id);
            return responseFile;
        }

        [HttpPost]
        public string ExportAsync(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.ExportAsync(
                context: context,
                id: id);
            return json;
        }

        [HttpGet]
        public ActionResult ExportCrosstab(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var responseFile = controller.ExportCrosstab(
                context: context,
                id: id);
            return responseFile;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Search()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var htmlOrJson = controller.Search(context: context);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [HttpPost]
        public ActionResult SearchDropDown(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SearchDropDown(
                context: context,
                id: id);
            return Content(json);
        }

        [HttpPost]
        public ActionResult RelatingDropDown(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.RelatingDropDown(
                context: context,
                id: id);
            return Content(json);
        }

        [HttpPost]
        public ActionResult SelectSearchDropDown(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SelectSearchDropDown(
                context: context,
                id: id);
            return Content(json);
        }

        [HttpPost]
        public string GridRows(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.GridRows(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string EditOnGrid(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.EditOnGrid(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string ReloadRow(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.ReloadRow(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string CopyRow(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.CopyRow(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string TrashBoxGridRows(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.TrashBoxGridRows(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string ImageLibNext(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.ImageLibNext(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string Create(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.Create(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string PreviewTemplate(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.PreviewTemplate(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string Templates(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.Templates(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string CreateByTemplate(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.CreateByTemplate(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string SiteMenu(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SiteMenu(
                context: context,
                id: id);
            return json;
        }

        [HttpPut]
        public string Update(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.Update(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string BulkUpdate(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.BulkUpdate(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string UpdateByGrid(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.UpdateByGrid(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string Copy(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.Copy(
                context: context,
                id: id);
            return json;
        }

        [HttpGet]
        public string MoveTargets(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.MoveTargets(
                context: context,
                id: id);
            return json;
        }

        [HttpPut]
        public string Move(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.Move(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string MoveSiteMenu(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.MoveSiteMenu(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string CreateLink(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.CreateLink(
                context: context,
                id: id);
            return json;
        }

        [HttpPut]
        public string SortSiteMenu(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SortSiteMenu(
                context: context,
                id: id);
            return json;
        }

        [HttpPut]
        public string BulkMove(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.BulkMove(
                context: context,
                id: id);
            return json;
        }

        [HttpDelete]
        public string Delete(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.Delete(
                context: context,
                id: id);
            return json;
        }

        [HttpDelete]
        public string BulkDelete(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.BulkDelete(
                context: context,
                id: id);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.DeleteComment(
                context: context,
                id: id);
            return json;
        }

        [HttpDelete]
        public string DeleteHistory(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.DeleteHistory(
                context: context,
                id: id);
            return json;
        }

        [HttpDelete]
        public string PhysicalDelete(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.PhysicalDelete(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string Restore(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.Restore(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string RestoreFromHistory(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.RestoreFromHistory(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string EditSeparateSettings(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.EditSeparateSettings(
                context: context,
                id: id);
            return json;
        }

        [HttpPut]
        public string Separate(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.Separate(
                context: context,
                id: id);
            return json;
        }

        [AcceptVerbs(HttpVerbs.Put | HttpVerbs.Post | HttpVerbs.Delete)]
        public string SetSiteSettings(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SetSiteSettings(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string OpenImportSitePackageDialog(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.OpenImportSitePackageDialog(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string ImportSitePackage(long id, HttpPostedFileBase[] file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.ImportSitePackage(
                context: context,
                id: id,
                file: Libraries.Requests.HttpPostedFile.Create(file));
            return json;
        }

        [HttpPost]
        public string OpenExportSitePackageDialog(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.OpenExportSitePackageDialog(
                context: context,
                id: id);
            return json;
        }

        [HttpGet]
        public ActionResult ExportSitePackage(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var responseFile = controller.ExportSitePackage(
                context: context,
                id: id);
            return responseFile;
        }

        [HttpPost]
        public string RebuildSearchIndexes(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.RebuildSearchIndexes(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string Histories(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.Histories(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string History(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.History(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string Permissions(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.Permissions(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string SearchPermissionElements(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SearchPermissionElements(
                context: context,
                id: id);
            return json;
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Delete)]
        public string SetPermissions(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SetPermissions(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string OpenPermissionsDialog(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.OpenPermissionsDialog(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string PermissionForCreating(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.PermissionForCreating(
                context: context,
                id: id);
            return json;
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Delete)]
        public string SetPermissionForCreating(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SetPermissionForCreating(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string OpenPermissionForCreatingDialog(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.OpenPermissionForCreatingDialog(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string ColumnAccessControl(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.ColumnAccessControl(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string SetColumnAccessControl(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SetColumnAccessControl(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string OpenColumnAccessControlDialog(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.OpenColumnAccessControlDialog(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string SearchColumnAccessControl(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SearchColumnAccessControl(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string BurnDownRecordDetails(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.BurnDownRecordDetails(
                context: context,
                id: id);
            return json;
        }

        public string UpdateByCalendar(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.UpdateByCalendar(
                context: context,
                id: id);
            return json;
        }

        public string UpdateByKamban(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.UpdateByKamban(
                context: context,
                id: id);
            return json;
        }

        [HttpPut]
        public string SynchronizeTitles(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SynchronizeTitles(
                context: context,
                id: id);
            return json;
        }

        [HttpPut]
        public string SynchronizeSummaries(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SynchronizeSummaries(
                context: context,
                id: id);
            return json;
        }

        [HttpPut]
        public string SynchronizeFormulas(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.SynchronizeFormulas(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string LockTable(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.LockTable(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string UnlockTable(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.UnlockTable(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string ForceUnlockTable(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.ForceUnlockTable(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public string UnlockRecord(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var json = controller.UnlockRecord(
                context: context,
                id: id);
            return json;
        }

        [HttpPost]
        public ContentResult Get(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ItemsController();
            var result = controller.Get(
                context: context,
                id: id);
            return result;
        }
    }
}
