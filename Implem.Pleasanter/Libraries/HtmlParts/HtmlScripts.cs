using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.App_Start;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
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
            if (!context.Ajax)
            {
                var extendedScripts = ExtendedScripts(context: context);
                return hb
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery-3.1.0.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery-ui.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery.datetimepicker.full.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery.multiselect.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery.multiselect.filter.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery.validate.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "scripts/plugins/d3.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "scripts/plugins/hogan-3.0.2.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "scripts/plugins/marked.min.js"))
                    .Generals(context: context)
                    .Script(
                        src: Responses.Locations.Get(
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
            Context context)
        {
            return ExtendedScripts(
                deptId: context.DeptId,
                userId: context.UserId,
                siteId: context.SiteId,
                id: context.Id,
                controller: context.Controller,
                action: context.Action);
        }

        public static string ExtendedScripts(
            int deptId,
            int userId,
            long siteId,
            long id,
            string controller,
            string action)
        {
            return ExtensionUtilities.ExtensionWhere<ExtendedScript>(
                extensions: Parameters.ExtendedScripts,
                deptId: deptId,
                userId: userId,
                siteId: siteId,
                id: id,
                controller: controller,
                action: action)
                    .Select(o => o.Script)
                    .Join("\n");
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
