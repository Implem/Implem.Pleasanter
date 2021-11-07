using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Search;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class View
    {
        public enum DisplayTypes
        {
            Displayed = 0,
            Hidden = 1,
            AlwaysDisplayed = 2,
            AlwaysHidden = 3,
        }

        public enum CommandDisplayTypes : int
        {
            Displayed = 0,
            None = 1,
            Disabled = 2,
            Hidden = 3,
        }

        public int Id;
        public string Name;
        public List<string> GridColumns;
        public DisplayTypes? FiltersDisplayType;
        public bool? FiltersReduced;
        public DisplayTypes? AggregationsDisplayType;
        public bool? AggregationsReduced;
        public bool? Incomplete;
        public bool? Own;
        public bool? NearCompletionTime;
        public bool? Delay;
        public bool? Overdue;
        public CommandDisplayTypes? BulkMoveTargetsCommand;
        public CommandDisplayTypes? BulkDeleteCommand;
        public CommandDisplayTypes? EditImportSettings;
        public CommandDisplayTypes? OpenExportSelectorDialogCommand;
        public CommandDisplayTypes? OpenBulkUpdateSelectorDialogCommand;
        public CommandDisplayTypes? EditOnGridCommand;
        public CommandDisplayTypes? ExportCrosstabCommand;
        public CommandDisplayTypes? UpdateCommand;
        public CommandDisplayTypes? OpenCopyDialogCommand;
        public CommandDisplayTypes? ReferenceCopyCommand;
        public CommandDisplayTypes? MoveTargetsCommand;
        public CommandDisplayTypes? EditOutgoingMail;
        public CommandDisplayTypes? DeleteCommand;
        public CommandDisplayTypes? OpenDeleteSiteDialogCommand;
        public Dictionary<string, string> ColumnFilterHash;
        public Dictionary<string, Column.SearchTypes> ColumnFilterSearchTypes;
        public string Search;
        public Dictionary<string, SqlOrderBy.Types> ColumnSorterHash;
        public string CalendarTimePeriod;
        public string CalendarFromTo;
        public DateTime? CalendarDate;
        public string CalendarGroupBy;
        public string CrosstabGroupByX;
        public string CrosstabGroupByY;
        public string CrosstabColumns;
        public string CrosstabAggregateType;
        public string CrosstabValue;
        public string CrosstabTimePeriod;
        public DateTime? CrosstabMonth;
        public string GanttGroupBy;
        public string GanttSortBy;
        public int? GanttPeriod;
        public DateTime? GanttStartDate;
        public string TimeSeriesGroupBy;
        public string TimeSeriesAggregateType;
        public string TimeSeriesValue;
        public string KambanGroupByX;
        public string KambanGroupByY;
        public string KambanAggregateType;
        public string KambanValue;
        public int? KambanColumns;
        public bool? KambanAggregationView;
        [NonSerialized]
        public SqlWhereCollection AdditionalWhere;
        [NonSerialized]
        public bool WhenViewProcessingServerScriptExecuted;
        [NonSerialized]
        public List<string> AlwaysGetColumns;
        [NonSerialized]
        public string OnSelectingWhere;
        [NonSerialized]
        public Dictionary<string, string> ColumnPlaceholders;
        // compatibility Version 1.008
        public string KambanGroupBy;
        // compatibility Version 1.012
        public string CalendarColumn;
        public bool? ShowHistory;

        public View()
        {
        }

        public View(Context context, SiteSettings ss)
        {
            SetByForm(context: context, ss: ss);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
        }

        public string GetCalendarTimePeriod(SiteSettings ss)
        {
            if (CalendarTimePeriod.IsNullOrEmpty())
            {
                CalendarTimePeriod = Definition(ss, "Calendar")?.Option1;
            }
            return CalendarTimePeriod;
        }

        public string GetCalendarFromTo(SiteSettings ss)
        {
            if (CalendarFromTo.IsNullOrEmpty())
            {
                CalendarFromTo = Definition(ss, "Calendar")?.Option2;
            }
            return CalendarFromTo;
        }

        public string GetCalendarFromColumn(SiteSettings ss)
        {
            return GetCalendarFromTo(ss).Split_1st('-');
        }

        public string GetCalendarToColumn(SiteSettings ss)
        {
            return GetCalendarFromTo(ss).Split_2nd('-');
        }

        public DateTime GetCalendarDate()
        {
            return CalendarDate ?? DateTime.Now;
        }

        public string GetCalendarGroupBy()
        {
            return !CalendarGroupBy.IsNullOrEmpty()
                ? CalendarGroupBy
                : string.Empty;
        }

        public string GetCrosstabGroupByX(Context context, SiteSettings ss)
        {
            var options = ss.CrosstabGroupByXOptions(context: context);
            if (CrosstabGroupByX.IsNullOrEmpty())
            {
                CrosstabGroupByX = options.ContainsKey(Definition(ss, "Crosstab")?.Option1)
                    ? Definition(ss, "Crosstab")?.Option1
                    : options.FirstOrDefault().Key;
            }
            return CrosstabGroupByX;
        }

        public string GetCrosstabGroupByY(Context context, SiteSettings ss)
        {
            var options = ss.CrosstabGroupByYOptions(context: context);
            if (CrosstabGroupByY.IsNullOrEmpty())
            {
                CrosstabGroupByY = options.ContainsKey(Definition(ss, "Crosstab")?.Option2)
                    ? Definition(ss, "Crosstab")?.Option2
                    : options.FirstOrDefault().Key;
            }
            return CrosstabGroupByY;
        }

        public string GetCrosstabAggregateType(SiteSettings ss)
        {
            if (CrosstabAggregateType.IsNullOrEmpty())
            {
                CrosstabAggregateType = Definition(ss, "Crosstab")?.Option3;
            }
            return CrosstabAggregateType;
        }

        public string GetCrosstabValue(Context context, SiteSettings ss)
        {
            var options = ss.CrosstabColumnsOptions(context: context);
            if (CrosstabValue.IsNullOrEmpty())
            {
                CrosstabValue = options.ContainsKey(Definition(ss, "Crosstab")?.Option4)
                    ? Definition(ss, "Crosstab")?.Option4
                    : options.FirstOrDefault().Key;
            }
            return CrosstabValue;
        }

        public string GetCrosstabTimePeriod(SiteSettings ss)
        {
            if (CrosstabTimePeriod.IsNullOrEmpty())
            {
                CrosstabTimePeriod = Definition(ss, "Crosstab")?.Option5;
            }
            return CrosstabTimePeriod;
        }

        public DateTime GetCrosstabMonth(SiteSettings ss)
        {
            if (CrosstabMonth?.InRange() != true)
            {
                CrosstabMonth = GetCrosstabMonthDefault();
            }
            return CrosstabMonth.ToDateTime();
        }

        private DateTime GetCrosstabMonthDefault()
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, 1);
        }

        public string GetGanttGroupBy()
        {
            return !GanttGroupBy.IsNullOrEmpty()
                ? GanttGroupBy
                : string.Empty;
        }

        public string GetGanttSortBy()
        {
            return !GanttSortBy.IsNullOrEmpty()
                ? GanttSortBy
                : string.Empty;
        }

        public string GetTimeSeriesGroupBy(Context context, SiteSettings ss)
        {
            var options = ss.TimeSeriesGroupByOptions(context: context);
            if (TimeSeriesGroupBy.IsNullOrEmpty())
            {
                TimeSeriesGroupBy = options.ContainsKey(Definition(ss, "TimeSeries")?.Option1)
                    ? Definition(ss, "TimeSeries")?.Option1
                    : options.FirstOrDefault().Key;
            }
            return TimeSeriesGroupBy;
        }

        public string GetTimeSeriesAggregationType(Context context, SiteSettings ss)
        {
            var options = ss.TimeSeriesAggregationTypeOptions(context: context);
            if (TimeSeriesAggregateType.IsNullOrEmpty())
            {
                TimeSeriesAggregateType = options.ContainsKey(Definition(ss, "TimeSeries")?.Option2)
                    ? Definition(ss, "TimeSeries")?.Option2
                    : options.FirstOrDefault().Key;
            }
            return TimeSeriesAggregateType;
        }

        public string GetTimeSeriesValue(Context context, SiteSettings ss)
        {
            var options = ss.TimeSeriesValueOptions(context: context);
            if (TimeSeriesValue.IsNullOrEmpty())
            {
                TimeSeriesValue = options.ContainsKey(Definition(ss, "TimeSeries")?.Option3)
                    ? Definition(ss, "TimeSeries")?.Option3
                    : options.FirstOrDefault().Key;
            }
            return TimeSeriesValue;
        }

        public string GetKambanGroupByX(Context context, SiteSettings ss)
        {
            var options = ss.KambanGroupByOptions(context: context);
            if (KambanGroupByX.IsNullOrEmpty())
            {
                KambanGroupByX = options.ContainsKey(Definition(ss, "Kamban")?.Option1)
                    ? Definition(ss, "Kamban")?.Option1
                    : options.FirstOrDefault().Key;
            }
            return KambanGroupByX;
        }

        public string GetKambanGroupByY(Context context, SiteSettings ss)
        {
            var options = ss.KambanGroupByOptions(context: context);
            if (KambanGroupByY.IsNullOrEmpty())
            {
                KambanGroupByY = options.ContainsKey(Definition(ss, "Kamban")?.Option2)
                    ? Definition(ss, "Kamban")?.Option2
                    : options.FirstOrDefault().Key;
            }
            return KambanGroupByY;
        }

        public string GetKambanAggregationType(Context context, SiteSettings ss)
        {
            var options = ss.KambanAggregationTypeOptions(context: context);
            if (KambanAggregateType.IsNullOrEmpty())
            {
                KambanAggregateType = options.ContainsKey(Definition(ss, "Kamban")?.Option3)
                    ? Definition(ss, "Kamban")?.Option3
                    : options.FirstOrDefault().Key;
            }
            return KambanAggregateType;
        }

        public string GetKambanValue(Context context, SiteSettings ss)
        {
            var options = ss.KambanValueOptions(context: context);
            if (KambanValue.IsNullOrEmpty())
            {
                KambanValue = options.ContainsKey(Definition(ss, "Kamban")?.Option4)
                    ? Definition(ss, "Kamban")?.Option4
                    : options.FirstOrDefault().Key;
            }
            return KambanValue;
        }

        public int GetKambanColumns()
        {
            return KambanColumns ?? Parameters.General.KambanColumns;
        }

        private ViewModeDefinition Definition(SiteSettings ss, string name)
        {
            return Def.ViewModeDefinitionCollection.FirstOrDefault(o =>
                o.Id == ss.ReferenceType + "_" + name);
        }

        public void SetByForm(Context context, SiteSettings ss)
        {
            var columnFilterPrefix = "ViewFilters__";
            var columnFilterOnGridPrefix = "ViewFiltersOnGridHeader__";
            var columnSorterPrefix = "ViewSorters__";
            switch (context.Forms.ControlId())
            {
                case "ReduceViewFilters":
                    FiltersReduced = true;
                    break;
                case "ExpandViewFilters":
                    FiltersReduced = false;
                    break;
                case "ReduceAggregations":
                    AggregationsReduced = true;
                    break;
                case "ExpandAggregations":
                    AggregationsReduced = false;
                    break;
                case "ViewFilters_Reset":
                    Id = 0;
                    Name = null;
                    Incomplete = null;
                    Own = null;
                    NearCompletionTime = null;
                    Delay = null;
                    Overdue = null;
                    ColumnFilterHash = null;
                    Search = null;
                    ShowHistory = null;
                    break;
                case "ViewSorters_Reset":
                    ColumnSorterHash = null;
                    break;
                default:
                    foreach (string controlId in context.Forms.Keys)
                    {
                        switch (controlId)
                        {
                            case "ViewName":
                                Name = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewGridColumnsAll":
                                GridColumns = String(
                                    context: context,
                                    controlId: controlId).Deserialize<List<string>>();
                                break;
                            case "ViewFilters_FiltersDisplayType":
                                FiltersDisplayType = (DisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_AggregationsDisplayType":
                                AggregationsDisplayType = (DisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_BulkMoveTargetsCommand":
                                BulkMoveTargetsCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_BulkDeleteCommand":
                                BulkDeleteCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_EditImportSettings":
                                EditImportSettings = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_OpenExportSelectorDialogCommand":
                                OpenExportSelectorDialogCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_OpenBulkUpdateSelectorDialogCommand":
                                OpenBulkUpdateSelectorDialogCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_EditOnGridCommand":
                                EditOnGridCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_Incomplete":
                                Incomplete = Bool(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_Own":
                                Own = Bool(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_NearCompletionTime":
                                NearCompletionTime = Bool(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_Delay":
                                Delay = Bool(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_Overdue":
                                Overdue = Bool(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_ShowHistory":
                                ShowHistory = Bool(
                                    context: context,
                                    controlId: controlId);
                                ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
                                if (ShowHistory == true)
                                {
                                    ColumnSorterHash.Add(
                                        Rds.IdColumn(ss.ReferenceType),
                                        SqlOrderBy.Types.desc);
                                    ColumnSorterHash.Add(
                                        "Ver",
                                        SqlOrderBy.Types.desc);
                                }
                                break;
                            case "ViewFilters_Search":
                                Search = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewSorters":
                                SetSorters(
                                    context: context,
                                    ss: ss);
                                break;
                            case "ViewFilters_UpdateCommand":
                                UpdateCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_OpenCopyDialogCommand":
                                OpenCopyDialogCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_ReferenceCopyCommand":
                                ReferenceCopyCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_MoveTargetsCommand":
                                MoveTargetsCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_EditOutgoingMail":
                                EditOutgoingMail = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_DeleteCommand":
                                DeleteCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_OpenDeleteSiteDialogCommand":
                                OpenDeleteSiteDialogCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CalendarTimePeriod":
                                CalendarTimePeriod = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CalendarFromTo":
                                CalendarFromTo = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CalendarDate":
                                CalendarDate = Time(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CalendarGroupBy":
                                CalendarGroupBy = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CrosstabGroupByX":
                                CrosstabGroupByX = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CrosstabGroupByY":
                                CrosstabGroupByY = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CrosstabColumns":
                                CrosstabColumns = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CrosstabAggregateType":
                                CrosstabAggregateType = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CrosstabValue":
                                CrosstabValue = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CrosstabTimePeriod":
                                CrosstabTimePeriod = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CrosstabMonth":
                                CrosstabMonth = Time(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_ExportCrosstabCommand":
                                ExportCrosstabCommand = (CommandDisplayTypes)Int(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "GanttGroupBy":
                                GanttGroupBy = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "GanttSortBy":
                                GanttSortBy = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "GanttPeriod":
                                GanttPeriod = context.Forms.Int(controlId);
                                break;
                            case "GanttStartDate":
                                GanttStartDate = Time(
                                    context: context,
                                    controlId: controlId)
                                        .ToDateTime()
                                        .ToUniversal(context: context);
                                break;
                            case "TimeSeriesGroupBy":
                                TimeSeriesGroupBy = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "TimeSeriesAggregateType":
                                TimeSeriesAggregateType = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "TimeSeriesValue":
                                TimeSeriesValue = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "KambanGroupByX":
                                KambanGroupByX = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "KambanGroupByY":
                                KambanGroupByY = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "KambanAggregateType":
                                KambanAggregateType = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "KambanValue":
                                KambanValue = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "KambanColumns":
                                KambanColumns = context.Forms.Int(controlId);
                                break;
                            case "KambanAggregationView":
                                KambanAggregationView = Bool(
                                    context: context,
                                    controlId: controlId);
                                break;
                            default:
                                if (controlId.StartsWith(columnFilterPrefix))
                                {
                                    AddColumnFilterHash(
                                        context: context,
                                        ss: ss,
                                        columnName: controlId.Substring(columnFilterPrefix.Length),
                                        value: context.Forms.Data(controlId));
                                }
                                else if (controlId.StartsWith(columnFilterOnGridPrefix))
                                {
                                    AddColumnFilterHash(
                                        context: context,
                                        ss: ss,
                                        columnName: controlId.Substring(columnFilterOnGridPrefix.Length),
                                        value: context.Forms.Data(controlId));
                                }
                                else if (controlId.StartsWith(columnSorterPrefix))
                                {
                                    AddColumnSorterHash(
                                        context: context,
                                        ss: ss,
                                        columnName: controlId.Substring(columnSorterPrefix.Length),
                                        value: OrderByType(context.Forms.Data(controlId)));
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        private bool? Bool(Context context, string controlId)
        {
            var data = context.Forms.Bool(controlId);
            if (data)
            {
                return true;
            }
            else
            {
                return null;
            }
        }

        private int? Int(Context context, string controlId)
        {
            var data = context.Forms.Data(controlId).Trim();
            if (data != string.Empty)
            {
                return data.ToInt();
            }
            else
            {
                return null;
            }
        }

        private DateTime? Time(Context context, string controlId)
        {
            var data = context.Forms.DateTime(controlId);
            if (data.InRange())
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        private string String(Context context, string controlId)
        {
            var data = context.Forms.Data(controlId).Trim();
            if (data != string.Empty)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public string ColumnFilter(string columnName)
        {
            return ColumnFilterHash?.ContainsKey(columnName) == true
                ? ColumnFilterHash[columnName]
                : null;
        }

        public SqlOrderBy.Types ColumnSorter(string columnName)
        {
            return ColumnSorterHash?.ContainsKey(columnName) == true
                ? ColumnSorterHash[columnName]
                : SqlOrderBy.Types.release;
        }

        private void AddColumnFilterHash(
            Context context,
            SiteSettings ss,
            string columnName,
            string value)
        {
            if (ColumnFilterHash == null)
            {
                ColumnFilterHash = new Dictionary<string, string>();
            }
            var column = ss.GetColumnOrExtendedFieldColumn(
                context: context,
                columnName: columnName,
                extendedFieldType: "Filter");
            if (column != null)
            {
                if (value == "false"
                    && column.TypeName?.CsTypeSummary() == "bool"
                    && column.CheckFilterControlType == ColumnUtilities.CheckFilterControlTypes.OnOnly
                    && ColumnFilterHash.ContainsKey(columnName))
                {
                    ColumnFilterHash.Remove(columnName);
                }
                else if (value != string.Empty)
                {
                    if (ColumnFilterHash.ContainsKey(columnName))
                    {
                        ColumnFilterHash[columnName] = value;
                    }
                    else
                    {
                        ColumnFilterHash.Add(columnName, value);
                    }
                }
                else if (ColumnFilterHash.ContainsKey(columnName))
                {
                    ColumnFilterHash.Remove(columnName);
                }
            }
        }

        private void AddColumnSorterHash(
            Context context, SiteSettings ss, string columnName, SqlOrderBy.Types value)
        {
            if (ColumnSorterHash == null)
            {
                ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
            }
            var column = ss.GetColumn(context: context, columnName: columnName);
            if (column != null)
            {
                if (value != SqlOrderBy.Types.release)
                {
                    if (ColumnSorterHash.ContainsKey(columnName))
                    {
                        ColumnSorterHash.Remove(columnName);
                        ColumnSorterHash = ColumnSorterHash.ToDictionary(o => o.Key, o => o.Value);
                        ColumnSorterHash.Add(columnName, value);
                    }
                    else
                    {
                        ColumnSorterHash.Add(columnName, value);
                    }
                }
                else if (ColumnSorterHash.ContainsKey(columnName))
                {
                    ColumnSorterHash.Remove(columnName);
                    ColumnSorterHash = ColumnSorterHash.ToDictionary(o => o.Key, o => o.Value);
                }
            }
        }

        private void SetSorters(Context context, SiteSettings ss)
        {
            ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
            context.Forms.List("ViewSorters").ForEach(data =>
            {
                var columnName = data.Split_1st('&');
                var type = OrderByType(data.Split_2nd('&'));
                switch (type)
                {
                    case SqlOrderBy.Types.asc:
                    case SqlOrderBy.Types.desc:
                        if (ColumnSorterHash.ContainsKey(columnName))
                        {
                            ColumnSorterHash[columnName] = type;
                        }
                        else
                        {
                            ColumnSorterHash.Add(columnName, type);
                        }
                        break;
                }
            });
        }

        private SqlOrderBy.Types OrderByType(string type)
        {
            switch (type)
            {
                case "asc": return SqlOrderBy.Types.asc;
                case "desc": return SqlOrderBy.Types.desc;
                default: return SqlOrderBy.Types.release;
            }
        }

        public View GetRecordingData(Context context, SiteSettings ss)
        {
            var view = new View();
            view.Id = Id;
            view.Name = Name;
            if (GridColumns != null && GridColumns.Join() != ss.GridColumns.Join())
            {
                view.GridColumns = GridColumns;
            }
            if (FiltersDisplayType != DisplayTypes.Displayed)
            {
                view.FiltersDisplayType = FiltersDisplayType;
            }
            if (FiltersReduced != null)
            {
                view.FiltersReduced = FiltersReduced;
            }
            if (AggregationsDisplayType != DisplayTypes.Displayed)
            {
                view.AggregationsDisplayType = AggregationsDisplayType;
            }
            if (AggregationsReduced != null)
            {
                view.AggregationsReduced = AggregationsReduced;
            }
            if (Incomplete == true)
            {
                view.Incomplete = true;
            }
            if (Own == true)
            {
                view.Own = true;
            }
            if (NearCompletionTime == true)
            {
                view.NearCompletionTime = true;
            }
            if (Delay == true)
            {
                view.Delay = true;
            }
            if (Overdue == true)
            {
                view.Overdue = true;
            }
            if (BulkMoveTargetsCommand != CommandDisplayTypes.Displayed)
            {
                view.BulkMoveTargetsCommand = BulkMoveTargetsCommand;
            }
            if (BulkDeleteCommand != CommandDisplayTypes.Displayed)
            {
                view.BulkDeleteCommand = BulkDeleteCommand;
            }
            if (EditImportSettings != CommandDisplayTypes.Displayed)
            {
                view.EditImportSettings = EditImportSettings;
            }
            if (OpenExportSelectorDialogCommand != CommandDisplayTypes.Displayed)
            {
                view.OpenExportSelectorDialogCommand = OpenExportSelectorDialogCommand;
            }
            if (OpenBulkUpdateSelectorDialogCommand != CommandDisplayTypes.Displayed)
            {
                view.OpenBulkUpdateSelectorDialogCommand = OpenBulkUpdateSelectorDialogCommand;
            }
            if (EditOnGridCommand != CommandDisplayTypes.Displayed)
            {
                view.EditOnGridCommand = EditOnGridCommand;
            }
            if (ExportCrosstabCommand != CommandDisplayTypes.Displayed)
            {
                view.ExportCrosstabCommand = ExportCrosstabCommand;
            }
            if (UpdateCommand != CommandDisplayTypes.Displayed)
            {
                view.UpdateCommand = UpdateCommand;
            }
            if (OpenCopyDialogCommand != CommandDisplayTypes.Displayed)
            {
                view.OpenCopyDialogCommand = OpenCopyDialogCommand;
            }
            if (ReferenceCopyCommand != CommandDisplayTypes.Displayed)
            {
                view.ReferenceCopyCommand = ReferenceCopyCommand;
            }
            if (MoveTargetsCommand != CommandDisplayTypes.Displayed)
            {
                view.MoveTargetsCommand = MoveTargetsCommand;
            }
            if (EditOutgoingMail != CommandDisplayTypes.Displayed)
            {
                view.EditOutgoingMail = EditOutgoingMail;
            }
            if (DeleteCommand != CommandDisplayTypes.Displayed)
            {
                view.DeleteCommand = DeleteCommand;
            }
            if (OpenDeleteSiteDialogCommand != CommandDisplayTypes.Displayed)
            {
                view.OpenDeleteSiteDialogCommand = OpenDeleteSiteDialogCommand;
            }
            if (ColumnFilterHash?.Any() == true)
            {
                view.ColumnFilterHash = new Dictionary<string, string>();
                ColumnFilterHash
                    .Where(o =>
                    {
                        var column = ss?.GetColumnOrExtendedFieldColumn(
                            context: context,
                            columnName: o.Key,
                            extendedFieldType: "Filter");
                        if (column?.TypeName == null)
                        {
                            return false;
                        }
                        else if (column.TypeName.CsTypeSummary() == Types.CsString
                            && column.HasChoices() != true)
                        {
                            return o.Value?.IsNullOrEmpty() != true;
                        }
                        else if (column.TypeName.CsTypeSummary() == Types.CsBool)
                        {
                            return o.Value?.IsNullOrEmpty() != true;
                        }
                        else
                        {
                            var data = o.Value?.Deserialize<List<object>>();
                            return data != null
                                ? data.Any()
                                : o.Value?.IsNullOrEmpty() != true;
                        }
                    })
                    .ForEach(o => view.ColumnFilterHash.Add(o.Key, o.Value));
            }
            if (ColumnFilterSearchTypes?.Any() == true)
            {
                view.ColumnFilterSearchTypes = new Dictionary<string, Column.SearchTypes>();
                ColumnFilterSearchTypes.ForEach(o => view.ColumnFilterSearchTypes.Add(o.Key, o.Value));
            }
            if (ColumnSorterHash?.Any() == true)
            {
                view.ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
                ColumnSorterHash.ForEach(o => view.ColumnSorterHash.Add(o.Key, o.Value));
            }
            if (!Search.IsNullOrEmpty())
            {
                view.Search = Search;
            }
            if (!CalendarTimePeriod.IsNullOrEmpty())
            {
                view.CalendarTimePeriod = CalendarTimePeriod;
            }
            if (!CalendarFromTo.IsNullOrEmpty())
            {
                view.CalendarFromTo = CalendarFromTo;
            }
            if (CalendarDate?.InRange() == true)
            {
                view.CalendarDate = CalendarDate;
            }
            if (!CalendarGroupBy.IsNullOrEmpty())
            {
                view.CalendarGroupBy = CalendarGroupBy;
            }
            if (!CrosstabGroupByX.IsNullOrEmpty())
            {
                view.CrosstabGroupByX = CrosstabGroupByX;
            }
            if (!CrosstabGroupByY.IsNullOrEmpty())
            {
                view.CrosstabGroupByY = CrosstabGroupByY;
            }
            if (!CrosstabColumns.IsNullOrEmpty())
            {
                view.CrosstabColumns = CrosstabColumns;
            }
            if (!CrosstabAggregateType.IsNullOrEmpty())
            {
                view.CrosstabAggregateType = CrosstabAggregateType;
            }
            if (!CrosstabValue.IsNullOrEmpty())
            {
                view.CrosstabValue = CrosstabValue;
            }
            if (!CrosstabTimePeriod.IsNullOrEmpty())
            {
                view.CrosstabTimePeriod = CrosstabTimePeriod;
            }
            if (CrosstabMonth != GetCrosstabMonthDefault())
            {
                view.CrosstabMonth = CrosstabMonth;
            }
            if (!GanttGroupBy.IsNullOrEmpty())
            {
                view.GanttGroupBy = GanttGroupBy;
            }
            if (!GanttSortBy.IsNullOrEmpty())
            {
                view.GanttSortBy = GanttSortBy;
            }
            if (!TimeSeriesGroupBy.IsNullOrEmpty())
            {
                view.TimeSeriesGroupBy = TimeSeriesGroupBy;
            }
            if (!TimeSeriesAggregateType.IsNullOrEmpty())
            {
                view.TimeSeriesAggregateType = TimeSeriesAggregateType;
            }
            if (!TimeSeriesValue.IsNullOrEmpty())
            {
                view.TimeSeriesValue = TimeSeriesValue;
            }
            if (!KambanGroupByX.IsNullOrEmpty())
            {
                view.KambanGroupByX = KambanGroupByX;
            }
            if (!KambanGroupByY.IsNullOrEmpty())
            {
                view.KambanGroupByY = KambanGroupByY;
            }
            if (!KambanAggregateType.IsNullOrEmpty())
            {
                view.KambanAggregateType = KambanAggregateType;
            }
            if (!KambanValue.IsNullOrEmpty())
            {
                view.KambanValue = KambanValue;
            }
            if (KambanColumns != null && KambanColumns != Parameters.General.KambanColumns)
            {
                view.KambanColumns = KambanColumns;
            }
            if (KambanAggregationView == true)
            {
                view.KambanAggregationView = KambanAggregationView;
            }
            if (ShowHistory == true)
            {
                view.ShowHistory = true;
            }
            return view;
        }

        public SqlWhereCollection Where(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where = null,
            bool checkPermission = true,
            bool itemJoin = true)
        {
            if (where == null) where = new SqlWhereCollection();
            SetGeneralsWhere(
                context: context,
                ss: ss,
                where: where);
            SetColumnsWhere(
                context: context,
                ss: ss,
                where: where);
            SetSearchWhere(
                context: context,
                ss: ss,
                where: where,
                itemJoin: itemJoin);
            Permissions.SetCanReadWhere(
                context: context,
                ss: ss,
                where: where,
                checkPermission: checkPermission);
            if (RequestSearchCondition(
                context: context,
                ss: ss))
            {
                where.Add(raw: "(0=1)");
            }
            return where;
        }

        private void SetGeneralsWhere(Context context, SiteSettings ss, SqlWhereCollection where)
        {
            if (Incomplete == true && HasIncompleteColumns(
                context: context,
                ss: ss))
            {
                where.Add(
                    tableName: ss.ReferenceType,
                    columnBrackets: "\"Status\"".ToSingleArray(),
                    _operator: "<" + Parameters.General.CompletionCode);
            }
            if (Own == true && HasOwnColumns(context, ss))
            {
                where.Add(
                    tableName: ss.ReferenceType,
                    columnBrackets: new string[] { "\"Manager\"", "\"Owner\"" },
                    name: "_U",
                    value: context.UserId);
            }
            if (NearCompletionTime == true && HasNearCompletionTimeColumns(
                context: context,
                ss: ss))
            {
                where.Add(
                    tableName: ss.ReferenceType,
                    columnBrackets: "\"CompletionTime\"".ToSingleArray(),
                    _operator: " between '{0}' and '{1}'".Params(
                        DateTime.Now.ToLocal(context: context).Date
                            .AddDays(ss.NearCompletionTimeBeforeDays.ToInt() * (-1)),
                        DateTime.Now.ToLocal(context: context).Date
                            .AddDays(ss.NearCompletionTimeAfterDays.ToInt() + 1)
                            .AddMilliseconds(Parameters.Rds.MinimumTime * -1)
                            .ToString("yyyy/M/d H:m:s.fff")));
            }
            if (Delay == true && HasDelayColumns(
                context: context,
                ss: ss))
            {
                where
                    .Add(
                        tableName: ss.ReferenceType,
                        columnBrackets: "\"Status\"".ToSingleArray(),
                        name: "_U",
                        _operator: "<{0}".Params(Parameters.General.CompletionCode))
                    .Add(
                        tableName: ss.ReferenceType,
                        columnBrackets: "\"ProgressRate\"".ToSingleArray(),
                        _operator: "<",
                        raw: Def.Sql.ProgressRateDelay
                            .Replace("#TableName#", ss.ReferenceType));
            }
            if (Overdue == true && HasOverdueColumns(
                context: context,
                ss: ss))
            {
                where
                    .Add(
                        tableName: ss.ReferenceType,
                        columnBrackets: "\"Status\"".ToSingleArray(),
                        name: "_U",
                        _operator: "<{0}".Params(Parameters.General.CompletionCode))
                    .Add(
                        tableName: ss.ReferenceType,
                        columnBrackets: "\"CompletionTime\"".ToSingleArray(),
                        _operator: $"<{context.Sqls.CurrentDateTime}");
            }
        }

        public bool HasIncompleteColumns(Context context, SiteSettings ss)
        {
            return ss.HasAllColumns(
                context: context,
                parts: new string[]
                {
                    "Status"
                });
        }

        public bool HasOwnColumns(Context context, SiteSettings ss)
        {
            return ss.HasAllColumns(
                context: context,
                parts: new string[]
                {
                    "Manager",
                    "Owner"
                });
        }

        public bool HasNearCompletionTimeColumns(Context context, SiteSettings ss)
        {
            return ss.HasAllColumns(
                context: context,
                parts: new string[]
                {
                    "CompletionTime"
                });
        }

        public bool HasDelayColumns(Context context, SiteSettings ss)
        {
            return ss.HasAllColumns(
                context: context,
                parts: new string[]
                {
                    "Status",
                    "ProgressRate",
                    "CompletionTime"
                });
        }

        public bool HasOverdueColumns(Context context, SiteSettings ss)
        {
            return ss.HasAllColumns(
                context: context,
                parts: new string[]
                {
                    "Status",
                    "CompletionTime"
                });
        }

        public void SetAlwaysGetColumns(
            Context context,
            SiteSettings ss,
            List<Column> columns)
        {
            if (columns == null) return;
            SetByWhenViewProcessingServerScript(
                context: context,
                ss: ss);
            AlwaysGetColumns?.ForEach(columnName =>
            {
                var column = ss?.GetColumn(
                    context: context,
                    columnName: columnName);
                if (column != null)
                {
                    columns.Add(column);
                }
            });
        }

        public void SetColumnsWhere(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where,
            long? siteId = null,
            long? id = null,
            DateTime? timestamp = null)
        {
            SetByWhenViewProcessingServerScript(
                context: context,
                ss: ss);
            SetColumnsWhere(
                context: context,
                ss: ss,
                where: where,
                columnFilterHash: ColumnFilterHash,
                siteId: siteId,
                id: id,
                timestamp: timestamp);
            if (AdditionalWhere?.Any() == true)
            {
                where.AddRange(AdditionalWhere);
            }
            where.OnSelectingWhereExtendedSqls(
                context: context,
                ss: ss,
                extendedSqls: Parameters.ExtendedSqls?.Where(o => o.OnSelectingWhere),
                siteId: siteId,
                id: id,
                timestamp: timestamp,
                name: OnSelectingWhere,
                columnFilterHash: ColumnFilterHash,
                columnPlaceholders: ColumnPlaceholders);
        }

        private void SetColumnsWhere(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where,
            Dictionary<string, string> columnFilterHash,
            long? siteId,
            long? id,
            DateTime? timestamp)
        {
            columnFilterHash?
                .Select(data => new
                {
                    Column = ss.GetColumn(
                        context: context,
                        columnName: data.Key),
                    ColumnName = data.Key,
                    data.Value,
                    Or = data.Key.StartsWith("or_"),
                    And = data.Key.StartsWith("and_"),
                    OnSelectingWhere = data.Key == "OnSelectingWhere"
                })
                .Where(o => o.Column != null || o.Or || o.And || o.OnSelectingWhere)
                .ForEach(data =>
                {
                    if (data.Or)
                    {
                        var orColumnFilterHash = data.Value.Deserialize<Dictionary<string, string>>();
                        if (orColumnFilterHash?.Any() == true)
                        {
                            var or = new SqlWhereCollection();
                            SetColumnsWhere(
                                context: context,
                                ss: ss,
                                where: or,
                                columnFilterHash: orColumnFilterHash,
                                siteId: siteId,
                                id: id,
                                timestamp: timestamp);
                            if (or.Any()) where.Or(or: or);
                        }
                    }
                    else if (data.And)
                    {
                        var andColumnFilterHash = data.Value.Deserialize<Dictionary<string, string>>();
                        if (andColumnFilterHash?.Any() == true)
                        {
                            var and = new SqlWhereCollection();
                            SetColumnsWhere(
                                context: context,
                                ss: ss,
                                where: and,
                                columnFilterHash: andColumnFilterHash,
                                siteId: siteId,
                                id: id,
                                timestamp: timestamp);
                            if (and.Any()) where.Add(and: and);
                        }
                    }
                    else if (data.OnSelectingWhere)
                    {
                        where.OnSelectingWhereExtendedSqls(
                            context: context,
                            ss: ss,
                            extendedSqls: Parameters.ExtendedSqls?.Where(o => o.OnSelectingWhere),
                            siteId: siteId,
                            id: id,
                            timestamp: timestamp,
                            name: data.Value,
                            columnFilterHash: ColumnFilterHash,
                            columnPlaceholders: ColumnPlaceholders);
                    }
                    else if (data.ColumnName == "SiteTitle")
                    {
                        CsNumericColumns(
                            column: ss.GetColumn(
                                context: context,
                                columnName: "SiteId"),
                            value: data.Value,
                            where: where);
                    }
                    else
                    {
                        switch (data.Column.TypeName.CsTypeSummary())
                        {
                            case Types.CsBool:
                                CsBoolColumns(
                                    column: data.Column,
                                    value: data.Value,
                                    where: where);
                                break;
                            case Types.CsNumeric:
                                CsNumericColumns(
                                    column: data.Column,
                                    value: data.Value,
                                    where: where);
                                break;
                            case Types.CsDateTime:
                                CsDateTimeColumns(
                                    context: context,
                                    column: data.Column,
                                    value: data.Value,
                                    where: where);
                                break;
                            case Types.CsString:
                                CsStringColumns(
                                    context: context,
                                    column: data.Column,
                                    value: data.Value,
                                    where: where);
                                break;
                        }
                    }
                });
        }

        private void CsBoolColumns(Column column, string value, SqlWhereCollection where)
        {
            switch (value?.ToLower())
            {
                case "1":
                case "true":
                    where.Bool(column, true);
                    break;
                case "2":
                case "false":
                    where.Add(or: new SqlWhereCollection()
                        .Bool(column, null)
                        .Bool(column, false));
                    break;
            }
        }

        private void CsNumericColumns(Column column, string value, SqlWhereCollection where)
        {
            long number = 0;
            var collection = new SqlWhereCollection();
            if (long.TryParse(value, out number))
            {
                collection.Add(CsNumericColumnsWhere(column, number));
            }
            else
            {
                var param = value.Deserialize<List<string>>();
                if (param?.Any() != true) return;
                collection.AddRange(param
                    .Where(o => o.RegexExists(@"^-?[0-9\.]*,-?[0-9\.]*$"))
                    .SelectMany(o => CsNumericRangeColumns(column, o)));
                collection.AddRange(param
                    .Where(o => o == "\t")
                    .SelectMany(o => CsNumericColumnsWhereNull(column)));
                var valueWhere = CsNumericColumnsWhere(
                    column,
                    param
                        .Where(o => !o.RegexExists(@"^-?[0-9\.]*,-?[0-9\.]*$"))
                        .Where(o => o != "\t"));
                if (valueWhere != null)
                {
                    collection.Add(valueWhere);
                }
            }
            if (collection.Any())
            {
                where.Add(or: collection);
            }
        }

        private SqlWhere CsNumericColumnsWhere(Column column, long param)
        {
            return new SqlWhere(
                tableName: column.TableName(),
                columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                name: column.Name,
                _operator: $"={param}");
        }

        private SqlWhere CsNumericColumnsWhere(Column column, IEnumerable<string> param)
        {
            var numList = param.Select(o => o.ToDecimal());
            if (!numList.Any()) { return null; }
            return new SqlWhere(
                tableName: column.TableName(),
                columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                name: column.Name,
                _operator: " in ({0})".Params(numList.Join()));
        }

        private IEnumerable<SqlWhere> CsNumericColumnsWhereNull(Column column)
        {
            yield return new SqlWhere(
                tableName: column.TableName(),
                    columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                    _operator: " is null");
            if (column.Nullable != true)
            {
                yield return new SqlWhere(
                tableName: column.TableName(),
                columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                _operator: "=0");
                if (column.Type == Column.Types.User && SiteInfo.AnonymousId != 0)
                {
                    yield return new SqlWhere(
                    tableName: column.TableName(),
                    columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                    _operator: $"={SiteInfo.AnonymousId}");
                }
            }
        }

        private IEnumerable<SqlWhere> CsNumericRangeColumns(Column column, string param)
        {
            var from = param.Split_1st();
            var to = param.Split_2nd();
            yield return new SqlWhere(
                tableName: column.TableName(),
                columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                _operator: from == string.Empty
                    ? "<={0}".Params(to.ToDecimal())
                    : to == string.Empty
                        ? ">={0}".Params(from.ToDecimal())
                        : " between {0} and {1}".Params(from.ToDecimal(), to.ToDecimal()));
            if (column.Nullable != true
                && (to == string.Empty && from.ToDecimal() <= 0)
                    || (from == string.Empty && to.ToDecimal() >= 0)
                    || (from != string.Empty && to != string.Empty && from.ToDecimal() <= 0 && to.ToDecimal() >= 0))
            {
                yield return new SqlWhere(
                    tableName: column.TableName(),
                    columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                    _operator: " is null");
            }
        }

        private void CsDateTimeColumns(
            Context context, Column column, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
            if (param?.Any() == true)
            {
                where.Add(or: new SqlWhereCollection(
                    CsDateTimeColumnsWhere(
                        context: context,
                        column: column,
                        param: param),
                    CsDateTimeColumnsWhereNull(
                        context: context,
                        column: column,
                        param: param)));
            }
        }

        private SqlWhere CsDateTimeColumnsWhere(
            Context context, Column column, List<string> param)
        {
            var today = DateTime.Now.ToDateTime().ToLocal(context: context).Date;
            var addMilliseconds = Parameters.Rds.MinimumTime * -1;
            var between = "#TableBracket#.\"{0}\" between '{1}' and '{2}'";
            var ymdhms = "yyyy/M/d H:m:s";
            var ymdhmsfff = "yyyy/M/d H:m:s.fff";
            return param.Any(o => o != "\t")
                ? new SqlWhere(
                    tableName: column.TableName(),
                    raw: param.Select(range =>
                    {
                        var from = range.Split_1st();
                        var to = range.Split_2nd();
                        switch (from)
                        {
                            case "Today":
                                return between.Params(
                                    column.Name,
                                    ConvertDateTimeParam(
                                        context: context,
                                        column: column,
                                        dt: today,
                                        format: ymdhms),
                                    ConvertDateTimeParam(
                                        context: context,
                                        column: column,
                                        dt: today
                                            .AddDays(1)
                                            .AddMilliseconds(addMilliseconds),
                                        format: ymdhmsfff));
                            case "ThisMonth":
                                return between.Params(
                                    column.Name,
                                    ConvertDateTimeParam(
                                        context: context,
                                        column: column,
                                        dt: new DateTime(today.Year, today.Month, 1),
                                        format: ymdhms),
                                    ConvertDateTimeParam(
                                        context: context,
                                        column: column,
                                        dt: new DateTime(today.Year, today.Month, 1)
                                            .AddMonths(1)
                                            .AddMilliseconds(addMilliseconds),
                                        format: ymdhmsfff));
                            case "ThisYear":
                                return between.Params(
                                    column.Name,
                                    ConvertDateTimeParam(
                                        context: context,
                                        column: column,
                                        dt: new DateTime(today.Year, 1, 1),
                                        format: ymdhms),
                                    ConvertDateTimeParam(
                                        context: context,
                                        column: column,
                                        dt: new DateTime(today.Year, 1, 1)
                                            .AddYears(1)
                                            .AddMilliseconds(addMilliseconds),
                                        format: ymdhmsfff));
                        }
                        if (!from.IsNullOrEmpty() && !to.IsNullOrEmpty())
                        {
                            return between.Params(
                                column.Name,
                                ConvertDateTimeParam(
                                    context: context,
                                    column: column,
                                    dateTimeString: from,
                                    format: ymdhms),
                                ConvertDateTimeParam(
                                    context: context,
                                    column: column,
                                    dateTimeString: to,
                                    format: ymdhmsfff));
                        }
                        else if (to.IsNullOrEmpty())
                        {
                            return "#TableBracket#.\"{0}\">='{1}'".Params(
                                column.Name,
                                ConvertDateTimeParam(
                                    context: context,
                                    column: column,
                                    dateTimeString: from,
                                    format: ymdhms));
                        }
                        else
                        {
                            return "#TableBracket#.\"{0}\"<='{1}'".Params(
                                column.Name,
                                ConvertDateTimeParam(
                                    context: context,
                                    column: column,
                                    dateTimeString: to,
                                    format: ymdhmsfff));
                        }
                    }).Join(" or "))
                : null;
        }

        private string ConvertDateTimeParam(
            Context context,
            Column column,
            string dateTimeString,
            string format)
        {
            return ConvertDateTimeParam(
                context: context,
                dt: dateTimeString.ToDateTime(),
                column: column,
                format: format);
        }

        private static string ConvertDateTimeParam(
            Context context,
            Column column,
            DateTime dt,
            string format)
        {
            switch (column.Name)
            {
                case "CompletionTime":
                    dt = column.DateTimepicker()
                        ? dt
                        : dt.AddDays(1);
                    break;
            }
            return dt
                .ToDateTime()
                .ToUniversal(context: context)
                .ToString(format);
        }

        private SqlWhere CsDateTimeColumnsWhereNull(
            Context context, Column column, List<string> param)
        {
            return param.Any(o => o == "\t")
                ? new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        _operator: " is null"),
                    new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        _operator: " not between '{0}' and '{1}'".Params(
                            Parameters.General.MinTime.ToUniversal(context: context)
                                .ToString("yyyy/M/d H:m:s"),
                            Parameters.General.MaxTime.ToUniversal(context: context)
                                .ToString("yyyy/M/d H:m:s")))))
                : null;
        }

        private void CsStringColumns(
            Context context,
            Column column,
            string value,
            SqlWhereCollection where)
        {
            if (value.IsNullOrEmpty())
            {
                return;
            }
            var searchType = ColumnFilterSearchTypes?.ContainsKey(column.ColumnName) == true
                ? ColumnFilterSearchTypes.Get(column.ColumnName)
                : column.SearchType;
            var param = value.Deserialize<List<string>>();
            var json = param?.ToJson();
            if (column.HasChoices() || column.ColumnName == "DeptCode")
            {
                if (column.MultipleSelections == true)
                {
                    switch (searchType)
                    {
                        case Column.SearchTypes.ExactMatch:
                        case Column.SearchTypes.ExactMatchMultiple:
                            if (param?.Any() == true)
                            {
                                CreateCsStringSqlWhereCollection(
                                    context: context,
                                    column: column,
                                    where: where,
                                    param: json?.ToSingleList(),
                                    nullable: param.All(o => o == "\t"),
                                    _operator: "=");
                            }
                            break;
                        case Column.SearchTypes.ForwardMatch:
                        case Column.SearchTypes.ForwardMatchMultiple:
                            if (param?.Count() == 1 && param.FirstOrDefault() == "\t")
                            {
                                where.Add(CsStringColumnsWhereNull(column: column));
                            }
                            else
                            {
                                CreateCsStringSqlWhereLike(
                                    context: context,
                                    column: column,
                                    value: context.Sqls.EscapeValue(!json.IsNullOrEmpty()
                                        ? json.Substring(0, json.Length - 1)
                                        : value),
                                    where: where,
                                    query: "(\"{0}\".\"{1}\"" + context.Sqls.Like + "@{3}{4}" + context.Sqls.Escape + ")");
                            }
                            break;
                        default:
                            if (param?.Any() == true)
                            {
                                CreateCsStringSqlWhereCollection(
                                    context: context,
                                    column: column,
                                    where: where,
                                    param: param
                                        .Where(o => o != "\t")
                                        .Select(o => StringInJson(value: o)),
                                    nullable: param.Any(o => o == "\t"),
                                    _operator: context.Sqls.Like);
                            }
                            break;
                    }
                }
                else if (param?.Any() == true)
                {
                    CreateCsStringSqlWhereCollection(
                        context: context,
                        column: column,
                        where: where,
                        param: param.Where(o => o != "\t"),
                        nullable: param.Any(o => o == "\t"),
                        _operator: "=");
                }
            }
            else
            {
                if (value == " " || value == "　")
                {
                    where.Add(CsStringColumnsWhereNull(column: column));
                }
                else
                {
                    var or = new SqlWhereCollection();
                    switch (searchType)
                    {
                        case Column.SearchTypes.ExactMatch:
                            CreateCsStringSqlWhereCollection(
                                context: context,
                                column: column,
                                where: where,
                                param: value.ToSingleList(),
                                nullable: false,
                                _operator: "=");
                            break;
                        case Column.SearchTypes.ForwardMatch:
                            CreateCsStringSqlWhereLike(
                                context: context,
                                column: column,
                                value: context.Sqls.EscapeValue(value),
                                where: where,
                                query: "(\"{0}\".\"{1}\"" + context.Sqls.Like + "@{3}{4}" + context.Sqls.Escape + ")");
                            break;
                        case Column.SearchTypes.PartialMatch:
                            CreateCsStringSqlWhereLike(
                                context: context,
                                column: column,
                                value: context.Sqls.EscapeValue(value),
                                where: where,
                                query: "(\"{0}\".\"{1}\"" + context.Sqls.Like + "{2}@{3}{4}" + context.Sqls.Escape + ")");
                            break;
                        case Column.SearchTypes.ExactMatchMultiple:
                            if (param?.Any() == true)
                            {
                                CreateCsStringSqlWhereCollection(
                                    context: context,
                                    column: column,
                                    where: where,
                                    param: param.Where(o => o != "\t"),
                                    nullable: param.Any(o => o == "\t"),
                                    _operator: "=");
                            }
                            break;
                        case Column.SearchTypes.ForwardMatchMultiple:
                            if (param?.Any() == true)
                            {
                                CreateCsStringSqlWhereCollection(
                                    context: context,
                                    column: column,
                                    where: where,
                                    param: param
                                        .Where(o => o != "\t")
                                        .Select(o => $"{o}%"),
                                    nullable: param.Any(o => o == "\t"),
                                    _operator: context.Sqls.Like);
                            }
                            break;
                        case Column.SearchTypes.PartialMatchMultiple:
                            if (param?.Any() == true)
                            {
                                CreateCsStringSqlWhereCollection(
                                    context: context,
                                    column: column,
                                    where: where,
                                    param: param
                                        .Where(o => o != "\t")
                                        .Select(o => $"%{o}%"),
                                    nullable: param.Any(o => o == "\t"),
                                    _operator: context.Sqls.Like);
                            }
                            break;
                    }
                }
            }
        }

        private void CreateCsStringSqlWhereCollection(
            Context context,
            Column column,
            SqlWhereCollection where,
            IEnumerable<string> param,
            bool nullable,
            string _operator)
        {
            var or = new SqlWhereCollection();
            if (param.Any())
            {
                or.Add(CsStringColumnsWhere(
                    column: column,
                    param: param,
                    _operator: _operator));
            }
            if (nullable)
            {
                or.Add(CsStringColumnsWhereNull(column: column));
            }
            or.RemoveAll(o => o == null);
            if (or.Any())
            {
                where.Add(or: or);
            }
        }

        private SqlWhere CsStringColumnsWhere(
            Column column,
            IEnumerable<string> param,
            string _operator)
        {
            return new SqlWhere(
                tableName: column.TableItemTitleCases(),
                columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                name: Strings.NewGuid(),
                value: param,
                _operator: _operator,
                multiParamOperator: " or ");
        }

        private string StringInJson(string value)
        {
            if (value.IsNullOrEmpty()) return string.Empty;
            var json = value.ToSingleList().ToJson();
            return $"%{json.Substring(1, json.Length - 2)}%";
        }

        private SqlWhere CsStringColumnsWhereNull(Column column)
        {
            return new SqlWhere(or: new SqlWhereCollection(
                new SqlWhere(
                    tableName: column.TableItemTitleCases(),
                    columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                    _operator: " is null"),
                new SqlWhere(
                    tableName: column.TableItemTitleCases(),
                    columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                    _operator: "=''"),
                new SqlWhere(
                    tableName: column.TableItemTitleCases(),
                    columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                    _operator: "='[]'",
                    _using: column.MultipleSelections == true)));
        }

        private void CreateCsStringSqlWhereLike(
            Context context,
            Column column,
            string value,
            SqlWhereCollection where,
            string query)
        {
            var tableName = column.TableItemTitleCases();
            var name = Strings.NewGuid();
            where.SqlWhereLike(
                tableName: tableName,
                name: name,
                searchText: value,
                clauseCollection: query
                    .Params(
                        tableName,
                        column.Name,
                        context.Sqls.WhereLikeTemplateForward,
                        name,
                        context.Sqls.WhereLikeTemplate)
                    .ToSingleList());
        }

        private void SetByWhenViewProcessingServerScript(
            Context context,
            SiteSettings ss)
        {
            if (!WhenViewProcessingServerScriptExecuted)
            {
                ServerScriptUtilities.Execute(
                    context: context,
                    ss: ss,
                    itemModel: null,
                    view: this,
                    where: script => script.WhenViewProcessing == true,
                    condition: "WhenViewProcessing");
                WhenViewProcessingServerScriptExecuted = true;
            }
        }

        public SqlOrderByCollection OrderBy(
            Context context,
            SiteSettings ss,
            SqlOrderByCollection orderBy = null)
        {
            orderBy = orderBy ?? new SqlOrderByCollection();
            if (ColumnSorterHash?.Any() == true)
            {
                ColumnSorterHash?.ForEach(data =>
                {
                    var column = ss.GetColumn(
                        context: context,
                        columnName: data.Key);
                    if (column != null)
                    {
                        OrderBy(
                            context: context,
                            ss: ss,
                            orderBy: orderBy,
                            data: data,
                            column: column);
                    }
                });
            }
            return orderBy?.Any() != true
                ? new SqlOrderByCollection().Add(
                    tableName: ss.ReferenceType,
                    columnBracket: "\"UpdatedTime\"",
                    orderType: SqlOrderBy.Types.desc)
                : orderBy;
        }

        private static void OrderBy(
            Context context,
            SiteSettings ss,
            SqlOrderByCollection orderBy,
            KeyValuePair<string, SqlOrderBy.Types> data,
            Column column)
        {
            var tableName = column.TableName();
            switch (column.Name)
            {
                case "Title":
                case "ItemTitle":
                    orderBy.Add(new SqlOrderBy(
                        columnBracket: "\"Title\"",
                        orderType: data.Value,
                        tableName: $"{tableName}_Items",
                        isNullValue: "''"));
                    break;
                case "TitleBody":
                    orderBy.Add(new SqlOrderBy(
                        columnBracket: "\"Title\"",
                        orderType: data.Value,
                        tableName: $"{tableName}_Items",
                        isNullValue: "''"));
                    orderBy.Add(
                        column: ss.GetColumn(
                            context: context,
                            columnName: column.Joined
                                ? $"{tableName},Body"
                                : "Body"),
                        orderType: data.Value,
                        isNullValue: column.IsNullValue(context: context));
                    break;
                default:
                    if (column.Linked(withoutWiki: true))
                    {
                        orderBy.Add(new SqlOrderBy(
                            orderType: data.Value,
                            sub: Rds.SelectItems(
                                column: Rds.ItemsColumn().Title(),
                                where: Rds.ItemsWhere()
                                    .SiteId_In(column.SiteSettings.Links
                                        .Where(o => o.SiteId > 0)
                                        .Where(o => o.ColumnName == column.Name)
                                        .Select(o => o.SiteId))
                                    .ReferenceId(raw: $"{context.SqlCommandText.CreateTryCast(tableName, column.Name, column.TypeName, "bigint")}"))));
                    }
                    else
                    {
                        orderBy.Add(
                            column: column,
                            orderType: data.Value,
                            isNullValue: column.IsNullValue(context: context));
                    }
                    break;
            }
        }

        private void SetSearchWhere(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where,
            bool itemJoin)
        {
            var or = new SqlWhereCollection();
            Search?
                .Replace("　", " ")
                .Replace(" or ", "\n")
                .Split('\n')
                .Where(o => !o.IsNullOrEmpty())
                .ForEach(search =>
                    or.Add(and: new SqlWhereCollection().FullTextWhere(
                        context: context,
                        ss: ss,
                        searchText: search,
                        itemJoin: itemJoin)));
            if (or.Any()) where.Add(or: or);
        }

        public bool RequestSearchCondition(Context context, SiteSettings ss)
        {
            var where = new SqlWhereCollection();
            SetColumnsWhere(
                context: context,
                ss: ss,
                where: where);
            return (ss.AlwaysRequestSearchCondition == true)
                && (Incomplete != true
                    && Own != true
                    && NearCompletionTime != true
                    && Delay != true
                    && Overdue != true
                    && where.Any() != true
                    && Search.IsNullOrEmpty());
        }

        public SqlParamCollection Param(
            Context context,
            SiteSettings ss,
            SqlParamCollection param = null)
        {
            if (param == null) param = new SqlParamCollection();
            context.ExtendedFields
                ?.Where(extendedField =>
                    extendedField.FieldType == "Filter"
                    && extendedField.SqlParam
                    && ColumnFilterHash?.ContainsKey(extendedField.Name) == true)
                .Select(extendedField => new SqlParam()
                {
                    VariableName = extendedField.Name,
                    Value = ColumnFilterHash[extendedField.Name],
                    NoCount = true
                })
                .ForEach(o => param.Add(o));
            return param;
        }

        public bool GetFiltersReduced()
        {
            if (FiltersReduced != null)
            {
                return FiltersReduced.ToBool();
            }
            switch (FiltersDisplayType)
            {
                case DisplayTypes.Hidden:
                    return true;
                default:
                    return false;
            }
        }

        public bool GetAggregationsReduced()
        {
            if (AggregationsReduced != null)
            {
                return AggregationsReduced.ToBool();
            }
            switch (AggregationsDisplayType)
            {
                case DisplayTypes.Hidden:
                    return true;
                default:
                    return false;
            }
        }
    }
}
