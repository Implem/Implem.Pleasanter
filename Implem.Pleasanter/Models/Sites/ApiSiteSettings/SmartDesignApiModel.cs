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

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class SmartDesignApiModel
    {
        public SiteSettings SiteSettings = new SiteSettings();

        public Dictionary<string, DragParamsApiSettingModel> SmartDesignParamHash = new Dictionary<string, DragParamsApiSettingModel>();

        public List<string> DefaultColumns = new List<string>();


        public SmartDesignApiModel()
        {
        }

        public SmartDesignApiModel(Context context, SiteSettings ss)
        {
            DefaultColumns = ss.GetDefaultColumns(context);
            SetSmartDesignSiteSettings(context, ss);
            SmartDesignParamHash = GetSmartDesignParam(context, ss);
        }

        public Dictionary<string, DragParamsApiSettingModel> GetSmartDesignParam(Context context, SiteSettings ss)
        {
            var editorColumnList = ss.GetEditorColumnNames();
            var defaultColumns = ss.GetDefaultColumns(context);
            var smartDesignParamHash = new Dictionary<string, DragParamsApiSettingModel>();
            SetColumnsParams(
                ss,
                smartDesignParamHash,
                editorColumnList);
            if (ss.Destinations != null && ss.Destinations.Count() > 0)
            {
                SetDestinationLink(
                    ss,
                    editorColumnList,
                    smartDesignParamHash);
            }
            if (ss.Sources != null && ss.Sources.Count > 0)
            {
                SetSourcesLink(
                    ss,
                    editorColumnList,
                    smartDesignParamHash);
            }
            return smartDesignParamHash;
        }

        public void SetColumnsParams(
            SiteSettings ss,
            Dictionary<string,DragParamsApiSettingModel> smartDesignParamHash,
            List<string> editorColumnList)
        {
            ss.Columns.ForEach(column =>
            {
                var dragParamsApiSettingModel = new DragParamsApiSettingModel();
                dragParamsApiSettingModel.SetType(column);
                dragParamsApiSettingModel.SetCategory(column);
                dragParamsApiSettingModel = SetState(
                    ss,
                    column,
                    editorColumnList,
                    dragParamsApiSettingModel);
                smartDesignParamHash.Add(column.ColumnName, dragParamsApiSettingModel);
            });
        }

        public DragParamsApiSettingModel SetState(
            SiteSettings ss,
            Libraries.Settings.Column column,
            List<string> editorColumnList,
            DragParamsApiSettingModel dragParamsApiSettingModel)
        {
            if (!column.GridColumn)
            {
                dragParamsApiSettingModel.State.Grid = -1;
            }
            else if (!ss.GridColumns.Contains(column.ColumnName) && column.GridColumn)
            {
                dragParamsApiSettingModel.State.Grid = 0;
            }
            else if (ss.GridColumns.Contains(column.ColumnName) && column.GridColumn)
            {
                dragParamsApiSettingModel.State.Grid = 1;
            }

            if (!column.EditorColumn)
            {
                dragParamsApiSettingModel.State.Edit = -1;
            }
            else if (!editorColumnList.Contains(column.ColumnName) && column.EditorColumn)
            {
                dragParamsApiSettingModel.State.Edit = 0;
            }
            else if (editorColumnList.Contains(column.ColumnName) && column.EditorColumn)
            {
                dragParamsApiSettingModel.State.Edit = 1;
            }

            if (!column.FilterColumn)
            {
                dragParamsApiSettingModel.State.Filter = -1;
            }
            else if (!ss.FilterColumns.Contains(column.ColumnName) && column.FilterColumn)
            {
                dragParamsApiSettingModel.State.Filter = 0;
            }
            else if (ss.FilterColumns.Contains(column.ColumnName) && column.FilterColumn)
            {
                dragParamsApiSettingModel.State.Filter = 1;
            }
            return dragParamsApiSettingModel;
        }

        public void SetSmartDesignSiteSettings(Context context, SiteSettings ss)
        {
            var editorColumnList = ss.EditorColumnHash.Get("General");
            if (ss.EditorColumnHash != null) SiteSettings.EditorColumnHash = ss.EditorColumnHash;
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
            Dictionary<string, DragParamsApiSettingModel> smartDesignParamHash)
        {
            var linkColumnList = ss.Links.Select(link => ss.LinkId(link.SiteId)).ToList();
            foreach (var linkColumn in linkColumnList)
            {
                var dragParamsApiSettingModel = new DragParamsApiSettingModel();
                dragParamsApiSettingModel.LinkName = ss.Destinations
                    .Where(o => o.Key == ss.LinkId(linkColumn))
                    .Select(o => o.Value.Title)
                    .FirstOrDefault();
                dragParamsApiSettingModel.State.Edit =
                        editorColumnList.Contains(linkColumn)
                        ? 1
                        : 0;
                dragParamsApiSettingModel.State.Grid = -1;
                dragParamsApiSettingModel.State.Filter = -1;
                dragParamsApiSettingModel.Category = "Links";
                dragParamsApiSettingModel.Type = "LinkTable";
                if (!SmartDesignParamHash.ContainsKey(linkColumn))
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
            Dictionary<string, DragParamsApiSettingModel> smartDesignParamHash)
        {
            ss.Sources.ForEach(source =>
            {
                var dragParamsApiSettingModel = new DragParamsApiSettingModel();
                SiteSettings.Links.Add(new Link { SiteId = source.Key });
                var linkColumn = ss.LinkId(source.Key);
                dragParamsApiSettingModel.LinkName = source.Value.Title;
                dragParamsApiSettingModel.State.Edit =
                    editorColumnList.Contains(linkColumn)
                    ? 1
                    : 0;
                dragParamsApiSettingModel.State.Grid = -1;
                dragParamsApiSettingModel.State.Filter = -1;
                dragParamsApiSettingModel.Category = "Links";
                dragParamsApiSettingModel.Type = "LinkTable";
                if (!SmartDesignParamHash.ContainsKey(linkColumn))
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
