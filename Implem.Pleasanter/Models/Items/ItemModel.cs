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
        [NonSerialized] public long SavedReferenceId = 0;
        [NonSerialized] public string SavedReferenceType = string.Empty;
        [NonSerialized] public long SavedSiteId = 0;
        [NonSerialized] public string SavedTitle = string.Empty;
        [NonSerialized] public SiteModel SavedSite = null;
        [NonSerialized] public string SavedFullText = string.Empty;
        [NonSerialized] public DateTime SavedSearchIndexCreatedTime = 0.ToDateTime();

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

        public ItemModel(Context context, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null) Set(context, dataRow, tableAlias);
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

        public string Index(Context context)
        {
            if (ReferenceId == 0)
            {
                return SiteUtilities.SiteTop(context: context);
            }
            if (ReferenceType != "Sites")
            {
                return HtmlTemplates.Error(context, Error.Types.NotFound);
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
                    return HtmlTemplates.Error(context, Error.Types.NotFound);
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
                return HtmlTemplates.Error(context, Error.Types.NotFound);
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
                    return HtmlTemplates.Error(context, Error.Types.NotFound);
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
                    return HtmlTemplates.Error(context, Error.Types.NotFound);
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
                            setSiteIntegration: true));
                case "Results":
                    return ResultUtilities.Crosstab(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
                default:
                    return HtmlTemplates.Error(context, Error.Types.NotFound);
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
                            setSiteIntegration: true));
                case "Results":
                    return ResultUtilities.CrosstabJson(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
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
                    return HtmlTemplates.Error(context, Error.Types.NotFound);
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
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BurnDown(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(context, Error.Types.NotFound);
            }
        }

        public string BurnDownJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BurnDownJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BurnDownRecordDetailsJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BurnDownRecordDetails(
                        context: context,
                        ss: Site.SiteSettings);
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
                            setSiteIntegration: true));
                case "Results":
                    return ResultUtilities.TimeSeries(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
                default:
                    return HtmlTemplates.Error(context, Error.Types.NotFound);
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
                            setSiteIntegration: true));
                case "Results":
                    return ResultUtilities.TimeSeriesJson(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true));
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
                    return HtmlTemplates.Error(context, Error.Types.NotFound);
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
                    return HtmlTemplates.Error(context, Error.Types.NotFound);
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
                    return HtmlTemplates.Error(context, Error.Types.NotFound);
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
                    return HtmlTemplates.Error(context, Error.Types.NotFound);
            }
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
                            setSiteIntegration: true),
                        siteModel: Site);
                case "Results":
                    return ResultUtilities.Export(
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
            var searchText = context.Forms.Data("DropDownSearchText");
            string parentClass = context.Forms.Data("DropDownSearchParentClass");
            int parentId = context.Forms.Int("DropDownSearchParentDataId");
            switch (context.Forms.ControlId())
            {
                case "DropDownSearchResults":
                    return
                        AppendSearchDropDown(
                            context: context,
                            controlId: controlId,
                            searchText: searchText,
                            parentClass: parentClass,
                            parentId: parentId);
                default:
                    return SearchDropDown(
                        context: context,
                        controlId: controlId,
                        searchText: searchText,
                        parentClass: parentClass,
                        parentId: parentId);
            }
        }

        private string AppendSearchDropDown(
            Context context,
            string controlId,
            string searchText,
            string parentClass = "",
            int parentId = 0)
        {
            var offset = context.Forms.Int("DropDownSearchResultsOffset");
            var column = SearchDropDownColumn(
                context: context,
                controlId: controlId,
                searchText: searchText,
                offset: offset,
                parentClass: parentClass,
                parentId: parentId);
            var nextOffset = Paging.NextOffset(
                offset: offset,
                totalCount: column.TotalCount,
                pageSize: Parameters.General.DropDownSearchPageSize);
            return new ResponseCollection()
                .Append("#DropDownSearchResults", new HtmlBuilder()
                    .SelectableItems(
                        listItemCollection: column?.EditChoices(
                            context: context,
                            addNotSet: nextOffset == -1)))
                .Val("#DropDownSearchResultsOffset", nextOffset)
                .ToJson();
        }

        private string SearchDropDown(
            Context context,
            string controlId,
            string searchText,
            string parentClass = "",
            int parentId = 0)
        {
            var column = SearchDropDownColumn(
                context: context,
                controlId: controlId,
                searchText: searchText,
                parentClass: parentClass,
                parentId: parentId);
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

        public string SelectSearchDropDown(Context context)
        {
            SetSite(context: context);
            var controlId = context.Forms.Data("DropDownSearchTarget");
            var searchText = context.Forms.Data("DropDownSearchText");
            var column = SearchDropDownColumn(
                context: context,
                controlId: controlId,
                searchText: searchText);
            var selected = context.Forms.List("DropDownSearchResults");
            var editor = context.Forms.Bool("DropDownSearchOnEditor");
            var multiple = context.Forms.Bool("DropDownSearchMultiple");
            if (multiple)
            {
                return SelectSearchDropDownResponse(
                    context: context,
                    controlId: controlId,
                    column: column,
                    selected: selected,
                    editor: editor,
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
                    editor: editor,
                    multiple: multiple);
            }
        }

        private Column SearchDropDownColumn(
            Context context,
            string controlId,
            string searchText,
            int offset = 0,
            string parentClass = "",
            int parentId = 0)
        {
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteModel: Site,
                referenceId: ReferenceId,
                setSiteIntegration: true);
            var column = ss.GetColumn(
                context: context,
                columnName: controlId.Substring(
                    controlId.StartsWith("ViewFilters__")
                        ? "ViewFilters__".Length
                        : (ss.ReferenceType + "_").Length));
            if (column?.Linked() == true)
            {
                column?.SetChoiceHash(
                    context: context,
                    siteId: column.SiteId,
                    linkHash: column.SiteSettings.LinkHash(
                        context: context,
                        columnName: column.Name,
                        searchText: searchText,
                        offset: offset,
                        parentClass: parentClass,
                        parentId: parentId),
                    searchIndexes: searchText.SearchIndexes(context: context));
            }
            else
            {
                ss.SetChoiceHash(
                    context: context,
                    columnName: column?.ColumnName,
                    searchText: context.Forms.Data("DropDownSearchText"));
            }
            return column;
        }

        private static string SelectSearchDropDownResponse(
            Context context,
            string controlId,
            Column column,
            List<string> selected,
            bool editor,
            bool multiple)
        {
            column.SiteSettings.SetChoiceHash(
                context: context,
                columnName: column.ColumnName,
                selectedValues: selected);
            var optionCollection = column?.EditChoices(
                context: context,
                addNotSet: true)?
                    .Where(o => selected.Contains(o.Key))
                    .ToDictionary(o => o.Key, o => o.Value);
            return optionCollection?.Any() == true
                ? new ResponseCollection()
                    .CloseDialog("#DropDownSearchDialog")
                    .Html("[id=\"" + controlId + "\"]", new HtmlBuilder()
                        .OptionCollection(
                            context: context,
                            optionCollection: optionCollection,
                            selectedValue: SelectSearchDropDownSelectedValue(
                                context: context,
                                selected: selected,
                                editor: editor,
                                multiple: multiple),
                            multiple: multiple,
                            insertBlank: editor))
                    .Invoke("setDropDownSearch")
                    .Trigger("#" + controlId, "change")
                    .ToJson()
                : new ResponseCollection()
                    .Message(Messages.NotFound(context: context))
                    .ToJson();
        }

        public static string SelectSearchDropDownSelectedValue(
            Context context, List<string> selected, bool editor, bool multiple)
        {
            if (multiple)
            {
                return selected.ToJson();
            }
            else
            {
                var value = selected.FirstOrDefault();
                return editor && value == "\t"
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
                        offset: DataViewGrid.Offset(context: context));
                case "Results":
                    return ResultUtilities.GridRows(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        offset: DataViewGrid.Offset(context: context));
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
                        offset: DataViewGrid.Offset(context: context),
                        action: "TrashBoxGridRows");
                case "Results":
                    return ResultUtilities.GridRows(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            tableType: Sqls.TableTypes.Deleted),
                        offset: DataViewGrid.Offset(context: context),
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

        public System.Web.Mvc.ContentResult GetByApi(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    if (SiteId == ReferenceId)
                    {
                        return IssueUtilities.GetByApi(
                            context: context,
                            ss: Site.IssuesSiteSettings(
                                context: context,
                                referenceId: ReferenceId));
                    }
                    else
                    {
                        return IssueUtilities.GetByApi(
                            context: context,
                            ss: Site.IssuesSiteSettings(
                                context: context,
                                referenceId: ReferenceId),
                            issueId: ReferenceId);
                    }
                case "Results":
                    if (SiteId == ReferenceId)
                    {
                        return ResultUtilities.GetByApi(
                            context: context,
                            ss: Site.ResultsSiteSettings(
                                context: context,
                                referenceId: ReferenceId));
                    }
                    else
                    {
                        return ResultUtilities.GetByApi(
                            context: context,
                            ss: Site.ResultsSiteSettings(
                                context: context,
                                referenceId: ReferenceId),
                            resultId: ReferenceId);
                    }
                default:
                    return ApiResults.Get(ApiResponses.BadRequest(context: context));
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
                    return ApiResults.Get(ApiResponses.BadRequest(context: context));
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

        public System.Web.Mvc.ContentResult UpdateByApi(Context context)
        {
            SetSite(context: context);
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
                    return ApiResults.Get(ApiResponses.BadRequest(context: context));
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
                    optionCollection: MoveTargets(
                        context: context,
                        sites: Rds.ExecuteTable(
                            context: context,
                            statements: new SqlStatement(
                                commandText: Def.Sql.MoveTarget,
                                param: Rds.SitesParam()
                                    .TenantId(context.TenantId)
                                    .ReferenceType(Site.ReferenceType)
                                    .SiteId(Site.SiteId)
                                    .Add(name: "HasPrivilege", value: context.HasPrivilege)))
                                        .AsEnumerable())))
                                            .ToJson();
        }

        private Dictionary<string, ControlData> MoveTargets(
            Context context, IEnumerable<DataRow> sites)
        {
            var moveTargets = new Dictionary<string, ControlData>();
            sites
                .Where(dataRow => dataRow.String("ReferenceType") == Site.ReferenceType)
                .ForEach(dataRow =>
                {
                    var current = dataRow;
                    var titles = new List<string>()
                    {
                        current.String("Title")
                    };
                    while(sites.Any(o =>
                        o.Long("SiteId") == current.Long("ParentId")))
                        {
                            current = sites.First(o =>
                                o.Long("SiteId") == current.Long("ParentId"));
                            titles.Insert(0, current.String("Title"));
                        }
                    moveTargets.Add(
                        dataRow.String("SiteId"),
                        new ControlData(titles.Join(" / ")));
                });
            return moveTargets;
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
                    return ApiResults.Get(ApiResponses.BadRequest(context: context));
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

        public string PhysicalDelete(Context context)
        {
            SetSite(context: context, tableType: Sqls.TableTypes.Deleted);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.PhysicalDelete(
                        context: context,
                        ss: Site.SitesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            tableType: Sqls.TableTypes.Deleted));
                case "Issues":
                    return IssueUtilities.PhysicalDelete(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true,
                            tableType: Sqls.TableTypes.Deleted));
                case "Results":
                    return ResultUtilities.PhysicalDelete(
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
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return
                ReferenceId_Updated(context: context) ||
                Ver_Updated(context: context) ||
                ReferenceType_Updated(context: context) ||
                SiteId_Updated(context: context) ||
                Title_Updated(context: context) ||
                FullText_Updated(context: context) ||
                SearchIndexCreatedTime_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
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
