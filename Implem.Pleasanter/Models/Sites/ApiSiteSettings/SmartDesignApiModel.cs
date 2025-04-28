using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.DefinitionAccessor;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;
using System.Web.Razor.Generator;
using System.Collections;
using System.Net.Security;
using NLog.Targets;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.ParameterAccessor.Parts;

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

        public void RemoveLinks(Context context)
        {
            SiteSettings?.Links?.RemoveAll(link =>
            {
                var site = SiteInfo.Sites(context: context).Get(link.SiteId);
                return site == null
                    || (site.String("ReferenceType") == "Wikis"
                    || link.SiteId == 0);
            });
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
            SetColumnsParams(
                ss,
                smartDesignParamHash,
                editorColumnList);
            if (ss.Destinations != null && ss.Destinations.Count() > 0)
            {
                SetDestinationLink(
                    ss,
                    editorColumnList,
                    smartDesignParamHash,
                    otherEditorColumns);
            }
            if (ss.Sources != null && ss.Sources.Count > 0)
            {
                SetSourcesLink(
                    ss,
                    editorColumnList,
                    smartDesignParamHash,
                    otherEditorColumns);
            }
            return smartDesignParamHash;
        }

        public void SetColumnsParams(
            SiteSettings ss,
            Dictionary<string,DragParamsApiSettingModel> smartDesignParamHash,
            List<string> editorColumnList)
        {
            var notInEditorColumnList = ss.GetEditorColumnNames()
                .Where(o => !editorColumnList.Contains(o))
                .ToList(); ;
            //SmartDesignで利用するパラメータの設定
            ss.Columns.ForEach(column =>
            {
                var dragParamsApiSettingModel = new DragParamsApiSettingModel();
                if (notInEditorColumnList.Contains(column.ColumnName))
                {
                    dragParamsApiSettingModel.SetType(column);
                    dragParamsApiSettingModel.SetCategory(column);
                    dragParamsApiSettingModel = SetState(
                        ss,
                        column,
                        editorColumnList,
                        notInEditorColumnList,
                        dragParamsApiSettingModel);
                    smartDesignParamHash.Add(column.ColumnName, dragParamsApiSettingModel);
                    return;
                }
                dragParamsApiSettingModel.SetType(column);
                dragParamsApiSettingModel.SetCategory(column);
                dragParamsApiSettingModel = SetState(
                    ss,
                    column,
                    editorColumnList,
                    notInEditorColumnList,
                    dragParamsApiSettingModel);
                smartDesignParamHash.Add(column.ColumnName, dragParamsApiSettingModel);
            });
            //リンク先の項目に-1を設定
            var linksColumn =  ss.GridColumns
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
                };
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
            SiteSettings ss,
            Libraries.Settings.Column column,
            List<string> editorColumnList,
            List<string> notInEditorColumnList,
            DragParamsApiSettingModel dragParamsApiSettingModel)
        {
            if (editorColumnList.Contains(column.ColumnName) || !notInEditorColumnList.Contains(column.ColumnName))
            {
                dragParamsApiSettingModel.State.Grid = GetStateValue(column.GridColumn, ss.GridColumns, column.ColumnName);
                dragParamsApiSettingModel.State.Edit = GetStateValue(column.EditorColumn, editorColumnList, column.ColumnName);
                dragParamsApiSettingModel.State.Filter = GetStateValue(column.FilterColumn, ss.FilterColumns, column.ColumnName);
            }
            else
            {
                dragParamsApiSettingModel.State.Grid = -1;
                dragParamsApiSettingModel.State.Edit = -1;
                dragParamsApiSettingModel.State.Filter = -1;
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
                var generalColumns = ss.EditorColumnHash["General"];
                var removeLinksColumnList = generalColumns
                    .Where(column => column.StartsWith("_Links-"))
                    .Select(column => new { Column = column, Id = long.Parse(column.Substring("_Links-".Length)) })
                    .Where(x => !ss.Destinations.ContainsKey(x.Id) && !ss.Sources.ContainsKey(x.Id))
                    .Select(x => x.Column)
                    .ToList();
                foreach (var key in removeLinksColumnList)
                {
                    generalColumns.Remove(key);
                }
                ss.EditorColumnHash["General"] = generalColumns;
                SiteSettings.EditorColumnHash = ss.EditorColumnHash;
            }
            if (ss.GridColumns != null) SiteSettings.GridColumns = ss.GridColumns;
            if (ss.FilterColumns != null) SiteSettings.FilterColumns = ss.FilterColumns;
            if (ss.Links != null) SiteSettings.Links = ss.Links;
            if (ss.SectionLatestId != null) SiteSettings.SectionLatestId = ss.SectionLatestId;
            SiteSettings.Sections = ss.Sections != null ? ss.Sections : new List<Section>();
            if (ss.Columns != null) SiteSettings.Columns = ss.Columns.Where(o => editorColumnList.Contains(o.ColumnName) || DefaultColumns.Contains(o.ColumnName)).ToList();
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
                    dragParamsApiSettingModel.State.Edit = -1;
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
                SiteSettings.Links.Add(new Link { SiteId = source.Key });
                var linkColumn = ss.LinkId(source.Key);
                dragParamsApiSettingModel.LinkName = source.Value.Title;
                if (editorColumnList.Contains(linkColumn))
                {
                    dragParamsApiSettingModel.State.Edit = 1;
                }
                else if (otherEditorColumns.Any(column => column.StartsWith("_Links-") && long.Parse(column.Substring("_Links-".Length)) == ss.LinkId(linkColumn)))
                {
                    dragParamsApiSettingModel.State.Edit = -1;
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
    }
}
