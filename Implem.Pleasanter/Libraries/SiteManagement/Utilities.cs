using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Implem.Pleasanter.Libraries.SiteManagement
{
    public class Utilities
    {
        public static ResponseFile VisualizeSettings(Context context, SiteSettings ss)
        {
            var viewer = context.QueryStrings.Data("viewer")?.ToLower();
            var canUseError = ValidateCanUse(context: context);
            if (canUseError.Type != Error.Types.None)
            {
                return (viewer == "html")
                    ? ResponseErrorHtml(
                        context: context,
                        error: canUseError)
                    : ResponseFile.Get(apiResponse: ApiResponses.Error(
                        context: context,
                        errorData: canUseError,
                        canUseError.Data));
            }
            if (viewer == "html")
            {
                return ResponseViewerHtml(context: context);
            }
            var siteIds = new List<SettingsJsonConverter.SelectedSite>();
            var siteIdsString = context.QueryStrings.Data("SiteSelectableAll");
            var groupNameString = context.QueryStrings.Data("SiteGroupName");
            if (!siteIdsString.IsNullOrEmpty())
            {
                siteIds.AddRange(Regex.Replace(siteIdsString, @"[^0-9-,]", "")
                    .Split(",")
                    .Select(e => new SettingsJsonConverter.SelectedSite()
                    {
                        SiteId = e.Split_1st('-').ToLong(),
                        Ver = e.Split_2nd('-').ToInt()
                    })
                    .Distinct());
            }
            else if (!groupNameString.IsNullOrEmpty())
            {
                siteIds.AddRange(new SiteCollection(
                    context: context,
                    column: Rds.SitesColumn()
                        .SiteId()
                        .Title()
                        .ReferenceType()
                        .SiteSettings(),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteGroupName(groupNameString))
                    .Select(siteModel => new SettingsJsonConverter.SelectedSite() { SiteId = siteModel.SiteId })
                    .ToArray());
            }
            else
            {
                siteIds.Add(new SettingsJsonConverter.SelectedSite() { SiteId = ss.SiteId });
            }
            if (siteIds.Count == 0)
            {
                return ResponseFile.Get(apiResponse: ApiResponses.InvalidRequest(context: context));
            }
            else if (siteIds.Count > 256)
            {
                return ResponseFile.Get(apiResponse: ApiResponses.InvalidRequest(context: context));
            }
            var types = context.QueryStrings.Data("Type")?.ToLower()?.Split(",") ?? [];
            var exportType = context.QueryStrings.Data("ExportType")?.ToLower();
            switch(exportType)
            {
                case "json":
                case "xlsx":
                case "mermaid":
                    break;
                default:
                    return ResponseFile.Get(apiResponse: ApiResponses.BadRequest(context: context));
            }
            var jsonParam = new SettingsJsonConverter.Param
            {
                Types = types,
                SelectedSites = siteIds
            };
            var dump = SettingsJsonConverter.Convert(
                context: context,
                param: jsonParam);
            if (dump == null)
            {
                return ResponseFile.Get(apiResponse: ApiResponses.BadRequest(context: context));
            }
            if (exportType == "json")
            {
                var ms = new MemoryStream(dump.RecordingJson(context: context).ToBytes(), false);
                return new ResponseFile(
                    fileContent: ms,
                    fileDownloadName: ExportUtilities.FileName(
                        context: context,
                        title: "VisualizeSettings_" + ss.SiteId,
                        extension: "json"),
                    contentType: "application/json");
            }
            else if (exportType == "xlsx")
            {
                var xlsxParam = new Json2XlsxConvertor.Param();
                try
                {
                    xlsxParam.WorkDir = Path.Combine(DefinitionAccessor.Directories.Temp(), Strings.NewGuid());
                    xlsxParam.ZipFileName = ExportUtilities.FileName(
                        context: context,
                        title: "VisualizeSettings",
                        extension: "zip");
                    xlsxParam.CellStyleManager = new Json2XlsxConvertor.CellStyleManager();
                    bool flowControl = Json2XlsxConvertor.Convert(dump.RecordingJson(context: context), xlsxParam);
                    if (flowControl)
                    {
                        var bytes = System.IO.File.ReadAllBytes(System.IO.Path.Combine(xlsxParam.WorkDir, xlsxParam.ZipFileName));
                        return new ResponseFile(
                            fileContent: new MemoryStream(bytes, false),
                            fileDownloadName: System.IO.Path.GetFileName(xlsxParam.ZipFileName),
                            contentType: "application/zip");
                    }
                    else
                    {
                        return ResponseFile.Get(apiResponse: ApiResponses.BadRequest(context: context));
                    }
                }
                finally
                {
                    if (Directory.Exists(xlsxParam.WorkDir)) Directory.Delete(xlsxParam.WorkDir, true);
                }
            }
            else if (exportType == "mermaid")
            {
                var (mermaidText, flowControl) = Json2MermaidConvertor.Convert(dump);
                if (flowControl)
                {
                    var mem = new MemoryStream(mermaidText.ToBytes(), false);
                    return new ResponseFile(
                        fileContent: mem,
                        fileDownloadName: ExportUtilities.FileName(
                            context: context,
                            title: "VisualizeSettings",
                            extension: "mmd"),
                        contentType: "application/zip");
                }
                else
                {
                    return ResponseFile.Get(apiResponse: ApiResponses.BadRequest(context: context));
                }
            }
            return ResponseFile.Get(apiResponse: ApiResponses.BadRequest(context: context));
        }

        private static ResponseFile ResponseViewerHtml(Context context)
        {
            var path = Path.Combine(
                DefinitionAccessor.Directories.Wwwroot(),
                "Extensions",
                "smt-json-to-table.html");
            var text = System.IO.File.ReadAllText(path);
            text = text
                .Replace("{{ApplicationPath}}", context.ApplicationPath)
                .Replace(
                    "{{nonce}}",
                    context.Nonce.IsNullOrEmpty()
                        ? string.Empty
                        : $"nonce=\"{context.Nonce}\"");
            var bytes = text.ToBytes();
            return new ResponseFile(
                fileContent: new MemoryStream(bytes, false),
                fileDownloadName: string.Empty,
                contentType: "text/html");
        }

        private static ResponseFile ResponseErrorHtml(Context context, ErrorData error)
        {
            var errCode = ApiResponses.StatusCode(error.Type);
            var errText = error.Message(context: context).Text;
            var html = $"<html><title>Error</title><body>{errCode} {errText}</body></html>";
            return new ResponseFile(
                fileContent: new MemoryStream(html.ToBytes(), false),
                fileDownloadName: string.Empty,
                contentType: "text/html");
        }

        private static ErrorData ValidateCanUse(Context context)
        {
            var error = Error.Types.BadRequest;
            switch (DefinitionAccessor.Parameters.Environment()) {
                case 1:
                    error = Error.Types.BadRequest;
                    break;
                case 2:
                    error = Error.Types.None;
                    break;
                case 0:
                case 3:
                    error = Implem.DefinitionAccessor.Parameters.CommercialLicense()
                        ? Error.Types.None
                        : Error.Types.HasNotPermission;
                    break;
                default:
                    break;
            }
            if (error == Error.Types.None)
            {
                if (DefinitionAccessor.Parameters.PleasanterExtensions?.SiteVisualizer?.Disabled ?? false)
                {
                    return new ErrorData(
                        context: context,
                        type: Error.Types.BadRequest,
                        sysLogsStatus: ApiResponses.StatusCode(Error.Types.BadRequest),
                        sysLogsDescription: "Site Visualizer is disabled.");
                }
            }
            return new ErrorData(
                context: context,
                type: error,
                sysLogsStatus: ApiResponses.StatusCode(error),
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }
    }
}
