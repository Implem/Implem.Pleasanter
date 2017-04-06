using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using System.Linq;
using System.Web;
using System.Web.Optimization;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlScripts
    {
        public static HtmlBuilder Scripts(
            this HtmlBuilder hb,
            string script,
            string userScript,
            string referenceType)
        {
            return !Request.IsAjax()
                ? hb
                    .Script(src: Locations.Get("Scripts/Plugins/jquery-3.1.0.min.js"))
                    .Script(src: Locations.Get("Scripts/Plugins/jquery-ui.min.js"))
                    .Script(src: Locations.Get("Scripts/Plugins/jquery.datetimepicker.full.min.js"))
                    .Script(src: Locations.Get("Scripts/Plugins/jquery.multiselect.min.js"))
                    .Script(src: Locations.Get("Scripts/Plugins/jquery.multiselect.filter.min.js"))
                    .Script(src: Locations.Get("Scripts/Plugins/jquery.validate.min.js"))
                    .Script(src: Locations.Get("Scripts/Plugins/d3.min.js"))
                    .Script(src: Locations.Get("Scripts/Plugins/marked.min.js"))
                    .Generals()
                    .Script(script: script, _using: !script.IsNullOrEmpty())
                    .Script(script: userScript, _using: !userScript.IsNullOrEmpty())
                : hb;
        }

        public static HtmlBuilder Generals(this HtmlBuilder hb)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                hb.Script(src: ResolveBundleUrl("~/bundles/Generals"));
            }
            else
            {
                BundleConfig.Generals().ForEach(path =>
                    hb.Script(src: VirtualPathUtility.ToAbsolute(path)));
            }
            return hb;
        }

        private static string ResolveBundleUrl(string url)
        {
            return BundleTable.Bundles.ResolveBundleUrl(url);
        }
    }
}
