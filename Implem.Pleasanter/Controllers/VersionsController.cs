using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class VersionsController
    {
        public string Index(Context context)
        {
            var log = new SysLogModel(context: context);
            var html = new HtmlBuilder().AssemblyVersions(context: context);
            log.Finish(context: context, responseSize: html.Length);
            return html;
        }
    }
}