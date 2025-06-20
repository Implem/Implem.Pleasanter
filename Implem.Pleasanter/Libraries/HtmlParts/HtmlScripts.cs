using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.App_Start;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
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
                var path = Path.Combine(Environments.CurrentDirectoryPath, "wwwroot", "components", "manifest.json");
                var json = ManifestLoader(path);
                return hb
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "scripts/plugins/jquery-3.6.0.min.js"))
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
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jQuery-File-Upload/js/vendor/jquery.ui.widget.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jQuery-File-Upload/js/jquery.iframe-transport.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jQuery-File-Upload/js/jquery.fileupload.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jQuery-File-Upload/js/jquery.fileupload-process.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jQuery-File-Upload/js/jquery.fileupload-image.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jQuery-File-Upload/js/jquery.fileupload-video.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/jQuery-File-Upload/js/jquery.fileupload-validate.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/md5.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/moment.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/lightbox.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/gridstack.js/gridstack-all.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/fullcalendar/index.global.min.js"))
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/qrcode.min.js"))
                    .Script(src:
                        Responses.Locations.Get(
                            context: context,
                            parts: $"components/{json["main"]}"),
                            type: "module",
                            crossorigin: true
                    )
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
                        script: ss.GetScriptBody(
                            context: context,
                            peredicate: o =>
                                o.All == true
                                && o.Disabled != true),
                        _using: context.ContractSettings.Script != false
                            && ss.ScriptsAllDisabled != true
                            && ss.Scripts?.Any() == true)
                    .Script(
                        script: userScript,
                        _using: context.ContractSettings.Script != false
                            && ss.ScriptsAllDisabled != true
                            && !userScript.IsNullOrEmpty())
                    .Script(script: "$p.initDashboard();",
                        _using: ss.ReferenceType == "Dashboards"
                            && context.Action == "index")
                    .Script(script: "$p.setCalendar();",
                        _using: ss.ReferenceType == "Dashboards" &&
                             ss.DashboardParts?.Any(part => part.Type == DashboardPartType.Calendar) == true)
                    .Script(script: "$p.setKamban();",
                        _using: ss.ReferenceType == "Dashboards" &&
                             ss.DashboardParts?.Any(part => part.Type == DashboardPartType.Kamban) == true)
                    .Script(script: "$p.setDashboardAsync();",
                        _using: ss.ReferenceType == "Dashboards")
                    .Script(script: "$p.setDashboardGrid();",
                        _using: ss.ReferenceType == "Dashboards")
                    .OnEditorLoad(context: context);
            }
            else
            {
                return hb;
            }
        }

        private static string ExtendedScripts(Context context)
        {
            return ExtendedScripts(
                context: context,
                deptId: context.DeptId,
                groups: context.Groups,
                userId: context.UserId,
                siteTop: context.SiteTop(),
                siteId: context.SiteId,
                id: context.Id,
                controller: context.Controller,
                action: context.Action);
        }

        public static Dictionary<string, string> ManifestLoader(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("manifest.json not found", fileName);
            var json = File.ReadAllText(fileName);
            var manifest = JsonNode.Parse(json)?.AsObject();
            var result = new Dictionary<string, string>();
            if (manifest == null) return result;
            foreach (var entry in manifest)
            {
                var name = entry.Value?["name"]?.ToString();
                var file = entry.Value?["file"]?.ToString();
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(file))
                {
                    result[name] = file;
                }
            }
            return result;
        }

        public static string ExtendedScripts(
            Context context,
            int deptId,
            List<int> groups,
            int userId,
            bool siteTop,
            long siteId,
            long id,
            string controller,
            string action)
        {
            var scripts = (siteTop && !context.TopScript.IsNullOrEmpty()
                ? context.TopScript + '\n'
                : string.Empty)
                    + ExtensionUtilities.ExtensionWhere<ExtendedScript>(
                        extensions: Parameters.ExtendedScripts,
                        name: null,
                        deptId: deptId,
                        groups: groups,
                        userId: userId,
                        siteId: siteId,
                        id: id,
                        controller: controller,
                        action: action)
                            .Select(o => o.Script)
                            .Join("\n");
            return scripts;
        }

        private static HtmlBuilder Generals(this HtmlBuilder hb, Context context)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                hb.Script(src: context.VirtualPathToAbsolute($"~/bundles/generals.min.js?v={Environments.BundlesVersions.Get("generals.js")}"));
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
            if (context?.ResponseCollection?.Any() == true)
            {
                hb
                    .Hidden(
                        controlId: "ServerScriptResponseCollection",
                        value: context.ResponseCollection.ToJson())
                    .Script(script: $"$p.setByJson(undefined, undefined, $p.getData($('#MainForm')), $('#MainForm'), undefined, JSON.parse($('#ServerScriptResponseCollection').val()));");
            }
            switch (context.Action)
            {
                case "new":
                case "edit":
                    hb.Script(script: "$p.execEvents('on_editor_load','');");
                    break;
                case "index":
                    hb.Script(script: "$p.execEvents('on_grid_load','');");
                    break;
                case "calendar":
                    hb.Script(script: "$p.execEvents('on_calendar_load','');");
                    break;
                case "crosstab":
                    hb.Script(script: "$p.execEvents('on_crosstab_load','');");
                    break;
                case "timeseries":
                    hb.Script(script: "$p.execEvents('on_timeseries_load','');");
                    break;
                case "analy":
                    hb.Script(script: "$p.execEvents('on_analy_load','');");
                    break;
                case "kamban":
                    hb.Script(script: "$p.execEvents('on_kamban_load','');");
                    break;
                case "gantt":
                    hb.Script(script: "$p.execEvents('on_gantt_load','');");
                    break;
                case "burndown":
                    hb.Script(script: "$p.execEvents('on_burndown_load','');");
                    break;
            }
            return hb;
        }
    }
}
