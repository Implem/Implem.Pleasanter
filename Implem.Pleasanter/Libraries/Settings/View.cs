using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Search;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
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
        public int Id;
        public string Name;
        public List<string> GridColumns;
        public bool? Incomplete;
        public bool? Own;
        public bool? NearCompletionTime;
        public bool? Delay;
        public bool? Overdue;
        public Dictionary<string, string> ColumnFilterHash;
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

        public string GetCrosstabValue(SiteSettings ss)
        {
            var options = ss.CrosstabColumnsOptions();
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

        public string GetTimeSeriesGroupBy(SiteSettings ss)
        {
            var options = ss.TimeSeriesGroupByOptions();
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

        public string GetTimeSeriesValue(SiteSettings ss)
        {
            var options = ss.TimeSeriesValueOptions();
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

        public string GetKambanValue(SiteSettings ss)
        {
            var options = ss.KambanValueOptions();
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
            var data = context.Forms.Data(controlId);
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
            Context context, SiteSettings ss, string columnName, string value)
        {
            if (ColumnFilterHash == null)
            {
                ColumnFilterHash = new Dictionary<string, string>();
            }
            var column = ss.GetColumn(context: context, columnName: columnName);
            if (column != null)
            {
                if (value != string.Empty)
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

        public View GetRecordingData(SiteSettings ss)
        {
            var view = new View();
            view.Id = Id;
            view.Name = Name;
            if (GridColumns != null && GridColumns.Join() != ss.GridColumns.Join())
            {
                view.GridColumns = GridColumns;
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
            if (ColumnFilterHash?.Any() == true)
            {
                view.ColumnFilterHash = new Dictionary<string, string>();
                ColumnFilterHash
                    .Where(o => o.Value != "[]")
                    .ForEach(o => view.ColumnFilterHash.Add(o.Key, o.Value));
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
            where.OnSelectingWhereExtendedSqls(ss: ss);
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

        private void SetColumnsWhere(Context context, SiteSettings ss, SqlWhereCollection where)
        {
            var prefix = "ViewFilters_" + ss.ReferenceType + "_";
            var prefixLength = prefix.Length;
            ColumnFilterHash?
                .Select(data => new
                {
                    Column = ss.GetColumn(context: context, columnName: data.Key),
                    ColumnName = data.Key,
                    data.Value
                })
                .Where(o => o.Column != null)
                .ForEach(data =>
                {
                    if (data.ColumnName == "SiteTitle")
                    {
                        CsNumericColumns(
                            column: ss.GetColumn(context: context, columnName: "SiteId"),
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
            switch (column.CheckFilterControlType)
            {
                case ColumnUtilities.CheckFilterControlTypes.OnOnly:
                    if (value.ToBool())
                    {
                        where.Bool(column, true);
                    }
                    break;
                case ColumnUtilities.CheckFilterControlTypes.OnAndOff:
                    switch ((ColumnUtilities.CheckFilterTypes)value.ToInt())
                    {
                        case ColumnUtilities.CheckFilterTypes.On:
                            where.Bool(column, true);
                            break;
                        case ColumnUtilities.CheckFilterTypes.Off:
                            where.Add(or: new SqlWhereCollection()
                                .Bool(column, null)
                                .Bool(column, false));
                            break;
                    }
                    break;
            }
        }

        private void CsNumericColumns(Column column, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
            if (param.Count == 0) { return; }
            var collection = new SqlWhereCollection();
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
            where.Add(or: collection);
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
            yield return new SqlWhere(
                tableName: column.TableName(),
                columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                _operator: "={0}".Params(column.Type == Column.Types.User
                    ? User.UserTypes.Anonymous.ToInt()
                    : 0));
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
            if ((to == string.Empty && from.ToDecimal() <= 0)
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
            if (param.Any())
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
            return param.Any(o => o != "\t")
                ? new SqlWhere(
                    tableName: column.TableName(),
                    raw: param.Select(range =>
                    {
                        var from = range.Split_1st();
                        var to = range.Split_2nd();
                        if (!from.IsNullOrEmpty() && !to.IsNullOrEmpty())
                        {
                            return "#TableBracket#.\"{0}\" between '{1}' and '{2}'".Params(
                                column.Name,
                                ConvertDateTimeParam(from, column).ToUniversal(context: context)
                                    .ToString("yyyy/M/d H:m:s"),
                                ConvertDateTimeParam(to, column).ToDateTime().ToUniversal(context: context)
                                    .ToString("yyyy/M/d H:m:s.fff"));
                        }
                        else if (to.IsNullOrEmpty())
                        {
                            return "#TableBracket#.\"{0}\" >= '{1}'".Params(
                                column.Name,
                                ConvertDateTimeParam(from, column).ToDateTime().ToUniversal(context: context)
                                    .ToString("yyyy/M/d H:m:s"));
                        }
                        else
                        {
                            return "#TableBracket#.\"{0}\" <= '{1}'".Params(
                                column.Name,
                                ConvertDateTimeParam(to, column).ToDateTime().ToUniversal(context: context)
                                    .ToString("yyyy/M/d H:m:s.fff"));
                        }
                    }).Join(" or "))
                : null;
        }

        private DateTime ConvertDateTimeParam(string dateTimeString, Column column)
        {
            var dt = dateTimeString.ToDateTime();
            switch (column.Name)
            {
                case "CompletionTime":
                    return column.DateTimepicker() ? dt : dt.AddDays(1);
                default:
                    return dt;
            }
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

        private void CsStringColumns(Context context, Column column, string value, SqlWhereCollection where)
        {
            if (column.HasChoices())
            {
                var param = value.Deserialize<List<string>>();
                if (param?.Any() == true)
                {
                    where.Add(or: new SqlWhereCollection(
                        CsStringColumnsWhere(column, param),
                        CsStringColumnsWhereNull(column, param)));
                }
            }
            else
            {
                if (!value.IsNullOrEmpty())
                {
                    var tableName = column.TableName();
                    var name = Strings.NewGuid();
                    where.SqlWhereLike(
                        tableName: tableName,
                        name: name,
                        searchText: value,
                        clauseCollection: "(\"{0}\".\"{1}\" like {2}@{3}{4}"
                            .Params(
                                tableName, 
                                column.Name, 
                                context.Sqls.WhereLikeTemplateForward, 
                                name,
                                context.Sqls.WhereLikeTemplate)
                            .ToSingleList());
                }
            }
        }

        private SqlWhere CsStringColumnsWhere(Column column, List<string> param)
        {
            return param.Any(o => o != "\t")
                ? new SqlWhere(
                    tableName: column.TableName(),
                    columnBrackets: ("\"" + column.Name + "\"").ToSingleArray(),
                    name: column.ParamName(),
                    value: param.Where(o => o != "\t"),
                    multiParamOperator: " or ")
                : null;
        }

        private SqlWhere CsStringColumnsWhereNull(Column column, List<string> param)
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
                        _operator: "=''")))
                : null;
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
                    var tableName = column.TableName();
                    switch (column.Name)
                    {
                        case "Title":
                        case "ItemTitle":
                            orderBy.Add(new SqlOrderBy(
                                columnBracket: "\"Title\"",
                                orderType: data.Value,
                                tableName: $"{tableName}_Items"));
                            break;
                        case "TitleBody":
                            orderBy.Add(new SqlOrderBy(
                                columnBracket: "\"Title\"",
                                orderType: data.Value,
                                tableName: $"{tableName}_Items"));
                            orderBy.Add(
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: column.Joined
                                        ? $"{tableName},Body"
                                        : "Body"),
                                orderType: data.Value);
                            break;
                        default:
                            if (column.Linked())
                            {
                                orderBy.Add(new SqlOrderBy(
                                    orderType: data.Value,
                                    sub: Rds.SelectItems(
                                        column: Rds.ItemsColumn().Title(),
                                        where: Rds.ItemsWhere()
                                            .SiteId_In(column.SiteSettings.Links
                                                .Where(o => o.ColumnName == column.Name)
                                                .Select(o => o.SiteId))
                                            .ReferenceId(raw: $"{context.SqlCommandText.CreateTryCast(tableName, column.Name, column.TypeName, "bigint")}"))));
                            }
                            else
                            {
                                orderBy.Add(
                                    column: column,
                                    orderType: data.Value);
                            }
                            break;
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
    }
}
