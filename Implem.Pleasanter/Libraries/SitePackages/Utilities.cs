using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            return new ResponseCollection()
                .Html(
                    "#ImportSitePackageDialog",
                    new HtmlBuilder().ImportSitePackageDialog(
                        context: context,
                        ss: ss))
                .ToJson();
        }

        public static string ImportSitePackage(Context context, SiteSettings ss)
        {
            var includeData = context.Forms.Bool("IncludeData");
            var includeSitePermission = context.Forms.Bool("IncludeSitePermission");
            var includeRecordPermission = context.Forms.Bool("IncludeRecordPermission");
            var includeColumnPermission = context.Forms.Bool("IncludeColumnPermission");
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
            var serializer = new JsonSerializer();
            using (var ms = new System.IO.MemoryStream(
                buffer: context.PostedFiles.FirstOrDefault().Byte(),
                writable: false))
            using (var sr = new System.IO.StreamReader(ms, Encoding.UTF8))
            using (var reader = new JsonTextReader(sr))
            {
                var sitePackage = serializer.Deserialize<SitePackage>(reader);
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
                sitePackage.HeaderInfo.SavedBaseSiteId = context.SiteId;
                sitePackage.HeaderInfo.SavedInheritPermission = ss.InheritPermission;
                foreach (var conv in sitePackage.HeaderInfo.Convertors)
                {
                    var response = Rds.ExecuteScalar_response(
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
                        permissionIdList: sitePackage.PermissionIdList);
                    Rds.ExecuteScalar_response(
                        context: context,
                        transactional: true,
                        statements: new SqlStatement[]
                        {
                            Rds.InsertSites(param: Rds.SitesParam()
                                .SiteId(packageSiteModel.SavedSiteId)
                                .TenantId(packageSiteModel.SavedTenantId)
                                .Title(packageSiteModel.Title)
                                .Body(packageSiteModel.Body)
                                .GridGuide(packageSiteModel.GridGuide)
                                .EditorGuide(packageSiteModel.EditorGuide)
                                .ReferenceType(packageSiteModel.ReferenceType.MaxLength(32))
                                .ParentId(packageSiteModel.SavedParentId)
                                .InheritPermission(packageSiteModel.SavedInheritPermission)
                                .SiteSettings(packageSiteModel.SiteSettings.ToJson())
                                .Publish(packageSiteModel.Publish)
                                .Comments(packageSiteModel.Comments.ToJson())),
                            Rds.PhysicalDeleteLinks(
                                where: Rds.LinksWhere().SourceId(packageSiteModel.SavedSiteId)),
                            LinkUtilities.Insert(link: packageSiteModel.SiteSettings.Links?
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
                    Rds.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: Rds.UpdateItems(
                            where: Rds.ItemsWhere()
                                .ReferenceId(packageSiteModel.SavedSiteId),
                            param: Rds.ItemsParam()
                                .FullText(fullText, _using: fullText != null)
                                .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null)));
                }
                var idHash = sitePackage.GetIdHashFromConverters();
                foreach (long savedSiteId in sitePackage.HeaderInfo.Convertors.Select(e => e.SavedSiteId))
                {
                    var siteModel = new SiteModel(
                        context: context,
                        siteId: savedSiteId);
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
                            var wikiId = Rds.ExecuteScalar_long(
                                context: context,
                                statements: Rds.SelectWikis(
                                    top: 1,
                                    column: Rds.WikisColumn().WikiId(),
                                    where: Rds.WikisWhere().SiteId(siteModel.SiteId)));
                            idHash.Add(savedSiteId, wikiId);
                            break;
                        default:
                            Search.Indexes.Create(
                                context: context,
                                ss: siteModel.SiteSettings,
                                id: siteModel.SiteId);
                            break;
                    }
                }
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
                                    }.UpdateOrCreate(context: context);
                                }
                            }
                        }
                    }
                    var response = Rds.ExecuteScalar_response(
                    context: context,
                    transactional: true,
                    statements: StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.SitesUpdated));
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
                            var idConverter = new IdConverter(
                                context: context,
                                siteId: packageSiteModel.SavedSiteId,
                                permissionShortModel: permissionShortModel,
                                permissionIdList: sitePackage.PermissionIdList,
                                convertSiteId: idHash[permissionShortModel.ReferenceId]);
                            if (idConverter.Convert == true)
                            {
                                Rds.ExecuteNonQuery(
                                    context: context,
                                    transactional: true,
                                    statements: Rds.InsertPermissions(
                                        param: Rds.PermissionsParam()
                                            .ReferenceId(idHash[permissionShortModel.ReferenceId])
                                            .DeptId(idConverter.ConvertDeptId)
                                            .GroupId(idConverter.ConvertGroupId)
                                            .UserId(idConverter.ConvertUserId)
                                            .PermissionType(permissionShortModel.PermissionType)));
                            }
                        }
                    }
                }
                Rds.ExecuteNonQuery(
                    context: context,
                    statements: StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.UsersUpdated));
                SessionUtilities.Set(
                    context: context,
                    message: Messages.SitePackageImported(
                        context: context,
                        data: new string[]
                        {
                            sitePackage.HeaderInfo.Convertors.Count().ToString(),
                            dataCount.ToString()
                        }));
                return new ResponseCollection()
                    .Href(url: Locations.ItemIndex(
                        context: context,
                        id: ss.SiteId))
                    .ToJson();
            }
        }

        private static void ImportItems(Context context,
            IDictionary<long, long> idHash,
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
                            savedReferenceId = Rds.ExecuteScalar_response(
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
                            savedReferenceId = Rds.ExecuteScalar_response(
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

        private static int ImportData(Context context, SitePackage sitePackage, Dictionary<long, long> idHash)
        {
            var count = 0;
            sitePackage.Data.ForEach(jsonExport =>
            {
                jsonExport.Body.ForEach(body =>
                {
                    switch (body.GetReferenceType())
                    {
                        case "Issues":
                            ImportIssues(
                                context: context,
                                idHash: idHash,
                                exportModel: body);
                            count++;
                            break;
                        case "Results":
                            ImportResults(
                                context: context,
                                idHash: idHash,
                                exportModel: body);
                            count++;
                            break;
                    }
                });
            });
            return count;
        }

        private static void ImportIssues(Context context, IDictionary<long, long> idHash, IExportModel exportModel)
        {
            var model = exportModel as IssueExportModel;
            var siteId = idHash.Get(model.SiteId.ToLong());
            var referenceId = idHash.Get(model.IssueId.ToLong());
            var param = Rds.IssuesParam()
                .SiteId(siteId)
                .IssueId(referenceId)
                .Title(model.Title?.ToString() ?? string.Empty)
                .Body(model.Body, _using: model.Body != null)
                .StartTime(model.StartTime, _using: model.StartTime != null)
                .CompletionTime(value: model.CompletionTime?.Value, _using: model.CompletionTime != null)
                .WorkValue(value: model.WorkValue?.Value, _using: model.WorkValue != null)
                .ProgressRate(value: model.ProgressRate?.Value, _using: model.ProgressRate != null)
                .Status(model.Status?.Value, _using: model.Status != null)
                .Manager(model.Manager?.Id, _using: model.Manager != null)
                .Owner(model.Owner?.Id, _using: model.Owner != null)
                .Comments(model.Comments?.ToJson(), _using: model.Comments?.Any() == true);
            model.ClassHash?.ForEach(o =>
                param.Add(
                    columnBracket: o.Key,
                    name: o.Key,
                    value: o.Value.ToString().MaxLength(1024)));
            model.NumHash?.ForEach(o =>
                param.Add(
                    columnBracket: o.Key,
                    name: o.Key,
                    value: o.Value));
            model.DateHash?.ForEach(o =>
                param.Add(
                    columnBracket: o.Key,
                    name: o.Key,
                    value: o.Value));
            model.DescriptionHash?.ForEach(o =>
                param.Add(
                    columnBracket: o.Key,
                    name: o.Key,
                    value: o.Value));
            model.CheckHash?.ForEach(o =>
                param.Add(
                    columnBracket: o.Key,
                    name: o.Key,
                    value: o.Value));
            Rds.ExecuteScalar_response(
                context: context,
                selectIdentity: false,
                statements: Rds.InsertIssues(param: param));
            var ss = new SiteModel(
                context: context,
                siteId: siteId)
                    .IssuesSiteSettings(
                        context: context,
                        referenceId: referenceId);
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

        private static void ImportResults(Context context, Dictionary<long, long> idHash, IExportModel exportModel)
        {
            var model = exportModel as ResultExportModel;
            var siteId = idHash.Get(model.SiteId.ToLong());
            var referenceId = idHash.Get(model.ResultId.ToLong());
            var param = Rds.ResultsParam()
                .SiteId(siteId)
                .ResultId(referenceId)
                .Title(model.Title?.ToString(), _using: model.Title != null)
                .Body(model.Body, _using: model.Body != null)
                .Status(model.Status?.Value, _using: model.Status != null)
                .Manager(model.Manager?.Id, _using: model.Manager != null)
                .Owner(model.Owner?.Id, _using: model.Owner != null)
                .Comments(model.Comments?.ToJson(), _using: model.Comments?.Any() == true);
            model.ClassHash?.ForEach(o =>
                param.Add(
                    columnBracket: o.Key,
                    name: o.Key,
                    value: o.Value.ToString().MaxLength(1024)));
            model.NumHash?.ForEach(o =>
                param.Add(
                    columnBracket: o.Key,
                    name: o.Key,
                    value: o.Value));
            model.DateHash?.ForEach(o =>
                param.Add(
                    columnBracket: o.Key,
                    name: o.Key,
                    value: o.Value));
            model.DescriptionHash?.ForEach(o =>
                param.Add(
                    columnBracket: o.Key,
                    name: o.Key,
                    value: o.Value));
            model.CheckHash?.ForEach(o =>
                param.Add(
                    columnBracket: o.Key,
                    name: o.Key,
                    value: o.Value));
            Rds.ExecuteScalar_response(
                context: context,
                selectIdentity: false,
                statements: Rds.InsertResults(param: param));
            var ss = new SiteModel(
                context: context,
                siteId: siteId)
                    .ResultsSiteSettings(
                        context: context,
                        referenceId: referenceId);
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
            return new ResponseCollection()
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
                    siteId.ToString() + "-false",
                    new ControlData(
                        text: $"[ {context.SiteTitle} ]",
                        order: listItemCollection.Count + 1));
            }
            if (!recursive)
            {
                return listItemCollection;
            }
            var dataRaws = Rds.ExecuteTable(
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
                    dataRow.String("SiteId") + "-false",
                    new ControlData(name, title: dataRow.String("ReferenceType"),
                        css: " ui-icon ui-icon-folder-open",
                        style: " ui-icon ui-icon-folder-collapsed",
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
            if (!Parameters.SitePackage.Export
                || !context.CanManageSite(ss: ss))
            {
                return null;
            }
            var useIndentOption = context.QueryStrings.Bool("UseIndentOption");
            var includeSitePermission = context.QueryStrings.Bool("IncludeSitePermission");
            var includeRecordPermission = context.QueryStrings.Bool("IncludeRecordPermission");
            var includeColumnPermission = context.QueryStrings.Bool("IncludeColumnPermission");
            string sitePackagesSelectableAll = Regex.Replace(
                context.QueryStrings.Data("SitePackagesSelectableAll"),
                @"[^0-9-,(true|false)]", "");
            var sites = new List<SelectedSite>();
            sitePackagesSelectableAll.Split(',')
                .ForEach(e =>
                    sites.Add(new SelectedSite()
                    {
                        SiteId = e.Split_1st('-').ToLong(),
                        IncludeData = e.Split_2nd('-').ToBool()
                    }));
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
                    return null;
                }
            }
            var sitePackage = new SitePackage(
                context: context,
                siteList: sites,
                includeSitePermission: includeSitePermission,
                includeRecordPermission: includeRecordPermission,
                includeColumnPermission: includeColumnPermission);
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

        public static bool ExceededExportLimit(Context context, List<SelectedSite> sites)
        {
            var includeDataSites = sites
                .Where(o => o.IncludeData)
                .Select(o => o.SiteId)
                .ToList();
            return Parameters.SitePackage.ExportLimit > 0
                && includeDataSites.Any()
                    ? Rds.ExecuteScalar_long(
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

        public static int ConvertedDeptId(
            Context context,
            PermissionIdList permissionIdList,
            int deptId)
        {
            var deptCode = permissionIdList?.DeptIdList
                .FirstOrDefault(x => x.DeptId == deptId)
                ?.DeptCode;
            return !deptCode.IsNullOrEmpty()
                ? Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectDepts(
                        column: Rds.DeptsColumn().DeptId(),
                        where: Rds.DeptsWhere()
                            .TenantId(context.TenantId)
                            .DeptCode(deptCode)))
                                .AsEnumerable()
                                .FirstOrDefault()
                                ?.Int("DeptId") ?? 0
                : 0;
        }

        public static int ConvertedGroupId(
            Context context,
            PermissionIdList permissionIdList,
            int groupId)
        {
            var groupName = permissionIdList?.GroupIdList
                .FirstOrDefault(x => x.GroupId == groupId)
                ?.GroupName;
            return !groupName.IsNullOrEmpty()
                ? Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectGroups(
                        column: Rds.GroupsColumn().GroupId(),
                        where: Rds.GroupsWhere()
                            .TenantId(context.TenantId)
                            .GroupName(groupName)))
                                .AsEnumerable()
                                .FirstOrDefault()
                                ?.Int("GroupId") ?? 0
                : 0;
        }

        public static int ConvertedUserId(
            Context context,
            PermissionIdList permissionIdList,
            int userId)
        {
            var loginId = permissionIdList?.UserIdList
                .FirstOrDefault(x => x.UserId == userId)
                ?.LoginId;
            return !loginId.IsNullOrEmpty()
                ? Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UserId(),
                        where: Rds.UsersWhere()
                            .TenantId(context.TenantId)
                            .LoginId(loginId)))
                                .AsEnumerable()
                                .FirstOrDefault()
                                ?.Int("UserId") ?? 0
                : 0;
        }

        public static GridData GetGridData(
            Context context,
            SiteSettings ss,
            Export export)
        {
            var view = new View(context: context, ss: ss)
            {
                GridColumns = export.Columns
                    .Where(o => o.Column.CanRead)
                    .Select(o => o.ColumnName)
                    .ToList()
            };
            ss.SetColumnAccessControls(context: context);
            var gridData = new GridData(
                context: context,
                ss: ss,
                view: view);
            return gridData;
        }
    }
}