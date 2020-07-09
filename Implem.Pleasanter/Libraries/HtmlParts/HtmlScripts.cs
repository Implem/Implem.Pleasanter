using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
using System.Web;
using System.Web.Optimization;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlScripts
    {
        public static HtmlBuilder Scripts(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string script = null,
            string userScript = null)
        {

            if (!context.Ajax)
            { 
                var extendedScripts = ExtendedScripts(
                    siteId: context.SiteId,
                    id: context.Id,
                    controller: context.Controller,
                    action: context.Action);
                return hb
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jquery-3.1.0.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jquery-ui.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jquery.datetimepicker.full.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jquery.multiselect.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jquery.multiselect.filter.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jquery.validate.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/d3.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/hogan-3.0.2.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/marked.min.js"))
                    .Generals()
                    .Script(
                        src: Locations.Get(
                            context: context,
                            parts: $"resources/scripts?v={extendedScripts.Sha512Cng()}"
                                + $"&site-id={context.SiteId}"
                                + $"&id={context.Id}"
                                + $"&controller={context.Controller}"
                                + $"&action={context.Action}"),
                        _using: !extendedScripts.IsNullOrEmpty())
                    .Script(script: script, _using: !script.IsNullOrEmpty())
                    .Script(
                        script: ss.GetScriptBody(context: context, peredicate: o => o.All == true),
                        _using: context.ContractSettings.Script != false
                            && ss.Scripts?.Any() == true)
                    .Script(
                        script: userScript,
                        _using: context.ContractSettings.Script != false
                            && !userScript.IsNullOrEmpty())
                    .OnEditorLoad(context: context);
            }
            else
            {
                return hb;
            }
        }

        public static string ExtendedScripts(
            long siteId, long id, string controller, string action)
        {
            return Parameters.ExtendedScripts
                ?.Where(o => o.SiteIdList?.Any() != true || o.SiteIdList.Contains(siteId))
                .Where(o => o.IdList?.Any() != true || o.IdList.Contains(id))
                .Where(o => o.Controllers?.Any() != true || o.Controllers.Contains(controller))
                .Where(o => o.Actions?.Any() != true || o.Actions.Contains(action))
                .Where(o => !o.Disabled)
                .Select(o => o.Script)
                .Join("\n");
        }

        private static HtmlBuilder Generals(this HtmlBuilder hb)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                hb.Script(src: BundleTable.Bundles.ResolveBundleUrl("~/bundles/Generals"));
            }
            else
            {
                BundleConfig.Generals().ForEach(path =>
                    hb.Script(src: VirtualPathUtility.ToAbsolute(path)));
            }
            return hb;
        }

        private static HtmlBuilder OnEditorLoad(this HtmlBuilder hb, Context context)
        {
            switch (context.Action)
            {
                case "new":
                case "edit":
                    hb.Script(script: "$p.execEvents('on_editor_load','');");
                    break;
                case "index":
                    hb.Script(script: "$p.execEvents('on_grid_load','');");
                    break;
            }
            return hb;
        }
    }
}
