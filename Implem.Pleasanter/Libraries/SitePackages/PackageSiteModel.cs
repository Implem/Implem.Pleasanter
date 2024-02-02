using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Http.Connections;
using Newtonsoft.Json;
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
        public string SiteName = string.Empty;
        public string SiteGroupName = string.Empty;
        public string Body = string.Empty;
        public string GridGuide = string.Empty;
        public string EditorGuide = string.Empty;
        public string CalendarGuide = string.Empty;
        public string CrosstabGuide = string.Empty;
        public string GanttGuide = string.Empty;
        public string BurnDownGuide = string.Empty;
        public string TimeSeriesGuide = string.Empty;
        public string KambanGuide = string.Empty;
        public string ImageLibGuide = string.Empty;
        public string ReferenceType = "Sites";
        public long ParentId = 0;
        public long InheritPermission = 0;
        public SiteSettings SiteSettings;
        public bool Publish = false;
        public bool DisableCrossSearch = false;
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
            SiteName = siteModel.SiteName;
            SiteGroupName = siteModel.SiteGroupName;
            Body = siteModel.Body;
            GridGuide = siteModel.GridGuide;
            EditorGuide = siteModel.EditorGuide;
            CalendarGuide = siteModel.CalendarGuide;
            CrosstabGuide= siteModel.CrosstabGuide;
            GanttGuide = siteModel.GanttGuide;
            BurnDownGuide = siteModel.BurnDownGuide;
            TimeSeriesGuide = siteModel.TimeSeriesGuide;
            KambanGuide = siteModel.KambanGuide;
            ImageLibGuide = siteModel.ImageLibGuide;
            ReferenceType = siteModel.ReferenceType;
            ParentId = siteModel.ParentId;
            InheritPermission = siteModel.InheritPermission;
            SiteSettings = siteModel.SiteSettings;
            SiteSettings.Update_ColumnAccessControls();
            Publish = siteModel.Publish;
            DisableCrossSearch = siteModel.DisableCrossSearch;
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
            bool includeNotifications,
            bool includeReminders,
            PermissionIdList permissionIdList)
        {
            var ss = SiteSettings;
            ss.SiteId = SavedSiteId;
            ss.Links
                ?.Where(o => o.SiteId > 0)
                .ForEach(link =>
                    link.SiteId = header.GetConvertedId(link.SiteId));
            ss.Summaries?.ForEach(summary =>
                summary.SiteId = header.GetConvertedId(summary.SiteId));
            if (ss.IntegratedSites?.Any() == true)
            {
                var integratedSites = new List<string>();
                ss.IntegratedSites?.ForEach(site =>
                {
                    var convertedId = header.GetConvertedId(site.ToLong());
                    if (convertedId > 0)
                    {
                        integratedSites.Add(convertedId.ToString());
                    }
                    else if (!site.IsNullOrEmpty())
                    {
                        integratedSites.Add(site);
                    }
                });
                ss.IntegratedSites = integratedSites;
            }
            ss.EditorColumnHash = ss.EditorColumnHash?.ToDictionary(
                data => data.Key,
                data => ReplaceLinkedColumnName(
                    header: header,
                    columns: data.Value));
            ss.Columns
                ?.Where(e => e.ChoicesText != null)
                .ForEach(column =>
                    column.ChoicesText = ReplaceChoicesText(
                        header: header,
                        ss: ss,
                        columnName: column.ColumnName,
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
            ss.DashboardParts?.ForEach(dashboardPart =>
            {
                dashboardPart.SetSitesData();
                dashboardPart.SiteId = header.GetConvertedId(dashboardPart.SiteId);
                foreach (var quickAccessSite in dashboardPart.QuickAccessSitesData)
                {
                    if (long.TryParse(quickAccessSite.Id, out long siteId))
                    {
                        quickAccessSite.Id = header.GetConvertedId(siteId).ToString();
                    }
                }
                dashboardPart.SetQuickAccessSites();
                dashboardPart.TimeLineSitesData = dashboardPart.TimeLineSitesData
                    ?.Select(s =>
                        long.TryParse(s, out long siteId)
                            ? header.GetConvertedId(siteId).ToString()
                            : s)
                    .ToList();
                dashboardPart.SetTimeLineSites();
                dashboardPart.SetCalendarSites();
                dashboardPart.SetIndexSites();
            });
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
            if (!includeNotifications)
            {
                ss.Notifications?.Clear();
                ss.Processes?.ForEach(p => p.Notifications?.Clear());
            }
            if (!includeReminders)
            {
                ss.Reminders?.Clear();
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
            ReplaceProcesses(
                context: context,
                processes: ss.Processes,
                permissionIdList: permissionIdList);
            ReplaceViews(
                context: context,
                views: ss.Views,
                permissionIdList: permissionIdList);
            ReplaceExports(
                context: context,
                exports: ss.Exports,
                permissionIdList: permissionIdList);
            return ss;
        }

        internal static List<string> ReplaceLinkedColumnName(
            SitePackage.Header header,
            List<string> columns)
        {
            var replaced = new List<string>();
            columns.ForEach(columnName =>
            {
                if (columnName.StartsWith("_Links-"))
                {
                    var siteId = columnName.Split_2nd('-').ToLong();
                    columnName = $"_Links-{header.GetConvertedId(siteId)}";
                }
                replaced.Add(columnName);
            });
            return replaced;
        }

        internal static string ReplaceChoicesText(
            SitePackage.Header header,
            SiteSettings ss,
            string columnName,
            string source = "")
        {
            var links = ss?.Links?
                .Where(o => o.ColumnName == columnName)
                .Where(o => o.JsonFormat == true)
                .ToJson()
                .Deserialize<List<Link>>();
            if (links?.Any() == true)
            {
                links.ForEach(link =>
                {
                    link.SiteId = header.GetConvertedId(link.SiteId);
                    link.ColumnName = null;
                    link.JsonFormat = null;
                });
                return links.ToJson(
                    defaultValueHandling: DefaultValueHandling.Ignore,
                    formatting: Formatting.Indented);
            }
            var sites = System.Text.RegularExpressions.Regex.Matches(source, @"(?<=\[\[).+(?=\]\])")
                .Select(m =>
                {
                    var data = m.Value.Split(",");
                    var srcId = data.FirstOrDefault()?.ToLong() ?? 0;
                    if (srcId <= 0) return string.Empty;
                    data[0] = header.GetConvertedId(srcId).ToString();
                    return $"[[{data.Join()}]]";
                })
                .Where(s => !s.IsNullOrEmpty())
                .Join("\n");
            return sites.IsNullOrEmpty()
                ? source
                : sites;
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

        internal static void ReplaceProcesses(
            Context context,
            SettingList<Process> processes,
            PermissionIdList permissionIdList)
        {
            processes?.ForEach(process =>
            {
                process.Depts = process?.Depts
                    ?.Select(deptId => Utilities.ConvertedDeptId(
                        context: context,
                        permissionIdList: permissionIdList,
                        deptId: deptId))
                    .Where(deptId => deptId > 0)
                    .ToList();
                process.Groups = process?.Groups
                    ?.Select(groupId => Utilities.ConvertedGroupId(
                        context: context,
                        permissionIdList: permissionIdList,
                        groupId: groupId))
                    .Where(groupId => groupId > 0)
                    .ToList();
                process.Users = process?.Users
                    ?.Select(userId => Utilities.ConvertedUserId(
                        context: context,
                        permissionIdList: permissionIdList,
                        userId: userId))
                    .Where(userId => userId > 0)
                    .ToList();
            });
        }

        internal static void ReplaceViews(
            Context context,
            List<View> views,
            PermissionIdList permissionIdList)
        {
            views?.ForEach(view =>
            {
                view.Depts = view?.Depts
                    ?.Select(deptId => Utilities.ConvertedDeptId(
                        context: context,
                        permissionIdList: permissionIdList,
                        deptId: deptId))
                    .Where(deptId => deptId > 0)
                    .ToList();
                view.Groups = view?.Groups
                    ?.Select(groupId => Utilities.ConvertedGroupId(
                        context: context,
                        permissionIdList: permissionIdList,
                        groupId: groupId))
                    .Where(groupId => groupId > 0)
                    .ToList();
                view.Users = view?.Users
                    ?.Select(userId => Utilities.ConvertedUserId(
                        context: context,
                        permissionIdList: permissionIdList,
                        userId: userId))
                    .Where(userId => userId > 0)
                    .ToList();
            });
        }

        internal static void ReplaceExports(
            Context context,
            SettingList<Export> exports,
            PermissionIdList permissionIdList)
        {
            exports?.ForEach(export =>
            {
                export.Depts = export?.Depts
                    ?.Select(deptId => Utilities.ConvertedDeptId(
                        context: context,
                        permissionIdList: permissionIdList,
                        deptId: deptId))
                    .Where(deptId => deptId > 0)
                    .ToList();
                export.Groups = export?.Groups
                    ?.Select(groupId => Utilities.ConvertedGroupId(
                        context: context,
                        permissionIdList: permissionIdList,
                        groupId: groupId))
                    .Where(groupId => groupId > 0)
                    .ToList();
                export.Users = export?.Users
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