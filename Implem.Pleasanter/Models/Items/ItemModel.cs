using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class ItemModel : BaseModel
    {
        public long ReferenceId = 0;
        public string ReferenceType = string.Empty;
        public long SiteId = 0;
        public string Title = string.Empty;
        public SiteModel Site = null;
        public string FullText = string.Empty;
        public DateTime SearchIndexCreatedTime = 0.ToDateTime();
        public long SavedReferenceId = 0;
        public string SavedReferenceType = string.Empty;
        public long SavedSiteId = 0;
        public string SavedTitle = string.Empty;
        public SiteModel SavedSite = null;
        public string SavedFullText = string.Empty;
        public DateTime SavedSearchIndexCreatedTime = 0.ToDateTime();

        public bool ReferenceId_Updated(Context context, Column column = null)
        {
            return ReferenceId != SavedReferenceId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != ReferenceId);
        }

        public bool ReferenceType_Updated(Context context, Column column = null)
        {
            return ReferenceType != SavedReferenceType && ReferenceType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ReferenceType);
        }

        public bool SiteId_Updated(Context context, Column column = null)
        {
            return SiteId != SavedSiteId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != SiteId);
        }

        public bool Title_Updated(Context context, Column column = null)
        {
            return Title != SavedTitle && Title != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Title);
        }

        public bool FullText_Updated(Context context, Column column = null)
        {
            return FullText != SavedFullText && FullText != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != FullText);
        }

        public bool SearchIndexCreatedTime_Updated(Context context, Column column = null)
        {
            return SearchIndexCreatedTime != SavedSearchIndexCreatedTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != SearchIndexCreatedTime.Date);
        }

        public ItemModel(
            Context context,
            DataRow dataRow,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
        }

        public ItemModel Get(
            Context context,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(context, Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectItems(
                    tableType: tableType,
                    column: column ?? Rds.ItemsDefaultColumns(),
                    join: join ??  Rds.ItemsJoinDefault(),
                    where: where ?? Rds.ItemsWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public System.Web.Mvc.ContentResult ExportByApi(Context context)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits())
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: Parameters.Api.LimitPerSite));
            }
            switch (Site.ReferenceType)
            {
                case "Issues":
                    if (SiteId == ReferenceId)
                    {
                        return IssueUtilities.ExportByApi(
                            context: context,
                            ss: Site.IssuesSiteSettings(
                                context: context,
                                referenceId: ReferenceId),
                            siteModel: Site);
                    }
                    break;
                case "Results":
                    if (SiteId == ReferenceId)
                    {
                        return ResultUtilities.ExportByApi(
                            context: context,
                            ss: Site.ResultsSiteSettings(
                                context: context,
                                referenceId: ReferenceId),
                            siteModel: Site);
                    }
                    break;
                default:
                    return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            return ApiResults.Get(ApiResponses.BadRequest(context: context));
        }

        public string Index(Context context)
        {
            if (ReferenceId == 0)
            {
                return SiteUtilities.SiteTop(context: context);
            }
            if (ReferenceType != "Sites")
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.NotFound));
            }
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.SiteMenu(context: context, siteModel: Site);
                case "Issues":
                    return IssueUtilities.Index(context: context, ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.Index(context: context, ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string IndexJson(Context context)
        {
            if (ReferenceType != "Sites")
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.IndexJson(context: context, ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.IndexJson(context: context, ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string TrashBox(Context context)
        {
            if (ReferenceId != 0 && ReferenceType != "Sites")
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.NotFound));
            }
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                tableType: Sqls.TableTypes.Deleted);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            if (ReferenceId == 0)
            {
                return SiteUtilities.TrashBox(context: context, ss: Site.SiteSettings);
            }
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.TrashBox(context: context, ss: Site.SiteSettings);
                case "Issues":
                    return IssueUtilities.TrashBox(context: context, ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.TrashBox(context: context, ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string TrashBoxJson(Context context)
        {
            if (ReferenceType != "Sites")
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                tableType: Sqls.TableTypes.Deleted);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.TrashBoxJson(context: context, ss: Site.SiteSettings);
                case "Issues":
                    return IssueUtilities.TrashBoxJson(context: context, ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.TrashBoxJson(context: context, ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Calendar(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Calendar(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.Calendar(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string CalendarJson(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.CalendarJson(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.CalendarJson(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Crosstab(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Crosstab(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.Crosstab(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string CrosstabJson(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.CrosstabJson(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.CrosstabJson(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Gantt(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Gantt(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string GanttJson(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.GanttJson(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BurnDown(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BurnDown(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string BurnDownJson(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BurnDownJson(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BurnDownRecordDetailsJson(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BurnDownRecordDetails(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string TimeSeries(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.TimeSeries(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.TimeSeries(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string TimeSeriesJson(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.TimeSeriesJson(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.TimeSeriesJson(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Kamban(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Kamban(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.Kamban(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string KambanJson(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.KambanJson(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.KambanJson(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string ImageLib(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ImageLib(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.ImageLib(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string ImageLibJson(Context context)
        {
            SetSite(context: context);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ImageLibJson(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.ImageLibJson(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string New(Context context)
        {
            SetSite(
                context: context,
                siteOnly: true,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.EditorNew(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.EditorNew(
                        context: context,
                        ss: Site.SiteSettings);
                case "Wikis":
                    return WikiUtilities.EditorNew(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string NewJson(Context context)
        {
            return new ResponseCollection()
                .ReplaceAll("#MainContainer", New(context: context))
                .WindowScrollTop()
                .FocusMainForm()
                .ClearFormData()
                .PushState("Edit", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        "Items",
                        ReferenceId.ToString(),
                        "New"
                    }))
                .ToJson();
        }

        public string NewOnGrid(Context context)
        {
            SetSite(
                context: context,
                siteOnly: true,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string CancelNewRow(Context context)
        {
            SetSite(
                context: context,
                siteOnly: true,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.CancelNewRow(
                        context: context,
                        ss: Site.SiteSettings,
                        id: context.Forms.Long("CancelRowId"));
                case "Results":
                    return ResultUtilities.CancelNewRow(
                        context: context,
                        ss: Site.SiteSettings,
                        id: context.Forms.Long("CancelRowId"));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Editor(Context context)
        {
            SetSite(context: context);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Editor(
                        context: context,
                        siteId: ReferenceId,
                        clearSessions: true);
                case "Issues":
                    return IssueUtilities.Editor(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        issueId: ReferenceId,
                        clearSessions: true);
                case "Results":
                    return ResultUtilities.Editor(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        resultId: ReferenceId,
                        clearSessions: true);
                case "Wikis":
                    return WikiUtilities.Editor(
                        context: context,
                        ss: Site.WikisSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        wikiId: ReferenceId,
                        clearSessions: true);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound));
            }
        }

        public string LinkTable(Context context)
        {
            var dataTableName = context.Forms.Data("TableId");
            return new ResponseCollection()
                .ReplaceAll("#" + dataTableName, new HtmlBuilder()
                    .LinkTable(
                        context: context,
                        siteId: context.Forms.Long("TableSiteId"),
                        direction: context.Forms.Data("Direction"),
                        dataTableName: dataTableName))
                .ToJson();
        }

        public string Import(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Import(
                        context: context,
                        siteModel: Site);
                case "Results":
                    return ResultUtilities.Import(
                        context: context,
                        siteModel: Site);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string OpenExportSelectorDialog(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.OpenExportSelectorDialog(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        siteModel: Site);
                case "Results":
                    return ResultUtilities.OpenExportSelectorDialog(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        siteModel: Site);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string OpenSetNumericRangeDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (context.CanRead(Site.SiteSettings))
            {
                var columnName = context.Forms.ControlId()
                    .Replace("ViewFilters__", string.Empty)
                    .Replace("ViewFiltersOnGridHeader__", string.Empty)
                    .Replace("_NumericRange", string.Empty);
                var column = Site.SiteSettings.GetColumn(
                    context: context,
                    columnName: columnName);
                return new ResponseCollection()
                    .Html(
                        "#SetNumericRangeDialog",
                        new HtmlBuilder().SetNumericRangeDialog(
                            context: context,
                            ss: Site.SiteSettings,
                            column: column,
                            itemfilter: true))
                    .ToJson();
            }
            else
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string OpenSetDateRangeDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (context.CanRead(Site.SiteSettings))
            {
                var columnName = context.Forms.ControlId()
                    .Replace("ViewFilters__", string.Empty)
                    .Replace("ViewFiltersOnGridHeader__", string.Empty)
                    .Replace("_DateRange", string.Empty);
                var column = Site.SiteSettings.GetColumn(
                    context: context,
                    columnName: columnName);
                return new ResponseCollection()
                    .Html(
                        "#SetDateRangeDialog",
                        new HtmlBuilder().SetDateRangeDialog(
                            context: context,
                            ss: Site.SiteSettings,
                            column: column,
                            itemfilter: true))
                    .ToJson();
            }
            else
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public ResponseFile Export(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Export(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: false),
                        siteModel: Site);
                case "Results":
                    return ResultUtilities.Export(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: false),
                        siteModel: Site);
                default:
                    return null;
            }
        }

        public string ExportAsync(Context context)
        {
            SetSite(context: context);
            var export = Site.SiteSettings.Exports?
                .Where(exp => exp.Id == context.Forms.Int("ExportId"))?
                .FirstOrDefault();
            if(export?.ExecutionType != Libraries.Settings.Export.ExecutionTypes.MailNotify)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            if(MailAddressUtilities.Get(context: context, context.UserId).IsNullOrEmpty())
            {
                return Messages.ResponseExportNotSetEmail(
                    context: context, 
                    target: null, 
                    $"{context.User.Name}<{context.User.LoginId}>").ToJson();
            }
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ExportAsync(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: false),
                        siteModel: Site);
                case "Results":
                    return ResultUtilities.ExportAsync(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: false),
                        siteModel: Site);
                default:
                    return Error.Types.InvalidRequest.MessageJson(context: context);
            }
        }

        public ResponseFile ExportCrosstab(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ExportCrosstab(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        siteModel: Site);
                case "Results":
                    return ResultUtilities.ExportCrosstab(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        siteModel: Site);
                default:
                    return null;
            }
        }

        public string SearchDropDown(Context context)
        {
            SetSite(context: context);
            var controlId = context.Forms.Data("DropDownSearchTarget");
            var filter = controlId.StartsWith("ViewFilters__") 
                || controlId.StartsWith("ViewFiltersOnGridHeader__");
            var searchText = context.Forms.Data("DropDownSearchText");
            string parentClass = context.Forms.Data("DropDownSearchParentClass");
            var parentDataId = context.Forms.Data("DropDownSearchParentDataId");
            var parentIds = parentDataId.Deserialize<long[]>();
            switch (context.Forms.ControlId())
            {
                case "DropDownSearchResults":
                    return
                        AppendSearchDropDown(
                            context: context,
                            controlId: controlId,
                            searchText: searchText,
                            filter: filter,
                            parentClass: parentClass,
                            parentIds: parentIds);
                default:
                    return SearchDropDown(
                        context: context,
                        controlId: controlId,
                        searchText: searchText,
                        filter: filter,
                        parentClass: parentClass,
                        parentIds: parentIds);
            }
        }

        private string AppendSearchDropDown(
            Context context,
            string controlId,
            string searchText,
            bool filter,
            string parentClass = "",
            IEnumerable<long> parentIds = null)
        {
            var offset = context.Forms.Int("DropDownSearchResultsOffset");
            var column = SearchDropDownColumn(
                context: context,
                controlId: controlId,
                searchText: searchText,
                filter: filter,
                offset: offset,
                parentClass: parentClass,
                parentIds: parentIds);
            var nextOffset = Paging.NextOffset(
                offset: offset,
                totalCount: column.TotalCount,
                pageSize: Parameters.General.DropDownSearchPageSize);
            return new ResponseCollection()
                .Append("#DropDownSearchResults", new HtmlBuilder()
                    .SelectableItems(
                        listItemCollection: column?.EditChoices(
                            context: context,
                            addNotSet: offset == 0)))
                .Val("#DropDownSearchResultsOffset", nextOffset)
                .ToJson();
        }

        private string SearchDropDown(
            Context context,
            string controlId,
            string searchText,
            bool filter,
            string parentClass = "",
            IEnumerable<long> parentIds = null)
        {
            var column = SearchDropDownColumn(
                context: context,
                controlId: controlId,
                searchText: searchText,
                filter: filter,
                parentClass: parentClass,
                parentIds: parentIds);
            var nextOffset = Paging.NextOffset(
                offset: 0,
                totalCount: column.TotalCount,
                pageSize: Parameters.General.DropDownSearchPageSize);
            return new ResponseCollection()
                .ReplaceAll(
                    "#DropDownSearchResults",
                    new HtmlBuilder().Selectable(
                        controlId: "DropDownSearchResults",
                        listItemCollection: column?.EditChoices(
                            context: context,
                            addNotSet: true),
                        action: "SearchDropDown",
                        method: "post"))
                .Val("#DropDownSearchResultsOffset", nextOffset)
                .ClearFormData("DropDownSearchResults")
                .ToJson();
        }

        public string RelatingDropDown(Context context)
        {
            SetSite(context: context);
            var controlId = context.Forms.Data("RelatingDropDownControlId");
            var filter = controlId.StartsWith("ViewFilters__")
                || controlId.StartsWith("ViewFiltersOnGridHeader__");
            string parentClass = context.Forms.Data("RelatingDropDownParentClass");
            var selectedValue = context.Forms.Data("RelatingDropDownSelected");
            var parentDataId = context.Forms.Data("RelatingDropDownParentDataId");
            var parentIds = parentDataId.Deserialize<long[]>();
            return RelatingDropDown(
                context: context,
                controlId: controlId,
                selectedValue: selectedValue,
                searchText: "",
                filter: filter,
                parentClass: parentClass,
                parentIds: parentIds);
        }

        private string RelatingDropDown(
            Context context,
            string controlId,
            string searchText,
            string selectedValue,
            bool filter,
            string parentClass = "",
            IEnumerable<long> parentIds = null)
        {
            var column = SearchDropDownColumn(
                context: context,
                controlId: controlId,
                searchText: searchText,
                filter: filter,
                parentClass: parentClass,
                parentIds: parentIds,
                searchColumnOnly: false);
            Dictionary<string, ControlData> optionCollection
                = new Dictionary<string, ControlData>();
            if (filter || parentIds?.Any() == true)
            {
                optionCollection = column?.EditChoices(context: context, addNotSet: filter);
            }
            if (parentIds?.Any() != true)
            {
                selectedValue = null;
            }
            else if (!filter)
            {
                selectedValue = selectedValue.Deserialize<string[]>().FirstOrDefault();
            }
            return new ResponseCollection()
                .Html(
                    "#" + controlId,
                    new HtmlBuilder().OptionCollection(
                        context: context,
                        optionCollection: optionCollection,
                        selectedValue: selectedValue,
                        multiple: filter,
                        addSelectedValue: false,
                        insertBlank: !filter,
                        column: column))
                .Invoke("callbackRelatingColumn", "#" + controlId)
                .ClearFormData("#" + controlId)
                .ToJson();
        }

        public string SelectSearchDropDown(Context context)
        {
            SetSite(context: context);
            var controlId = context.Forms.Data("DropDownSearchTarget");
            var filter = controlId.StartsWith("ViewFilters__") 
                || controlId.StartsWith("ViewFiltersOnGridHeader__");
            var searchText = context.Forms.Data("DropDownSearchText");
            var column = SearchDropDownColumn(
                context: context,
                controlId: controlId,
                searchText: searchText,
                filter: filter);
            var selected = context.Forms.List("DropDownSearchResults");
            var multiple = context.Forms.Bool("DropDownSearchMultiple");
            if (multiple)
            {
                return SelectSearchDropDownResponse(
                    context: context,
                    controlId: controlId,
                    column: column,
                    selected: selected,
                    filter: filter,
                    multiple: multiple);
            }
            else if (selected.Count() != 1)
            {
                return new ResponseCollection()
                    .Message(Messages.SelectOne(context: context))
                    .ToJson();
            }
            else
            {
                return SelectSearchDropDownResponse(
                    context: context,
                    controlId: controlId,
                    column: column,
                    selected: selected,
                    filter: filter,
                    multiple: multiple);
            }
        }

        private Column SearchDropDownColumn(
            Context context,
            string controlId,
            string searchText,
            bool filter,
            int offset = 0,
            string parentClass = "",
            IEnumerable<long> parentIds = null,
            bool searchColumnOnly = true)
        {
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteModel: Site,
                referenceId: ReferenceId,
                setSiteIntegration: true);
            var column = ss.GetColumn(
                context: context,
                columnName: filter
                    ? (controlId.StartsWith("ViewFilters__")
                        ? controlId.Substring("ViewFilters__".Length)
                        : controlId.Substring("ViewFiltersOnGridHeader__".Length))
                    : controlId.Split_2nd('_'));
            var searchIndexes = searchText.SearchIndexes();
            if (column?.Linked() == true)
            {
                column?.SetChoiceHash(
                    context: context,
                    siteId: column.SiteId,
                    linkHash: column.SiteSettings.LinkHash(
                        context: context,
                        columnName: column.Name,
                        searchIndexes: searchIndexes,
                        searchColumnOnly: searchColumnOnly,
                        offset: offset,
                        parentClass: parentClass,
                        parentIds: parentIds,
                        setTotalCount: true),
                    searchIndexes: searchIndexes);
            }
            else
            {
                ss.SetChoiceHash(
                    context: context,
                    columnName: column?.ColumnName,
                    searchIndexes: searchIndexes,
                    setTotalCount: true);
            }
            return column;
        }

        private static string SelectSearchDropDownResponse(
            Context context,
            string controlId,
            Column column,
            List<string> selected,
            bool filter,
            bool multiple)
        {
            if (selected.Any()
                && column.UseSearch == true
                && column.Type != Column.Types.Normal
                && !selected.All(o => column.ChoiceHash.ContainsKey(o)))
            {
                switch (column.Type)
                {
                    case Column.Types.User:
                        selected
                            .Select(userId => SiteInfo.User(
                                context: context,
                                userId: userId.ToInt()))
                            .Where(o => !o.Anonymous())
                            .ForEach(user =>
                                column.ChoiceHash.AddIfNotConainsKey(
                                    user.Id.ToString(),
                                    new Choice(
                                        value: user.Id.ToString(),
                                        text: user.Name)));
                        break;
                    default:
                        selected
                            .Select(id =>
                            new
                            {
                                Id = id.ToInt(),
                                Name = SiteInfo.Name(
                                    context: context,
                                    id: id.ToInt(),
                                    type: column.Type)
                            })
                            .Where(o => o.Id != 0 && !o.Name.IsNullOrEmpty())
                            .ForEach(data =>
                                column.ChoiceHash.AddIfNotConainsKey(
                                    data.Id.ToString(),
                                    new Choice(
                                        value: data.Id.ToString(),
                                        text: data.Name)));
                        break;
                }
            }
            if (selected.Any() &&
                !selected.All(o => column.ChoiceHash.ContainsKey(o)))
            {
                column.SiteSettings.SetChoiceHash(
                    context: context,
                    columnName: column.ColumnName,
                    selectedValues: selected);
            }
            var optionCollection = column?.EditChoices(
                context: context,
                addNotSet: true)?
                    .Where(o => selected.Contains(o.Key))
                    .ToDictionary(o => o.Key, o => o.Value);
            return optionCollection?.Any() == true || !selected.Any()
                ? new ResponseCollection()
                    .CloseDialog("#DropDownSearchDialog")
                    .Html("[id=\"" + controlId + "\"]", new HtmlBuilder()
                        .OptionCollection(
                            context: context,
                            optionCollection: optionCollection,
                            selectedValue: SelectSearchDropDownSelectedValue(
                                context: context,
                                selected: selected,
                                filter: filter,
                                multiple: multiple),
                            multiple: multiple,
                            insertBlank: !filter))
                    .Invoke("setDropDownSearch")
                    .Trigger("#" + controlId, "change")
                    .ToJson()
                : new ResponseCollection()
                    .Message(Messages.NotFound(context: context))
                    .ToJson();
        }

        public static string SelectSearchDropDownSelectedValue(
            Context context, List<string> selected, bool filter, bool multiple)
        {
            if (multiple)
            {
                return selected.ToJson();
            }
            else
            {
                var value = selected.FirstOrDefault();
                return !filter && value == "\t"
                    ? null
                    : selected.FirstOrDefault();
            }
        }

        public string GridRows(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.GridRows(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        offset: context.Forms.Int("GridOffset"));
                case "Results":
                    return ResultUtilities.GridRows(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        offset: context.Forms.Int("GridOffset"));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string ReloadRow(Context context)
        {
            SetSite(context: context);
            var id = context.Forms.Long("Id");
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ReloadRow(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        issueId: id);
                case "Results":
                    return ResultUtilities.ReloadRow(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        resultId: id);
                default:
                    return ItemUtilities.ClearItemDataResponse(
                        context: context,
                        ss: Site.SiteSettings,
                        id: id)
                            .Remove($"[data-id=\"{id}\"]")
                            .Message(Messages.NotFound(context: context))
                            .ToJson();
            }
        }

        public string CopyRow(Context context)
        {
            SetSite(
                context: context,
                siteOnly: true,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string TrashBoxGridRows(Context context)
        {
            SetSite(context: context, tableType: Sqls.TableTypes.Deleted);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.GridRows(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            tableType: Sqls.TableTypes.Deleted),
                        offset: context.Forms.Int("GridOffset"),
                        action: "TrashBoxGridRows");
                case "Results":
                    return ResultUtilities.GridRows(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            tableType: Sqls.TableTypes.Deleted),
                        offset: context.Forms.Int("GridOffset"),
                        action: "TrashBoxGridRows");
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string ImageLibNext(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ImageLibNext(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        offset: context.Forms.Int("ImageLibOffset"));
                case "Results":
                    return ResultUtilities.ImageLibNext(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        offset: context.Forms.Int("ImageLibOffset"));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public System.Web.Mvc.ContentResult GetByApi(Context context, bool internalRequest = false)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits())
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: Parameters.Api.LimitPerSite));
            }
            switch (Site.ReferenceType)
            {
                case "Issues":
                    if (SiteId == ReferenceId)
                    {
                        return IssueUtilities.GetByApi(
                            context: context,
                            ss: Site.IssuesSiteSettings(
                                context: context,
                                referenceId: ReferenceId),
                            internalRequest: internalRequest);
                    }
                    else
                    {
                        return IssueUtilities.GetByApi(
                            context: context,
                            ss: Site.IssuesSiteSettings(
                                context: context,
                                referenceId: ReferenceId),
                            issueId: ReferenceId,
                            internalRequest: internalRequest);
                    }
                case "Results":
                    if (SiteId == ReferenceId)
                    {
                        return ResultUtilities.GetByApi(
                            context: context,
                            ss: Site.ResultsSiteSettings(
                                context: context,
                                referenceId: ReferenceId),
                            internalRequest: internalRequest);
                    }
                    else
                    {
                        return ResultUtilities.GetByApi(
                            context: context,
                            ss: Site.ResultsSiteSettings(
                                context: context,
                                referenceId: ReferenceId),
                            resultId: ReferenceId,
                            internalRequest: internalRequest);
                    }
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public string Create(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Create(
                        context: context,
                        parentId: Site.SiteId,
                        inheritPermission: Site.InheritPermission);
                case "Issues":
                    return IssueUtilities.Create(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId));
                case "Results":
                    return ResultUtilities.Create(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public System.Web.Mvc.ContentResult CreateByApi(Context context)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits())
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: Parameters.Api.LimitPerSite));
            }
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.CreateByApi(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId));
                case "Results":
                    return ResultUtilities.CreateByApi(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId));
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public string Templates(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Templates(
                        context: context,
                        parentId: Site.SiteId,
                        inheritPermission: Site.InheritPermission);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string CreateByTemplate(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.CreateByTemplate(
                        context: context,
                        parentId: Site.SiteId,
                        inheritPermission: Site.InheritPermission);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string SiteMenu(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.SiteMenuJson(
                        context: context,
                        siteModel: Site);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Update(Context context)
        {
            SetSite(context: context);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Update(
                        context: context,
                        siteModel: Site,
                        siteId: ReferenceId);
                case "Issues":
                    return IssueUtilities.Update(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.Update(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.Update(
                        context: context,
                        ss: Site.WikisSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string OpenBulkUpdateSelectorDialog(Context context)
        {
            SetSite(context: context, initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.OpenBulkUpdateSelectorDialog(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
                case "Results":
                    return ResultUtilities.OpenBulkUpdateSelectorDialog(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BulkUpdateSelectChanged(Context context)
        {
            SetSite(context: context, initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BulkUpdateSelectChanged(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
                case "Results":
                    return ResultUtilities.BulkUpdateSelectChanged(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BulkUpdate(Context context)
        {
            SetSite(context: context);
            switch (Site.SiteSettings.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BulkUpdate(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId));
                case "Results":
                    return ResultUtilities.BulkUpdate(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string UpdateByGrid(Context context)
        {
            SetSite(context: context);
            switch (Site.SiteSettings.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.UpdateByGrid(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId));
                case "Results":
                    return ResultUtilities.UpdateByGrid(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public System.Web.Mvc.ContentResult UpdateByApi(Context context)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits())
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: Parameters.Api.LimitPerSite));
            }
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.UpdateByApi(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.UpdateByApi(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        resultId: ReferenceId);
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public string DeleteComment(Context context)
        {
            SetSite(context: context);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Update(
                        context: context,
                        siteModel: Site,
                        siteId: ReferenceId);
                case "Issues":
                    return IssueUtilities.Update(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.Update(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.Update(
                        context: context,
                        ss: Site.WikisSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Copy(Context context)
        {
            SetSite(context: context);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Copy(
                        context: context,
                        siteModel: Site);
                case "Issues":
                    return IssueUtilities.Copy(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.Copy(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        resultId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string MoveTargets(Context context)
        {
            SetSite(context: context);
            return new ResponseCollection().Html("#MoveTargets", new HtmlBuilder()
                .OptionCollection(
                    context: context,
                    optionCollection: Site.SiteSettings.MoveTargetsSelectableOptions(
                        context: context,
                        enabled: true)))
                            .ToJson();
        }

        public string Move(Context context)
        {
            SetSite(context: context);
            switch (ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Move(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                    issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.Move(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                    resultId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BulkMove(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BulkMove(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
                case "Results":
                    return ResultUtilities.BulkMove(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Delete(Context context)
        {
            SetSite(context: context);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Delete(
                        context: context,
                        ss: Site.SitesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        siteId: ReferenceId);
                case "Issues":
                    return IssueUtilities.Delete(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.Delete(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.Delete(
                        context: context,
                        ss: Site.WikisSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public System.Web.Mvc.ContentResult DeleteByApi(Context context)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits())
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: Parameters.Api.LimitPerSite));
            }
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.DeleteByApi(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.DeleteByApi(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        resultId: ReferenceId);
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public string BulkDelete(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BulkDelete(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
                case "Results":
                    return ResultUtilities.BulkDelete(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public System.Web.Mvc.ContentResult BulkDeleteByApi(Context context)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits())
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: Parameters.Api.LimitPerSite));
            }
            if (context.RequestDataString.Deserialize<ApiDeleteOption>()?.PhysicalDelete == true)
            {
                switch (Site.ReferenceType)
                {
                    case "Issues":
                        return IssueUtilities.PhysicalBulkDeleteByApi(
                            context: context,
                            ss: Site.IssuesSiteSettings(
                                context: context,
                                referenceId: ReferenceId,
                                setSiteIntegration: true,
                                tableType: Sqls.TableTypes.Deleted));
                    case "Results":
                        return ResultUtilities.PhysicalBulkDeleteByApi(
                            context: context,
                            ss: Site.ResultsSiteSettings(
                                context: context,
                                referenceId: ReferenceId,
                                setSiteIntegration: true,
                                tableType: Sqls.TableTypes.Deleted));
                    default:
                        return ApiResults.Get(ApiResponses.NotFound(context: context));
                }
            }
            else
            {
                switch (Site.ReferenceType)
                {
                    case "Issues":
                        return IssueUtilities.BulkDeleteByApi(
                            context: context,
                            ss: Site.IssuesSiteSettings(
                                context: context,
                                referenceId: ReferenceId));
                    case "Results":
                        return ResultUtilities.BulkDeleteByApi(
                            context: context,
                            ss: Site.ResultsSiteSettings(
                                context: context,
                                referenceId: ReferenceId));
                    default:
                        return ApiResults.Get(ApiResponses.NotFound(context: context));
                }
            }
        }

        public string DeleteHistory(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                tableType: Sqls.TableTypes.History);
            if (SiteId == ReferenceId)
            {
                return SiteUtilities.DeleteHistory(
                    context: context,
                    ss: Site.SiteSettings,
                    siteId: ReferenceId);
            }
            else
            {
                switch (Site.ReferenceType)
                {
                    case "Issues":
                        return IssueUtilities.DeleteHistory(
                            context: context,
                            ss: Site.SiteSettings,
                            issueId: ReferenceId);
                    case "Results":
                        return ResultUtilities.DeleteHistory(
                            context: context,
                            ss: Site.SiteSettings,
                            resultId: ReferenceId);
                    case "Wikis":
                        return WikiUtilities.DeleteHistory(
                            context: context,
                            ss: Site.SiteSettings,
                            wikiId: ReferenceId);
                    default:
                        return Messages.ResponseNotFound(context: context).ToJson();
                }
            }
        }

        public string PhysicalBulkDelete(Context context)
        {
            SetSite(context: context, tableType: Sqls.TableTypes.Deleted);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.PhysicalBulkDelete(
                        context: context,
                        ss: Site.SitesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            tableType: Sqls.TableTypes.Deleted));
                case "Issues":
                    return IssueUtilities.PhysicalBulkDelete(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            tableType: Sqls.TableTypes.Deleted));
                case "Results":
                    return ResultUtilities.PhysicalBulkDelete(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            tableType: Sqls.TableTypes.Deleted));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Restore(Context context)
        {
            SetSite(context: context, tableType: Sqls.TableTypes.Deleted);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Restore(
                        context: context,
                        ss: SiteSettingsUtilities.SitesSiteSettings(
                            context: context,
                            siteModel: Site,
                            referenceId: ReferenceId,
                            tableType: Sqls.TableTypes.Deleted));
                case "Issues":
                    return IssueUtilities.Restore(
                        context: context,
                        ss: SiteSettingsUtilities.IssuesSiteSettings(
                            context: context,
                            siteModel: Site,
                            referenceId: ReferenceId,
                            tableType: Sqls.TableTypes.Deleted));
                case "Results":
                    return ResultUtilities.Restore(
                        context: context,
                        ss: SiteSettingsUtilities.ResultsSiteSettings(
                            context: context,
                            siteModel: Site,
                            referenceId: ReferenceId,
                            tableType: Sqls.TableTypes.Deleted));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string RestoreFromHistory(Context context)
        {
            SetSite(context: context, tableType: Sqls.TableTypes.History);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.RestoreFromHistory(
                        context: context,
                        ss: SiteSettingsUtilities.SitesSiteSettings(
                            context: context,
                            siteModel: Site,
                            referenceId: ReferenceId,
                            tableType: Sqls.TableTypes.History),
                        siteId: ReferenceId);
                case "Issues":
                    return IssueUtilities.RestoreFromHistory(
                        context: context,
                        ss: SiteSettingsUtilities.IssuesSiteSettings(
                            context: context,
                            siteModel: Site,
                            referenceId: ReferenceId,
                            tableType: Sqls.TableTypes.History),
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.RestoreFromHistory(
                        context: context,
                        ss: SiteSettingsUtilities.ResultsSiteSettings(
                            context: context,
                            siteModel: Site,
                            referenceId: ReferenceId,
                            tableType: Sqls.TableTypes.History),
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.RestoreFromHistory(
                        context: context,
                        ss: SiteSettingsUtilities.WikisSiteSettings(
                            context: context,
                            siteModel: Site,
                            referenceId: ReferenceId,
                            tableType: Sqls.TableTypes.History),
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string EditSeparateSettings(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.EditSeparateSettings(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        issueId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Separate(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Separate(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        issueId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Histories(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                tableType: Sqls.TableTypes.NormalAndHistory);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Histories(
                        context: context,
                        siteModel: Site);
                case "Issues":
                    return IssueUtilities.Histories(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.Histories(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.Histories(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string History(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                tableType: Sqls.TableTypes.History);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.History(
                        context: context,
                        siteModel: Site);
                case "Issues":
                    return IssueUtilities.History(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.History(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.History(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string EditorJson(Context context)
        {
            SetSite(context: context, initSiteSettings: true);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.EditorJson(
                context: context,
                        siteModel: Site);
                case "Issues":
                    return IssueUtilities.EditorJson(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.EditorJson(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.EditorJson(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string UpdateByCalendar(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.UpdateByCalendar(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.UpdateByCalendar(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string UpdateByKamban(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.UpdateByKamban(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                case "Results":
                    return ResultUtilities.UpdateByKamban(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            setAllChoices: true));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string OpenImportSitePackageDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return Libraries.SitePackages.Utilities.OpenImportSitePackageDialog(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                case "Results":
                case "Wikis":
                    return Libraries.SitePackages.Utilities.OpenImportSitePackageDialog(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string ImportSitePackage(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return Libraries.SitePackages.Utilities.ImportSitePackage(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                case "Results":
                case "Wikis":
                default:
                    throw new NotImplementedException();
            }
        }

        public string OpenExportSitePackageDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return Libraries.SitePackages.Utilities.OpenExportSitePackageDialog(
                        context: context,
                        ss: Site.SiteSettings,
                        recursive: true);
                case "Issues":
                case "Results":
                case "Wikis":
                    return Libraries.SitePackages.Utilities.OpenExportSitePackageDialog(
                        context: context,
                        ss: Site.SiteSettings,
                        recursive: false);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public ResponseFile ExportSitePackage(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return Libraries.SitePackages.Utilities.ExportSitePackage(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                case "Results":
                case "Wikis":
                    return Libraries.SitePackages.Utilities.ExportSitePackage(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return null;
            }
        }

        public string LockTable(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                case "Results":
                    return SiteUtilities.LockTable(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string UnlockTable(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                case "Results":
                    return SiteUtilities.UnlockTable(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string ForceUnlockTable(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                case "Results":
                    return SiteUtilities.ForceUnlockTable(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string UnlockRecord(Context context, long id)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.UnlockRecord(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: id);
                case "Results":
                    return ResultUtilities.UnlockRecord(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: id);
                case "Wikis":
                    return WikiUtilities.UnlockRecord(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: id);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string SynchronizeTitles(Context context)
        {
            SetSite(context: context, initSiteSettings: true);
            return SiteUtilities.SynchronizeTitles(
                context: context,
                siteModel: Site);
        }

        public string SynchronizeSummaries(Context context)
        {
            SetSite(context: context);
            return SiteUtilities.SynchronizeSummaries(
                context: context,
                siteModel: Site);
        }

        public string SynchronizeFormulas(Context context)
        {
            SetSite(context: context);
            return SiteUtilities.SynchronizeFormulas(
                context: context,
                siteModel: Site);
        }

        private void SetSite(
            Context context,
            bool siteOnly = false,
            bool initSiteSettings = false,
            bool setSiteIntegration = false,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Site = GetSite(
                context: context,
                siteOnly: siteOnly,
                initSiteSettings: initSiteSettings,
                setSiteIntegration: setSiteIntegration,
                tableType: tableType);
        }

        public SiteModel GetSite(
            Context context,
            bool siteOnly = false,
            bool initSiteSettings = false,
            bool setSiteIntegration = false,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            SiteModel siteModel;
            if (ReferenceType == "Sites" && context.Forms.Exists("Ver"))
            {
                siteModel = new SiteModel();
                siteModel.Get(
                    context: context,
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId(ReferenceId)
                        .Ver(context.Forms.Int("Ver")),
                    tableType: Sqls.TableTypes.NormalAndHistory);
                siteModel.VerType =  context.Forms.Bool("Latest")
                    ? Versions.VerTypes.Latest
                    : Versions.VerTypes.History;
            }
            else
            {
                siteModel = siteOnly
                    ? new SiteModel(
                        context: context,
                        siteId: ReferenceId)
                    : new SiteModel(
                        context: context,
                        siteId: ReferenceType == "Sites"
                            ? ReferenceId
                            : SiteId);
            }
            if (initSiteSettings)
            {
                siteModel.SiteSettings = SiteSettingsUtilities.Get(
                    context: context,
                    siteModel: siteModel,
                    referenceId: ReferenceId,
                    setSiteIntegration: setSiteIntegration,
                    tableType: tableType);
            }
            return siteModel;
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(Context context, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "ReferenceId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ReferenceId = dataRow[column.ColumnName].ToLong();
                                SavedReferenceId = ReferenceId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "ReferenceType":
                            ReferenceType = dataRow[column.ColumnName].ToString();
                            SavedReferenceType = ReferenceType;
                            break;
                        case "SiteId":
                            SiteId = dataRow[column.ColumnName].ToLong();
                            SavedSiteId = SiteId;
                            break;
                        case "Title":
                            Title = dataRow[column.ColumnName].ToString();
                            SavedTitle = Title;
                            break;
                        case "FullText":
                            FullText = dataRow[column.ColumnName].ToString();
                            SavedFullText = FullText;
                            break;
                        case "SearchIndexCreatedTime":
                            SearchIndexCreatedTime = dataRow[column.ColumnName].ToDateTime();
                            SavedSearchIndexCreatedTime = SearchIndexCreatedTime;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory":
                            VerType = dataRow.Bool(column.ColumnName)
                                ? Versions.VerTypes.History
                                : Versions.VerTypes.Latest; break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    Class(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedClass(
                                        columnName: column.Name,
                                        value: Class(columnName: column.Name));
                                    break;
                                case "Num":
                                    Num(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDecimal());
                                    SavedNum(
                                        columnName: column.Name,
                                        value: Num(columnName: column.Name));
                                    break;
                                case "Date":
                                    Date(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SavedDate(
                                        columnName: column.Name,
                                        value: Date(columnName: column.Name));
                                    break;
                                case "Description":
                                    Description(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedDescription(
                                        columnName: column.Name,
                                        value: Description(columnName: column.Name));
                                    break;
                                case "Check":
                                    Check(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SavedCheck(
                                        columnName: column.Name,
                                        value: Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    Attachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SavedAttachments(
                                        columnName: column.Name,
                                        value: Attachments(columnName: column.Name).ToJson());
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return Updated()
                || ReferenceId_Updated(context: context)
                || Ver_Updated(context: context)
                || ReferenceType_Updated(context: context)
                || SiteId_Updated(context: context)
                || Title_Updated(context: context)
                || FullText_Updated(context: context)
                || SearchIndexCreatedTime_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ItemModel()
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ItemModel(Context context, long referenceId)
        {
            OnConstructing(context: context);
            ReferenceId = referenceId;
            Get(
                context: context,
                join: Rds.ItemsJoin().Add(
                    new SqlJoin(
                        "[Sites]",
                        SqlJoin.JoinTypes.Inner,
                        "[Sites].[SiteId] = [Items].[SiteId] and [Sites].[TenantId] = @_T")));
            OnConstructed(context: context);
        }
    }
}
