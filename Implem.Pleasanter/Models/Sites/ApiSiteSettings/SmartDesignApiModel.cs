using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System.Linq;
using Implem.Pleasanter.Libraries.Server;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class SmartDesignApiModel
    {
        public SiteSettings SiteSettings = new SiteSettings();

        public Dictionary<string, DragParamsApiSettingModel> SmartDesignParamHash = new Dictionary<string, DragParamsApiSettingModel>();

        public List<string> DefaultColumns = new List<string>();

        public string Timestamp { get; set; }
        public SmartDesignApiModel()
        {
        }

        public SmartDesignApiModel(Context context, SiteSettings ss, string timestamp)
        {
            var editorColumnList = ss.EditorColumnHash.Get("General");
            DefaultColumns = ss.GetDefaultColumns(context);
            SetSmartDesignSiteSettings(context, ss, editorColumnList);
            SmartDesignParamHash = GetSmartDesignParam(context, ss, editorColumnList);
            Timestamp = timestamp;
            RemoveLinks(context: context);
        }

        public Dictionary<string, DragParamsApiSettingModel> GetSmartDesignParam(
            Context context,
            SiteSettings ss,
            List<string> editorColumnList)
        {
            var smartDesignParamHash = new Dictionary<string, DragParamsApiSettingModel>();
            var otherEditorColumns = ss.EditorColumnHash
                .Where(kvp => kvp.Key != "General")
                .SelectMany(kvp => kvp.Value)
                .ToList();
            var otherEditorColumnNames = otherEditorColumns.ToHashSet();
            SetColumnsParams(
                context: context,
                ss: ss,
                smartDesignParamHash: smartDesignParamHash,
                editorColumnList: editorColumnList,
                otherEditorColumnNames: otherEditorColumnNames);
            if (ss.Destinations != null && ss.Destinations.Count() > 0)
            {
                SetDestinationLink(
                    ss: ss,
                    editorColumnList: editorColumnList,
                    smartDesignParamHash: smartDesignParamHash,
                    otherEditorColumns: otherEditorColumns);
            }
            if (ss.Sources != null && ss.Sources.Count > 0)
            {
                SetSourcesLink(
                    ss: ss,
                    editorColumnList: editorColumnList,
                    smartDesignParamHash: smartDesignParamHash,
                    otherEditorColumns: otherEditorColumns);
            }
            return smartDesignParamHash;
        }

        public void SetColumnsParams(
            Context context,
            SiteSettings ss,
            Dictionary<string, DragParamsApiSettingModel> smartDesignParamHash,
            List<string> editorColumnList,
            HashSet<string> otherEditorColumnNames)
        {
            var notInEditorColumnList = ss.GetEditorColumnNames()
                .Where(o => !editorColumnList.Contains(o))
                .ToList();       
            var editorDefinitionColumnNames = ss.ColumnDefinitionHash
                .EditorDefinitions(context: context, enableOnly: false)
                .Select(def => def.ColumnName)
                .ToHashSet();
            ss.Columns.ForEach(column =>
            {
                var dragParamsApiSettingModel = new DragParamsApiSettingModel();
                dragParamsApiSettingModel.SetType(column);
                dragParamsApiSettingModel.SetCategory(column);
                dragParamsApiSettingModel = SetState(
                    context: context,
                    ss: ss,
                    column: column,
                    editorColumnList: editorColumnList,
                    notInEditorColumnList: notInEditorColumnList,
                    dragParamsApiSettingModel: dragParamsApiSettingModel,
                    editorDefinitionColumnNames: editorDefinitionColumnNames,
                    otherEditorColumnNames: otherEditorColumnNames);
                smartDesignParamHash.Add(column.ColumnName, dragParamsApiSettingModel);
            });
            var linksColumn = ss.GridColumns
                .Where(o => o.Contains("~"))
                .ToList();
            var filterColumns = ss.FilterColumns
                .Where(o => o.Contains("~"))
                .ToList();
            linksColumn.ForEach(column =>
            {
                var dragParamsApiSettingModel = new DragParamsApiSettingModel();
                UnusedColumn(dragParamsApiSettingModel, column);
                smartDesignParamHash.Add(column, dragParamsApiSettingModel);
            });
            filterColumns.ForEach(column =>
            {
                if (!smartDesignParamHash.ContainsKey(column))
                {
                    var dragParamsApiSettingModel = new DragParamsApiSettingModel();
                    UnusedColumn(dragParamsApiSettingModel, column);
                    smartDesignParamHash.Add(column, dragParamsApiSettingModel);
                }
            });
        }

        public void UnusedColumn(
            DragParamsApiSettingModel dragParamsApiSettingModel,
            string columnName)
        {
            dragParamsApiSettingModel.Type = null;
            dragParamsApiSettingModel.Category = "None";
            dragParamsApiSettingModel.State.Edit = -1;
            dragParamsApiSettingModel.State.Grid = -1;
            dragParamsApiSettingModel.State.Filter = -1;
        }

        public DragParamsApiSettingModel SetState(
            Context context,
            SiteSettings ss,
            Libraries.Settings.Column column,
            List<string> editorColumnList,
            List<string> notInEditorColumnList,
            DragParamsApiSettingModel dragParamsApiSettingModel,
            HashSet<string> editorDefinitionColumnNames,
            HashSet<string> otherEditorColumnNames)
        {
            if (editorColumnList.Contains(column.ColumnName) || !notInEditorColumnList.Contains(column.ColumnName))
            {
                var canEdit = editorDefinitionColumnNames.Contains(column.ColumnName);             
                dragParamsApiSettingModel.State.Grid = GetStateValue(
                    column.GridColumn,
                    ss.GridColumns,
                    column.ColumnName);
                dragParamsApiSettingModel.State.Edit = GetStateValue(
                    canEdit,
                    editorColumnList,
                    column.ColumnName);
                dragParamsApiSettingModel.State.Filter = GetStateValue(
                    column.FilterColumn,
                    ss.FilterColumns,
                    column.ColumnName);
            }
            else
            {
                if (otherEditorColumnNames.Contains(column.ColumnName))
                {
                    var canEdit = editorDefinitionColumnNames.Contains(column.ColumnName);
                    dragParamsApiSettingModel.State.Grid = GetStateValue(
                        column.GridColumn,
                        ss.GridColumns,
                        column.ColumnName);
                    dragParamsApiSettingModel.State.Edit = canEdit ? 1 : -1;
                    dragParamsApiSettingModel.State.Filter = GetStateValue(
                        column.FilterColumn,
                        ss.FilterColumns,
                        column.ColumnName);
                }
                else
                {
                    dragParamsApiSettingModel.State.Grid = -1;
                    dragParamsApiSettingModel.State.Edit = -1;
                    dragParamsApiSettingModel.State.Filter = -1;
                }
            }
            return dragParamsApiSettingModel;
        }

        private int GetStateValue(bool columnFlag, List<string> columnList, string columnName)
        {
            if (!columnFlag)
            {
                return -1;
            }
            else if (!columnList.Contains(columnName) && columnFlag)
            {
                return 0;
            }
            else if (columnList.Contains(columnName) && columnFlag)
            {
                return 1;
            }
            return -1;
        }

        public void SetSmartDesignSiteSettings(
            Context context,
            SiteSettings ss,
            List<string> editorColumnList)
        {
            if (ss.EditorColumnHash != null)
            {
                foreach (var kvp in ss.EditorColumnHash)
                {
                    var columns = kvp.Value;
                    var removeLinksColumnList = columns
                        .Where(column => column.StartsWith("_Links-"))
                        .Select(column => new
                        {
                            Column = column,
                            Id = long.Parse(column.Substring("_Links-".Length))
                        })
                        .Where(x => !ss.Destinations.ContainsKey(x.Id)
                            && !ss.Sources.ContainsKey(x.Id))
                        .Select(x => x.Column)
                        .ToList();
                    foreach (var key in removeLinksColumnList)
                    {
                        columns.Remove(key);
                    }
                }
                SiteSettings.EditorColumnHash = ss.EditorColumnHash;
            }
            if (ss.GridColumns != null) SiteSettings.GridColumns = ss.GridColumns;
            if (ss.FilterColumns != null) SiteSettings.FilterColumns = ss.FilterColumns;
            if (ss.Links != null) SiteSettings.Links = ss.Links.ToList();
            if (ss.SectionLatestId != null) SiteSettings.SectionLatestId = ss.SectionLatestId;
            if (ss.GeneralTabLabelText != null) SiteSettings.GeneralTabLabelText = ss.GeneralTabLabelText;
            if (ss.TabLatestId != null) SiteSettings.TabLatestId = ss.TabLatestId;
            if (ss.Tabs != null) SiteSettings.Tabs = ss.Tabs;
            SiteSettings.Sections = ss.Sections != null ? ss.Sections : new List<Section>();
            if (ss.Columns != null)
            {
                var enabledEditorColumns = ss.GetEditorColumnNames();
                SiteSettings.Columns = ss.Columns
                    .Where(o => enabledEditorColumns.Contains(o.ColumnName)
                        || DefaultColumns.Contains(o.ColumnName))
                    .ToList();
            }
        }

        public Dictionary<string, DragParamsApiSettingModel> SetDestinationLink(
            SiteSettings ss,
            List<string> editorColumnList,
            Dictionary<string, DragParamsApiSettingModel> smartDesignParamHash,
            List<string> otherEditorColumns)
        {
            var linkColumnList = ss.Links.Select(link => ss.LinkId(link.SiteId)).ToList();
            foreach (var linkColumn in linkColumnList)
            {
                var dragParamsApiSettingModel = new DragParamsApiSettingModel();
                dragParamsApiSettingModel.LinkName = ss.Destinations
                    .Where(o => o.Key == ss.LinkId(linkColumn))
                    .Select(o => o.Value.Title)
                    .FirstOrDefault();
                if (editorColumnList.Contains(linkColumn))
                {
                    dragParamsApiSettingModel.State.Edit = 1;
                }
                else if (otherEditorColumns.Any(column => column.StartsWith("_Links-") && long.Parse(column.Substring("_Links-".Length)) == ss.LinkId(linkColumn)))
                {
                    dragParamsApiSettingModel.State.Edit = 1;
                }
                else
                {
                    dragParamsApiSettingModel.State.Edit = 0;
                }
                dragParamsApiSettingModel.State.Grid = -1;
                dragParamsApiSettingModel.State.Filter = -1;
                dragParamsApiSettingModel.Category = "Links";
                dragParamsApiSettingModel.Type = "LinkTable";
                if (!smartDesignParamHash.ContainsKey(linkColumn))
                {
                    smartDesignParamHash.Add(linkColumn, dragParamsApiSettingModel);
                }
                else
                {
                    smartDesignParamHash[linkColumn] = dragParamsApiSettingModel;
                }
            }
            return smartDesignParamHash;
        }

        public Dictionary<string, DragParamsApiSettingModel> SetSourcesLink(
            SiteSettings ss,
            List<string> editorColumnList,
            Dictionary<string, DragParamsApiSettingModel> smartDesignParamHash,
            List<string> otherEditorColumns)
        {
            ss.Sources.ForEach(source =>
            {
                var dragParamsApiSettingModel = new DragParamsApiSettingModel();
                if (!SiteSettings.Links.Any(link => link.SiteId == source.Key))
                {
                    SiteSettings.Links.Add(new Link { SiteId = source.Key });
                }
                var linkColumn = ss.LinkId(source.Key);
                dragParamsApiSettingModel.LinkName = source.Value.Title;
                if (editorColumnList.Contains(linkColumn))
                {
                    dragParamsApiSettingModel.State.Edit = 1;
                }
                else if (otherEditorColumns.Any(column => column.StartsWith("_Links-") && long.Parse(column.Substring("_Links-".Length)) == ss.LinkId(linkColumn)))
                {
                    dragParamsApiSettingModel.State.Edit = 1;
                }
                else
                {
                    dragParamsApiSettingModel.State.Edit = 0;
                }
                dragParamsApiSettingModel.State.Grid = -1;
                dragParamsApiSettingModel.State.Filter = -1;
                dragParamsApiSettingModel.Category = "Links";
                dragParamsApiSettingModel.Type = "LinkTable";
                if (!smartDesignParamHash.ContainsKey(linkColumn))
                {
                    smartDesignParamHash.Add(linkColumn, dragParamsApiSettingModel);
                }
                else
                {
                    smartDesignParamHash[linkColumn] = dragParamsApiSettingModel;
                }
            });
            return smartDesignParamHash;
        }

        public void RemoveLinks(Context context)
        {
            var allEditorColumns = SiteSettings.EditorColumnHash
                ?.SelectMany(kvp => kvp.Value)
                .ToHashSet() ?? new HashSet<string>();
            SiteSettings?.Links?.RemoveAll(link =>
            {
                var site = SiteInfo.Sites(context: context).Get(link.SiteId);
                if (site == null
                    || site.String("ReferenceType") == "Wikis"
                    || link.SiteId == 0)
                {
                    return true;
                }
                if (!string.IsNullOrEmpty(link.ColumnName))
                {
                    return !allEditorColumns.Contains(link.ColumnName);
                }
                return false;
            });
            if (SiteSettings?.Links != null)
            {
                SiteSettings.Links = SiteSettings.Links
                    .GroupBy(link => link.SiteId)
                    .Select(group =>
                        group.FirstOrDefault(link => !string.IsNullOrEmpty(link.ColumnName))
                        ?? group.First())
                    .ToList();
            }
        }
    }
}
