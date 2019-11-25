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
            bool includeColumnPermission)
        {
            HeaderInfo = new Header(
                context: context,
                includeSitePermission: includeSitePermission,
                includeRecordPermission: includeRecordPermission,
                includeColumnPermission: includeColumnPermission);
            Sites = new List<PackageSiteModel>();
            Data = new List<JsonExport>();
            Permissions = new List<PackagePermissionModel>();
            Construct(
                context: context,
                siteList: siteList,
                includeRecordPermission: includeRecordPermission,
                includeColumnPermission: includeColumnPermission);
            if (includeSitePermission)
            {
                PermissionIdList = new PermissionIdList(
                    context: context,
                    sites: Sites,
                    packagePermissionModels: Permissions);
            }
        }

        private void Construct(
            Context context,
            List<SelectedSite> siteList,
            bool includeRecordPermission,
            bool includeColumnPermission)
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
                    if (site.IncludeData == true)
                    {
                        switch (packageSiteModel.ReferenceType)
                        {
                            case "Issues":
                            case "Results":
                                var ss = siteModel.SiteSettings;
                                ss.SetPermissions(
                                    context: context,
                                    referenceId: ss.SiteId);
                                var export = new Export(ss.DefaultExportColumns(context: context));
                                export.Header = true;
                                export.Type = Export.Types.Json;
                                export.Columns.ForEach(o => o.SiteId = ss.SiteId);
                                var gridData = Utilities.GetGridData(
                                    context: context,
                                    ss: ss,
                                    export: export);
                                if (gridData.TotalCount > 0)
                                {
                                    Data.Add(gridData.GetJsonExport(
                                        context: context,
                                        ss: ss,
                                        export: export));
                                    foreach (DataRow dr in gridData.DataRows)
                                    {
                                        recordIdList.Add(dr[0].ToLong());
                                    }
                                }
                                break;
                        }
                    }
                    if (includeRecordPermission == true)
                    {
                        var packagePermissionModel = new PackagePermissionModel(
                            context: context,
                            siteModel: siteModel,
                            recordIdList: recordIdList);
                        Permissions.Add(packagePermissionModel);
                    }
                }
            }
        }

        public class Header
        {
            public long BaseSiteId;
            public string Server;
            public string CreatorName;
            public DateTime PackageTime;
            public List<Convertor> Convertors;
            public bool IncludeSitePermission;
            public bool IncludeRecordPermission;
            public bool IncludeColumnPermission;
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
                bool includeColumnPermission)
            {
                BaseSiteId = context.SiteId;
                Server = context.Server;
                CreatorName = context.User.Name;
                PackageTime = DateTime.Now;
                Convertors = new List<Convertor>();
                IncludeSitePermission = includeSitePermission;
                IncludeRecordPermission = includeRecordPermission;
                IncludeColumnPermission = includeColumnPermission;
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
                var ss = new SiteModel(context: context, savedSiteId).SiteSettings;
                ss.Links.ForEach(link =>
                {
                    var destinationType = ss.Destinations?.Get(link.SiteId)?.ReferenceType
                        ?? string.Empty;
                    switch (destinationType)
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
            }
        }

        public Dictionary<long, long> GetIdHashFromConverters()
        {
            var idHash = new Dictionary<long, long>();
            HeaderInfo.Convertors.ForEach(e =>
                idHash.AddOrUpdate(e.SiteId, e.SavedSiteId ?? 0));
            return idHash;
        }

    }
}