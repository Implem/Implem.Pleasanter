using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.Models.Sites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Implem.ParameterAccessor.Parts.Types;
namespace Implem.Pleasanter.Libraries.SitePackages
{
    public class Utilities
    {
        public static string OpenImportSitePackageDialog(
            Context context, SiteSettings ss)
        {
            if (ss.SiteId == 0)
            {
                ss.PermissionType = context.SiteTopPermission();
            }
            if (!Parameters.SitePackage.Import
                || !context.CanManageSite(ss: ss))
            {
                return Messages.ResponseInvalidRequest(context: context).ToJson();
            }
            return new ResponseCollection(context: context)
                .Html(
                    "#ImportSitePackageDialog",
                    new HtmlBuilder().ImportSitePackageDialog(
                        context: context,
                        ss: ss))
                .ToJson();
        }

        public static string ImportSitePackage(
            Context context,
            SiteSettings ss,
            SitePackageApiModel apiData = null)
        {
            var includeData = Parameters.SitePackage.IncludeDataOnImport != OptionTypes.Disabled
                && (apiData?.SelectedSites.Any(site => site.IncludeData == true)
                    ?? context.Forms.Bool("IncludeData"));
            var includeSitePermission = Parameters.SitePackage.IncludeSitePermissionOnImport != OptionTypes.Disabled
                && (apiData?.IncludeSitePermission
                    ?? context.Forms.Bool("IncludeSitePermission"));
            var includeRecordPermission = Parameters.SitePackage.IncludeRecordPermissionOnImport != OptionTypes.Disabled
                && (apiData?.IncludeRecordPermission
                    ?? context.Forms.Bool("IncludeRecordPermission"));
            var includeColumnPermission = Parameters.SitePackage.IncludeColumnPermissionOnImport != OptionTypes.Disabled
                && (apiData?.IncludeColumnPermission
                    ?? context.Forms.Bool("IncludeColumnPermission"));
            var includeNotifications = Parameters.SitePackage.IncludeNotificationsOnImport != OptionTypes.Disabled
                && (apiData?.IncludeNotifications
                    ?? context.Forms.Bool("IncludeNotifications"));
            var includeReminders = Parameters.SitePackage.IncludeRemindersOnImport != OptionTypes.Disabled
                && (apiData?.IncludeReminders
                    ?? context.Forms.Bool("IncludeReminders"));
            if (ss.SiteId == 0)
            {
                ss.PermissionType = context.SiteTopPermission();
            }
            if (!Parameters.SitePackage.Import
                || !context.CanManageSite(ss: ss))
            {
                return Messages.ResponseInvalidRequest(context: context).ToJson();
            }
            if (includeData && context.ContractSettings.Import == false)
            {
                return Messages.ResponseInvalidRequest(context: context).ToJson();
            }
            try
            {
                var sitePackage = apiData == null
                    ? GetSitePackageFromPostedFile(context: context)
                    : GetSitePackage(
                        context: context,
                        ss: ss,
                        apiData: apiData);
                if (sitePackage == null)
                {
                    return Messages.ResponseFailedReadFile(context: context).ToJson();
                }
                return ImportSitePackage(
                    context: context,
                    ss: ss,
                    sitePackage: sitePackage,
                    apiData: apiData,
                    includeData: includeData,
                    includeSitePermission: includeSitePermission,
                    includeRecordPermission: includeRecordPermission,
                    includeColumnPermission: includeColumnPermission,
                    includeNotifications: includeNotifications,
                    includeReminders: includeReminders);
            }
            catch(Exception e)
            {
                new SysLogModel(
                    context: context,
                    e: e);
                return Messages.ResponseInternalServerError(context: context).ToJson();
            }
        }

        public static string ImportSitePackage(
            Context context,
            SiteSettings ss,
            SitePackage sitePackage,
            SitePackageApiModel apiData = null,
            bool includeData = true,
            bool includeSitePermission = true,
            bool includeRecordPermission = true,
            bool includeColumnPermission = true,
            bool includeNotifications = true,
            bool includeReminders = true)
        {
            if (sitePackage == null)
            {
                return Messages.ResponseInvalidRequest(context: context).ToJson();
            }
            if (context.ContractSettings.SitesLimit(
                context: context,
                number: sitePackage.Sites.Count()))
            {
                return Messages.ResponseSitesLimit(context: context).ToJson();
            }
            if (includeData)
            {
                if (Parameters.General.ImportMax > 0
                    && Parameters.General.ImportMax < sitePackage.Data
                        .SelectMany(o => o.Body)
                        .Count())
                {
                    return Messages.ResponseImportMax(
                        context: context,
                        data: Parameters.General.ImportMax.ToString())
                            .ToJson();
                }
                if (sitePackage.Data.Any(o => o.Header.Any(p =>
                    context.ContractSettings.ItemsLimit(
                        context: context,
                        siteId: p.SiteId,
                        number: o.Body.Count()))))
                {
                    return Messages.ResponseItemsLimit(
                        context: context,
                        data: Parameters.General.ImportMax.ToString())
                            .ToJson();
                }
            }
            sitePackage.ConvertInheritPermissionInNotIncluded();
            sitePackage.HeaderInfo.SavedBaseSiteId = apiData?.TargetSiteId
                ?? context.SiteId;
            sitePackage.HeaderInfo.SavedInheritPermission = ss.InheritPermission;
            foreach (var conv in sitePackage.HeaderInfo.Convertors)
            {
                conv.SiteTitle = conv.SiteId == sitePackage.HeaderInfo.BaseSiteId
                    && !String.IsNullOrEmpty(apiData?.SiteTitle)
                        ? apiData.SiteTitle
                        : conv.SiteTitle;
                var response = Repository.ExecuteScalar_response(
                    context: context,
                    transactional: true,
                    selectIdentity: true,
                    statements: new SqlStatement[]
                    {
                        Rds.InsertItems(
                            selectIdentity: true,
                            param: Rds.ItemsParam()
                                .ReferenceType("Sites")
                                .Title(conv.SiteTitle)),
                        Rds.UpdateItems(
                            where: Rds.ItemsWhere().ReferenceId(raw: Def.Sql.Identity),
                            param: Rds.ItemsParam().SiteId(raw: Def.Sql.Identity)),
                    });
                conv.SavedSiteId = response.Id;
            }
            foreach (var conv in sitePackage.HeaderInfo.Convertors)
            {
                var packageSiteModel = sitePackage.Sites
                    .Where(e => e.SiteId == conv.SiteId)
                    .FirstOrDefault();
                packageSiteModel.SetSavedIds(
                    context: context,
                    ss: ss,
                    sitePackage: sitePackage,
                    savedSiteId: conv.SavedSiteId.ToLong(),
                    includeSitePermission: includeSitePermission
                        && sitePackage.HeaderInfo.IncludeSitePermission);
                packageSiteModel.SiteSettings = packageSiteModel.GetSavedSiteSettings(
                    context: context,
                    header: sitePackage.HeaderInfo,
                    includeColumnPermission: includeColumnPermission,
                    permissionIdList: sitePackage.PermissionIdList,
                    includeNotifications: includeNotifications,
                    includeReminders: includeReminders);
                conv.SiteSettings = packageSiteModel.SiteSettings;
                Repository.ExecuteScalar_response(
                    context: context,
                    transactional: true,
                    statements: new SqlStatement[]
                    {
                        Rds.InsertSites(param: Rds.SitesParam()
                            .SiteId(packageSiteModel.SavedSiteId)
                            .TenantId(packageSiteModel.SavedTenantId)
                            .Title(conv.SiteTitle)
                            .SiteName(packageSiteModel.SiteName)
                            .SiteGroupName(packageSiteModel.SiteGroupName)
                            .Body(packageSiteModel.Body)
                            .GridGuide(packageSiteModel.GridGuide)
                            .EditorGuide(packageSiteModel.EditorGuide)
                            .CalendarGuide(packageSiteModel.CalendarGuide)
                            .CrosstabGuide(packageSiteModel.CrosstabGuide)
                            .GanttGuide(packageSiteModel.GanttGuide)
                            .BurnDownGuide(packageSiteModel.BurnDownGuide)
                            .TimeSeriesGuide(packageSiteModel.TimeSeriesGuide)
                            .AnalyGuide(packageSiteModel.AnalyGuide)
                            .KambanGuide(packageSiteModel.KambanGuide)
                            .ImageLibGuide(packageSiteModel.ImageLibGuide)
                            .ReferenceType(packageSiteModel.ReferenceType.MaxLength(32))
                            .ParentId(packageSiteModel.SavedParentId)
                            .InheritPermission(packageSiteModel.SavedInheritPermission)
                            .SiteSettings(packageSiteModel.SiteSettings.ToJson())
                            .Publish(packageSiteModel.Publish)
                            .DisableCrossSearch(packageSiteModel.DisableCrossSearch)
                            .Comments(packageSiteModel.Comments.ToJson())),
                        Rds.PhysicalDeleteLinks(
                            where: Rds.LinksWhere().SourceId(packageSiteModel.SavedSiteId)),
                        LinkUtilities.Insert(link: packageSiteModel.SiteSettings.Links
                            ?.Where(o => o.SiteId > 0)
                            .Select(o => o.SiteId)
                            .Distinct()
                            .ToDictionary(o => o, o => packageSiteModel.SavedSiteId)
                                ?? new Dictionary<long, long>() ),
                        Rds.InsertPermissions(
                            param: Rds.PermissionsParam()
                                .ReferenceId(packageSiteModel.SavedSiteId)
                                .DeptId(0)
                                .UserId(context.UserId)
                                .PermissionType(Permissions.Manager()),
                            _using: packageSiteModel.SavedInheritPermission == packageSiteModel.SavedSiteId),
                    });
                var siteModel = new SiteModel(
                    context: context,
                    siteId: packageSiteModel.SavedSiteId);
                var fullText = siteModel.FullText(
                    context: context,
                    ss: siteModel.SiteSettings);
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId(packageSiteModel.SavedSiteId),
                        param: Rds.ItemsParam()
                            .FullText(fullText, _using: fullText != null)
                            .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null)));
                var statements = siteModel.GetReminderSchedulesStatements(context: context);
                Repository.ExecuteScalar_response(
                    context: context,
                    transactional: true,
                    statements: statements.ToArray());
            }
            var idHash = sitePackage.GetIdHashFromConverters();
            foreach (var conv in sitePackage.HeaderInfo.Convertors)
            {
                var siteModel = new SiteModel(
                    context: context,
                    siteId: conv.SavedSiteId.ToLong());
                switch (siteModel.ReferenceType)
                {
                    case "Wikis":
                        var wikiModel = new WikiModel(
                            context: context,
                            ss: siteModel.SiteSettings)
                        {
                            SiteId = siteModel.SiteId,
                            Title = siteModel.Title,
                            Body = siteModel.Body,
                            Comments = siteModel.Comments
                        };
                        wikiModel.Create(
                            context: context,
                            ss: siteModel.SiteSettings);
                        var wikiId = Repository.ExecuteScalar_long(
                            context: context,
                            statements: Rds.SelectWikis(
                                top: 1,
                                column: Rds.WikisColumn().WikiId(),
                                where: Rds.WikisWhere().SiteId(siteModel.SiteId)));
                        var srcWikiId = sitePackage.Sites?.FirstOrDefault(o => o.SiteId == conv.SiteId)?.WikiId;
                        if (srcWikiId != null) idHash.Add(srcWikiId.ToLong(), wikiId);
                        break;
                    default:
                        Search.Indexes.Create(
                            context: context,
                            ss: siteModel.SiteSettings,
                            id: siteModel.SiteId);
                        break;
                }
            }
            SiteInfo.Refresh(context: context);
            int dataCount = 0;
            if (includeData)
            {
                if (sitePackage.Data.Any())
                {
                    ImportItems(
                        context: context,
                        sitePackage: sitePackage,
                        idHash: idHash);
                    sitePackage.ConvertDataId(
                        context: context,
                        idHash: idHash);
                    dataCount = ImportData(
                        context: context,
                        sitePackage: sitePackage,
                        idHash: idHash);
                }
            }
            if (sitePackage.HeaderInfo.Convertors.Any())
            {
                foreach (var conv in sitePackage.HeaderInfo.Convertors)
                {
                    if (conv.ReferenceType == "Sites")
                    {
                        if ((!conv.Order.IsNullOrEmpty() && conv.Order.Equals("[]") == false))
                        {
                            var newOrders = new List<long>();
                            var orders = conv.Order.Deserialize<List<long>>()?.ToList()
                                ?? new List<long>();
                            orders.ForEach(e => newOrders.Add(idHash.Get(e)));
                            if (newOrders.Any())
                            {
                                new OrderModel()
                                {
                                    ReferenceId = conv.SavedSiteId.ToLong(),
                                    ReferenceType = "Sites",
                                    OwnerId = 0,
                                    Data = newOrders
                                }.UpdateOrCreate(
                                    context: context,
                                    ss: ss);
                            }
                        }
                    }
                    conv.Updated |= ConvertScriptDataId(
                        ss: conv.SiteSettings,
                        idHash: idHash);
                    if (conv.Updated)
                    {
                        conv.SiteSettings.Init(context: context);
                        Repository.ExecuteNonQuery(
                            context: context,
                            statements: Rds.UpdateSites(
                                where: Rds.SitesWhere()
                                    .TenantId(context.TenantId)
                                    .SiteId(conv.SavedSiteId),
                                param: Rds.SitesParam()
                                    .SiteSettings(conv.SiteSettings.RecordingJson(
                                        context: context))));
                    }
                }
                var response = Repository.ExecuteScalar_response(
                    context: context,
                    transactional: true);
            }
            if (sitePackage.Permissions.Any())
            {
                foreach (var conv in sitePackage.HeaderInfo.Convertors)
                {
                    var packageSiteModel = sitePackage.Sites
                        .Where(e => e.SiteId == conv.SiteId)
                        .FirstOrDefault();
                    var packagePermissionModel = sitePackage.Permissions
                        .Where(e => e.SiteId == conv.SiteId)
                        .FirstOrDefault();
                    if (packagePermissionModel == null)
                    {
                        continue;
                    }
                    foreach (var permissionShortModel in packagePermissionModel.Permissions)
                    {
                        if (includeSitePermission == false)
                        {
                            if (permissionShortModel.ReferenceId == packagePermissionModel.SiteId)
                            {
                                continue;
                            }
                        }
                        if ((includeRecordPermission == false) || (dataCount == 0))
                        {
                            if (permissionShortModel.ReferenceId != packagePermissionModel.SiteId)
                            {
                                continue;
                            }
                        }
                        var referenceId = idHash.Get(permissionShortModel.ReferenceId);
                        if (referenceId > 0)
                        {
                            var idConverter = new IdConverter(
                                context: context,
                                siteId: packageSiteModel.SavedSiteId,
                                permissionShortModel: permissionShortModel,
                                permissionIdList: sitePackage.PermissionIdList,
                                convertSiteId: referenceId);
                            var exists = Rds.ExecuteScalar_int(
                                context: context,
                                statements: Rds.SelectPermissions(
                                    column: Rds.PermissionsColumn().ReferenceId(),
                                    where: Rds.PermissionsWhere()
                                        .ReferenceId(referenceId)
                                        .DeptId(idConverter.ConvertDeptId)
                                        .GroupId(idConverter.ConvertGroupId)
                                        .UserId(idConverter.ConvertUserId))) > 0;
                            if (idConverter.Convert == true && !exists)
                            {
                                Repository.ExecuteNonQuery(
                                    context: context,
                                    transactional: true,
                                    statements: Rds.InsertPermissions(
                                        param: Rds.PermissionsParam()
                                            .ReferenceId(referenceId)
                                            .DeptId(idConverter.ConvertDeptId)
                                            .GroupId(idConverter.ConvertGroupId)
                                            .UserId(idConverter.ConvertUserId)
                                            .PermissionType(permissionShortModel.PermissionType)));
                            }
                        }
                    }
                }
            }
            Repository.ExecuteNonQuery(
                context: context,
                statements: StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.UsersUpdated));
            SiteInfo.Refresh(context: context);
            if (apiData == null)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.SitePackageImported(
                        context: context,
                        data: new string[]
                        {
                            sitePackage.HeaderInfo.Convertors.Count().ToString(),
                            dataCount.ToString()
                        }));
                return new ResponseCollection(context: context)
                    .Href(url: Locations.ItemIndex(
                        context: context,
                        id: ss.SiteId))
                    .ToJson();
            }
            else
            {
                return sitePackage.HeaderInfo.Convertors
                    .Select(o => new
                    {
                        OldSiteId = o.SiteId,
                        NewSiteId = o.SavedSiteId,
                        ReferenceType = o.ReferenceType,
                        Title = o.SiteTitle
                    })
                    .ToJson();
            }
        }

        private static bool ConvertScriptDataId(
            SiteSettings ss,
            Dictionary<long, long> idHash)
        {
            var isUpdated = false;
            ss?.Scripts?.ForEach(script =>
            {
                var newScript = ConvertScriptDataId(
                    text: script.Body,
                    idHash: idHash);
                if (newScript != script.Body)
                {
                    isUpdated = true;
                    script.Body = newScript;
                }
            });
            ss?.ServerScripts?.ForEach(script =>
            {
                var newScript = ConvertScriptDataId(
                    text: script.Body,
                    idHash: idHash);
                if (newScript != script.Body)
                {
                    isUpdated = true;
                    script.Body = newScript;
                }
            });
            return isUpdated;
        }

        private static string ConvertScriptDataId(
            string text,
            Dictionary<long, long> idHash)
        {
            var regexSection = new Regex(@"^(?<pre>// @siteid list start@.*\r?)$(?<code>[\s\S]*?)^(?<post>// @siteid list end@)", RegexOptions.Multiline);
            var regexId = new Regex(@"\b(?<!\.)\d+(?!\.)\b");
            return regexSection.Replace(
                text,
                new MatchEvaluator((Match matchSection) =>
                    !matchSection.Groups.ContainsKey("code")
                        ? matchSection.Value
                        : matchSection.Groups["pre"].Value
                            + regexId.Replace(
                                matchSection.Groups["code"].Value,
                                new MatchEvaluator((Match matchId) =>
                                    (long.TryParse(matchId.Value, out var id)
                                        && idHash.ContainsKey(id))
                                        ? idHash[id].ToString()
                                        : matchId.Value))
                            + matchSection.Groups["post"].Value));
        }

        public static SitePackage GetSitePackageFromPostedFile(Context context)
        {
            var buffer = context.PostedFiles.FirstOrDefault()?.Byte();
            if (buffer == null) return null;
            using (var ms = new System.IO.MemoryStream(buffer, false))
            using (var sr = new System.IO.StreamReader(ms, Encoding.UTF8))
            using (var reader = new JsonTextReader(sr))
            {
                try
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<SitePackage>(reader);
                }
                catch
                {
                    return null;
                }
            }
        }

        private static void ImportItems(Context context,
            Dictionary<long, long> idHash,
            SitePackage sitePackage)
        {
            long savedReferenceId;
            sitePackage.Data.ForEach(jsonExport =>
            {
                jsonExport.Body.ForEach(body =>
                {
                    switch (body.GetReferenceType())
                    {
                        case "Issues":
                            var issuesExportModel = body as IssueExportModel;
                            savedReferenceId = Repository.ExecuteScalar_response(
                                context: context,
                                selectIdentity: true,
                                statements: Rds.InsertItems(
                                    selectIdentity: true,
                                    param: Rds.ItemsParam()
                                        .SiteId(idHash.Get(issuesExportModel.SiteId.ToLong()))
                                        .ReferenceType("Issues")))
                                            .Id.ToLong();
                            idHash.Add(issuesExportModel.IssueId.ToLong(), savedReferenceId);
                            break;
                        case "Results":
                            var resultExportModel = body as ResultExportModel;
                            savedReferenceId = Repository.ExecuteScalar_response(
                                context: context,
                                selectIdentity: true,
                                statements: Rds.InsertItems(
                                    selectIdentity: true,
                                    param: Rds.ItemsParam()
                                        .SiteId(idHash.Get(resultExportModel.SiteId.ToLong()))
                                        .ReferenceType("Results")))
                                            .Id.ToLong();
                            idHash.Add(resultExportModel.ResultId.ToLong(), savedReferenceId);
                            break;
                    }
                });

            });
        }

        private static int ImportData(
            Context context,
            SitePackage sitePackage,
            Dictionary<long, long> idHash)
        {
            var count = 0;
            sitePackage.Data.ForEach(jsonExport =>
            {
                if (jsonExport.Body.Any())
                {
                    var m = jsonExport.Body[0];
                    switch (m.GetReferenceType())
                    {
                        case "Issues":
                            {
                                var model = jsonExport.Body[0] as IssueExportModel;
                                var siteId = idHash.Get(model.SiteId.ToLong());
                                var referenceId = idHash.Get(model.IssueId.ToLong());
                                var ss = new SiteModel(
                                    context: context,
                                    siteId: siteId)
                                        .IssuesSiteSettings(
                                            context: context,
                                            referenceId: referenceId);
                                ss.SetChoiceHash(context: context);
                                jsonExport.Body.ForEach(body =>
                                {
                                    ImportIssues(
                                        context: context,
                                        ss: ss,
                                        permissionIdList: sitePackage.PermissionIdList,
                                        idHash: idHash,
                                        exportModel: body);
                                    count++;
                                });
                            }
                            break;
                        case "Results":
                            {
                                var model = jsonExport.Body[0] as ResultExportModel;
                                var siteId = idHash.Get(model.SiteId.ToLong());
                                var referenceId = idHash.Get(model.ResultId.ToLong());
                                var ss = new SiteModel(
                                    context: context,
                                    siteId: siteId)
                                        .ResultsSiteSettings(
                                            context: context,
                                            referenceId: referenceId);
                                ss.SetChoiceHash(context: context);
                                jsonExport.Body.ForEach(body =>
                                {
                                    ImportResults(
                                        context: context,
                                        ss: ss,
                                        permissionIdList: sitePackage.PermissionIdList,
                                        idHash: idHash,
                                        exportModel: body);
                                    count++;
                                });
                            }
                            break;
                    }
                }
            });
            return count;
        }

        private static void ImportIssues(
            Context context,
            SiteSettings ss,
            PermissionIdList permissionIdList,
            Dictionary<long, long> idHash,
            IExportModel exportModel)
        {
            var model = exportModel as IssueExportModel;
            var referenceId = idHash.Get(model.IssueId.ToLong());
            var param = Rds.IssuesParam()
                .SiteId(ss.SiteId)
                .IssueId(referenceId)
                .Title(model.Title?.ToString() ?? string.Empty)
                .Body(model.Body, _using: model.Body != null)
                .StartTime(model.StartTime, _using: model.StartTime != null)
                .CompletionTime(value: model.CompletionTime?.Value ?? new CompletionTime(
                    context: context,
                    ss: ss,
                    value: ss.GetColumn(
                        context: context,
                        columnName: "CompletionTime").DefaultTime(context: context),
                    status: model.Status).Value)
                .WorkValue(value: model.WorkValue?.Value, _using: model.WorkValue != null)
                .ProgressRate(value: model.ProgressRate?.Value, _using: model.ProgressRate != null)
                .Status(model.Status?.Value ?? ss.ColumnHash["Status"].DefaultInput.ToInt())
                .Manager(IdConvertUtilities.ConvertedUserId(
                    context: context,
                    permissionIdList: permissionIdList,
                    userId: model.Manager?.Id ?? 0),
                        _using: model.Manager != null)
                .Owner(IdConvertUtilities.ConvertedUserId(
                    context: context,
                    permissionIdList: permissionIdList,
                    userId: model.Owner?.Id ?? 0),
                        _using: model.Owner != null)
                .Comments(model.Comments?.ToJson(), _using: model.Comments?.Any() == true);
            model.ClassHash
                ?.Where(o => ss.ColumnDefinitionHash.ContainsKey(o.Key))
                .Where(o => o.Value != null)
                .ForEach(o =>
                    param.Add(
                        columnBracket: $"\"{o.Key}\"",
                        name: o.Key,
                        value: ConvertClassValue(
                            context: context,
                            ss: ss,
                            permissionIdList: permissionIdList,
                            columnName: o.Key,
                            value: o.Value.ToString()).MaxLength(1024)));
            model.NumHash
                ?.Where(o => ss.ColumnDefinitionHash.ContainsKey(o.Key))
                .ForEach(o =>
                    param.Add(
                        columnBracket: $"\"{o.Key}\"",
                        name: o.Key,
                        value: (object)o.Value ?? DBNull.Value));
            model.DateHash
                ?.Where(o => ss.ColumnDefinitionHash.ContainsKey(o.Key))
                .ForEach(o =>
                    param.Add(
                        columnBracket: $"\"{o.Key}\"",
                        name: o.Key,
                        value: o.Value));
            model.DescriptionHash
                ?.Where(o => ss.ColumnDefinitionHash.ContainsKey(o.Key))
                .ForEach(o =>
                    param.Add(
                        columnBracket: $"\"{o.Key}\"",
                        name: o.Key,
                        value: o.Value));
            model.CheckHash
                ?.Where(o => ss.ColumnDefinitionHash.ContainsKey(o.Key))
                .ForEach(o =>
                    param.Add(
                        columnBracket: $"\"{o.Key}\"",
                        name: o.Key,
                        value: o.Value));
            Repository.ExecuteScalar_response(
                context: context,
                selectIdentity: false,
                statements: Rds.InsertIssues(param: param));
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: referenceId);
            issueModel.UpdateRelatedRecords(
                context: context,
                ss: ss,
                extendedSqls: false,
                updateItems: true);
        }

        private static void ImportResults(
            Context context,
            SiteSettings ss,
            PermissionIdList permissionIdList,
            Dictionary<long, long> idHash,
            IExportModel exportModel)
        {
            var model = exportModel as ResultExportModel;
            var referenceId = idHash.Get(model.ResultId.ToLong());
            var param = Rds.ResultsParam()
                .SiteId(ss.SiteId)
                .ResultId(referenceId)
                .Title(model.Title?.ToString(), _using: model.Title != null)
                .Body(model.Body, _using: model.Body != null)
                .Status(model.Status?.Value, _using: model.Status != null)
                .Manager(IdConvertUtilities.ConvertedUserId(
                    context: context,
                    permissionIdList: permissionIdList,
                    userId: model.Manager?.Id ?? 0),
                        _using: model.Manager != null)
                .Owner(IdConvertUtilities.ConvertedUserId(
                    context: context,
                    permissionIdList: permissionIdList,
                    userId: model.Owner?.Id ?? 0),
                        _using: model.Owner != null)
                .Comments(model.Comments?.ToJson(), _using: model.Comments?.Any() == true);
            model.ClassHash
                ?.Where(o => ss.ColumnDefinitionHash.ContainsKey(o.Key))
                .Where(o => o.Value != null)
                .ForEach(o =>
                    param.Add(
                        columnBracket: $"\"{o.Key}\"",
                        name: o.Key,
                        value: ConvertClassValue(
                            context: context,
                            ss: ss,
                            permissionIdList: permissionIdList,
                            columnName: o.Key,
                            value: o.Value.ToString()).MaxLength(1024)));
            model.NumHash
                ?.Where(o => ss.ColumnDefinitionHash.ContainsKey(o.Key))
                .ForEach(o =>
                    param.Add(
                        columnBracket: $"\"{o.Key}\"",
                        name: o.Key,
                        value: (object)o.Value ?? DBNull.Value));
            model.DateHash
                ?.Where(o => ss.ColumnDefinitionHash.ContainsKey(o.Key))
                .ForEach(o =>
                    param.Add(
                        columnBracket: $"\"{o.Key}\"",
                        name: o.Key,
                        value: o.Value));
            model.DescriptionHash
                ?.Where(o => ss.ColumnDefinitionHash.ContainsKey(o.Key))
                .ForEach(o =>
                    param.Add(
                        columnBracket: $"\"{o.Key}\"",
                        name: o.Key,
                        value: o.Value));
            model.CheckHash
                ?.Where(o => ss.ColumnDefinitionHash.ContainsKey(o.Key))
                .ForEach(o =>
                    param.Add(
                        columnBracket: $"\"{o.Key}\"",
                        name: o.Key,
                        value: o.Value));
            Repository.ExecuteScalar_response(
                context: context,
                selectIdentity: false,
                statements: Rds.InsertResults(param: param));
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: referenceId);
            resultModel.UpdateRelatedRecords(
                context: context,
                ss: ss,
                extendedSqls: false,
                updateItems: true);
        }

        private static string ConvertClassValue(
            Context context,
            SiteSettings ss,
            PermissionIdList permissionIdList,
            string columnName,
            string value)
        {
            var column = ss.GetColumn(
                context: context,
                columnName: columnName);
            switch (column?.Type)
            {
                case Column.Types.Dept:
                    return IdConvertUtilities.ConvertedDeptId(
                        context: context,
                        permissionIdList: permissionIdList,
                        deptId: value.ToInt()).ToString();
                case Column.Types.Group:
                    return IdConvertUtilities.ConvertedGroupId(
                        context: context,
                        permissionIdList: permissionIdList,
                        groupId: value.ToInt()).ToString();
                case Column.Types.User:
                    return IdConvertUtilities.ConvertedUserId(
                        context: context,
                        permissionIdList: permissionIdList,
                        userId: value.ToInt()).ToString();
                default:
                    return value;
            }
        }

        public static string OpenExportSitePackageDialog(
            Context context,
            SiteSettings ss,
            bool recursive = false)
        {
            if (!Parameters.SitePackage.Export
                || !context.CanManageSite(ss: ss))
            {
                return Messages.ResponseInvalidRequest(context: context).ToJson();
            }
            return new ResponseCollection(context: context)
                .Html(
                    "#ExportSitePackageDialog",
                    new HtmlBuilder().ExportSitePackageDialog(
                        context: context,
                        ss: ss,
                        recursive: recursive))
                .ToJson();
        }

        internal static Dictionary<string, ControlData> SitePackageSelectableOptions(
            Context context,
            long siteId,
            Dictionary<string, ControlData> listItemCollection = null,
            int depts = 0,
            bool recursive = false)
        {
            if (listItemCollection == null)
            {
                listItemCollection = new Dictionary<string, ControlData>();
                listItemCollection.Add(
                    siteId.ToString() + (Parameters.SitePackage.IncludeDataOnExport == OptionTypes.On
                        ? "-true"
                        : "-false"),
                    new ControlData(
                        text: $"[ {context.SiteTitle} ]",
                        order: listItemCollection.Count + 1));
            }
            if (!recursive)
            {
                return listItemCollection;
            }
            var dataRaws = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .SiteId()
                        .Title()
                        .ReferenceType(),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .ParentId(siteId)))
                            .AsEnumerable();
            dataRaws.ForEach(dataRow =>
            {
                var name = new string('-', (depts + 1) * 2) + "> " + $"[ {dataRow.String("Title")} ]";
                listItemCollection.Add(
                    dataRow.String("SiteId") + (Parameters.SitePackage.IncludeDataOnExport == OptionTypes.On
                        ? "-true"
                        : "-false"),
                    new ControlData(name, title: dataRow.String("ReferenceType"),
                        order: listItemCollection.Count + 1));
                if (dataRow.String("ReferenceType") == "Sites")
                {
                    listItemCollection = SitePackageSelectableOptions(
                        context: context,
                        siteId: dataRow.Long("SiteId"),
                        listItemCollection: listItemCollection,
                        depts: depts + 1,
                        recursive: recursive);
                }
            });
            return listItemCollection;
        }

        public static ResponseFile ExportSitePackage(Context context, SiteSettings ss)
        {
            var sitePackage = GetSitePackage(
                context: context,
                ss: ss);
            if (sitePackage == null)
            {
                return null;
            }
            var useIndentOption = Parameters.SitePackage.UseIndentOptionOnExport != OptionTypes.Disabled
                && context.QueryStrings.Bool("UseIndentOption");
            var file = new ResponseFile(
                fileContent: sitePackage.RecordingJson(
                    context: context,
                    formatting: useIndentOption
                        ? Formatting.Indented
                        : Formatting.None),
                fileDownloadName: ExportUtilities.FileName(
                    context: context,
                    sitePackage.Sites.FirstOrDefault()?.Title,
                    extension: "json"));
            return file;
        }

        public static SitePackage GetSitePackage(Context context, SiteSettings ss, SitePackageApiModel apiData = null)
        {
            if (!Parameters.SitePackage.Export
                || !context.CanManageSite(ss: ss))
            {
                return null;
            }
            var includeSitePermission = Parameters.SitePackage.IncludeSitePermissionOnExport != OptionTypes.Disabled
                && (apiData?.IncludeSitePermission
                    ?? context.QueryStrings.Bool("IncludeSitePermission"));
            var includeRecordPermission = Parameters.SitePackage.IncludeRecordPermissionOnExport != OptionTypes.Disabled
                && (apiData?.IncludeRecordPermission
                    ?? context.QueryStrings.Bool("IncludeRecordPermission"));
            var includeColumnPermission = Parameters.SitePackage.IncludeColumnPermissionOnExport != OptionTypes.Disabled
                && (apiData?.IncludeColumnPermission
                    ?? context.QueryStrings.Bool("IncludeColumnPermission"));
            var includeNotifications = Parameters.SitePackage.IncludeNotificationsOnExport != OptionTypes.Disabled
                && (apiData?.IncludeNotifications
                    ?? context.QueryStrings.Bool("IncludeNotifications"));
            var includeReminders = Parameters.SitePackage.IncludeRemindersOnExport != OptionTypes.Disabled
                && (apiData?.IncludeReminders
                    ?? context.QueryStrings.Bool("IncludeReminders"));
            var sites = new List<SelectedSite>();
            if (apiData == null)
            {
                string sitePackagesSelectableAll = Regex.Replace(
                    context.QueryStrings.Data("SitePackagesSelectableAll"),
                    @"[^0-9-,(true|false)]", "");
                sitePackagesSelectableAll.Split(',')
                    .ForEach(e =>
                        sites.Add(new SelectedSite()
                        {
                            SiteId = e.Split_1st('-').ToLong(),
                            IncludeData = e.Split_2nd('-').ToBool()
                        }));
            }
            else
            {
                sites = apiData?.SelectedSites;
            }
            if (ExceededExportLimit(
                context: context,
                sites: sites))
            {
                return null;
            }
            foreach (var site in sites)
            {
                var currentSs = ss.SiteId == site.SiteId
                    ? ss
                    : SiteSettingsUtilities.Get(
                        context: context,
                        siteId: site.SiteId);
                if (site.IncludeData
                    && (context.ContractSettings.Export == false
                        || !context.CanExport(ss: currentSs)))
                {
                    site.IncludeData = false;
                }
            }
            return new SitePackage(
                context: context,
                siteList: sites,
                includeSitePermission: includeSitePermission,
                includeRecordPermission: includeRecordPermission,
                includeColumnPermission: includeColumnPermission,
                includeNotifications: includeNotifications,
                includeReminders: includeReminders);
        }

        public static bool ExceededExportLimit(Context context, List<SelectedSite> sites)
        {
            var includeDataSites = sites
                .Where(o => o.IncludeData)
                .Select(o => o.SiteId)
                .ToList();
            return Parameters.SitePackage.ExportLimit > 0
                && includeDataSites.Any()
                    ? Repository.ExecuteScalar_long(
                        context: context,
                        statements: Rds.SelectItems(
                        column: Rds.ItemsColumn().ItemsCount(),
                        where: Rds.ItemsWhere()
                            .SiteId_In(includeDataSites)
                            .Or(or: Rds.ItemsWhere()
                                .ReferenceType(raw: "'Issues'")
                                .ReferenceType(raw: "'Results'"))))
                                    > Parameters.SitePackage.ExportLimit
                    : false;
        }

        public static View GetView(
            Context context,
            SiteSettings ss,
            Export export)
        {
            var view = new View(context: context, ss: ss)
            {
                GridColumns = export.Columns
                    .Where(o => o.Column.CanRead(
                        context: context,
                        ss: ss,
                        mine: null))
                    .Select(o => o.ColumnName)
                    .ToList()
            };
            return view;
        }
    }
}