using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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

        public bool ReferenceId_Updated(Column column = null)
        {
            return ReferenceId != SavedReferenceId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != ReferenceId);
        }

        public bool ReferenceType_Updated(Column column = null)
        {
            return ReferenceType != SavedReferenceType && ReferenceType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ReferenceType);
        }

        public bool SiteId_Updated(Column column = null)
        {
            return SiteId != SavedSiteId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != SiteId);
        }

        public bool Title_Updated(Column column = null)
        {
            return Title != SavedTitle && Title != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Title);
        }

        public bool FullText_Updated(Column column = null)
        {
            return FullText != SavedFullText && FullText != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != FullText);
        }

        public bool SearchIndexCreatedTime_Updated(Column column = null)
        {
            return SearchIndexCreatedTime != SavedSearchIndexCreatedTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != SearchIndexCreatedTime.Date);
        }

        public ItemModel(DataRow dataRow, string tableAlias = null)
        {
            OnConstructing();
            Set(dataRow, tableAlias);
            OnConstructed();
        }

        private void OnConstructing()
        {
        }

        private void OnConstructed()
        {
        }

        public void ClearSessions()
        {
        }

        public ItemModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectItems(
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

        public string Index()
        {
            if (ReferenceId == 0)
            {
                return SiteUtilities.SiteTop();
            }
            if (ReferenceType != "Sites")
            {
                return HtmlTemplates.Error(Error.Types.NotFound);
            }
            SetSite(
                initSiteSettings: true,
                setSiteIntegration: true);
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Sites": return SiteUtilities.SiteMenu(
                    siteModel: Site);
                case "Issues": return IssueUtilities.Index(ss: Site.SiteSettings);
                case "Results": return ResultUtilities.Index(ss: Site.SiteSettings);
                default: return HtmlTemplates.Error(Error.Types.NotFound);
            }
        }

        public string IndexJson()
        {
            if (ReferenceType != "Sites")
            {
                return Messages.ResponseNotFound().ToJson();
            }
            SetSite(
                initSiteSettings: true,
                setSiteIntegration: true);
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.IndexJson(ss: Site.SiteSettings);
                case "Results": return ResultUtilities.IndexJson(ss: Site.SiteSettings);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Calendar()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Calendar(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                case "Results": return ResultUtilities.Calendar(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                default: return HtmlTemplates.Error(Error.Types.NotFound);
            }
        }

        public string CalendarJson()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.CalendarJson(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                case "Results": return ResultUtilities.CalendarJson(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Crosstab()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Crosstab(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                case "Results": return ResultUtilities.Crosstab(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                default: return HtmlTemplates.Error(Error.Types.NotFound);
            }
        }

        public string CrosstabJson()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.CrosstabJson(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                case "Results": return ResultUtilities.CrosstabJson(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Gantt()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Gantt(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                default: return HtmlTemplates.Error(Error.Types.NotFound);
            }
        }

        public string GanttJson()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.GanttJson(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string BurnDown()
        {
            SetSite(initSiteSettings: true, setSiteIntegration: true);
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.BurnDown(ss: Site.SiteSettings);
                default: return HtmlTemplates.Error(Error.Types.NotFound);
            }
        }

        public string BurnDownJson()
        {
            SetSite(initSiteSettings: true, setSiteIntegration: true);
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.BurnDownJson(ss: Site.SiteSettings);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string BurnDownRecordDetailsJson()
        {
            SetSite(initSiteSettings: true, setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.BurnDownRecordDetails(Site.SiteSettings);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string TimeSeries()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.TimeSeries(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                case "Results": return ResultUtilities.TimeSeries(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                default: return HtmlTemplates.Error(Error.Types.NotFound);
            }
        }

        public string TimeSeriesJson()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.TimeSeriesJson(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                case "Results": return ResultUtilities.TimeSeriesJson(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Kamban()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Kamban(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                case "Results": return ResultUtilities.Kamban(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                default: return HtmlTemplates.Error(Error.Types.NotFound);
            }
        }

        public string KambanJson()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.KambanJson(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                case "Results": return ResultUtilities.KambanJson(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string ImageLib()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.ImageLib(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                case "Results": return ResultUtilities.ImageLib(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                default: return HtmlTemplates.Error(Error.Types.NotFound);
            }
        }

        public string ImageLibJson()
        {
            SetSite();
            ViewModes.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.ImageLibJson(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                case "Results": return ResultUtilities.ImageLibJson(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string New()
        {
            SetSite(siteOnly: true, initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.EditorNew(Site.SiteSettings);
                case "Results": return ResultUtilities.EditorNew(Site.SiteSettings);
                case "Wikis": return WikiUtilities.EditorNew(Site.SiteSettings);
                default: return HtmlTemplates.Error(Error.Types.NotFound);
            }
        }

        public string NewJson()
        {
            return new ResponseCollection()
                .ReplaceAll("#MainContainer", New())
                .WindowScrollTop()
                .FocusMainForm()
                .ClearFormData()
                .PushState("Edit", Locations.Get("Items", ReferenceId.ToString(), "New"))
                .ToJson();
        }

        public string Editor()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Editor(
                    siteId: ReferenceId,
                    clearSessions: true);
                case "Issues": return IssueUtilities.Editor(
                    ss: Site.IssuesSiteSettings(ReferenceId),
                    issueId: ReferenceId,
                    clearSessions: true);
                case "Results": return ResultUtilities.Editor(
                    ss: Site.ResultsSiteSettings(ReferenceId),
                    resultId: ReferenceId,
                    clearSessions: true);
                case "Wikis": return WikiUtilities.Editor(
                    ss: Site.WikisSiteSettings(ReferenceId),
                    wikiId: ReferenceId,
                    clearSessions: true);
                default: return HtmlTemplates.Error(Error.Types.NotFound);
            }
        }

        public string Import()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Import(siteModel: Site);
                case "Results": return ResultUtilities.Import(siteModel: Site);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string OpenExportSelectorDialog()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.OpenExportSelectorDialog(
                    Site.IssuesSiteSettings(ReferenceId, setSiteIntegration: true),
                    siteModel: Site);
                case "Results": return ResultUtilities.OpenExportSelectorDialog(
                    Site.ResultsSiteSettings(ReferenceId, setSiteIntegration: true),
                    siteModel: Site);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public ResponseFile Export()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Export(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true),
                    siteModel: Site);
                case "Results": return ResultUtilities.Export(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true),
                    siteModel: Site);
                default: return null;
            }
        }

        public ResponseFile ExportCrosstab()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.ExportCrosstab(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true),
                    siteModel: Site);
                case "Results": return ResultUtilities.ExportCrosstab(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true),
                    siteModel: Site);
                default: return null;
            }
        }

        public string SearchDropDown()
        {
            SetSite();
            var controlId = Forms.Data("DropDownSearchTarget");
            var searchText = Forms.Data("DropDownSearchText");
            switch (Forms.ControlId())
            {
                case "DropDownSearchResults":
                    return AppendSearchDropDown(controlId, searchText);
                default:
                    return SearchDropDown(controlId, searchText);
            }
        }

        private string AppendSearchDropDown(string controlId, string searchText)
        {
            var offset = Forms.Int("DropDownSearchResultsOffset");
            var column = SearchDropDownColumn(controlId, searchText, offset);
            var nextOffset = Paging.NextOffset(
                offset, column.TotalCount, Parameters.General.DropDownSearcPageSize);
            return new ResponseCollection()
                .Append("#DropDownSearchResults", new HtmlBuilder()
                    .SelectableItems(
                        listItemCollection: column?.EditChoices(addNotSet: nextOffset == -1)))
                .Val("#DropDownSearchResultsOffset", nextOffset)
                .ToJson();
        }

        private string SearchDropDown(string controlId, string searchText)
        {
            var column = SearchDropDownColumn(controlId, searchText);
            var nextOffset = Paging.NextOffset(
                0, column.TotalCount, Parameters.General.DropDownSearcPageSize);
            return new ResponseCollection()
                .ReplaceAll(
                    "#DropDownSearchResults",
                    new HtmlBuilder().Selectable(
                        controlId: "DropDownSearchResults",
                        listItemCollection: column?.EditChoices(addNotSet: nextOffset == -1),
                        action: "SearchDropDown",
                        method: "post"))
                .Val("#DropDownSearchResultsOffset", nextOffset)
                .ClearFormData("DropDownSearchResults")
                .ToJson();
        }

        public string SelectSearchDropDown()
        {
            SetSite();
            var controlId = Forms.Data("DropDownSearchTarget");
            var searchText = Forms.Data("DropDownSearchText");
            var column = SearchDropDownColumn(controlId, searchText);
            var selected = Forms.List("DropDownSearchResults");
            var editor = Forms.Bool("DropDownSearchOnEditor");
            var multiple = Forms.Bool("DropDownSearchMultiple");
            if (multiple)
            {
                return SelectSearchDropDownResponse(controlId, column, selected, editor, multiple);
            }
            else if (selected.Count() != 1)
            {
                return new ResponseCollection()
                    .Message(Messages.SelectOne())
                    .ToJson();
            }
            else
            {
                return SelectSearchDropDownResponse(controlId, column, selected, editor, multiple);
            }
        }

        private Column SearchDropDownColumn(string controlId, string searchText, int offset = 0)
        {
            var ss = SiteSettingsUtilities.Get(Site, ReferenceId, setSiteIntegration: true);
            var column = ss.GetColumn(controlId.Substring(
                controlId.StartsWith("ViewFilters__")
                    ? "ViewFilters__".Length
                    : (ss.ReferenceType + "_").Length));
            if (column?.Linked() == true)
            {
                column?.SetChoiceHash(
                    siteId: column.SiteId,
                    linkHash: column.SiteSettings.LinkHash(
                        columnName: column.Name,
                        searchText: searchText,
                        offset: offset),
                    searchIndexes: searchText.SearchIndexes());    
            }
            else
            {
                ss.SetChoiceHash(
                     columnName: column?.ColumnName,
                     searchText: Forms.Data("DropDownSearchText"));
            }
            return column;
        }

        private static string SelectSearchDropDownResponse(
            string controlId, Column column, List<string> selected, bool editor, bool multiple)
        {
            var optionCollection = column?.EditChoices(addNotSet: true)?
                .Where(o => selected.Contains(o.Key))
                .ToDictionary(o => o.Key, o => o.Value);
            return optionCollection?.Any() == true
                ? new ResponseCollection()
                    .CloseDialog()
                    .Html("[id=\"" + controlId + "\"]", new HtmlBuilder()
                        .OptionCollection(
                            optionCollection: optionCollection,
                            selectedValue: SelectSearchDropDownSelectedValue(
                                selected, editor, multiple),
                            multiple: multiple,
                            insertBlank: editor))
                    .Invoke("setDropDownSearch")
                    .ToJson()
                : new ResponseCollection()
                    .Message(Messages.NotFound())
                    .ToJson();
        }

        public static string SelectSearchDropDownSelectedValue(
            List<string> selected, bool editor, bool multiple)
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

        public string GridRows()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.GridRows(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true),
                    offset: DataViewGrid.Offset());
                case "Results": return ResultUtilities.GridRows(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true),
                    offset: DataViewGrid.Offset());
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string ImageLibNext()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.ImageLibNext(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true),
                    offset: Forms.Int("ImageLibOffset"));
                case "Results": return ResultUtilities.ImageLibNext(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true),
                    offset: Forms.Int("ImageLibOffset"));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public System.Web.Mvc.ContentResult GetByApi()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues":
                    if (SiteId == ReferenceId)
                    {
                        return IssueUtilities.GetByApi(
                            ss: Site.IssuesSiteSettings(ReferenceId));
                    }
                    else
                    {
                        return IssueUtilities.GetByApi(
                            ss: Site.IssuesSiteSettings(ReferenceId),
                            issueId: ReferenceId);
                    }
                case "Results":
                    if (SiteId == ReferenceId)
                    {
                        return ResultUtilities.GetByApi(
                            ss: Site.ResultsSiteSettings(ReferenceId));
                    }
                    else
                    {
                        return ResultUtilities.GetByApi(
                            ss: Site.ResultsSiteSettings(ReferenceId),
                            resultId: ReferenceId);
                    }
                default: return ApiResults.Get(ApiResponses.BadRequest());
            }
        }

        public string Create()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Sites": return SiteUtilities.Create(
                    parentId: Site.SiteId,
                    inheritPermission: Site.InheritPermission);
                case "Issues": return IssueUtilities.Create(
                    ss: Site.IssuesSiteSettings(ReferenceId));
                case "Results": return ResultUtilities.Create(
                    ss: Site.ResultsSiteSettings(ReferenceId));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public System.Web.Mvc.ContentResult CreateByApi()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.CreateByApi(
                    ss: Site.IssuesSiteSettings(ReferenceId));
                case "Results": return ResultUtilities.CreateByApi(
                    ss: Site.ResultsSiteSettings(ReferenceId));
                default: return ApiResults.Get(ApiResponses.BadRequest());
            }
        }

        public string Templates()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Sites": return SiteUtilities.Templates(
                    siteModel: Site);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string CreateByTemplate()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Sites": return SiteUtilities.CreateByTemplate(
                    siteModel: Site);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string SiteMenu()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Sites": return SiteUtilities.SiteMenuJson(
                    siteModel: Site);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Update()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Update(
                    siteModel: Site,
                    siteId: ReferenceId);
                case "Issues": return IssueUtilities.Update(
                    ss: Site.IssuesSiteSettings(ReferenceId),
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.Update(
                    ss: Site.ResultsSiteSettings(ReferenceId),
                    resultId: ReferenceId);
                case "Wikis": return WikiUtilities.Update(
                    ss: Site.WikisSiteSettings(ReferenceId),
                    wikiId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public System.Web.Mvc.ContentResult UpdateByApi()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.UpdateByApi(
                    ss: Site.IssuesSiteSettings(ReferenceId),
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.UpdateByApi(
                    ss: Site.ResultsSiteSettings(ReferenceId),
                    resultId: ReferenceId);
                default: return ApiResults.Get(ApiResponses.BadRequest());
            }
        }

        public string DeleteComment()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Update(
                    siteModel: Site,
                    siteId: ReferenceId);
                case "Issues": return IssueUtilities.Update(
                    ss: Site.IssuesSiteSettings(ReferenceId),
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.Update(
                    ss: Site.ResultsSiteSettings(ReferenceId),
                    resultId: ReferenceId);
                case "Wikis": return WikiUtilities.Update(
                    ss: Site.WikisSiteSettings(ReferenceId),
                    wikiId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Copy()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Copy(
                    siteModel: Site);
                case "Issues": return IssueUtilities.Copy(
                    ss: Site.IssuesSiteSettings(ReferenceId),
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.Copy(
                    ss: Site.ResultsSiteSettings(ReferenceId),
                    resultId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string MoveTargets()
        {
            SetSite();
            return new ResponseCollection().Html("#MoveTargets", new HtmlBuilder()
                .OptionCollection(
                    optionCollection: MoveTargets(
                        Rds.ExecuteTable(statements: new SqlStatement(
                            commandText: Def.Sql.MoveTarget,
                            param: Rds.SitesParam()
                                .TenantId(Sessions.TenantId())
                                .ReferenceType(Site.ReferenceType)
                                .SiteId(Site.SiteId)
                                .Add(name: "HasPrivilege", value: Permissions.HasPrivilege())))
                                    .AsEnumerable())))
                                        .ToJson();
        }

        private Dictionary<string, ControlData> MoveTargets(IEnumerable<DataRow> siteCollection)
        {
            var moveTargets = new Dictionary<string, ControlData>();
            siteCollection
                .Where(o => o["ReferenceType"].ToString() == Site.ReferenceType)
                .ForEach(dataRow =>
                {
                    var current = dataRow;
                    var titles = new List<string>() { current["Title"].ToString() };
                    while(siteCollection.Any(o =>
                        o["SiteId"].ToLong() == current["ParentId"].ToLong()))
                        {
                            current = siteCollection.First(o =>
                                o["SiteId"].ToLong() == current["ParentId"].ToLong());
                            titles.Insert(0, current["Title"].ToString());
                        }
                    moveTargets.Add(
                        dataRow["SiteId"].ToString(),
                        new ControlData(titles.Join(" / ")));
                });
            return moveTargets;
        }

        public string Move()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Issues": return IssueUtilities.Move(
                    ss: Site.IssuesSiteSettings(ReferenceId),
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.Move(
                    ss: Site.ResultsSiteSettings(ReferenceId),
                    resultId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string BulkMove()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.BulkMove(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                case "Results": return ResultUtilities.BulkMove(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Delete()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Delete(
                    ss: Site.SitesSiteSettings(ReferenceId),
                    siteId: ReferenceId);
                case "Issues": return IssueUtilities.Delete(
                    ss: Site.IssuesSiteSettings(ReferenceId),
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.Delete(
                    ss: Site.ResultsSiteSettings(ReferenceId),
                    resultId: ReferenceId);
                case "Wikis": return WikiUtilities.Delete(
                    ss: Site.WikisSiteSettings(ReferenceId),
                    wikiId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public System.Web.Mvc.ContentResult DeleteByApi()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.DeleteByApi(
                    ss: Site.IssuesSiteSettings(ReferenceId),
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.DeleteByApi(
                    ss: Site.ResultsSiteSettings(ReferenceId),
                    resultId: ReferenceId);
                default: return ApiResults.Get(ApiResponses.BadRequest());
            }
        }

        public string BulkDelete()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.BulkDelete(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                case "Results": return ResultUtilities.BulkDelete(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Restore(long referenceId)
        {
            ReferenceId = referenceId;
            Get(Sqls.TableTypes.Deleted, where: Rds.ItemsWhere().ReferenceId(ReferenceId));
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Restore(
                    siteId: ReferenceId);
                case "Issues": return IssueUtilities.Restore(
                    ss: SiteSettingsUtilities.IssuesSiteSettings(Site, ReferenceId),
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.Restore(
                    ss: SiteSettingsUtilities.ResultsSiteSettings(Site, ReferenceId),
                    resultId: ReferenceId);
                case "Wikis": return WikiUtilities.Restore(
                    ss: SiteSettingsUtilities.WikisSiteSettings(Site, ReferenceId),
                    wikiId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string EditSeparateSettings()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.EditSeparateSettings(
                    ss: Site.IssuesSiteSettings(ReferenceId),
                    issueId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Separate()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Separate(
                    ss: Site.IssuesSiteSettings(ReferenceId),
                    issueId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Histories()
        {
            SetSite(initSiteSettings: true);
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Histories(
                    siteModel: Site);
                case "Issues": return IssueUtilities.Histories(
                    ss: Site.SiteSettings,
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.Histories(
                    ss: Site.SiteSettings,
                    resultId: ReferenceId);
                case "Wikis": return WikiUtilities.Histories(
                    ss: Site.SiteSettings,
                    wikiId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string History()
        {
            SetSite(initSiteSettings: true);
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.History(
                    siteModel: Site);
                case "Issues": return IssueUtilities.History(
                    ss: Site.SiteSettings,
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.History(
                    ss: Site.SiteSettings,
                    resultId: ReferenceId);
                case "Wikis": return WikiUtilities.History(
                    ss: Site.SiteSettings,
                    wikiId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string EditorJson()
        {
            SetSite(initSiteSettings: true);
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.EditorJson(
                    siteModel: Site);
                case "Issues": return IssueUtilities.EditorJson(
                    Site.SiteSettings, ReferenceId);
                case "Results": return ResultUtilities.EditorJson(
                    Site.SiteSettings, ReferenceId);
                case "Wikis": return WikiUtilities.EditorJson(
                    Site.SiteSettings, ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string UpdateByCalendar()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.UpdateByCalendar(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                case "Results": return ResultUtilities.UpdateByCalendar(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string UpdateByKamban()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.UpdateByKamban(
                    ss: Site.IssuesSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                case "Results": return ResultUtilities.UpdateByKamban(
                    ss: Site.ResultsSiteSettings(
                        referenceId: ReferenceId,
                        setSiteIntegration: true,
                        setAllChoices: true));
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string SynchronizeTitles()
        {
            SetSite(initSiteSettings: true);
            return SiteUtilities.SynchronizeTitles(Site);
        }

        public string SynchronizeSummaries()
        {
            SetSite();
            return SiteUtilities.SynchronizeSummaries(Site);
        }

        public string SynchronizeFormulas()
        {
            SetSite();
            return SiteUtilities.SynchronizeFormulas(Site);
        }

        private void SetSite(
            bool siteOnly = false,
            bool initSiteSettings = false,
            bool setSiteIntegration = false)
        {
            Site = GetSite(
                siteOnly: siteOnly,
                initSiteSettings: initSiteSettings,
                setSiteIntegration: setSiteIntegration);
        }

        public SiteModel GetSite(
            bool siteOnly = false,
            bool initSiteSettings = false,
            bool setSiteIntegration = false)
        {
            SiteModel siteModel;
            if (ReferenceType == "Sites" && Forms.Exists("Ver"))
            {
                siteModel = new SiteModel();
                siteModel.Get(
                    where: Rds.SitesWhere()
                        .SiteId(ReferenceId)
                        .Ver(Forms.Int("Ver")),
                    tableType: Sqls.TableTypes.NormalAndHistory);
                siteModel.VerType =  Forms.Bool("Latest")
                    ? Versions.VerTypes.Latest
                    : Versions.VerTypes.History;
            }
            else
            {
                siteModel = siteOnly
                    ? new SiteModel(ReferenceId)
                    : new SiteModel(ReferenceType == "Sites" ? ReferenceId : SiteId);
            }
            if (initSiteSettings)
            {
                siteModel.SiteSettings = SiteSettingsUtilities.Get(
                    siteModel: siteModel,
                    referenceId: ReferenceId,
                    setSiteIntegration: setSiteIntegration);
            }
            return siteModel;
        }

        private void SetBySession()
        {
        }

        private void Set(DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(DataRow dataRow, string tableAlias = null)
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
                            Creator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(dataRow, column.ColumnName);
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated()
        {
            return
                ReferenceId_Updated() ||
                Ver_Updated() ||
                ReferenceType_Updated() ||
                SiteId_Updated() ||
                Title_Updated() ||
                FullText_Updated() ||
                SearchIndexCreatedTime_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
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
        public ItemModel(long referenceId)
        {
            OnConstructing();
            ReferenceId = referenceId;
            Get();
            OnConstructed();
        }
    }
}
