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

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class SmartDesignApiModel
    {
        public SiteSettings SiteSettings  = new SiteSettings();

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

        public static Dictionary<string, DragParamsApiSettingModel> GetSmartDesignParam(Context context,SiteSettings ss)
        {
            var editorColumnList = ss.GetEditorColumnNames();
            var gridColumnList = ss.GridColumns;
            var filterColumnList = ss.FilterColumns;
            var defaultColumns = ss.GetDefaultColumns(context);
            var smartDesignParamHash = new Dictionary<string, DragParamsApiSettingModel>();
            ss.Columns.ForEach(column =>
            {
                var dragParamsApiSettingModel = new DragParamsApiSettingModel();
                dragParamsApiSettingModel.SetType(column);
                dragParamsApiSettingModel.SetCategory(column);
                if (!column.GridColumn)
                {
                    dragParamsApiSettingModel.State.Grid = -1;
                }
                else if (!gridColumnList.Contains(column.ColumnName) && column.GridColumn)
                {
                    dragParamsApiSettingModel.State.Grid = 0;
                }
                else if (gridColumnList.Contains(column.ColumnName) && column.GridColumn)
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
                else if (!filterColumnList.Contains(column.ColumnName) && column.FilterColumn)
                {
                    dragParamsApiSettingModel.State.Filter = 0;
                }
                else if (filterColumnList.Contains(column.ColumnName) && column.FilterColumn)
                {
                    dragParamsApiSettingModel.State.Filter = 1;
                }               
                smartDesignParamHash.Add(column.ColumnName, dragParamsApiSettingModel);
            });
            return smartDesignParamHash;
        }
        public void SetSmartDesignSiteSettings(Context context, SiteSettings ss)
        {
            var editorColumnList = ss.EditorColumnHash.Get("General");
            if (ss.EditorColumnHash != null) SiteSettings.EditorColumnHash = ss.EditorColumnHash;
            if (ss.GridColumns != null) SiteSettings.GridColumns = ss.GridColumns;
            if (ss.FilterColumns != null) SiteSettings.FilterColumns = ss.FilterColumns;
            if (ss.Links != null) SiteSettings.Links = ss.Links;
            if (ss.SectionLatestId != null) SiteSettings.SectionLatestId = ss.SectionLatestId;
            if (ss.Sections != null) SiteSettings.Sections = ss.Sections;
            if (ss.Columns != null) SiteSettings.Columns = ss.Columns.Where(o => editorColumnList.Contains(o.ColumnName) || DefaultColumns.Contains(o.ColumnName)).ToList();
        }
    }
}
