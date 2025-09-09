using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Manifests;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.IO;
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
                        parts: "assets/plugins/jquery-3.6.0.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/plugins/jquery-ui.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/plugins/jquery.datetimepicker/jquery.datetimepicker.full.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/plugins/jquery.multiselect/jquery.multiselect.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/plugins/jquery.multiselect/jquery.multiselect.filter.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/plugins/jquery.validate.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/plugins/d3.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/plugins/hogan-3.0.2.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/plugins/marked.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/jQuery-File-Upload/jquery.iframe-transport.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/jQuery-File-Upload/jquery.fileupload.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/jQuery-File-Upload/jquery.fileupload-process.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/jQuery-File-Upload/jquery.fileupload-image.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/jQuery-File-Upload/jquery.fileupload-video.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/jQuery-File-Upload/jquery.fileupload-validate.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/md5.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/moment.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/lightbox/lightbox.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/gridstack.js/gridstack-all.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/fullcalendar/index.global.min.js"),
                        nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: "assets/Plugins/qrcode.min.js"),
                        nonce: context.Nonce)
                    .ManifestScripts(ManifestLoader.Load(
                        Path.Combine(Environments.CurrentDirectoryPath, "wwwroot", "components", "manifest.json")
                    ), "components", context)
                    .ManifestScripts(ManifestLoader.Load(
                        Path.Combine(Environments.CurrentDirectoryPath, "wwwroot", "assets", "manifest.json")
                    ), "assets", context)
                    .Script(script: script, _using: !script.IsNullOrEmpty(), nonce: context.Nonce)
                    .Script(src: Responses.Locations.Get(
                        context: context,
                        parts: $"resources/scripts?v={extendedScripts.Sha512Cng()}"
                            + $"&site-id={context.SiteId}"
                            + $"&id={context.Id}"
                            + $"&controller={context.Controller}"
                            + $"&action={context.Action}"),
                        nonce: context.Nonce,
                        _using: !extendedScripts.IsNullOrEmpty())
                    .Script(
                        script: ss.GetScriptBody(
                            context: context,
                            peredicate: o =>
                                o.All == true
                                && o.Disabled != true),
                        _using: context.ContractSettings.Script != false
                            && ss.ScriptsAllDisabled != true
                            && ss.Scripts?.Any() == true,
                        nonce: context.Nonce)
                    .Script(
                        script: userScript,
                        _using: context.ContractSettings.Script != false
                            && ss.ScriptsAllDisabled != true
                            && !userScript.IsNullOrEmpty(),
                        nonce: context.Nonce)
                    .Script(script: "$p.initDashboard();",
                        _using: ss.ReferenceType == "Dashboards"
                            && context.Action == "index",
                        nonce: context.Nonce)
                    .Script(script: "$p.setCalendar();",
                        _using: ss.ReferenceType == "Dashboards" &&
                            ss.DashboardParts?.Any(part => part.Type == DashboardPartType.Calendar) == true,
                        nonce: context.Nonce)
                    .Script(script: "$p.setKamban();",
                        _using: ss.ReferenceType == "Dashboards" &&
                            ss.DashboardParts?.Any(part => part.Type == DashboardPartType.Kamban) == true,
                        nonce: context.Nonce)
                    .Script(script: "$p.setDashboardAsync();",
                        _using: ss.ReferenceType == "Dashboards",
                        nonce: context.Nonce)
                    .Script(script: "$p.setDashboardGrid();",
                        _using: ss.ReferenceType == "Dashboards",
                        nonce: context.Nonce)
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

        public static HtmlBuilder ManifestScripts(
            this HtmlBuilder hb,
            List<ManifestLoader.ResultEntry> entries,
            string path,
            Context context)
        {
            foreach (var entry in entries)
            {
                var src = Responses.Locations.Raw(context, parts: $"{path}/{entry.File}");
                hb.Script(
                    src: src,
                    type: entry.Imports != null && entry.Imports.Count > 0 ? "module" : null,
                    crossorigin: entry.Imports != null && entry.Imports.Count > 0 ? true : false,
                    nonce: context.Nonce
                );
            }
            return hb;
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

        private static HtmlBuilder OnEditorLoad(this HtmlBuilder hb, Context context)
        {
            if (context?.ResponseCollection?.Any() == true)
            {
                hb
                    .Hidden(
                        controlId: "ServerScriptResponseCollection",
                        value: context.ResponseCollection.ToJson())
                    .Script(
                        script: $"$p.setByJson(undefined, undefined, $p.getData($('#MainForm')), $('#MainForm'), undefined, JSON.parse($('#ServerScriptResponseCollection').val()));",
                        nonce: context.Nonce);
            }
            switch (context.Action)
            {
                case "new":
                case "edit":
                    hb.Script(
                        script: "$p.execEvents('on_editor_load','');",
                        nonce: context.Nonce);
                    break;
                case "index":
                    hb.Script(
                        script: "$p.execEvents('on_grid_load','');",
                        nonce: context.Nonce);
                    break;
                case "calendar":
                    hb.Script(
                        script: "$p.execEvents('on_calendar_load','');",
                        nonce: context.Nonce);
                    break;
                case "crosstab":
                    hb.Script(
                        script: "$p.execEvents('on_crosstab_load','');",
                        nonce: context.Nonce);
                    break;
                case "timeseries":
                    hb.Script(
                        script: "$p.execEvents('on_timeseries_load','');",
                        nonce: context.Nonce);
                    break;
                case "analy":
                    hb.Script(
                        script: "$p.execEvents('on_analy_load','');",
                        nonce: context.Nonce);
                    break;
                case "kamban":
                    hb.Script(
                        script: "$p.execEvents('on_kamban_load','');",
                        nonce: context.Nonce);
                    break;
                case "gantt":
                    hb.Script(
                        script: "$p.execEvents('on_gantt_load','');",
                        nonce: context.Nonce);
                    break;
                case "burndown":
                    hb.Script(
                        script: "$p.execEvents('on_burndown_load','');",
                        nonce: context.Nonce);
                    break;
            }
            return hb;
        }
    }
}
