using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Libraries.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
namespace Implem.Pleasanter.NetCore.Controllers
{
    [Authorize]
    public class BinariesController : Controller
    {
        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ActionResult SiteImageThumbnail(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var fileContent = controller.SiteImageThumbnail(context: context, reference: reference, id: id);
            return ((System.Web.Mvc.FileContentResult)fileContent).ToFileContentResult();
        }

        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ActionResult SiteImageIcon(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var fileContent = controller.SiteImageIcon(context: context, reference: reference, id: id);
            return ((System.Web.Mvc.FileContentResult)fileContent).ToFileContentResult();
        }

        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ActionResult TenantImageLogo()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var fileContent = controller.TenantImageLogo(context: context);
            return ((System.Web.Mvc.FileContentResult)fileContent).ToFileContentResult();
        }

        [HttpPost]
        public string UpdateSiteImage(string reference, long id, ICollection<IFormFile> file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var json = controller.UpdateSiteImage(context: context, reference: reference, id: id, file: HttpPostedFile.Create(file));
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string UpdateTenantImage(ICollection<IFormFile> file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var json = controller.UpdateTenantImage(context: context, file: HttpPostedFile.Create(file));
            return json;
        }

        [HttpDelete]
        public string DeleteSiteImage(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var json = controller.DeleteSiteImage(context: context, reference: reference, id: id);
            return json;
        }

        [HttpDelete]
        public string DeleteTenantImage()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var json = controller.DeleteTenantImage(context: context);
            return json;
        }

        [HttpPost]
        public string UploadImage(string reference, long id, ICollection<IFormFile> file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var json = controller.UploadImage(context: context, reference: reference, id: id, file: HttpPostedFile.Create(file));
            return json;
        }

        [HttpDelete]
        public string DeleteImage(string reference, string guid)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var json = controller.DeleteImage(context: context, reference: reference, guid: guid);
            return json;
        }

        [HttpPost]
        public string MultiUpload(string reference, long id, ICollection<IFormFile> file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var json = controller.MultiUpload(context: context, reference: reference, id: id, file: HttpPostedFile.Create(file));
            return json;
        }

        [HttpGet]
        public FileContentResult Download(string reference, string guid)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var fileContent = controller.Download(context: context, reference: reference, guid: guid);
            return ((System.Web.Mvc.FileContentResult)fileContent).ToFileContentResult();
        }

        [HttpGet]
        public FileContentResult DownloadTemp(string reference, string guid)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var fileContent = controller.DownloadTemp(context: context, reference: reference, guid: guid);
            return ((System.Web.Mvc.FileContentResult)fileContent).ToFileContentResult();
        }

        [HttpGet]
        public ActionResult Show(string reference, string guid)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var fileContent = controller.Show(context: context, reference: reference, guid: guid);
            return fileContent != null
                ? File(fileContent.FileContents, fileContent.ContentType)
                : null;
        }

        [HttpGet]
        public ActionResult ShowTemp(string reference, string guid)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var fileContent = controller.ShowTemp(context: context, reference: reference, guid: guid);
            return fileContent != null
                ? File(fileContent.FileContents, fileContent.ContentType)
                : null;
        }

        [HttpPost]
        public string DeleteTemp(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.BinariesController();
            var json = controller.DeleteTemp(context: context, reference: reference, id: id);
            return json.ToString();
        }
    }
}