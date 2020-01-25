using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.SitePackages
{
    public class PackageSiteModel
    {
        public int TenantId = 0;
        public long SiteId = 0;
        public string Title = string.Empty;
        public string Body = string.Empty;
        public string GridGuide = string.Empty;
        public string EditorGuide = string.Empty;
        public string ReferenceType = "Sites";
        public long ParentId = 0;
        public long InheritPermission = 0;
        public SiteSettings SiteSettings;
        public bool Publish = false;
        public Comments Comments = new Comments();
        public long? WikiId;
        [NonSerialized]
        public int SavedTenantId = 0;
        [NonSerialized]
        public long SavedSiteId = 0;
        [NonSerialized]
        public long SavedParentId = 0;
        [NonSerialized]
        public long SavedInheritPermission = 0;

        internal PackageSiteModel()
        {
        }

        internal PackageSiteModel(SiteModel siteModel)
        {
            TenantId = siteModel.TenantId;
            SiteId = siteModel.SiteId;
            Title = siteModel.Title.Value.MaxLength(1024);
            Body = siteModel.Body;
            GridGuide = siteModel.GridGuide;
            EditorGuide = siteModel.EditorGuide;
            ReferenceType = siteModel.ReferenceType;
            ParentId = siteModel.ParentId;
            InheritPermission = siteModel.InheritPermission;
            SiteSettings = siteModel.SiteSettings;
            Publish = siteModel.Publish;
            Comments = siteModel.Comments;
        }

        internal void SetSavedIds(
            Context context,
            SiteSettings ss,
            SitePackage sitePackage,
            long savedSiteId,
            bool includeSitePermission)
        {
            SavedTenantId = context.TenantId;
            SavedSiteId = savedSiteId;
            SavedParentId = sitePackage.HeaderInfo.GetConvertedId(
                siteId: ParentId,
                isParentId: true);
            if (SiteId == InheritPermission && includeSitePermission)
            {
                SavedInheritPermission = savedSiteId;
            }
            else
            {
                SavedInheritPermission = sitePackage.HeaderInfo.Convertors.FirstOrDefault(o =>
                    o.SiteId == InheritPermission)?.SavedSiteId
                        ?? sitePackage.Sites.FirstOrDefault()?.InheritPermission
                            ?? 0;
            }
            if (SavedInheritPermission == 0)
            {
                SavedInheritPermission = ss.SiteId == 0
                    ? savedSiteId
                    : ss.InheritPermission;
            }
        }

        internal SiteSettings GetSavedSiteSettings(
            Context context,
            SitePackage.Header header,
            bool includeColumnPermission,
            PermissionIdList permissionIdList)
        {
            var ss = SiteSettings;
            ss.SiteId = SavedSiteId;
            ss.Links?.ForEach(link =>
                link.SiteId = header.GetConvertedId(link.SiteId));
            ss.Summaries?.ForEach(summary =>
                summary.SiteId = header.GetConvertedId(summary.SiteId));
            if (ss.IntegratedSites?.Any() == true)
            {
                var integratedSites = new List<long>();
                ss.IntegratedSites?.ForEach(id =>
                    integratedSites.Add(header.GetConvertedId(id)));
                ss.IntegratedSites = integratedSites;
            }
            ss.Columns?
                .Where(e => e.ChoicesText != null)
                .ForEach(column =>
                    column.ChoicesText = ReplaceChoicesText(
                        header: header,
                        source: column.ChoicesText));
            if (ss.GridColumns != null)
            {
                var gridColumns = new List<string>();
                ss.GridColumns.ForEach(column =>
                    gridColumns.Add(
                        ReplaceJoinColumn(
                            header: header,
                            source: column)));
                ss.GridColumns = gridColumns;
            }
            if (ss.FilterColumns != null)
            {
                var filterColumns = new List<string>();
                ss.FilterColumns.ForEach(column =>
                    filterColumns.Add(
                        ReplaceJoinColumn(
                            header: header,
                            source: column)));
                ss.FilterColumns = filterColumns;
            }
            ss.Exports?.ForEach(export =>
                export.Columns?.ForEach(column =>
                    column.ColumnName = ReplaceJoinColumn(
                        header: header,
                        source: column.ColumnName)));
            ss.Views?.ForEach(view =>
            {
                if (view.GridColumns?.Any() == true)
                {
                    var gridColumns = new List<string>();
                    view.GridColumns.ForEach(column =>
                        gridColumns.Add(
                            ReplaceJoinColumn(
                                header: header,
                                source: column)));
                    view.GridColumns = gridColumns;
                }
                if (view.ColumnFilterHash?.Any() == true)
                {
                    var hash = new Dictionary<string, string>();
                    view.ColumnFilterHash
                        .Where(o => o.Value != "[]")
                        .ForEach(o => hash.Add(
                            ReplaceJoinColumn(
                                header: header,
                                source: o.Key),
                            o.Value));
                    view.ColumnFilterHash = hash;
                }
                if (view.ColumnSorterHash?.Any() == true)
                {
                    var hash = new Dictionary<string, SqlOrderBy.Types>();
                    view.ColumnSorterHash
                        .ForEach(o => hash.Add(
                            ReplaceJoinColumn(
                                header: header,
                                source: o.Key),
                            o.Value));
                    view.ColumnSorterHash = hash;
                }
                if (view.CrosstabGroupByX?.Any() == true)
                {
                    if (view.CrosstabGroupByX.Contains("~"))
                    {
                        view.CrosstabGroupByX = ReplaceJoinColumn(
                            header: header,
                            source: view.CrosstabGroupByX);
                    }
                }
                if (view.CrosstabGroupByY?.Any() == true)
                {
                    if (view.CrosstabGroupByY.Contains("~"))
                    {
                        view.CrosstabGroupByY = ReplaceJoinColumn(
                            header: header,
                            source: view.CrosstabGroupByY);
                    }
                }
                if (view.CrosstabValue?.Any() == true)
                {
                    if (view.CrosstabValue.Contains("~"))
                    {
                        view.CrosstabValue = ReplaceJoinColumn(
                            header: header,
                            source: view.CrosstabValue);
                    }
                }
            });
            if (!includeColumnPermission)
            {
                ss.CreateColumnAccessControls?.Clear();
                ss.ReadColumnAccessControls?.Clear();
                ss.UpdateColumnAccessControls?.Clear();
            }
            else
            {
                ReplaceColumnAccessControls(
                    context: context,
                    columnAccessControls: ss.CreateColumnAccessControls,
                    permissionIdList: permissionIdList);
                ReplaceColumnAccessControls(
                    context: context,
                    columnAccessControls: ss.ReadColumnAccessControls,
                    permissionIdList: permissionIdList);
                ReplaceColumnAccessControls(
                    context: context,
                    columnAccessControls: ss.UpdateColumnAccessControls,
                    permissionIdList: permissionIdList);
            }
            return ss;
        }

        internal static string ReplaceChoicesText(SitePackage.Header header, string source = "")
        {
            var match = System.Text.RegularExpressions.Regex.Match(source, @"(?<=\[\[)\d+(?=\]\])");
            string srcId = (match?.Success == true) ? match.Value.Split_1st().ToString() : "";
            if (!srcId.IsNullOrEmpty())
            {
                var newId = header.GetConvertedId(srcId.ToLong());
                var rep = source.Replace(srcId, newId.ToString());
                return rep;
            }
            else
            {
                return source;
            }
        }

        internal static string ReplaceJoinColumn(SitePackage.Header header, string source = "")
        {
            if (source.Contains("~") == true)
            {
                var rep = string.Empty;
                foreach (var str in source.Split_1st(',').Split('-'))
                {
                    if (!rep.IsNullOrEmpty()) rep += "-";
                    string[] delimiter;
                    if (str.Contains("~~") == true)
                    {
                        delimiter = new string[] { "~~" };
                    }
                    else
                    {
                        delimiter = new string[] { "~" };
                    }
                    var split = str.Split(delimiter, StringSplitOptions.None);
                    if (split.Length == 2)
                    {
                        long newId = header.GetConvertedId(split[1].ToLong());
                        rep += split[0] + string.Join("", delimiter) + newId;
                    }
                    else
                    {
                        rep += str;
                    }
                }
                rep += "," + source.Split_2nd();
                return rep;
            }
            else
            {
                return source;
            }
        }

        internal static void ReplaceColumnAccessControls(
            Context context,
            List<ColumnAccessControl> columnAccessControls,
            PermissionIdList permissionIdList)
        {
            columnAccessControls?.ForEach(columnAccessControl =>
            {
                columnAccessControl.Depts = columnAccessControl?.Depts
                    ?.Select(deptId => Utilities.ConvertedDeptId(
                        context: context,
                        permissionIdList: permissionIdList,
                        deptId: deptId))
                    .Where(deptId => deptId > 0)
                    .ToList();
                columnAccessControl.Groups = columnAccessControl?.Groups
                    ?.Select(groupId => Utilities.ConvertedGroupId(
                        context: context,
                        permissionIdList: permissionIdList,
                        groupId: groupId))
                    .Where(groupId => groupId > 0)
                    .ToList();
                columnAccessControl.Users = columnAccessControl?.Users
                    ?.Select(userId => Utilities.ConvertedUserId(
                        context: context,
                        permissionIdList: permissionIdList,
                        userId: userId))
                    .Where(userId => userId > 0)
                    .ToList();
            });
        }
    }
}