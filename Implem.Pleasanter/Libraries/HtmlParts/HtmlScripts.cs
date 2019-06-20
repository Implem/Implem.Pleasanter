using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.App_Start;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
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
            return !context.Ajax
                ? hb
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery-3.1.0.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery-ui.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery.datetimepicker.full.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery.multiselect.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery.multiselect.filter.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery.validate.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "scripts/plugins/d3.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "scripts/plugins/hogan-3.0.2.min.js"))
                    .Script(src: Locations.Get(
                        context: context,
                        parts: "scripts/plugins/marked.min.js"))
                    .Generals(context: context)
                    .Script(
                        src: Locations.Get(
                            context: context,
                            parts: "resources/scripts?v="
                                + Parameters.ExtendedScripts.Join().Sha512Cng()),
                        _using: Parameters.ExtendedScripts?.Any() == true)
                    .Script(script: script, _using: !script.IsNullOrEmpty())
                    .Script(
                        script: ss.GetScriptBody(context: context, peredicate: o => o.All == true),
                        _using: context.ContractSettings.Script != false
                            && ss.Scripts?.Any() == true)
                    .Script(
                        script: userScript,
                        _using: context.ContractSettings.Script != false
                            && !userScript.IsNullOrEmpty())
                    .OnEditorLoad(context: context)
                : hb;
        }

        private static HtmlBuilder Generals(this HtmlBuilder hb, Context context)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                hb.Script(src: context.VirtualPathToAbsolute("~/bundles/generals.min.js"));
            }
            else
            {
                BundleConfig.Generals().ForEach(path =>
                    hb.Script(src: context.VirtualPathToAbsolute(path)));
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
