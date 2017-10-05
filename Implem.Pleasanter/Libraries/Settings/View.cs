using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class View
    {
        public int Id;
        public string Name;
        public bool? Incomplete;
        public bool? Own;
        public bool? NearCompletionTime;
        public bool? Delay;
        public bool? Overdue;
        public Dictionary<string, string> ColumnFilterHash;
        public string Search;
        public Dictionary<string, SqlOrderBy.Types> ColumnSorterHash;
        public string CalendarColumn;
        public DateTime? CalendarMonth;
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

        public View()
        {
        }

        public View(SiteSettings ss)
        {
            SetByForm(ss);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            KambanColumns = KambanColumns ?? Parameters.General.KambanColumns;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
        }

        public string GetCalendarColumn(SiteSettings ss)
        {
            return !CalendarColumn.IsNullOrEmpty()
                ? CalendarColumn
                : Definition(ss, "Calendar")?.Option1;
        }

        public string GetCrosstabGroupByX(SiteSettings ss)
        {
            var options = ss.CrosstabGroupByXOptions();
            return !CrosstabGroupByX.IsNullOrEmpty()
                ? CrosstabGroupByX
                : options.ContainsKey(Definition(ss, "Crosstab")?.Option1)
                    ? Definition(ss, "Crosstab")?.Option1
                    : options.FirstOrDefault().Key;
        }

        public string GetCrosstabGroupByY(SiteSettings ss)
        {
            var options = ss.CrosstabGroupByYOptions();
            return !CrosstabGroupByY.IsNullOrEmpty()
                ? CrosstabGroupByY
                : options.ContainsKey(Definition(ss, "Crosstab")?.Option2)
                    ? Definition(ss, "Crosstab")?.Option2
                    : options.FirstOrDefault().Key;
        }

        public string GetCrosstabAggregateType(SiteSettings ss)
        {
            return !CrosstabAggregateType.IsNullOrEmpty()
                ? CrosstabAggregateType
                : Definition(ss, "Crosstab")?.Option3;
        }

        public string GetCrosstabValue(SiteSettings ss)
        {
            var options = ss.CrosstabColumnsOptions();
            return !CrosstabValue.IsNullOrEmpty()
                ? CrosstabValue
                : options.ContainsKey(Definition(ss, "Crosstab")?.Option4)
                    ? Definition(ss, "Crosstab")?.Option4
                    : options.FirstOrDefault().Key;
        }

        public string GetCrosstabTimePeriod(SiteSettings ss)
        {
            return !CrosstabTimePeriod.IsNullOrEmpty()
                ? CrosstabTimePeriod
                : Definition(ss, "Crosstab")?.Option5;
        }

        public DateTime GetCrosstabMonth(SiteSettings ss)
        {
            if (CrosstabMonth != null)
            {
                return CrosstabMonth.ToDateTime();
            }
            else
            {
                var now = DateTime.Now;
                return new DateTime(now.Year, now.Month, 1);
            }
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

        private ViewModeDefinition Definition(SiteSettings ss, string name)
        {
            return Def.ViewModeDefinitionCollection.FirstOrDefault(o =>
                o.Id == ss.ReferenceType + "_" + name);
        }

        public void SetByForm(SiteSettings ss)
        {
            var columnFilterPrefix = "ViewFilters__";
            var columnSorterPrefix = "ViewSorters__";
            switch (Forms.Data("ControlId"))
            {
                case "ViewFilters_Reset":
                    Incomplete = null;
                    Own = null;
                    NearCompletionTime = null;
                    Delay = null;
                    Overdue = null;
                    ColumnFilterHash = null;
                    Search = null;
                    break;
                case "ViewSorters_Reset":
                    ColumnSorterHash = null;
                    break;
            }
            foreach (string controlId in HttpContext.Current.Request.Form)
            {
                switch (controlId)
                {
                    case "ViewName":
                        Name = String(controlId);
                        break;
                    case "ViewFilters_Incomplete":
                        Incomplete = Bool(controlId);
                        break;
                    case "ViewFilters_Own":
                        Own = Bool(controlId);
                        break;
                    case "ViewFilters_NearCompletionTime":
                        NearCompletionTime = Bool(controlId);
                        break;
                    case "ViewFilters_Delay":
                        Delay = Bool(controlId);
                        break;
                    case "ViewFilters_Overdue":
                        Overdue = Bool(controlId);
                        break;
                    case "ViewFilters_Search":
                        Search = String(controlId);
                        break;
                    case "ViewSorters":
                        SetSorters(ss);
                        break;
                    case "CalendarColumn":
                        CalendarColumn = String(controlId);
                        break;
                    case "CalendarMonth":
                        CalendarMonth = Time(controlId);
                        break;
                    case "CrosstabGroupByX":
                        CrosstabGroupByX = String(controlId);
                        break;
                    case "CrosstabGroupByY":
                        CrosstabGroupByY = String(controlId);
                        break;
                    case "CrosstabColumns":
                        CrosstabColumns = String(controlId);
                        break;
                    case "CrosstabAggregateType":
                        CrosstabAggregateType = String(controlId);
                        break;
                    case "CrosstabValue":
                        CrosstabValue = String(controlId);
                        break;
                    case "CrosstabTimePeriod":
                        CrosstabTimePeriod = String(controlId);
                        break;
                    case "CrosstabMonth":
                        CrosstabMonth = Time(controlId);
                        break;
                    case "GanttGroupBy":
                        GanttGroupBy = String(controlId);
                        break;
                    case "GanttSortBy":
                        GanttSortBy = String(controlId);
                        break;
                    case "GanttPeriod":
                        GanttPeriod = Forms.Int(controlId);
                        break;
                    case "GanttStartDate":
                        GanttStartDate = Time(controlId).ToDateTime().ToUniversal();
                        break;
                    case "TimeSeriesGroupBy":
                        TimeSeriesGroupBy = String(controlId);
                        break;
                    case "TimeSeriesAggregateType":
                        TimeSeriesAggregateType = String(controlId);
                        break;
                    case "TimeSeriesValue":
                        TimeSeriesValue = String(controlId);
                        break;
                    case "KambanGroupByX":
                        KambanGroupByX = String(controlId);
                        break;
                    case "KambanGroupByY":
                        KambanGroupByY = String(controlId);
                        break;
                    case "KambanAggregateType":
                        KambanAggregateType = String(controlId);
                        break;
                    case "KambanValue":
                        KambanValue = String(controlId);
                        break;
                    case "KambanColumns":
                        KambanColumns = Forms.Int(controlId);
                        break;
                    case "KambanAggregationView":
                        KambanAggregationView = Forms.Bool(controlId);
                        break;
                    default:
                        if (controlId.StartsWith(columnFilterPrefix))
                        {
                            AddColumnFilterHash(
                                ss,
                                controlId.Substring(columnFilterPrefix.Length),
                                Forms.Data(controlId));
                        }
                        else if (controlId.StartsWith(columnSorterPrefix))
                        {
                            AddColumnSorterHash(
                                ss,
                                controlId.Substring(columnSorterPrefix.Length),
                                OrderByType(Forms.Data(controlId)));
                        }
                        break;
                }
            }
            KambanColumns = KambanColumns ?? Parameters.General.KambanColumns;
        }

        private bool? Bool(string controlId)
        {
            var data = Forms.Bool(controlId);
            if (data)
            {
                return true;
            }
            else
            {
                return null;
            }
        }

        private DateTime? Time(string controlId)
        {
            var data = Forms.DateTime(controlId);
            if (data.InRange())
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        private string String(string controlId)
        {
            var data = Forms.Data(controlId);
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

        public bool FilterContains(string name)
        {
            return ColumnFilterHash != null && ColumnFilterHash.ContainsKey(name);
        }

        public SqlOrderBy.Types ColumnSorter(string columnName)
        {
            return ColumnSorterHash?.ContainsKey(columnName) == true
                ? ColumnSorterHash[columnName]
                : SqlOrderBy.Types.release;
        }

        public bool SorterContains(string name)
        {
            return ColumnSorterHash != null && ColumnSorterHash.ContainsKey(name);
        }

        private void AddColumnFilterHash(SiteSettings ss, string columnName, string value)
        {
            if (ColumnFilterHash == null)
            {
                ColumnFilterHash = new Dictionary<string, string>();
            }
            var column = ss.GetColumn(columnName);
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
            SiteSettings ss, string columnName, SqlOrderBy.Types value)
        {
            if (ColumnSorterHash == null)
            {
                ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
            }
            var column = ss.GetColumn(columnName);
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

        private void SetSorters(SiteSettings ss)
        {
            ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
            Forms.List("ViewSorters").ForEach(data =>
            {
                var columnName = data.Split_1st();
                var type = OrderByType(data.Split_2nd());
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
                case "Asc": return SqlOrderBy.Types.asc;
                case "Desc": return SqlOrderBy.Types.desc;
                default: return SqlOrderBy.Types.release;
            }
        }

        public View GetRecordingData()
        {
            var view = new View();
            view.Id = Id;
            view.Name = Name;
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
            if (!CalendarColumn.IsNullOrEmpty())
            {
                view.CalendarColumn = CalendarColumn;
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
            if (KambanColumns != Parameters.General.KambanColumns)
            {
                view.KambanColumns = KambanColumns;
            }
            if (KambanAggregationView == true)
            {
                view.KambanAggregationView = KambanAggregationView;
            }
            return view;
        }

        public SqlWhereCollection Where(SiteSettings ss, SqlWhereCollection where = null)
        {
            var tableName = ss.ReferenceType;
            if (where == null) where = new SqlWhereCollection();
            SetGeneralsWhere(ss, where, tableName);
            SetColumnsWhere(ss, where, tableName);
            SetSearchWhere(ss, where, tableName);
            Permissions.SetCanReadWhere(ss, where);
            return where;
        }

        private void SetGeneralsWhere(SiteSettings ss, SqlWhereCollection where, string tableName)
        {
            if (Incomplete == true)
            {
                where.Add(
                    tableName: tableName,
                    columnBrackets: new string[] { "[Status]" },
                    name: "_U",
                    _operator: "<{0}".Params(Parameters.General.CompletionCode));
            }
            if (Own == true)
            {
                where.Add(
                    tableName: tableName,
                    columnBrackets: new string[] { "[Manager]", "[Owner]" },
                    name: "_U",
                    value: Sessions.UserId());
            }
            if (NearCompletionTime == true)
            {
                where.Add(
                    tableName: tableName,
                    columnBrackets: new string[] { "[CompletionTime]" },
                    _operator: " between '{0}' and '{1}'".Params(
                        DateTime.Now.ToLocal().Date
                            .AddDays(ss.NearCompletionTimeBeforeDays.ToInt() * (-1)),
                        DateTime.Now.ToLocal().Date
                            .AddDays(ss.NearCompletionTimeAfterDays.ToInt() + 1)
                            .AddMilliseconds(Parameters.Rds.MinimumTime * -1)
                            .ToString("yyyy/M/d H:m:s.fff")));
            }
            if (Delay == true)
            {
                where
                    .Add(
                        tableName: tableName,
                        columnBrackets: new string[] { "[Status]" },
                        name: "_U",
                        _operator: "<{0}".Params(Parameters.General.CompletionCode))
                    .Add(
                        tableName: tableName,
                        columnBrackets: new string[] { "[ProgressRate]" },
                        _operator: "<",
                        raw: Def.Sql.ProgressRateDelay
                            .Replace("#TableName#", ss.ReferenceType));
            }
            if (Overdue == true)
            {
                where
                    .Add(
                        tableName: tableName,
                        columnBrackets: new string[] { "[Status]" },
                        name: "_U",
                        _operator: "<{0}".Params(Parameters.General.CompletionCode))
                    .Add(
                        tableName: tableName,
                        columnBrackets: new string[] { "[CompletionTime]" },
                        _operator: "<getdate()");
            }
        }

        private void SetColumnsWhere(SiteSettings ss, SqlWhereCollection where, string tableName)
        {
            var prefix = "ViewFilters_" + ss.ReferenceType + "_";
            var prefixLength = prefix.Length;
            ColumnFilterHash?
                .Select(data => new
                {
                    Column = ss.GetColumn(data.Key),
                    ColumnName = data.Key,
                    Value = data.Value
                })
                .Where(o => o.Column != null)
                .ForEach(data =>
                {
                    if (data.ColumnName == "SiteTitle")
                    {
                        CsNumericColumns(ss.GetColumn("SiteId"), data.Value, where);
                    }
                    else
                    {
                        switch (data.Column.TypeName.CsTypeSummary())
                        {
                            case Types.CsBool:
                                CsBoolColumns(data.Column, data.Value, where);
                                break;
                            case Types.CsNumeric:
                                CsNumericColumns(data.Column, data.Value, where);
                                break;
                            case Types.CsDateTime:
                                CsDateTimeColumns(data.Column, data.Value, where);
                                break;
                            case Types.CsString:
                                CsStringColumns(data.Column, data.Value, where);
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
                        where.Add(
                            tableName: column.TableName(),
                            raw: "[" + column.Name + "]=1");
                    }
                    break;
                case ColumnUtilities.CheckFilterControlTypes.OnAndOff:
                    switch ((ColumnUtilities.CheckFilterTypes)value.ToInt())
                    {
                        case ColumnUtilities.CheckFilterTypes.On:
                            where.Add(
                                tableName: column.TableName(),
                                raw: "[" + column.Name + "]=1");
                            break;
                        case ColumnUtilities.CheckFilterTypes.Off:
                            where.Add(
                                tableName: column.TableName(),
                                raw: "(#TableName#.[{0}] is null or #TableName#.[{0}]=0)"
                                    .Params(column.Name));
                            break;
                    }
                    break;
            }
        }

        private void CsNumericColumns(Column column, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
            if (param.Any())
            {
                if (param.All(o => o.RegexExists(@"^[0-9\.]*,[0-9\.]*$")))
                {
                    CsNumericRangeColumns(column, param, where);
                }
                else
                {
                    CsNumericColumns(column, param, where);
                }
            }
        }

        private void CsNumericColumns(
            Column column, List<string> param, SqlWhereCollection where)
        {
            if (param.Any())
            {
                where.Add(or: new SqlWhereCollection(
                    CsNumericColumnsWhere(column, param),
                    CsNumericColumnsWhereNull(column, param)));
            }
        }

        private SqlWhere CsNumericColumnsWhere(Column column, List<string> param)
        {
            return param.Any(o => o != "\t")
                ? new SqlWhere(
                    tableName: column.TableName(),
                    columnBrackets: new string[] { "[" + column.Name + "]" },
                    name: column.Name,
                    _operator: " in ({0})".Params(param
                        .Where(o => o != "\t")
                        .Select(o => o.ToDecimal())
                        .Join()))
                : null;
        }

        private SqlWhere CsNumericColumnsWhereNull(Column column, List<string> param)
        {
            return param.Any(o => o == "\t")
                ? new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[] { "[" + column.Name + "]" },
                        _operator: " is null"),
                    new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[] { "[" + column.Name + "]" },
                        _operator: "={0}".Params(column.UserColumn
                            ? User.UserTypes.Anonymous.ToInt()
                            : 0))))
                : null;
        }

        private void CsNumericRangeColumns(
            Column column,
            List<string> param,
            SqlWhereCollection where)
        {
            var parts = new SqlWhereCollection();
            param.ForEach(data =>
            {
                var from = data.Split_1st();
                var to = data.Split_2nd();
                if (from == string.Empty)
                {
                    parts.Add(new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[] { "[" + column.Name + "]" },
                        _operator: "<{0}".Params(to.ToDecimal())));
                }
                else if (to == string.Empty)
                {
                    parts.Add(new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[] { "[" + column.Name + "]" },
                        _operator: ">={0}".Params(from.ToDecimal())));
                }
                else
                {
                    parts.Add(new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[] { "[" + column.Name + "]" },
                        _operator: " between {0} and {1}".Params(
                            from.ToDecimal(), to.ToDecimal())));
                }
            });
            where.Add(or: parts);
        }

        private void CsDateTimeColumns(
            Column column, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
            if (param.Any())
            {
                where.Add(or: new SqlWhereCollection(
                    CsDateTimeColumnsWhere(column, param),
                    CsDateTimeColumnsWhereNull(column, param)));
            }
        }

        private SqlWhere CsDateTimeColumnsWhere(Column column, List<string> param)
        {
            return param.Any(o => o != "\t")
                ? new SqlWhere(
                    tableName: column.TableName(),
                    raw: param.Select(range =>
                        "#TableBracket#.[{0}] between '{1}' and '{2}'".Params(
                            column.Name,
                            range.Split_1st().ToDateTime().ToUniversal()
                                .ToString("yyyy/M/d H:m:s"),
                            range.Split_2nd().ToDateTime().ToUniversal()
                                .ToString("yyyy/M/d H:m:s.fff"))).Join(" or "))
                : null;
        }

        private SqlWhere CsDateTimeColumnsWhereNull(Column column, List<string> param)
        {
            return param.Any(o => o == "\t")
                ? new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[] { "[" + column.Name + "]" },
                        _operator: " is null"),
                    new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[] { "[" + column.Name + "]" },
                        _operator: " not between '{0}' and '{1}'".Params(
                            Parameters.General.MinTime.ToUniversal()
                                .ToString("yyyy/M/d H:m:s"),
                            Parameters.General.MaxTime.ToUniversal()
                                .ToString("yyyy/M/d H:m:s")))))
                : null;
        }

        private void CsStringColumns(Column column, string value, SqlWhereCollection where)
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
                    where.SqlWhereLike(
                        value,
                        "([{0}].[{1}] like '%' + @SearchText#ParamCount#_#CommandCount# + '%')"
                            .Params(column.TableName(), column.Name));
                }
            }
        }

        private SqlWhere CsStringColumnsWhere(Column column, List<string> param)
        {
            return param.Any(o => o != "\t")
                ? new SqlWhere(
                    tableName: column.TableName(),
                    columnBrackets: new string[] { "[" + column.Name + "]" },
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
                        columnBrackets: new string[] { "[" + column.Name + "]" },
                        _operator: " is null"),
                    new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[] { "[" + column.Name + "]" },
                        _operator: "=''")))
                : null;
        }

        public SqlOrderByCollection OrderBy(SiteSettings ss, SqlOrderByCollection orderBy = null)
        {
            if (ColumnSorterHash?.Any() == true)
            {
                orderBy = orderBy ?? new SqlOrderByCollection();
                ColumnSorterHash?.ForEach(data =>
                    orderBy.Add(ss.GetColumn(data.Key), data.Value));
            }
            return orderBy;
        }

        private void SetSearchWhere(SiteSettings ss, SqlWhereCollection where, string tableName)
        {
            if (Search.IsNullOrEmpty()) return;
            var select = SearchIndexUtilities.Select(
                searchText: Search,
                siteIdList: ss.AllowedIntegratedSites != null
                    ? ss.AllowedIntegratedSites
                    : new List<long> { ss.SiteId });
            if (select != null)
            {
                switch (ss.ReferenceType)
                {
                    case "Issues":
                        where.Add(
                            tableName: tableName,
                            columnBrackets: new string[] { "[IssueId]" },
                            name: "IssueId",
                            _operator: " in ",
                            sub: select);
                        break;
                    case "Results":
                        where.Add(
                            tableName: tableName,
                            columnBrackets: new string[] { "[ResultId]" },
                            name: "ResultId",
                            _operator: " in ",
                            sub: select);
                        break;
                    case "Wikis":
                        where.Add(
                            tableName: tableName,
                            columnBrackets: new string[] { "[WikiId]" },
                            name: "WikiId",
                            _operator: " in ",
                            sub: select);
                        break;
                }
            }
            else
            {
                where.Add(tableName: null, raw: "0=1");
            }
        }
    }
}
