using Azure.Core;
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

        public enum ApiDataTypes : int
        {
            Default = 0,
            KeyValues = 1
        }

        public int Id;
        public string Name;
        public string DefaultMode;
        public List<string> GridColumns;
        public List<string> FilterColumns;
        public DisplayTypes? FiltersDisplayType;
        public bool? GuidesReduced;
        public bool? FiltersReduced;
        public DisplayTypes? AggregationsDisplayType;
        public bool? AggregationsReduced;
        public bool? KeepFilterState;
        public bool? Incomplete;
        public bool? Own;
        public bool? NearCompletionTime;
        public bool? Delay;
        public bool? Overdue;
        public bool? KeepSorterState;
        public CommandDisplayTypes? BulkMoveTargetsCommand;
        public CommandDisplayTypes? BulkDeleteCommand;
        public CommandDisplayTypes? EditImportSettings;
        public CommandDisplayTypes? OpenExportSelectorDialogCommand;
        public CommandDisplayTypes? OpenBulkUpdateSelectorDialogCommand;
        public CommandDisplayTypes? EditOnGridCommand;
        public CommandDisplayTypes? ExportCrosstabCommand;
        public CommandDisplayTypes? CreateCommand;
        public CommandDisplayTypes? UpdateCommand;
        public CommandDisplayTypes? OpenCopyDialogCommand;
        public CommandDisplayTypes? ReferenceCopyCommand;
        public CommandDisplayTypes? MoveTargetsCommand;
        public CommandDisplayTypes? EditOutgoingMail;
        public CommandDisplayTypes? DeleteCommand;
        public CommandDisplayTypes? OpenDeleteSiteDialogCommand;
        public Dictionary<string, string> ColumnFilterHash;
        public Dictionary<string, string> ColumnFilterExpressions;
        public Dictionary<string, Column.SearchTypes> ColumnFilterSearchTypes;
        public List<string> ColumnFilterNegatives;
        public string Search;
        public Dictionary<string, SqlOrderBy.Types> ColumnSorterHash;
        public Dictionary<string, string> ViewExtensionsHash;
        public Dictionary<string, ApiColumn> ApiColumnHash;
        public ApiColumn.KeyDisplayTypes ApiColumnKeyDisplayType;
        public ApiColumn.ValueDisplayTypes ApiColumnValueDisplayType;
        public string CalendarSuffix;
        public long CalendarSiteId;
        public string CalendarTimePeriod;
        public string CalendarFromTo;
        public DateTime? CalendarDate;
        public DateTime? CalendarStart;
        public DateTime? CalendarEnd;
        public string CalendarType;
        public string CalendarViewType;
        public string CalendarGroupBy;
        public bool? CalendarShowStatus;
        public Dictionary<string, DateTime?> CalendarDateHash;
        public Dictionary<string, DateTime?> CalendarStartHash;
        public Dictionary<string, DateTime?> CalendarEndHash;
        public Dictionary<string, string> CalendarViewTypeHash;
        public Dictionary<string, DashboardPartLayout> DashboardPartLayoutHash;
        public string IndexSuffix;
        public string CrosstabGroupByX;
        public string CrosstabGroupByY;
        public string CrosstabColumns;
        public string CrosstabAggregateType;
        public string CrosstabValue;
        public string CrosstabTimePeriod;
        public DateTime? CrosstabMonth;
        public bool? CrosstabNotShowZeroRows;
        public string GanttGroupBy;
        public string GanttSortBy;
        public int? GanttPeriod;
        public DateTime? GanttStartDate;
        public string TimeSeriesGroupBy;
        public string TimeSeriesAggregateType;
        public string TimeSeriesValue;
        public string TimeSeriesChartType;
        public string TimeSeriesHorizontalAxis;
        public List<AnalyPartSetting> AnalyPartSettings;
        public string KambanGroupByX;
        public string KambanGroupByY;
        public string KambanAggregateType;
        public string KambanValue;
        public int? KambanColumns;
        public bool? KambanAggregationView;
        public bool? KambanShowStatus;
        public string KambanSuffix;
        public List<int> Depts;
        public List<int> Groups;
        public List<int> Users;
        public ApiDataTypes ApiDataType;
        public bool? ApiGetMailAddresses;
        [NonSerialized]
        public SqlWhereCollection AdditionalWhere;
        [NonSerialized]
        public bool WhenViewProcessingServerScriptExecuted;
        [NonSerialized]
        public List<string> AlwaysGetColumns;
        [NonSerialized]
        public string OnSelectingWhere;
        [NonSerialized]
        public string OnSelectingOrderBy;
        [NonSerialized]
        public Dictionary<string, string> ColumnPlaceholders;
        // compatibility Version 1.008
        public string KambanGroupBy;
        // compatibility Version 1.012
        public string CalendarColumn;
        public bool? ShowHistory;
        public bool? MergeSessionViewFilters;
        public bool? MergeSessionViewSorters;

        public View()
        {
        }

        public View(Context context, SiteSettings ss, string prefix = "")
        {
            SetByForm(
                context: context,
                ss: ss,
                prefix: prefix);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
        }

        public long GetCalendarSiteId(SiteSettings ss)
        {
            if (ss.DashboardParts.Count != 0)
            {
                return CalendarSiteId;
            }
            return ss.SiteId;
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
            if (!CalendarSuffix.IsNullOrEmpty()
                && CalendarDateHash?.TryGetValue($"CalendarDate{CalendarSuffix}", out var calendarDate) == true)
            {
                return calendarDate ?? DateTime.Now;
            }
            return CalendarDate ?? DateTime.Now;
        }

        public DateTime? GetCalendarStart()
        {
            if (!CalendarSuffix.IsNullOrEmpty()
                && CalendarStartHash?.TryGetValue($"CalendarStart{CalendarSuffix}", out var calendarStart) == true)
            {
                return calendarStart;
            }
            return CalendarStart;
        }

        public DateTime? GetCalendarEnd()
        {
            if (!CalendarSuffix.IsNullOrEmpty()
                && CalendarEndHash?.TryGetValue($"CalendarEnd{CalendarSuffix}", out var calendarEnd) == true)
            {
                return calendarEnd;
            }
            return CalendarEnd;
        }

        public string GetCalendarViewType()
        {
            if (!CalendarSuffix.IsNullOrEmpty()
                && CalendarViewTypeHash?.TryGetValue($"CalendarViewType{CalendarSuffix}", out var calendarViewType) == true)
            {
                return calendarViewType ?? "dayGridMonth";
            }
            return CalendarViewType ?? "dayGridMonth";
        }

        public string GetCalendarType(SiteSettings ss)
        {
            if (ss.DashboardParts.Count != 0)
            {
                return CalendarType;
            }
            return ss.CalendarType.ToString();
        }

        public string GetCalendarSuffix()
        {
            return CalendarSuffix;
        }

        public bool GetCalendarShowStatus()
        {
            return CalendarShowStatus ?? false;
        }

        public string GetCalendarGroupBy()
        {
            return !CalendarGroupBy.IsNullOrEmpty()
                ? CalendarGroupBy
                : string.Empty;
        }

        public string GetIndexSuffix()
        {
            return IndexSuffix;
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

        public string GetTimeSeriesChartType(Context context, SiteSettings ss)
        {
            var options = ss.TimeSeriesChartTypeOptions(context: context);
            if (TimeSeriesChartType.IsNullOrEmpty())
            {
                TimeSeriesChartType = options.ContainsKey(Definition(ss, "TimeSeries")?.Option4)
                    ? Definition(ss, "TimeSeries")?.Option4
                    : options.FirstOrDefault().Key;
            }
            return TimeSeriesChartType;
        }

        public string GetTimeSeriesHorizontalAxis(Context context, SiteSettings ss)
        {
            var options = ss.TimeSeriesHorizontalAxisOptions(context: context);
            if (TimeSeriesHorizontalAxis.IsNullOrEmpty())
            {
                TimeSeriesHorizontalAxis = options.ContainsKey(Definition(ss, "TimeSeries")?.Option5)
                    ? Definition(ss, "TimeSeries")?.Option5
                    : options.FirstOrDefault().Key;
            }
            switch (TimeSeriesHorizontalAxis)
            {
                case "Histories":
                    return TimeSeriesHorizontalAxis;
                default:
                    return ss.GetColumn(
                        context: context,
                        columnName: TimeSeriesHorizontalAxis)?.ColumnName;
            }
        }

        public string GetAnalyGroupBy(Context context, SiteSettings ss, string value)
        {
            var options = ss.AnalyGroupByOptions(context: context);
            var ret = value;
            if (value.IsNullOrEmpty())
            {
                ret = options.ContainsKey(Definition(ss, "TimeSeries")?.Option1)
                    ? Definition(ss, "TimeSeries")?.Option1
                    : options.FirstOrDefault().Key;
            }
            return ret;
        }

        public string GetAnalyAggregationTarget(Context context, SiteSettings ss, string value)
        {
            var options = ss.AnalyAggregationTargetOptions(context: context);
            var ret = value;
            if (value.IsNullOrEmpty())
            {
                ret = options.ContainsKey(Definition(ss, "TimeSeries")?.Option1)
                    ? Definition(ss, "TimeSeries")?.Option1
                    : options.FirstOrDefault().Key;
            }
            return ret;
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

        public string GetKambanSuffix()
        {
            return KambanSuffix;
        }

        private ViewModeDefinition Definition(SiteSettings ss, string name)
        {
            return Def.ViewModeDefinitionCollection.FirstOrDefault(o =>
                o.Id == ss.ReferenceType + "_" + name);
        }

        public void SetByForm(
            Context context,
            SiteSettings ss,
            string prefix = "")
        {
            var columnFilterPrefix = $"{prefix}ViewFilters__";
            var columnFilterOnGridPrefix = $"{prefix}ViewFiltersOnGridHeader__";
            var columnSorterPrefix = $"{prefix}ViewSorters__";
            var columnViewExtensionPrefix = $"{prefix}ViewExtensions__";
            switch (context.Forms.ControlId())
            {
                case "ReduceGuides":
                    GuidesReduced = true;
                    break;
                case "ExpandGuides":
                    GuidesReduced = false;
                    break;
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
                    ResetViewFilters(ss: ss);
                    break;
                case "ViewSorters_Reset":
                    ResetViewSorters(ss: ss);
                    break;
                case "AddAnalyPart":
                    AddAnalyPart(context: context);
                    break;
                default:
                    if (context.Forms.ControlId().StartsWith("DeleteAnalyPart_"))
                    {
                        DeleteAnalyPart(context: context);
                    }
                    foreach (var controlId in context.Forms.Keys)
                    {
                        switch (ControlIdWithOutPrefix(
                            controlId: controlId,
                            prefix: prefix))
                        {
                            case "ViewName":
                                Name = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "DefaultViewMode":
                                DefaultMode = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewGridColumnsAll":
                                GridColumns = String(
                                    context: context,
                                    controlId: controlId).Deserialize<List<string>>();
                                break;
                            case "ViewFiltersFilterColumnsAll":
                                FilterColumns = String(
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
                            case "ViewFilters_KeepFilterState":
                                KeepFilterState = Bool(
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
                                if (ShowHistory == true)
                                {
                                    SetSorterHashOnShowHistory(ss);
                                }
                                break;
                            case "ViewFilters_Search":
                                Search = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewFilters_Negative":
                                if (!context.Forms.Get(controlId).IsNullOrEmpty())
                                {
                                    var filterName = String(
                                        context: context,
                                        controlId: controlId);
                                    if (filterName.Contains(columnFilterPrefix))
                                    {
                                        filterName = filterName.Substring(columnFilterPrefix.Length);
                                        filterName = filterName.Replace("_NumericRange", string.Empty);
                                        filterName = filterName.Replace("_DateRange", string.Empty);
                                    }
                                    if (UseNegativeFilters(
                                        ss: ss,
                                        name: filterName) != true)
                                    {
                                        if (ColumnFilterNegatives == null)
                                        {
                                            ColumnFilterNegatives = new List<string>();
                                        }
                                        ColumnFilterNegatives.Add(filterName);
                                    }
                                }
                                break;
                            case "ViewFilters_Positive":
                                if (!context.Forms.Get(controlId).IsNullOrEmpty())
                                {
                                    var filterName = String(
                                        context: context,
                                        controlId: controlId);
                                    if (filterName.Contains(columnFilterPrefix))
                                    {
                                        filterName = filterName.Substring(columnFilterPrefix.Length);
                                        filterName = filterName.Replace("_NumericRange", string.Empty);
                                        filterName = filterName.Replace("_DateRange", string.Empty);
                                    }
                                    if (UseNegativeFilters(
                                        ss: ss,
                                        name: filterName) == true)
                                    {
                                        ColumnFilterNegatives.Remove(filterName);
                                    }
                                }
                                break;
                            case "KeepSorterState":
                                KeepSorterState = Bool(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "ViewSorters":
                                SetSorters(
                                    context: context,
                                    ss: ss,
                                    prefix: prefix);
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
                            case "CalendarStart":
                                CalendarStart = Time(
                                    context: context,
                                    controlId: controlId,
                                    useDateFormat: false);
                                break;
                            case "CalendarEnd":
                                CalendarEnd = Time(
                                    context: context,
                                    controlId: controlId,
                                    useDateFormat: false);
                                break;
                            case "CalendarViewType":
                                CalendarViewType = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CalendarGroupBy":
                                CalendarGroupBy = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "CalendarShowStatus":
                                CalendarShowStatus = Bool(
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
                                    controlId: controlId,
                                    useDateFormat: false);
                                break;
                            case "CrosstabNotShowZeroRows":
                                CrosstabNotShowZeroRows = Bool(
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
                            case "TimeSeriesChartType":
                                TimeSeriesChartType = String(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "TimeSeriesHorizontalAxis":
                                TimeSeriesHorizontalAxis = String(
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
                            case "KambanShowStatus":
                                KambanShowStatus = Bool(
                                    context: context,
                                    controlId: controlId);
                                break;
                            case "DashboardPartLayout":
                                AddDashboardPartLayoutHash(context: context);
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
                                else if (controlId.StartsWith(columnViewExtensionPrefix))
                                {
                                    AddViewExtensionsHash(
                                        context: context,
                                        ss: ss,
                                        columnName: controlId.Substring(columnViewExtensionPrefix.Length),
                                        value: context.Forms.Data(controlId));
                                }
                                break;
                        }
                    }
                    if (ss.DashboardParts?.Count.Equals(1) == true && ss.DashboardParts.First().Type.ToString() == "Calendar")
                    {
                        var dashboardPart = ss.DashboardParts.FirstOrDefault();

                        CalendarSiteId = dashboardPart.SiteId;
                        CalendarSuffix = $"_{dashboardPart.Id}";
                        CalendarTimePeriod = dashboardPart.CalendarTimePeriod;
                        CalendarFromTo = dashboardPart.CalendarFromTo;
                        CalendarType = dashboardPart.CalendarType.ToString();
                        CalendarShowStatus = dashboardPart.CalendarShowStatus;
                        CalendarGroupBy = dashboardPart.CalendarGroupBy;
                        if (context.Forms.Keys.Contains($"CalendarDate{CalendarSuffix}"))
                        {
                            AddCalendarDateHash(value: context.Forms.DateTime($"CalendarDate{CalendarSuffix}"), key: $"CalendarDate{CalendarSuffix}");
                        }
                        if (context.Forms.Keys.Contains($"CalendarStart{CalendarSuffix}"))
                        {
                            AddCalendarStartHash(value: context.Forms.DateTime($"CalendarStart{CalendarSuffix}"), key: $"CalendarStart{CalendarSuffix}");
                        }
                        if (context.Forms.Keys.Contains($"CalendarEnd{CalendarSuffix}"))
                        {
                            AddCalendarEndHash(value: context.Forms.DateTime($"CalendarEnd{CalendarSuffix}"), key: $"CalendarEnd{CalendarSuffix}");
                        }
                        if (context.Forms.Keys.Contains($"CalendarViewType{CalendarSuffix}"))
                        {
                            AddCalendarViewTypeHash(value: context.Forms.Data($"CalendarViewType{CalendarSuffix}"), key: $"CalendarViewType{CalendarSuffix}");
                        }
                    }
                    if (ss.DashboardParts?.FirstOrDefault()?.Type == DashboardPartType.Kamban)
                    {
                        var dashboardPart = ss.DashboardParts.FirstOrDefault();
                        KambanSuffix = $"_{dashboardPart.Id}";
                        KambanGroupByX = dashboardPart.KambanGroupByX;
                        KambanGroupByY = dashboardPart.KambanGroupByY;
                        KambanAggregateType = dashboardPart.KambanAggregateType;
                        KambanValue = dashboardPart.KambanValue;
                        KambanColumns = dashboardPart.KambanColumns.ToInt();
                        KambanAggregationView = dashboardPart.KambanAggregationView;
                        KambanShowStatus = dashboardPart.KambanShowStatus;
                    }
                    if (ss.DashboardParts?.FirstOrDefault()?.Type == DashboardPartType.Index)
                    {
                        var dashboardPart = ss.DashboardParts.FirstOrDefault();
                        IndexSuffix = $"_{dashboardPart.Id}";
                    }
                    break;
            }
        }

        private static string ControlIdWithOutPrefix(string controlId, string prefix)
        {
            return !prefix.IsNullOrEmpty() && controlId.StartsWith(prefix)
                ? controlId.Substring(prefix.Length)
                : controlId;
        }

        private void ResetViewFilters(SiteSettings ss)
        {
            var view = ss.Views?.FirstOrDefault(o => o.Id == Id)
                ?? new View();
            Name = view.Name;
            DefaultMode = view.DefaultMode;
            Incomplete = view.Incomplete;
            Own = view.Own;
            NearCompletionTime = view.NearCompletionTime;
            Delay = view.Delay;
            Overdue = view.Overdue;
            ColumnFilterHash = view.ColumnFilterHash;
            ColumnFilterNegatives = view.ColumnFilterNegatives;
            Search = view.Search;
            ShowHistory = view.ShowHistory;
        }

        private void ResetViewSorters(SiteSettings ss)
        {
            var view = ss.Views?.FirstOrDefault(o => o.Id == Id)
                ?? new View();
            ColumnSorterHash = view.ColumnSorterHash;
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

        private DateTime? Time(Context context, string controlId, bool useDateFormat = true)
        {
            // クロス集計の場合のみ日付書式を指定しないように処理を分岐（クロス集計での前の月、次の月を計算する際に日付書式を指定すると正しく算出されないため）
            var data = (useDateFormat)
                ? context.Forms.DateTime(context: context, key: controlId)
                : context.Forms.DateTime(controlId);
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

        public string ViewExtension(string columnName)
        {
            return ViewExtensionsHash?.ContainsKey(columnName) == true
                ? ViewExtensionsHash[columnName]
                : null;
        }

        public SqlOrderBy.Types ColumnSorter(string columnName)
        {
            return ColumnSorterHash?.ContainsKey(columnName) == true
                ? ColumnSorterHash[columnName]
                : SqlOrderBy.Types.release;
        }

        private void AddViewExtensionsHash(
            Context context,
            SiteSettings ss,
            string columnName,
            string value)
        {
            var column = context.ExtendedFieldColumn(
                ss: ss,
                columnName: columnName,
                extendedFieldType: "ViewExtensions");
            if (ViewExtensionsHash == null)
            {
                ViewExtensionsHash = new Dictionary<string, string>();
            }
            if (column != null)
            {
                ViewExtensionsHash.AddOrUpdate(columnName, value);
            }
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

        public void AddColumnFilterHash(
            Context context,
            SiteSettings ss,
            Column column,
            object objectValue)
        {
            string value = null;
            switch (column?.TypeName)
            {
                case "bit":
                    value = objectValue.ToBool().ToOneOrZeroString();
                    break;
                case "int":
                case "bigint":
                case "nvarchar":
                    value = column.HasChoices()
                        ? $"[\"{objectValue}\"]"
                        : objectValue.ToString();
                    break;
                case "decimal":
                    var num = objectValue.ToString();
                    value = $"[\"{num},{num}\"]";
                    break;
                case "datetime":
                    var dt = objectValue.ToDateTime().ToString("yyyy/MM/dd HH:mm:ss.fff");
                    value = $"[\"{dt},{dt}\"]";
                    break;
                default:
                    break;
            }
            if (value != null)
            {
                AddColumnFilterHash(
                    context: context,
                    ss: ss,
                    columnName: column.ColumnName,
                    value: value);
            }
        }

        public void AddColumnFilterSearchTypes(string columnName, Column.SearchTypes searchType)
        {
            if (ColumnFilterSearchTypes == null)
            {
                ColumnFilterSearchTypes = new Dictionary<string, Column.SearchTypes>();
            }
            ColumnFilterSearchTypes.AddOrUpdate(columnName, searchType);
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

        private void AddCalendarDateHash(DateTime? value, string key)
        {
            if (CalendarDateHash == null)
            {
                CalendarDateHash = new Dictionary<string, DateTime?>();
            }
            CalendarDateHash.AddOrUpdate(key: key, value: value);
        }

        private void AddCalendarStartHash(DateTime? value, string key)
        {
            if (CalendarStartHash == null)
            {
                CalendarStartHash = new Dictionary<string, DateTime?>();
            }
            CalendarStartHash.AddOrUpdate(key: key, value: value);
        }

        private void AddCalendarEndHash(DateTime? value, string key)
        {
            if (CalendarEndHash == null)
            {
                CalendarEndHash = new Dictionary<string, DateTime?>();
            }
            CalendarEndHash.AddOrUpdate(key: key, value: value);
        }

        private void AddCalendarViewTypeHash(string value, string key)
        {
            if (CalendarViewTypeHash == null)
            {
                CalendarViewTypeHash = new Dictionary<string, string>();
            }
            CalendarViewTypeHash.AddOrUpdate(key: key, value: value);
        }

        private void AddDashboardPartLayoutHash(Context context)
        {
            if(DashboardPartLayoutHash == null)
            {
                DashboardPartLayoutHash = new Dictionary<string, DashboardPartLayout>();
            }
            var dashboardPartLayouts = context.Forms.Data("DashboardPartLayout")
                ?.Deserialize<List<DashboardPartLayout>>();
            foreach (var dashboardPartLayout in dashboardPartLayouts)
            {
                var value = new DashboardPartLayout();
                value.X = dashboardPartLayout.X;
                value.Y = dashboardPartLayout.Y;
                value.W = dashboardPartLayout.W;
                value.H = dashboardPartLayout.H;
                value.Id = dashboardPartLayout.Id;
                DashboardPartLayoutHash.AddOrUpdate(key: value.Id.ToString(), value: value);
            }
        }

        private void SetSorters(Context context, SiteSettings ss, string prefix = "")
        {
            ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
            context.Forms.List($"{prefix}ViewSorters").ForEach(data =>
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

        public void SetPermissions(List<Permission> permissions)
        {
            Depts?.Clear();
            Groups?.Clear();
            Users?.Clear();
            foreach (var permission in permissions)
            {
                switch (permission.Name)
                {
                    case "Dept":
                        if (Depts == null)
                        {
                            Depts = new List<int>();
                        }
                        if (!Depts.Contains(permission.Id))
                        {
                            Depts.Add(permission.Id);
                        }
                        break;
                    case "Group":
                        if (Groups == null)
                        {
                            Groups = new List<int>();
                        }
                        if (!Groups.Contains(permission.Id))
                        {
                            Groups.Add(permission.Id);
                        }
                        break;
                    case "User":
                        if (Users == null)
                        {
                            Users = new List<int>();
                        }
                        if (!Users.Contains(permission.Id))
                        {
                            Users.Add(permission.Id);
                        }
                        break;
                }
            }
        }

        public List<Permission> GetPermissions(SiteSettings ss)
        {
            var permissions = new List<Permission>();
            Depts?.ForEach(deptId => permissions.Add(new Permission(
                ss: ss,
                name: "Dept",
                id: deptId)));
            Groups?.ForEach(groupId => permissions.Add(new Permission(
                ss: ss,
                name: "Group",
                id: groupId)));
            Users?.ForEach(userId => permissions.Add(new Permission(
                ss: ss,
                name: "User",
                id: userId)));
            return permissions;
        }

        public bool Accessable(Context context)
        {
            if (context.HasPrivilege)
            {
                return true;
            }
            if (Depts?.Any() != true
                && Groups?.Any() != true
                && Users?.Any() != true)
            {
                return true;
            }
            if (Depts?.Contains(context.DeptId) == true)
            {
                return true;
            }
            if (Groups?.Any(groupId => context.Groups.Contains(groupId)) == true)
            {
                return true;
            }
            if (Users?.Contains(context.UserId) == true)
            {
                return true;
            }
            return false;
        }

        public View GetRecordingData(Context context, SiteSettings ss)
        {
            var view = new View();
            view.Id = Id;
            view.Name = Name;
            if (!DefaultMode.IsNullOrEmpty())
            {
                view.DefaultMode = DefaultMode;
            }
            if (GridColumns != null && GridColumns.Join() != ss.GridColumns?.Join())
            {
                view.GridColumns = GridColumns;
            }
            if (FilterColumns != null && FilterColumns.Join() != ss.FilterColumns?.Join())
            {
                view.FilterColumns = FilterColumns;
            }
            if (FiltersDisplayType != DisplayTypes.Displayed)
            {
                view.FiltersDisplayType = FiltersDisplayType;
            }
            if (GuidesReduced != null)
            {
                view.GuidesReduced = GuidesReduced;
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
            if (KeepFilterState == true)
            {
                view.KeepFilterState = KeepFilterState;
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
            if (KeepSorterState == true)
            {
                view.KeepSorterState = KeepSorterState;
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
                        switch (o.Key)
                        {
                            case "Groups":
                                return true;
                        }
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
            if (ColumnFilterExpressions?.Any() == true)
            {
                view.ColumnFilterExpressions = new Dictionary<string, string>();
                ColumnFilterExpressions.ForEach(o => view.ColumnFilterExpressions.Add(o.Key, o.Value));
            }
            if (ViewExtensionsHash?.Any() == true)
            {
                view.ViewExtensionsHash = new Dictionary<string, string>();
                ViewExtensionsHash
                    .Where(o =>
                    {
                        var column = context.ExtendedFieldColumn(
                            ss: ss,
                            columnName: o.Key,
                            extendedFieldType: "ViewExtensions");
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
                    .ForEach(o => view.ViewExtensionsHash.Add(o.Key, o.Value));
            }
            if (ColumnFilterSearchTypes?.Any() == true)
            {
                view.ColumnFilterSearchTypes = new Dictionary<string, Column.SearchTypes>();
                ColumnFilterSearchTypes.ForEach(o => view.ColumnFilterSearchTypes.Add(o.Key, o.Value));
            }
            if (ColumnFilterNegatives?.Any() == true)
            {
                view.ColumnFilterNegatives = new List<string>();
                ColumnFilterNegatives.ForEach(o => view.ColumnFilterNegatives.Add(o));
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
            if (CalendarStart?.InRange() == true)
            {
                view.CalendarStart = CalendarStart;
            }
            if (CalendarEnd?.InRange() == true)
            {
                view.CalendarEnd = CalendarEnd;
            }
            if (!CalendarViewType.IsNullOrEmpty())
            {
                view.CalendarViewType = CalendarViewType;
            }
            if (!CalendarGroupBy.IsNullOrEmpty())
            {
                view.CalendarGroupBy = CalendarGroupBy;
            }
            if (CalendarShowStatus == true)
            {
                view.CalendarShowStatus = CalendarShowStatus;
            }
            if (CalendarDateHash?.Any() == true)
            {
                view.CalendarDateHash = CalendarDateHash;
            }
            if (CalendarStartHash?.Any() == true)
            {
                view.CalendarStartHash = CalendarStartHash;
            }
            if (CalendarEndHash?.Any() == true)
            {
                view.CalendarEndHash = CalendarEndHash;
            }
            if (CalendarViewTypeHash?.Any() == true)
            {
                view.CalendarViewTypeHash = CalendarViewTypeHash;
            }
            if(DashboardPartLayoutHash?.Any() == true)
            {
                view.DashboardPartLayoutHash = DashboardPartLayoutHash;
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
            if (CrosstabNotShowZeroRows == true)
            {
                view.CrosstabNotShowZeroRows = CrosstabNotShowZeroRows;
            }
            if (!GanttGroupBy.IsNullOrEmpty())
            {
                view.GanttGroupBy = GanttGroupBy;
            }
            if (!GanttSortBy.IsNullOrEmpty())
            {
                view.GanttSortBy = GanttSortBy;
            }
            view.GanttPeriod = GanttPeriod;
            view.GanttStartDate = GanttStartDate;
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
            if (!TimeSeriesChartType.IsNullOrEmpty())
            {
                view.TimeSeriesChartType = TimeSeriesChartType;
            }
            if (!TimeSeriesHorizontalAxis.IsNullOrEmpty())
            {
                view.TimeSeriesHorizontalAxis = TimeSeriesHorizontalAxis;
            }
            if (AnalyPartSettings?.Any() == true)
            {
                view.AnalyPartSettings = AnalyPartSettings;
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
            if (KambanShowStatus == true)
            {
                view.KambanShowStatus = KambanShowStatus;
            }
            if (Depts?.Any() == true)
            {
                view.Depts = Depts;
            }
            if (Groups?.Any() == true)
            {
                view.Groups = Groups;
            }
            if (Users?.Any() == true)
            {
                view.Users = Users;
            }
            if (ShowHistory == true)
            {
                view.ShowHistory = true;
            }
            if (MergeSessionViewFilters == true)
            {
                view.MergeSessionViewFilters = true;
            }
            if (MergeSessionViewSorters == true)
            {
                view.MergeSessionViewSorters = true;
            }
            return view;
        }

        public SqlWhereCollection Where(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where = null,
            bool checkPermission = true,
            bool itemJoin = true,
            bool requestSearchCondition = true)
        {
            if (where == null) where = new SqlWhereCollection();
            var process = ss.GetProcess(
                context: context,
                id: context.Forms.Int("BulkProcessingItems"));
            SetBulkProcessingFilter(
                context: context,
                ss: ss,
                process: process,
                where: where);
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
            Permissions.SetPermissionsWhere(
                context: context,
                ss: ss,
                where: where,
                /// 一括処理を行う場合には読み取り権限だけでなく書き込み権限をチェック
                permissionType: process == null
                    ? Permissions.Types.Read
                    : Permissions.Types.Read | Permissions.Types.Update,
                checkPermission: checkPermission);
            if (requestSearchCondition
                && RequestSearchCondition(
                    context: context,
                    ss: ss))
            {
                where.Add(raw: "(0=1)");
            }
            return where;
        }

        private void SetBulkProcessingFilter(
            Context context,
            SiteSettings ss,
            Process process,
            SqlWhereCollection where)
        {
            if (process != null)
            {
                process.ValidateInputs?
                    .Where(validateInput => validateInput.Required == true)
                    .ForEach(validateInput =>
                    {
                        var column = ss.GetColumn(
                            context: context,
                            columnName: validateInput.ColumnName);
                        if (column != null)
                        {
                            switch (column.TypeName.CsTypeSummary())
                            {
                                case Types.CsBool:
                                    where.Bool(column, true);
                                    break;
                                case Types.CsNumeric:
                                    if (column.Nullable == true)
                                    {
                                        where.AddRange(CsNumericColumnsWhereNull(
                                            column: column,
                                            param: "\t".ToSingleList(),
                                            negative: true));
                                    }
                                    break;
                                case Types.CsDateTime:
                                    where.Add(CsDateTimeColumnsWhereNull(
                                        column: column,
                                        param: "\t".ToSingleList(),
                                        negative: true));
                                    break;
                                case Types.CsString:
                                    where.Add(CsStringColumnsWhereNull(
                                        context: context,
                                        column: column,
                                        negative: true));
                                    break;
                            }
                        }
                    });
            }
        }

        private void SetGeneralsWhere(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where)
        {
            if (Incomplete == true && HasIncompleteColumns(
                context: context,
                ss: ss))
            {
                where.Add(
                    tableName: ss.ReferenceType,
                    columnBrackets: "\"Status\"".ToSingleArray(),
                    _operator: ((UseNegativeFilters(
                        ss: ss,
                        name: "ViewFilters_Incomplete") == true)
                            ? ">="
                            : "<")
                                + Parameters.General.CompletionCode);
            }
            if (Own == true && HasOwnColumns(context, ss))
            {
                where.Add(
                    tableName: ss.ReferenceType,
                    columnBrackets: new string[] { "\"Manager\"", "\"Owner\"" },
                    name: "_U",
                    value: context.UserId,
                    _operator: (UseNegativeFilters(
                        ss: ss,
                        name: "ViewFilters_Own") == true)
                            ? "!="
                            : "=",
                    multiColumnOperator: (UseNegativeFilters(
                        ss: ss,
                        name: "ViewFilters_Own") == true)
                            ? " and "
                            : " or ");
            }
            if (NearCompletionTime == true && HasNearCompletionTimeColumns(
                context: context,
                ss: ss))
            {
                where.Add(
                    tableName: ss.ReferenceType,
                    columnBrackets: "\"CompletionTime\"".ToSingleArray(),
                    _operator: ((UseNegativeFilters(
                        ss: ss,
                        name: "ViewFilters_NearCompletionTime") == true)
                            ? " not between"
                            : " between")
                                + " '{0}' and '{1}'".Params(
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
                if (UseNegativeFilters(
                    ss: ss,
                    name: "ViewFilters_Delay") == true)
                {
                    where.Or(or: new SqlWhereCollection()
                        .Add(
                            tableName: ss.ReferenceType,
                            columnBrackets: "\"Status\"".ToSingleArray(),
                            name: "_U",
                            _operator: ">={0}".Params(Parameters.General.CompletionCode))
                        .Add(
                            tableName: ss.ReferenceType,
                            columnBrackets: "\"ProgressRate\"".ToSingleArray(),
                            _operator: ">=",
                            raw: Def.Sql.ProgressRateDelay
                                .Replace("#TableName#", ss.ReferenceType)));
                }
                else
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
            }
            if (Overdue == true && HasOverdueColumns(
                context: context,
                ss: ss))
            {
                if (UseNegativeFilters(
                    ss: ss,
                    name: "ViewFilters_Overdue") == true)
                {
                    where.Or(or: new SqlWhereCollection()
                        .Add(
                            tableName: ss.ReferenceType,
                            columnBrackets: "\"Status\"".ToSingleArray(),
                            name: "_U",
                            _operator: ">={0}".Params(Parameters.General.CompletionCode))
                        .Add(
                            tableName: ss.ReferenceType,
                            columnBrackets: "\"CompletionTime\"".ToSingleArray(),
                            _operator: ">=" + context.Sqls.CurrentDateTime));
                }
                else
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
                            _operator: "<" + context.Sqls.CurrentDateTime);
                }
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
            DateTime? timestamp,
            string filterColumnName = null)
        {
            columnFilterHash?
                .Select(data => new
                {
                    Column = ss.GetColumn(
                        context: context,
                        columnName: data.Key),
                    ColumnName = data.Key,
                    FilterColumnName = filterColumnName.IsNullOrEmpty()
                        ? data.Key
                        : filterColumnName + "\\" + data.Key,
                    data.Value,
                    Or = data.Key.StartsWith("or_"),
                    And = data.Key.StartsWith("and_"),
                    Eq = data.Key.StartsWith("eq_"),
                    NotEq = data.Key.StartsWith("notEq_"),
                    Groups = data.Key == "Groups",
                    GroupMembers = data.Key == "GroupMembers",
                    OnSelectingWhere = data.Key == "OnSelectingWhere",
                })
                .Where(o => o.Column != null
                    || o.Or
                    || o.And
                    || o.Eq
                    || o.NotEq
                    || o.Groups
                    || o.GroupMembers
                    || o.OnSelectingWhere)
                .ForEach(data =>
                {
                    var negative = UseNegativeFilters(
                        ss: ss,
                        name: data.FilterColumnName) == true;
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
                                timestamp: timestamp,
                                filterColumnName: data.FilterColumnName);
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
                                timestamp: timestamp,
                                filterColumnName: data.FilterColumnName);
                            if (and.Any()) where.Add(and: and);
                        }
                    }
                    else if (data.Eq || data.NotEq)
                    {
                        var column1 = ss.GetColumn(
                            context: context,
                            columnName: data.Value.Split_1st('|'));
                        var column2 = ss.GetColumn(
                            context: context,
                            columnName: data.Value.Split_2nd('|'));
                        var csType1 = column1?.TypeName.CsTypeSummary();
                        var csType2 = column2?.TypeName.CsTypeSummary();
                        if (column1 != null && column2 != null && csType1 == csType2)
                        {
                            AddEqWhere(
                                context: context,
                                where: where,
                                column1: column1,
                                column2: column2,
                                csType: csType1,
                                notEq: data.NotEq);
                        }
                    }
                    else if (data.Groups)
                    {
                        var ids = data.Value.Deserialize<List<int>>();
                        switch (ss.ReferenceType)
                        {
                            case "Depts":
                                where.Add(sub: Rds.ExistsGroupMembers(
                                    join: Rds.GroupMembersJoinDefault()
                                        .Add(new SqlJoin(
                                            tableBracket: "\"Groups\"",
                                            joinType: SqlJoin.JoinTypes.Inner,
                                            joinExpression: "\"GroupMembers\".\"GroupId\"=\"Groups\".\"GroupId\""))
                                        .Add(new SqlJoin(
                                            tableBracket: "\"Depts\"",
                                            joinType: SqlJoin.JoinTypes.Inner,
                                            joinExpression: "\"GroupMembers\".\"DeptId\"=\"Depts\".\"DeptId\"",
                                            _as: "GroupMemberDepts")),
                                    where: Rds.GroupMembersWhere()
                                        .GroupId_In(ids)
                                        .DeptId(0, _operator: "<>")
                                        .DeptId(raw: "\"Depts\".\"DeptId\"")
                                        .Groups_Disabled(false)
                                        .Depts_Disabled(false, tableName: "GroupMemberDepts")));
                                break;
                            case "Users":
                                where.Add(sub: Rds.Exists(statements: new SqlStatement[]
                                {
                                    Rds.SelectGroupMembers(
                                        column: Rds.GroupMembersColumn().GroupId(),
                                        join: Rds.GroupMembersJoinDefault()
                                            .Add(new SqlJoin(
                                                tableBracket: "\"Groups\"",
                                                joinType: SqlJoin.JoinTypes.Inner,
                                                joinExpression: "\"GroupMembers\".\"GroupId\"=\"Groups\".\"GroupId\"")),
                                        where: Rds.GroupMembersWhere()
                                            .GroupId_In(ids)
                                            .UserId(0, _operator: "<>")
                                            .UserId(raw: "\"Users\".\"UserId\"")
                                            .Groups_Disabled(false)),
                                    Rds.SelectGroupMembers(
                                        column: Rds.GroupMembersColumn().GroupId(),
                                        join: Rds.GroupMembersJoinDefault()
                                            .Add(new SqlJoin(
                                                tableBracket: "\"Groups\"",
                                                joinType: SqlJoin.JoinTypes.Inner,
                                                joinExpression: "\"GroupMembers\".\"GroupId\"=\"Groups\".\"GroupId\""))
                                            .Add(new SqlJoin(
                                                tableBracket: "\"Depts\"",
                                                joinType: SqlJoin.JoinTypes.Inner,
                                                joinExpression: "\"GroupMembers\".\"DeptId\"=\"Depts\".\"DeptId\"")),
                                        where: Rds.GroupMembersWhere()
                                            .GroupId_In(ids)
                                            .DeptId(0, _operator: "<>")
                                            .DeptId(raw: "\"Users\".\"DeptId\"")
                                            .Groups_Disabled(false)
                                            .Depts_Disabled(false),
                                        unionType: Sqls.UnionTypes.UnionAll)
                                }));
                                break;
                        }
                    }
                    else if (data.GroupMembers)
                    {
                        var ids = data.Value.Deserialize<List<int>>();
                        if (ids?.Any(id => id > 0) == true)
                        {
                            where.Add(sub: Rds.Exists(statements: new SqlStatement[]
                            {
                                Rds.SelectGroupMembers(
                                    column: Rds.GroupMembersColumn().GroupId(),
                                    where: Rds.GroupMembersWhere()
                                        .UserId_In(ids)
                                        .GroupMembers_GroupId(raw: "\"Groups\".\"GroupId\"")
                                        .Groups_Disabled(false)),
                                Rds.SelectGroupMembers(
                                    column: Rds.GroupMembersColumn().GroupId(),
                                    join: Rds.GroupMembersJoinDefault()
                                        .Add(new SqlJoin(
                                            tableBracket: "\"Depts\"",
                                            joinType: SqlJoin.JoinTypes.Inner,
                                            joinExpression: "\"GroupMembers\".\"DeptId\"=\"Depts\".\"DeptId\""))
                                        .Add(new SqlJoin(
                                            tableBracket: "\"Users\"",
                                            joinType: SqlJoin.JoinTypes.Inner,
                                            joinExpression: "\"Depts\".\"DeptId\"=\"Users\".\"DeptId\"")),
                                    where: Rds.UsersWhere()
                                        .UserId_In(ids)
                                        .GroupMembers_GroupId(raw: "\"Groups\".\"GroupId\"")
                                        .Groups_Disabled(false)
                                        .Depts_Disabled(false),
                                    unionType: Sqls.UnionTypes.UnionAll)
                            }));
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
                            where: where,
                            negative: negative);
                    }
                    else
                    {
                        var value = ConvertedValue(
                            context: context,
                            column: data.Column,
                            value: data.Value);
                        switch (data.Column.TypeName.CsTypeSummary())
                        {
                            case Types.CsBool:
                                CsBoolColumns(
                                    column: data.Column,
                                    value: value,
                                    where: where,
                                    negative: negative);
                                break;
                            case Types.CsNumeric:
                                CsNumericColumns(
                                    column: data.Column,
                                    value: value,
                                    where: where,
                                    negative: negative);
                                break;
                            case Types.CsDateTime:
                                CsDateTimeColumns(
                                    context: context,
                                    column: data.Column,
                                    value: value,
                                    where: where,
                                    negative: negative);
                                break;
                            case Types.CsString:
                                CsStringColumns(
                                    context: context,
                                    column: data.Column,
                                    value: value,
                                    where: where,
                                    negative: negative);
                                break;
                        }
                    }
                });
        }

        private string ConvertedValue(
            Context context,
            Column column,
            string value)
        {
            switch (column.Type)
            {
                case Column.Types.Dept:
                    return ConvertedOwn(
                        value: value,
                        id: context.DeptId);
                case Column.Types.User:
                    return ConvertedOwn(
                        value: value,
                        id: context.UserId);
                default:
                    switch (column.Id)
                    {
                        case "Depts_DeptId":
                        case "Users_DeptId":
                            return ConvertedOwn(
                                value: value,
                                id: context.DeptId);
                        case "Users_UserId":
                            return ConvertedOwn(
                                value: value,
                                id: context.UserId);
                    }
                    return value;
            }
        }

        private static string ConvertedOwn(string value, int id)
        {
            return value
                ?.Deserialize<List<string>>()
                ?.Select(o => (o == "Own"
                    ? id.ToString()
                    : o))
                .ToJson()
                    // JSON形式ではない場合の対処
                    ?? (value == "Own"
                        ? id.ToString()
                        : value);
        }

        private static void AddEqWhere(
            Context context,
            SqlWhereCollection where,
            Column column1,
            Column column2,
            string csType,
            bool notEq)
        {
            var columnName1 = $"\"{column1.TableName()}\".\"{column1.Name}\"";
            var columnName2 = $"\"{column2.TableName()}\".\"{column2.Name}\"";
            if (csType == Types.CsDateTime
                || (csType == Types.CsNumeric && column1.Nullable == true))
            {
                //x = null、x <> null の判定ができない（どちらもHITしない）ため X is nullで判定する
                //not eq では、「一方がNULLで一方がNULLでない場合」または「(どちらもNULLでなく)値が異なる場合」で判定
                var raw = notEq
                    ? $"(({columnName1} is not null and {columnName2} is null)"
                        + $" or ({columnName1} is null and {columnName2} is not null)"
                        + $" or {columnName1} <> {columnName2})"
                    : $"(({columnName1} is null and {columnName2} is null)"
                        + $" or {columnName1} = {columnName2})";
                where.Add(
                    joinTableNames: new[] { column1.TableName(), column2.TableName() },
                    raw: raw);
            }
            else
            {
                var defaultValue = string.Empty;
                switch (csType)
                {
                    case Types.CsBool:
                        defaultValue = context.Sqls.FalseString;
                        break;
                    case Types.CsNumeric:
                        defaultValue = "0";
                        break;
                    case Types.CsDateTime:
                        break;
                    case Types.CsString:
                        defaultValue = "''";
                        break;
                }
                where.Add(
                    joinTableNames: new[] { column1.TableName(), column2.TableName() },
                    raw: $"{context.Sqls.IsNull}({columnName1}, {defaultValue})"
                        + $" {(notEq ? "<>" : "=")}"
                        + $" {context.Sqls.IsNull}({columnName2}, {defaultValue})");
            }
        }

        private void CsBoolColumns(
            Column column,
            string value,
            SqlWhereCollection where,
            bool negative)
        {
            switch (value?.ToLower())
            {
                case "1":
                case "true":
                    if (negative)
                    {
                        where.Add(or: new SqlWhereCollection()
                            .Bool(column, null)
                            .Bool(column, false));
                    }
                    else
                    {
                        where.Bool(column, true);
                    }
                    break;
                case "2":
                case "false":
                    if (negative)
                    {
                        where.Bool(column, true);
                    }
                    else
                    {
                        where.Add(or: new SqlWhereCollection()
                            .Bool(column, null)
                            .Bool(column, false));
                    }
                    break;
            }
        }

        private void CsNumericColumns(
            Column column,
            string value,
            SqlWhereCollection where,
            bool negative)
        {
            long number = 0;
            var collection = new SqlWhereCollection();
            if (long.TryParse(value, out number))
            {
                collection.Add(CsNumericColumnsWhere(
                    column: column,
                    param: number,
                    negative: negative));
            }
            else
            {
                var param = value.Deserialize<List<string>>();
                if (param?.Any() != true) return;
                collection.AddRange(param
                    .Where(o => o.RegexExists(@"^-?[0-9\.]*,-?[0-9\.]*$"))
                    .SelectMany(o => CsNumericRangeColumns(
                        column: column,
                        param: o,
                        negative: negative)));
                collection.AddRange(CsNumericColumnsWhereNull(
                    column: column,
                    param: param,
                    negative: negative));
                var valueWhere = CsNumericColumnsWhere(
                    column: column,
                    param: param
                        .Where(o => !o.RegexExists(@"^-?[0-9\.]*,-?[0-9\.]*$"))
                        .Where(o => o != "\t"),
                    negative: negative);
                if (valueWhere != null)
                {
                    collection.Add(valueWhere);
                }
            }
            if (collection.Any())
            {
                if (negative)
                {
                    var param = value.Deserialize<List<string>>();
                    where.Or(or: new SqlWhereCollection()
                        .Add(and: collection)
                        .Add(
                            tableName: column.TableName(),
                            columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                            _operator: " is null",
                            // NULL許容で未設定が指定されていない場合にはNULLを検索条件に含める
                            _using: (column.Nullable == true && !param.Any(o => o == "\t"))
                                // NULL非許容でゼロを含む範囲が指定されていない場合にはNULLを検索条件に含める
                                || (column.Nullable != true && !param.Any(o => ContainsZero(
                                    from: o.Split_1st(),
                                    to: o.Split_2nd())))));
                }
                else
                {
                    where.Add(or: collection);
                }
            }
        }

        private SqlWhere CsNumericColumnsWhere(
            Column column,
            long param,
            bool negative)
        {
            return new SqlWhere(
                tableName: column.TableName(),
                columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                name: column.Name,
                _operator: negative
                    ? $"!={param}"
                    : $"={param}");
        }

        private SqlWhere CsNumericColumnsWhere(
            Column column,
            IEnumerable<string> param,
            bool negative)
        {
            var numList = param.Select(o => o.ToDecimal());
            if (!numList.Any()) { return null; }
            return new SqlWhere(
                tableName: column.TableName(),
                columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                name: column.Name,
                _operator: (negative
                    ? " not in ({0})"
                    : " in ({0})")
                        .Params(numList.Join()));
        }

        private SqlWhereCollection CsNumericColumnsWhereNull(
            Column column,
            List<string> param,
            bool negative = false)
        {
            var where = new SqlWhereCollection();
            if (param.Any(o => o == "\t"))
            {
                where.Add(new SqlWhere(
                    tableName: column.TableName(),
                    columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                    _operator: negative
                        ? " is not null"
                        : " is null"));
                if (column.Nullable != true)
                {
                    where.Add(new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        _operator: negative
                            ? "!=0"
                            : "=0"));
                    if (column.Type == Column.Types.User && SiteInfo.AnonymousId != 0)
                    {
                        where.Add(new SqlWhere(
                            tableName: column.TableName(),
                            columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                            _operator: (negative
                                ? "!="
                                : "=")
                                    + SiteInfo.AnonymousId));
                    }
                }
            }
            return where;
        }

        private IEnumerable<SqlWhere> CsNumericRangeColumns(
            Column column,
            string param,
            bool negative)
        {
            var from = param.Split_1st();
            var to = param.Split_2nd();
            yield return new SqlWhere(
                tableName: column.TableName(),
                columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                _operator: negative
                    ? from == string.Empty
                        ? $">{to.ToDecimal()}"
                        : to == string.Empty
                            ? $"<{from.ToDecimal()}"
                            : " not between {0} and {1}".Params(from.ToDecimal(), to.ToDecimal())
                    : from == string.Empty
                        ? $"<={to.ToDecimal()}"
                        : to == string.Empty
                            ? $">={from.ToDecimal()}"
                            : " between {0} and {1}".Params(from.ToDecimal(), to.ToDecimal()));
            // NULL非許容の項目で0を範囲に含み否定条件ではない場合 is null を検索条件に含める
            if (column.Nullable != true && !negative)
            {
                if (ContainsZero(
                    from: from,
                    to: to))
                {
                    yield return new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        _operator: " is null");
                }
            }
        }

        private bool ContainsZero(string from, string to)
        {
            return (to == string.Empty && from.ToDecimal() <= 0)
                || (from == string.Empty && to.ToDecimal() >= 0)
                || (from != string.Empty && to != string.Empty && from.ToDecimal() <= 0 && to.ToDecimal() >= 0);
        }

        private void CsDateTimeColumns(
            Context context,
            Column column,
            string value,
            SqlWhereCollection where,
            bool negative)
        {
            var param = value.Deserialize<List<string>>();
            if (param?.Any(o => !o.IsNullOrEmpty()) == true)
            {
                if (negative)
                {
                    where.Add(and: new SqlWhereCollection(
                        CsDateTimeColumnsWhere(
                            context: context,
                            column: column,
                            param: param,
                            negative: negative),
                        CsDateTimeColumnsWhereNull(
                            column: column,
                            param: param,
                            negative: negative)));
                }
                else
                {
                    where.Add(or: new SqlWhereCollection(
                        CsDateTimeColumnsWhere(
                            context: context,
                            column: column,
                            param: param,
                            negative: negative),
                        CsDateTimeColumnsWhereNull(
                            column: column,
                            param: param,
                            negative: negative)));
                }
            }
        }

        private SqlWhere CsDateTimeColumnsWhere(
            Context context,
            Column column,
            List<string> param,
            bool negative)
        {
            var today = DateTime.Now.ToDateTime().ToLocal(context: context).Date;
            var addMilliseconds = Parameters.Rds.MinimumTime * -1;
            var between = "#TableBracket#.\"{0}\" between '{1}' and '{2}'";
            var notBetween = "(#TableBracket#.\"{0}\" not between '{1}' and '{2}' or #TableBracket#.\"{0}\" is null)";
            var ymdhms = "yyyy/M/d H:m:s";
            var ymdhmsfff = "yyyy/M/d H:m:s.fff";
            return param.Any(o => o != "\t")
                ? new SqlWhere(
                    tableName: column.TableName(),
                    raw: param
                        .Where(o => o != "\t")
                        .Select(range =>
                        {
                            var from = range.Split_1st();
                            var to = range.Split_2nd();
                            switch (from)
                            {
                                case "Today":
                                    return (negative
                                        ? notBetween
                                        : between)
                                            .Params(
                                                column.Name,
                                                column.ConvertDateTimeParam(
                                                    context: context,
                                                    dt: today,
                                                    format: ymdhms),
                                                column.ConvertDateTimeParam(
                                                    context: context,
                                                    dt: today
                                                        .AddDays(1)
                                                        .AddMilliseconds(addMilliseconds),
                                                    format: ymdhmsfff));
                                case "ThisMonth":
                                    return (negative
                                        ? notBetween
                                        : between)
                                            .Params(
                                                column.Name,
                                                column.ConvertDateTimeParam(
                                                    context: context,
                                                    dt: new DateTime(today.Year, today.Month, 1),
                                                    format: ymdhms),
                                                column.ConvertDateTimeParam(
                                                    context: context,
                                                    dt: new DateTime(today.Year, today.Month, 1)
                                                        .AddMonths(1)
                                                        .AddMilliseconds(addMilliseconds),
                                                    format: ymdhmsfff));
                                case "ThisYear":
                                    return (negative
                                        ? notBetween
                                        : between)
                                            .Params(
                                                column.Name,
                                                column.ConvertDateTimeParam(
                                                    context: context,
                                                    dt: new DateTime(today.Year, 1, 1),
                                                    format: ymdhms),
                                                column.ConvertDateTimeParam(
                                                    context: context,
                                                    dt: new DateTime(today.Year, 1, 1)
                                                        .AddYears(1)
                                                        .AddMilliseconds(addMilliseconds),
                                                    format: ymdhmsfff));
                            }
                            if (!from.IsNullOrEmpty() && !to.IsNullOrEmpty())
                            {
                                return (negative
                                    ? notBetween
                                    : between)
                                        .Params(
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
                                return (negative
                                    ? "(#TableBracket#.\"{0}\"<'{1}' or #TableBracket#.\"{0}\" is null)"
                                    : "#TableBracket#.\"{0}\">='{1}'")
                                        .Params(
                                            column.Name,
                                            ConvertDateTimeParam(
                                                context: context,
                                                column: column,
                                                dateTimeString: from,
                                                format: ymdhms));
                            }
                            else
                            {
                                return (negative
                                    ? "(#TableBracket#.\"{0}\">'{1}' or #TableBracket#.\"{0}\" is null)"
                                    : "#TableBracket#.\"{0}\"<='{1}'")
                                        .Params(
                                            column.Name,
                                            ConvertDateTimeParam(
                                                context: context,
                                                column: column,
                                                dateTimeString: to,
                                                format: ymdhmsfff));
                            }
                        }).Join(negative
                            ? " and "
                            : " or "))
                : null;
        }

        private string ConvertDateTimeParam(
            Context context,
            Column column,
            string dateTimeString,
            string format)
        {
            return column.ConvertDateTimeParam(
                context: context,
                dt: dateTimeString.ToDateTime(),
                format: format);
        }

        private SqlWhere CsDateTimeColumnsWhereNull(
            Column column,
            List<string> param,
            bool negative)
        {
            return param.Any(o => o == "\t")
                ? new SqlWhere(
                    tableName: column.TableName(),
                    columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                    _operator: negative
                        ? " is not null"
                        : " is null")
                : null;
        }

        private void CsStringColumns(
            Context context,
            Column column,
            string value,
            SqlWhereCollection where,
            bool negative)
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
            if (column.HasChoices())
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
                                    negative: negative);
                            }
                            break;
                        case Column.SearchTypes.ForwardMatch:
                        case Column.SearchTypes.ForwardMatchMultiple:
                            if (param?.Count() == 1 && param.FirstOrDefault() == "\t")
                            {
                                where.Add(CsStringColumnsWhereNull(
                                    context: context,
                                    column: column));
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
                                    query: "(\"{0}\".\"{1}\"" + (negative
                                        ? context.Sqls.NotLike
                                        : context.Sqls.Like)
                                            + "@{3}{4}" + context.Sqls.Escape
                                            +" or " + context.Sqls.IsNull + "(\"{0}\".\"{1}\", \'\') = \'\'" + ")");
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
                                        .Select(o => o.StringInJson()),
                                    nullable: param.Any(o => o == "\t"),
                                    negative: negative,
                                    format: "%{0}%",
                                    like: true);
                            }
                            break;
                    }
                }
                else if (param?.Any() == true)
                {
                    switch (searchType)
                    {
                        case Column.SearchTypes.ExactMatch:
                        case Column.SearchTypes.ExactMatchMultiple:
                            CreateCsStringSqlWhereCollection(
                                context: context,
                                column: column,
                                where: where,
                                param: param.Where(o => o != "\t"),
                                nullable: param.Any(o => o == "\t"),
                                negative: negative);
                            break;
                        case Column.SearchTypes.ForwardMatch:
                        case Column.SearchTypes.ForwardMatchMultiple:
                            CreateCsStringSqlWhereCollection(
                                context: context,
                                column: column,
                                where: where,
                                param: param.Where(o => o != "\t"),
                                nullable: param.Any(o => o == "\t"),
                                negative: negative,
                                format: "{0}%",
                                like: true);
                            break;
                        default:
                            CreateCsStringSqlWhereCollection(
                                context: context,
                                column: column,
                                where: where,
                                param: param.Where(o => o != "\t"),
                                nullable: param.Any(o => o == "\t"),
                                negative: negative,
                                format: "%{0}%",
                                like: true);
                            break;
                    }
                }
            }
            else
            {
                if (value == " " || value == "　")
                {
                    where.Add(CsStringColumnsWhereNull(
                        context: context,
                        column: column,
                        negative: negative));
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
                                negative: negative);
                            break;
                        case Column.SearchTypes.ForwardMatch:
                            CreateCsStringSqlWhereLike(
                                context: context,
                                column: column,
                                value: context.Sqls.EscapeValue(value),
                                where: where,
                                query: "(\"{0}\".\"{1}\"" + (negative
                                    ? context.Sqls.NotLike
                                    : context.Sqls.Like)
                                        + "@{3}{4}" + context.Sqls.Escape
                                        + (negative
                                            ? " or " + context.Sqls.IsNull + "(\"{0}\".\"{1}\", \'\') = \'\'"
                                            : string.Empty)
                                                + ")");
                            break;
                        case Column.SearchTypes.PartialMatch:
                            CreateCsStringSqlWhereLike(
                                context: context,
                                column: column,
                                value: context.Sqls.EscapeValue(value),
                                where: where,
                                query: "(\"{0}\".\"{1}\"" + (negative
                                    ? context.Sqls.NotLike
                                    : context.Sqls.Like)
                                        + "{2}@{3}{4}" + context.Sqls.Escape
                                        + (negative
                                            ? " or " + context.Sqls.IsNull + "(\"{0}\".\"{1}\", \'\') = \'\'"
                                            : string.Empty)
                                                + ")");
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
                                    negative: negative);
                            }
                            break;
                        case Column.SearchTypes.ForwardMatchMultiple:
                            if (param?.Any() == true)
                            {
                                CreateCsStringSqlWhereCollection(
                                    context: context,
                                    column: column,
                                    where: where,
                                    param: param.Where(o => o != "\t"),
                                    nullable: param.Any(o => o == "\t"),
                                    negative: negative,
                                    format: "{0}%",
                                    like: true);
                            }
                            break;
                        case Column.SearchTypes.PartialMatchMultiple:
                            if (param?.Any() == true)
                            {
                                CreateCsStringSqlWhereCollection(
                                    context: context,
                                    column: column,
                                    where: where,
                                    param: param.Where(o => o != "\t"),
                                    nullable: param.Any(o => o == "\t"),
                                    negative: negative,
                                    format: "%{0}%",
                                    like: true);
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
            bool negative,
            string format = null,
            bool like = false)
        {
            var collection = new SqlWhereCollection();
            if (param.Any())
            {
                collection.Add(CsStringColumnsWhere(
                    context: context,
                    column: column,
                    param: param,
                    format: format,
                    like: like,
                    negative: negative));
            }
            if (nullable)
            {
                collection.Add(CsStringColumnsWhereNull(
                    context: context,
                    column: column,
                    negative: negative));
            }
            collection.RemoveAll(o => o == null);
            if (collection.Any())
            {
                if (negative)
                {
                    where.AddRange(collection);
                }
                else
                {
                    where.Add(or: collection);
                }
            }
        }

        private SqlWhere CsStringColumnsWhere(
            Context context,
            Column column,
            IEnumerable<string> param,
            string format,
            bool like,
            bool negative = false)
        {
            if (negative)
            {
                return new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        tableName: column.TableItemTitleCases(context: context),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        name: Strings.NewGuid(),
                        value: param
                            ?.Select(o => like
                                ? context.Sqls.EscapeValue(o)
                                : o)
                            ?.Select(o => !format.IsNullOrEmpty()
                                ? format.Params(o)
                                : o)
                            .ToList(),
                        _operator: like
                            ? context.Sqls.NotLikeWithEscape
                            : "!=",
                        multiParamOperator: " and "),
                    new SqlWhere(
                        tableName: column.TableItemTitleCases(context: context),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        _operator: " is null")));
            }
            else
            {
                return new SqlWhere(
                    tableName: column.TableItemTitleCases(context: context),
                    columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                    name: Strings.NewGuid(),
                    value: param
                        ?.Select(o => like
                            ? context.Sqls.EscapeValue(o)
                            : o)
                        ?.Select(o => !format.IsNullOrEmpty()
                            ? format.Params(o)
                            : o)
                        .ToList(),
                    _operator: like
                        ? context.Sqls.LikeWithEscape
                        : "=",
                    multiParamOperator: " or ");
            }
        }

        private SqlWhere CsStringColumnsWhereNull(
            Context context,
            Column column,
            bool negative = false)
        {
            if (negative)
            {
                return new SqlWhere(and: new SqlWhereCollection(
                    new SqlWhere(
                        tableName: column.TableItemTitleCases(context: context),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        _operator: " is not null"),
                    new SqlWhere(
                        tableName: column.TableItemTitleCases(context: context),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        _operator: "!=''"),
                    new SqlWhere(
                        tableName: column.TableItemTitleCases(context: context),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        _operator: "!='[]'",
                        _using: column.MultipleSelections == true)));
            }
            else
            {
                return new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        tableName: column.TableItemTitleCases(context: context),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        _operator: " is null"),
                    new SqlWhere(
                        tableName: column.TableItemTitleCases(context: context),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        _operator: "=''"),
                    new SqlWhere(
                        tableName: column.TableItemTitleCases(context: context),
                        columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                        _operator: "='[]'",
                        _using: column.MultipleSelections == true)));
            }
        }

        private void CreateCsStringSqlWhereLike(
            Context context,
            Column column,
            string value,
            SqlWhereCollection where,
            string query)
        {
            var tableName = column.TableItemTitleCases(context: context);
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
                    gridData: null,
                    itemModel: null,
                    view: this,
                    where: script => script.WhenViewProcessing == true,
                    condition: ServerScriptModel.ServerScriptConditions.WhenViewProcessing);
                WhenViewProcessingServerScriptExecuted = true;
            }
        }

        public SqlOrderByCollection OrderBy(
            Context context,
            SiteSettings ss,
            SqlOrderByCollection orderBy = null)
        {
            orderBy = orderBy ?? new SqlOrderByCollection();
            orderBy.OnSelectingOrderByExtendedSqls(
                context: context,
                ss: ss,
                extendedSqls: Parameters.ExtendedSqls?.Where(o => o.OnSelectingOrderBy),
                name: OnSelectingOrderBy,
                columnSorterHash: ColumnSorterHash,
                columnPlaceholders: ColumnPlaceholders);
            if (ShowHistory == true)
            {
                SetSorterHashOnShowHistory(ss);
            }
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
            if (!orderBy.Any(o => o.ColumnBracket == "\"UpdatedTime\""))
            {
                orderBy.Add(
                    tableName: ss.ReferenceType,
                    columnBracket: "\"UpdatedTime\"",
                    orderType: SqlOrderBy.Types.desc);
            }
            if (!orderBy.Any(o => o.ColumnBracket == $"\"{Rds.IdColumn(ss.ReferenceType)}\""))
            {
                orderBy.Add(
                    tableName: ss.ReferenceType,
                    columnBracket: $"\"{Rds.IdColumn(ss.ReferenceType)}\"",
                    orderType: SqlOrderBy.Types.desc);
            }
            return orderBy;
        }

        private void SetSorterHashOnShowHistory(SiteSettings ss)
        {
            ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
            ColumnSorterHash.Add(
                Rds.IdColumn(ss.ReferenceType),
                SqlOrderBy.Types.desc);
            ColumnSorterHash.Add(
                "Ver",
                SqlOrderBy.Types.desc);
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
                    if (column.Linked(
                        context: context,
                        withoutWiki: true))
                    {
                        SqlJoinCollection join = null;
                        if (tableName.Contains("~"))
                        {
                            // 親テーブルのリンク項目でソートする場合にはサブクエリのselectにjoinが必要
                            join = ss.SqlJoinCollection(
                                context: context,
                                tableNames: tableName.ToSingleList());
                        }
                        orderBy.Add(new SqlOrderBy(
                            orderType: data.Value,
                            sub: Rds.SelectItems(
                                column: Rds.ItemsColumn().Title(),
                                join: join,
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
            var negative = UseNegativeFilters(
                ss: ss,
                name: "ViewFilters_Search") == true;
            var collection = new SqlWhereCollection();
            Search?
                .Replace("　", " ")
                .Replace(" or ", "\n")
                .Split('\n')
                .Where(o => !o.IsNullOrEmpty())
                .ForEach(search =>
                    collection.Add(and: new SqlWhereCollection().FullTextWhere(
                        context: context,
                        ss: ss,
                        searchText: search,
                        itemJoin: itemJoin,
                        negative: negative)));
            if (collection.Any())
                if (negative)
                {
                    where.Add(and: collection);
                }
                else
                {
                    where.Add(or: collection);
                }
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
            AddExtendedFieldParam(
                context: context,
                param: param,
                extendedFieldType: "Filter",
                sourceHash: ColumnFilterHash);
            AddExtendedFieldParam(
                context: context,
                param: param,
                extendedFieldType: "ViewExtensions",
                sourceHash: ViewExtensionsHash);
            return param;
        }

        private void AddExtendedFieldParam(
            Context context,
            SqlParamCollection param,
            string extendedFieldType,
            Dictionary<string, string> sourceHash)
        {
            context.ExtendedFields
                ?.Where(extendedField =>
                    extendedField.FieldType == extendedFieldType
                    && extendedField.SqlParam
                    && sourceHash?.ContainsKey(extendedField.Name) == true)
                .Select(extendedField => new SqlParam()
                {
                    VariableName = extendedField.Name,
                    Value = sourceHash[extendedField.Name],
                    NoCount = true
                })
                .ForEach(o => param.Add(o));
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

        public void MergeSession(View sessionView)
        {
            if (MergeSessionViewFilters == true)
            {
                Incomplete |= sessionView.Incomplete;
                Own |= sessionView.Own;
                NearCompletionTime |= sessionView.NearCompletionTime;
                Delay |= sessionView.Delay;
                Overdue |= sessionView.Overdue;
                Search = Strings.CoalesceEmpty(Search, sessionView.Search);
                sessionView?.ColumnFilterHash?.ForEach(item =>
                {
                    if (ColumnFilterHash == null)
                    {
                        ColumnFilterHash = new Dictionary<string, string>();
                    }
                    ColumnFilterHash.AddIfNotConainsKey(item.Key, item.Value);
                });
            }
            if (MergeSessionViewSorters == true)
            {
                sessionView?.ColumnSorterHash?.ForEach(item =>
                {
                    if (ColumnSorterHash == null)
                    {
                        ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
                    }
                    ColumnSorterHash.AddIfNotConainsKey(item.Key, item.Value);
                });
            }
        }

        public void SetColumnFilterHashByExpression(
            SiteSettings ss,
            Column targetColumn,
            string columnName,
            string expression,
            bool raw)
        {
            if (raw)
            {
                if (expression == "=")
                {
                    ColumnFilterHash[Rds.IdColumn(ss.ReferenceType)] = "-1";
                }
                else
                {
                    ColumnFilterHash[columnName] = expression.Substring(1);
                }
            }
            else if (expression.IsNullOrEmpty())
            {
                ColumnFilterHash[Rds.IdColumn(ss.ReferenceType)] = "-1";
            }
            else
            {
                switch (targetColumn.TypeName.CsTypeSummary())
                {
                    case Types.CsBool:
                        ColumnFilterHash[columnName] = expression.ToBool().ToString();
                        break;
                    case Types.CsNumeric:
                        ColumnFilterHash[columnName] = targetColumn.HasChoices()
                            || targetColumn.TypeName == "decimal"
                                ? expression.ToSingleList().ToJson()
                                : expression;
                        break;
                    case Types.CsDateTime:
                        ColumnFilterHash[columnName] = $"[\"{expression},{expression}\"]";
                        break;
                    case Types.CsString:
                        ColumnFilterHash[columnName] = targetColumn.HasChoices()
                            ? expression.ToSingleList().ToJson()
                            : expression;
                        break;
                }
            }
        }

        public bool UseNegativeFilters(SiteSettings ss, string name)
        {
            return ss.UseNegativeFilters == true
                && ColumnFilterNegatives?.Contains(name) == true;
        }

        public void CopyViewFilters(View view)
        {
            view.Own = Own;
            view.NearCompletionTime = NearCompletionTime;
            view.Delay = Delay;
            view.Overdue = Overdue;
            view.ColumnFilterHash = ColumnFilterHash?.ToDictionary(
                o => o.Key,
                o => o.Value)
                    ?? new Dictionary<string, string>();
            view.ColumnFilterSearchTypes = ColumnFilterSearchTypes?.ToDictionary(
                o => o.Key,
                o => o.Value)
                    ?? new Dictionary<string, Column.SearchTypes>();
        }

        public void AddAnalyPart(Context context)
        {
            var analyPartSetting = new AnalyPartSetting()
            {
                Id = AnalyPartSettings?.Max(o => o.Id) + 1 ?? 1,
                GroupBy = context.Forms.Data("AnalyPartGroupBy"),
                TimePeriodValue = context.Forms.Decimal(
                    context: context,
                    key: "AnalyPartTimePeriodValue"),
                TimePeriod = context.Forms.Data("AnalyPartTimePeriod"),
                AggregationType = context.Forms.Data("AnalyPartAggregationType"),
                AggregationTarget = context.Forms.Data("AnalyPartAggregationTarget")
            };
            if (AnalyPartSettings == null)
            {
                AnalyPartSettings = new List<AnalyPartSetting>();
            }
            AnalyPartSettings.Add(analyPartSetting);
        }

        public void DeleteAnalyPart(Context context)
        {
            var controlId = context.Forms.ControlId();
            AnalyPartSettings.RemoveAll(o => o.Id == controlId.Split_2nd('_').ToInt());
        }
    }
}
