using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            PermissionIdList = new PermissionIdList();
            var deptIds = new HashSet<int>();
            var groupIds = new HashSet<int>();
            var userIds = new HashSet<int>();
            Construct(
                context: context,
                siteList: siteList,
                includeSitePermission: includeSitePermission,
                includeRecordPermission: includeRecordPermission,
                includeColumnPermission: includeColumnPermission,
                includeNotifications: includeNotifications,
                includeReminders: includeReminders,
                deptIds: deptIds,
                groupIds: groupIds,
                userIds: userIds);
            SetPermissionIdList(
                context: context,
                sites: Sites,
                packagePermissionModels: Permissions,
                includeSitePermission: includeSitePermission,
                deptIds: deptIds,
                groupIds: groupIds,
                userIds: userIds);
            SetDeptIdList(
                context: context,
                deptIds: deptIds);
            SetGroupIdList(
                context: context,
                groupIds: groupIds);
            SetUserIdList(
                context: context,
                userIds: userIds);
        }

        private void Construct(
            Context context,
            List<SelectedSite> siteList,
            bool includeSitePermission,
            bool includeRecordPermission,
            bool includeColumnPermission,
            bool includeNotifications,
            bool includeReminders,
            HashSet<int> deptIds,
            HashSet<int> groupIds,
            HashSet<int> userIds)
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
                    ss.SetChoiceHash(context: context);
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
                                    view: view,
                                    where: packageSiteModel.ReferenceType == "Issues"
                                        ? Rds.IssuesWhere().SiteId(ss.SiteId)
                                        : Rds.ResultsWhere().SiteId(ss.SiteId));
                                if (gridData.TotalCount > 0)
                                {
                                    Data.Add(gridData.GetJsonExport(
                                        context: context,
                                        ss: ss,
                                        export: export));
                                }
                                var columns = ss.Columns.Where(
                                    o => o.Type == Column.Types.Dept
                                        || o.Type == Column.Types.Group
                                        || o.Type == Column.Types.User)
                                            .ToList();
                                foreach (var dataRow in gridData.DataRows)
                                {
                                    foreach (var column in columns)
                                    {
                                        var id = dataRow.Int(column.ColumnName);
                                        if (id > 0)
                                        {
                                            switch (column.Type)
                                            {
                                                case Column.Types.Dept:
                                                    deptIds.Add(id);
                                                    break;
                                                case Column.Types.Group:
                                                    groupIds.Add(id);
                                                    break;
                                                case Column.Types.User:
                                                    userIds.Add(id);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
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

        private void SetPermissionIdList(
            Context context,
            List<PackageSiteModel> sites,
            List<PackagePermissionModel> packagePermissionModels,
            bool includeSitePermission,
            HashSet<int> deptIds,
            HashSet<int> groupIds,
            HashSet<int> userIds)
        {
            if (includeSitePermission)
            {
                foreach (var packagePermissionModel in packagePermissionModels)
                {
                    foreach (var permission in packagePermissionModel.Permissions)
                    {
                        if (permission.DeptId > 0 && !deptIds.Contains(permission.DeptId))
                        {
                            deptIds.Add(permission.DeptId);
                        }
                        if (permission.GroupId > 0 && !groupIds.Contains(permission.GroupId))
                        {
                            groupIds.Add(permission.GroupId);
                        }
                        if (permission.UserId > 0 && !userIds.Contains(permission.UserId))
                        {
                            userIds.Add(permission.UserId);
                        }
                    }
                }
            }
            foreach (var packageSiteModel in sites)
            {
                foreach (var cca in packageSiteModel.SiteSettings.CreateColumnAccessControls)
                {
                    foreach (var dept in cca.Depts ?? new List<int>() { 0 })
                    {
                        deptIds.Add(dept);
                    }
                    foreach (var group in cca.Groups ?? new List<int>() { 0 })
                    {
                        groupIds.Add(group);
                    }
                    foreach (var user in cca.Users ?? new List<int>() { 0 })
                    {
                        userIds.Add(user);
                    }
                }
                foreach (var rca in packageSiteModel.SiteSettings.ReadColumnAccessControls)
                {
                    foreach (var dept in rca.Depts ?? new List<int>() { 0 })
                    {
                        deptIds.Add(dept);
                    }
                    foreach (var group in rca.Groups ?? new List<int>() { 0 })
                    {
                        groupIds.Add(group);
                    }
                    foreach (var user in rca.Users ?? new List<int>() { 0 })
                    {
                        userIds.Add(user);
                    }
                }
                foreach (var uca in packageSiteModel.SiteSettings.UpdateColumnAccessControls)
                {
                    foreach (var dept in uca.Depts ?? new List<int>() { 0 })
                    {
                        deptIds.Add(dept);
                    }
                    foreach (var group in uca.Groups ?? new List<int>() { 0 })
                    {
                        groupIds.Add(group);
                    }
                    foreach (var user in uca.Users ?? new List<int>() { 0 })
                    {
                        userIds.Add(user);
                    }
                }
                foreach (var process in packageSiteModel.SiteSettings.Processes)
                {
                    foreach (var dept in process.Depts ?? new List<int>() { 0 })
                    {
                        deptIds.Add(dept);
                    }
                    foreach (var group in process.Groups ?? new List<int>() { 0 })
                    {
                        groupIds.Add(group);
                    }
                    foreach (var user in process.Users ?? new List<int>() { 0 })
                    {
                        userIds.Add(user);
                    }
                }
                foreach (var view in packageSiteModel.SiteSettings.Views)
                {
                    foreach (var dept in view.Depts ?? new List<int>() { 0 })
                    {
                        deptIds.Add(dept);
                    }
                    foreach (var group in view.Groups ?? new List<int>() { 0 })
                    {
                        groupIds.Add(group);
                    }
                    foreach (var user in view.Users ?? new List<int>() { 0 })
                    {
                        userIds.Add(user);
                    }
                }
                foreach (var export in packageSiteModel.SiteSettings.Exports)
                {
                    foreach (var dept in export.Depts ?? new List<int>() { 0 })
                    {
                        deptIds.Add(dept);
                    }
                    foreach (var group in export.Groups ?? new List<int>() { 0 })
                    {
                        groupIds.Add(group);
                    }
                    foreach (var user in export.Users ?? new List<int>() { 0 })
                    {
                        userIds.Add(user);
                    }
                }
            }
        }

        private void SetDeptIdList(Context context, HashSet<int> deptIds)
        {
            PermissionIdList.DeptIdList = deptIds
                .Where(deptId => deptId > 0)
                .Select(deptId => SiteInfo.Dept(
                    tenantId: context.TenantId,
                    deptId: deptId))
                .Select(dept => new DeptIdHash()
                {
                    DeptId = dept.Id,
                    DeptCode = dept.Code,
                    DeptName = dept.Name
                })
                .ToList();
        }

        private void SetGroupIdList(Context context, HashSet<int> groupIds)
        {
            PermissionIdList.GroupIdList = groupIds
                .Where(groupId => groupId > 0)
                .Select(groupId => SiteInfo.Group(
                    tenantId: context.TenantId,
                    groupId: groupId))
                .Select(group => new GroupIdHash()
                {
                    GroupId = group.Id,
                    GroupName = group.Name
                })
                .ToList();
        }

        private void SetUserIdList(Context context, HashSet<int> userIds)
        {
            PermissionIdList.UserIdList = userIds
                .Where(userId => userId > 0)
                .Select(userId => SiteInfo.User(
                    context: context,
                    userId: userId))
                .Where(user => !user.Anonymous())
                .Select(user => new UserIdHash()
                {
                    UserId = user.Id,
                    LoginId = user.LoginId,
                    UserCode = user.UserCode,
                    Name = user.Name
                })
                .ToList();
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