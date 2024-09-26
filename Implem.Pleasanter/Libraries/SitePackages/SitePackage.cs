using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
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
    public class SitePackage
    {
        public Header HeaderInfo;
        public List<PackageSiteModel> Sites;
        public List<JsonExport> Data;
        public List<PackagePermissionModel> Permissions;
        public PermissionIdList PermissionIdList;

        public SitePackage()
        {
        }

        public SitePackage(
            Context context,
            List<SelectedSite> siteList,
            bool includeSitePermission,
            bool includeRecordPermission,
            bool includeColumnPermission,
            bool includeNotifications,
            bool includeReminders)
        {
            HeaderInfo = new Header(
                context: context,
                includeSitePermission: includeSitePermission,
                includeRecordPermission: includeRecordPermission,
                includeColumnPermission: includeColumnPermission,
                includeNotifications: includeNotifications,
                includeReminders: includeReminders);
            Sites = new List<PackageSiteModel>();
            Data = new List<JsonExport>();
            Permissions = new List<PackagePermissionModel>();
            Construct(
                context: context,
                siteList: siteList,
                includeSitePermission: includeSitePermission,
                includeRecordPermission: includeRecordPermission,
                includeColumnPermission: includeColumnPermission,
                includeNotifications: includeNotifications,
                includeReminders: includeReminders);
            PermissionIdList = new PermissionIdList(
                context: context,
                sites: Sites,
                packagePermissionModels: Permissions,
                includeSitePermission: includeSitePermission);
        }

        private void Construct(
            Context context,
            List<SelectedSite> siteList,
            bool includeSitePermission,
            bool includeRecordPermission,
            bool includeColumnPermission,
            bool includeNotifications,
            bool includeReminders)
        {
            var recordIdList = new List<long>();
            if ((siteList != null) && (siteList.Count > 0))
            {
                foreach (SelectedSite site in siteList)
                {
                    var siteModel = new SiteModel(
                        context: context,
                        siteId: site.SiteId);
                    var packageSiteModel = new PackageSiteModel(new SiteModel(
                        context: context,
                        siteId: site.SiteId));
                    if (packageSiteModel.ReferenceType == "Wikis")
                    {
                        var wikiId = Repository.ExecuteScalar_long(
                            context: context,
                            statements: Rds.SelectWikis(
                                top: 1,
                                column: Rds.WikisColumn().WikiId(),
                                where: Rds.WikisWhere().SiteId(siteModel.SiteId)));
                        var wikiModel = new WikiModel(
                            context: context,
                            ss: siteModel.SiteSettings,
                            wikiId: wikiId);
                        packageSiteModel.WikiId = wikiId;
                        packageSiteModel.Body = wikiModel.Body;
                    }
                    if (includeColumnPermission == false)
                    {
                        packageSiteModel.SiteSettings.CreateColumnAccessControls?.Clear();
                        packageSiteModel.SiteSettings.ReadColumnAccessControls?.Clear();
                        packageSiteModel.SiteSettings.UpdateColumnAccessControls?.Clear();
                    }
                    if (includeNotifications == false)
                    {
                        packageSiteModel.SiteSettings.Notifications?.Clear();
                        packageSiteModel.SiteSettings
                            .Processes?.ForEach(p => { p.Notifications?.Clear(); });
                    }
                    if (includeReminders == false)
                    {
                        packageSiteModel.SiteSettings.Reminders?.Clear();
                    }
                    Sites.Add(packageSiteModel);
                    string order = null;
                    if (siteModel.ReferenceType == "Sites")
                    {
                        var orderModel = new OrderModel(
                            context: context,
                            ss: siteModel.SiteSettings,
                            referenceId: siteModel.SiteId,
                            referenceType: "Sites");
                        order = orderModel.SavedData;
                    }
                    HeaderInfo.Add(
                        siteId: site.SiteId,
                        referenceType: packageSiteModel.ReferenceType,
                        includeData: site.IncludeData,
                        order: order,
                        siteTitle: packageSiteModel.Title);
                    recordIdList.Clear();
                    View view = null;
                    var ss = siteModel.SiteSettings;
                    ss.SetPermissions(
                        context: context,
                        referenceId: ss.SiteId);
                    var export = new Export(ss.DefaultExportColumns(context: context));
                    export.Header = true;
                    export.Type = Export.Types.Json;
                    export.Columns.ForEach(o => o.SiteId = ss.SiteId);
                    view = Utilities.GetView(
                        context: context,
                        ss: ss,
                        export: export);
                    if (site.IncludeData == true)
                    {
                        switch (packageSiteModel.ReferenceType)
                        {
                            case "Issues":
                            case "Results":
                                var gridData = new GridData(
                                    context: context,
                                    ss: ss,
                                    view: view);
                                if (gridData.TotalCount > 0)
                                {
                                    Data.Add(gridData.GetJsonExport(
                                        context: context,
                                        ss: ss,
                                        export: export));
                                }
                                break;
                        }
                    }
                    if (includeSitePermission || includeRecordPermission)
                    {
                        var packagePermissionModel = GetPackagePermissionModel(
                            context: context,
                            siteModel: siteModel,
                            view: view,
                            includeSitePermission: includeSitePermission,
                            includeRecordPermission: includeRecordPermission);
                        Permissions.Add(packagePermissionModel);
                    }
                }
            }
        }

        private static PackagePermissionModel GetPackagePermissionModel(
            Context context,
            SiteModel siteModel,
            View view,
            bool includeSitePermission,
            bool includeRecordPermission)
        {
            switch (siteModel.ReferenceType)
            {
                case "Dashboards":
                    includeRecordPermission = false;
                    break;
                default:
                    break;
            }
            return new PackagePermissionModel(
                context: context,
                siteModel: siteModel,
                view: view,
                includeSitePermission: includeSitePermission,
                includeRecordPermission: includeRecordPermission);
        }

        public class Header
        {
            public string AssemblyVersion;
            public long BaseSiteId;
            public string Server;
            public string CreatorName;
            public DateTime PackageTime;
            public List<Convertor> Convertors;
            public bool IncludeSitePermission;
            public bool IncludeRecordPermission;
            public bool IncludeColumnPermission;
            public bool IncludeNotifications;
            public bool IncludeReminders;
            [NonSerialized]
            public long? SavedBaseSiteId;
            [NonSerialized]
            public long? SavedInheritPermission;

            public Header()
            {
            }

            public Header(
                Context context,
                bool includeSitePermission,
                bool includeRecordPermission,
                bool includeColumnPermission,
                bool includeNotifications,
                bool includeReminders)
            {
                AssemblyVersion = Environments.AssemblyVersion;
                BaseSiteId = context.SiteId;
                Server = Strings.CoalesceEmpty(Parameters.Service.AbsoluteUri, context.Server);
                CreatorName = context.User.Name;
                PackageTime = DateTime.Now;
                Convertors = new List<Convertor>();
                IncludeSitePermission = includeSitePermission;
                IncludeRecordPermission = includeRecordPermission;
                IncludeColumnPermission = includeColumnPermission;
                IncludeNotifications = includeNotifications;
                IncludeReminders = includeReminders;
            }

            public class Convertor
            {
                public long SiteId;
                public string SiteTitle;
                public string ReferenceType;
                public bool IncludeData;
                public string Order;
                [NonSerialized]
                public long? SavedSiteId;
                [NonSerialized]
                public bool Updated;
                [NonSerialized]
                public SiteSettings SiteSettings;
            }

            internal void Add(
                long siteId,
                string siteTitle,
                string referenceType,
                bool includeData = false,
                string order = null)
            {
                Convertors.Add(new Convertor
                {
                    SiteId = siteId,
                    SiteTitle = siteTitle,
                    ReferenceType = referenceType,
                    IncludeData = includeData,
                    Order = order,
                    SavedSiteId = null,
                });
            }

            internal long GetConvertedId(
                long siteId,
                bool isInherit = false,
                bool isParentId = false)
            {
                long? newId;
                newId = Convertors
                    .Where(e => e.SiteId == siteId)
                    .Select(e => e.SavedSiteId)
                    .FirstOrDefault();
                if (isParentId)
                {
                    if ((siteId == 0) || (newId == null)) newId = SavedBaseSiteId;
                }
                if (isInherit) return SavedInheritPermission.ToLong();
                if (newId == null) newId = siteId;
                return newId.ToLong();
            }
        }

        public string RecordingJson(Context context, Formatting formatting = Formatting.None)
        {
            Sites.ForEach(siteModel =>
                siteModel.SiteSettings = siteModel.SiteSettings.RecordingData(context: context));
            return this.ToJson(formatting: formatting);
        }

        public void ConvertDataId(Context context, Dictionary<long, long> idHash)
        {
            foreach (var conv in HeaderInfo.Convertors)
            {
                var savedSiteId = conv.SavedSiteId.ToLong();
                var ss = conv.SiteSettings;
                ss?.Links
                    ?.Where(link => link.SiteId > 0)
                    .ForEach(link =>
                    {
                        switch (ss.ReferenceType)
                        {
                            case "Issues":
                            case "Results":
                                Data.ForEach(jsonExport =>
                                    jsonExport.Body
                                        .Where(e => e.SiteId == conv.SiteId)
                                        .ForEach(body =>
                                            body.ReplaceIdHash(
                                                columnName: link.ColumnName,
                                                idHash: idHash)));
                                break;
                        }
                    });
                ss?.Views?
                    .ForEach(view =>
                    {
                        conv.Updated |= ConvertFilterHashDataId(
                            ss: ss,
                            filterHash: view.ColumnFilterHash,
                            idHash: idHash); 
                    });
                ss?.StatusControls?
                    .ForEach(statusControl =>
                    {
                        conv.Updated |= ConvertFilterHashDataId(
                            ss: ss,
                            filterHash: statusControl.View?.ColumnFilterHash,
                            idHash: idHash);
                    });
                ss?.Processes?
                    .ForEach(process =>
                    {
                        conv.Updated |= ConvertFilterHashDataId(
                            ss: ss,
                            filterHash: process.View?.ColumnFilterHash,
                            idHash: idHash);
                    });
                ss?.DashboardParts?
                    .ForEach(part =>
                    {
                        conv.Updated |= ConvertFilterHashDataId(
                            ss: HeaderInfo.Convertors.FirstOrDefault(o => o.SavedSiteId == part.SiteId)?.SiteSettings,
                            filterHash: part.View?.ColumnFilterHash,
                            idHash: idHash);
                    });
            }
        }

        private static Regex RegexId = new Regex(@"\b(?<!\.)\d+(?!\.)\b");

        private bool ConvertFilterHashDataId(
            SiteSettings ss,
            Dictionary<string, string> filterHash,
            Dictionary<long, long> idHash)
        {
            if (filterHash == null || ss?.Columns == null) return false;
            var isUpdated = false;
            foreach (var key in filterHash.Keys)
            {
                if (ss.Columns.FirstOrDefault(o => o.ColumnName == key)?.ChoicesText?
                    .RegexExists(
                        pattern: @"(?<=\[\[).+(?=\]\])",
                        regexOptions: RegexOptions.Multiline) != true) continue;
                var orgStr = filterHash[key];
                var newStr = RegexId.Replace(
                    orgStr,
                    new MatchEvaluator((Match matchId) => idHash.Get(matchId.Value.ToLong()).ToStr()));
                if (newStr != orgStr)
                {
                    isUpdated = true;
                    filterHash[key] = newStr;
                }
            }
            return isUpdated;
        }

        public Dictionary<long, long> GetIdHashFromConverters()
        {
            var idHash = new Dictionary<long, long>();
            HeaderInfo.Convertors.ForEach(e =>
                idHash.AddOrUpdate(e.SiteId, e.SavedSiteId ?? 0));
            return idHash;
        }

        public void ConvertInheritPermissionInNotIncluded()
        {
            var includeSiteIds = HeaderInfo.Convertors
                .Where(o => o != null)
                .Where(o => o.SiteId > 0)
                .Select(o => o.SiteId)
                .ToList();
            Sites
                .Where(site => !includeSiteIds.Contains(site.InheritPermission))
                .ForEach(site => site.InheritPermission = includeSiteIds.FirstOrDefault());
        }
    }
}