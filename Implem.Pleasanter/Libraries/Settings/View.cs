using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
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
        public string GanttGroupBy;
        public string TimeSeriesGroupBy;
        public string TimeSeriesAggregateType;
        public string TimeSeriesValue;
        public string KambanGroupByX;
        public string KambanGroupByY;
        public string KambanAggregateType;
        public string KambanValue;
        public int? KambanColumns;
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

        public void SetByForm(SiteSettings ss)
        {
            var columnFilterPrefix = "ViewFilters_{0}_".Params(ss.ReferenceType);
            var columnSorterPrefix = "ViewSorters_{0}_".Params(ss.ReferenceType);
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
                    case "GanttGroupBy":
                        GanttGroupBy = String(controlId);
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
                    default:
                        if (controlId.StartsWith(columnFilterPrefix))
                        {
                            AddColumnFilterHash(
                                ss,
                                controlId.Substring(columnFilterPrefix.Length),
                                Forms.Data(controlId));
                        }
                        if (controlId.StartsWith(columnSorterPrefix))
                        {
                            AddColumnSorterHash(
                                ss,
                                controlId.Substring(columnFilterPrefix.Length),
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
            if (ss.Columns.Any(o => o.ColumnName == columnName))
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
            if (ss.Columns.Any(o => o.ColumnName == columnName))
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

        public void SetRecordingData()
        {
            if (!Incomplete.ToBool()) Incomplete = null;
            if (!Own.ToBool()) Own = null;
            if (!NearCompletionTime.ToBool()) NearCompletionTime = null;
            if (!Delay.ToBool()) Delay = null;
            if (!Overdue.ToBool()) Overdue = null;
            ColumnFilterHash?.RemoveAll((key, value) => value == "[]");
            if (!ColumnFilterHash?.Any() == true) ColumnFilterHash = null;
            if (!ColumnSorterHash?.Any() == true) ColumnSorterHash = null;
            if (Search == string.Empty) Search = null;
            if (GanttGroupBy == string.Empty) GanttGroupBy = null;
            if (TimeSeriesGroupBy == string.Empty) TimeSeriesGroupBy = null;
            if (TimeSeriesAggregateType == string.Empty) TimeSeriesAggregateType = null;
            if (TimeSeriesValue == string.Empty) TimeSeriesValue = null;
            if (KambanGroupByX == string.Empty) KambanGroupByX = null;
            if (KambanGroupByY == string.Empty) KambanGroupByY = null;
            if (KambanAggregateType == string.Empty) KambanAggregateType = null;
            if (KambanValue == string.Empty) KambanValue = null;
            if (KambanColumns == Parameters.General.KambanColumns) KambanColumns = null;
        }

        public SqlWhereCollection Where(SiteSettings ss, SqlWhereCollection where = null)
        {
            if (where == null) where = new SqlWhereCollection();
            GeneralsWhere(ss, where);
            ColumnsWhere(ss, where);
            SearchWhere(ss, where);
            return where;
        }

        private SqlWhereCollection GeneralsWhere(SiteSettings ss, SqlWhereCollection where)
        {
            if (Incomplete == true)
            {
                where.Add(
                    columnBrackets: new string[] { "[Status]" },
                    name: "_U",
                    _operator: "<{0}".Params(Parameters.General.CompletionCode));
            }
            if (Own == true)
            {
                where.Add(
                    columnBrackets: new string[] { "[Manager]", "[Owner]" },
                    name: "_U",
                    value: Sessions.UserId());
            }
            if (NearCompletionTime == true)
            {
                where.Add(
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
                        columnBrackets: new string[] { "[Status]" },
                        name: "_U",
                        _operator: "<{0}".Params(Parameters.General.CompletionCode))
                    .Add(
                        columnBrackets: new string[] { "[ProgressRate]" },
                        _operator: "<",
                        raw: Def.Sql.ProgressRateDelay
                            .Replace("#TableName#", ss.ReferenceType));
            }
            if (Overdue == true)
            {
                where
                    .Add(
                        columnBrackets: new string[] { "[Status]" },
                        name: "_U",
                        _operator: "<{0}".Params(Parameters.General.CompletionCode))
                    .Add(
                        columnBrackets: new string[] { "[CompletionTime]" },
                        _operator: "<getdate()");
            }
            return where;
        }

        private SqlWhereCollection ColumnsWhere(SiteSettings ss, SqlWhereCollection where)
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
                    switch (data.Column.TypeName.CsTypeSummary())
                    {
                        case Types.CsBool:
                            CsBoolColumns(data.Column, data.ColumnName, data.Value, where);
                            break;
                        case Types.CsNumeric:
                            CsNumericColumns(data.Column, data.ColumnName, data.Value, where);
                            break;
                        case Types.CsDateTime:
                            CsDateTimeColumns(data.Column, data.ColumnName, data.Value, where);
                            break;
                        case Types.CsString:
                            CsStringColumns(data.ColumnName, data.Value, where);
                            break;
                    }
                });
            return where;
        }

        private void CsBoolColumns(
            Column column, string columnName, string value, SqlWhereCollection where)
        {
            switch (column.CheckFilterControlType)
            {
                case ColumnUtilities.CheckFilterControlTypes.OnOnly:
                    if (value.ToBool())
                    {
                        where.Add(raw: "[" + columnName + "]=1");
                    }
                    break;
                case ColumnUtilities.CheckFilterControlTypes.OnAndOff:
                    switch ((ColumnUtilities.CheckFilterTypes)value.ToInt())
                    {
                        case ColumnUtilities.CheckFilterTypes.On:
                            where.Add(raw: "[" + columnName + "]=1");
                            break;
                        case ColumnUtilities.CheckFilterTypes.Off:
                            where.Add(raw: "($[{0}] is null or $[{0}]=0)"
                                .Params(columnName));
                            break;
                    }
                    break;
            }
        }

        private void CsNumericColumns(
            Column column, string columnName, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
            if (param.Any())
            {
                if (param.All(o => o.RegexExists(@"^[0-9\.]*,[0-9\.]*$")))
                {
                    CsNumericRangeColumns(column, columnName, param, where);
                }
                else
                {
                    CsNumericColumns(column, columnName, param, where);
                }
            }
        }

        private void CsNumericColumns(
            Column column, string columnName, List<string> param, SqlWhereCollection where)
        {
            if (param.Any())
            {
                where.Add(or: new SqlWhereCollection(
                    CsNumericColumnsWhere(columnName, param),
                    CsNumericColumnsWhereNull(column, columnName, param)));
            }
        }

        private SqlWhere CsNumericColumnsWhere(string columnName, List<string> param)
        {
            return param.Any(o => o != "\t")
                ? new SqlWhere(
                    columnBrackets: new string[] { "[" + columnName + "]" },
                    name: columnName,
                    _operator: " in ({0})".Params(param
                        .Where(o => o != "\t")
                        .Select(o => o.ToDecimal())
                        .Join()))
                : null;
        }

        private SqlWhere CsNumericColumnsWhereNull(
            Column column, string columnName, List<string> param)
        {
            return param.Any(o => o == "\t")
                ? new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        columnBrackets: new string[] { "[" + columnName + "]" },
                        _operator: " is null"),
                    new SqlWhere(
                        columnBrackets: new string[] { "[" + columnName + "]" },
                        _operator: "={0}".Params(column.UserColumn
                            ? User.UserTypes.Anonymous.ToInt()
                            : 0))))
                : null;
        }

        private void CsNumericRangeColumns(
            Column column, string columnName, List<string> param, SqlWhereCollection where)
        {
            var parts = new SqlWhereCollection();
            param.ForEach(data =>
            {
                var from = data.Split_1st();
                var to = data.Split_2nd();
                if (from == string.Empty)
                {
                    parts.Add(new SqlWhere(
                        columnBrackets: new string[] { "[" + columnName + "]" },
                        _operator: "<{0}".Params(to.ToDecimal())));
                }
                else if (to == string.Empty)
                {
                    parts.Add(new SqlWhere(
                        columnBrackets: new string[] { "[" + columnName + "]" },
                        _operator: ">={0}".Params(from.ToDecimal())));
                }
                else
                {
                    parts.Add(new SqlWhere(
                        columnBrackets: new string[] { "[" + columnName + "]" },
                        _operator: " between {0} and {1}".Params(
                            from.ToDecimal(), to.ToDecimal())));
                }
            });
            where.Add(or: parts);
        }

        private void CsDateTimeColumns(
            Column column, string columnName, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
            if (param.Any())
            {
                where.Add(or: new SqlWhereCollection(
                    CsDateTimeColumnsWhere(columnName, param),
                    CsDateTimeColumnsWhereNull(column, columnName, param)));
            }
        }

        private SqlWhere CsDateTimeColumnsWhere(string columnName, List<string> param)
        {
            return param.Any(o => o != "\t")
                ? new SqlWhere(raw: param.Select(range =>
                    "[{0}] between '{1}' and '{2}'".Params(
                        columnName,
                        range.Split_1st().ToDateTime().ToUniversal()
                            .ToString("yyyy/M/d H:m:s"),
                        range.Split_2nd().ToDateTime().ToUniversal()
                            .ToString("yyyy/M/d H:m:s.fff"))).Join(" or "))
                : null;
        }

        private SqlWhere CsDateTimeColumnsWhereNull(
            Column column, string columnName, List<string> param)
        {
            return param.Any(o => o == "\t")
                ? new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        columnBrackets: new string[] { "[" + columnName + "]" },
                        _operator: " is null"),
                    new SqlWhere(
                        columnBrackets: new string[] { "[" + columnName + "]" },
                        _operator: " not between '{0}' and '{1}'".Params(
                            Parameters.General.MinTime.ToUniversal()
                                .ToString("yyyy/M/d H:m:s"),
                            Parameters.General.MaxTime.ToUniversal()
                                .ToString("yyyy/M/d H:m:s")))))
                : null;
        }

        private void CsStringColumns(string columnName, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
            if (param.Any())
            {
                where.Add(or: new SqlWhereCollection(
                    CsStringColumnsWhere(columnName, param),
                    CsStringColumnsWhereNull(columnName, param)));
            }
        }

        private SqlWhere CsStringColumnsWhere(string columnName, List<string> param)
        {
            return param.Any(o => o != "\t")
                ? new SqlWhere(
                    columnBrackets: new string[] { "[" + columnName + "]" },
                    name: columnName,
                    value: param.Where(o => o != "\t"),
                    multiParamOperator: " or ")
                : null;
        }

        private SqlWhere CsStringColumnsWhereNull(string columnName, List<string> param)
        {
            return param.Any(o => o == "\t")
                ? new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        columnBrackets: new string[] { "[" + columnName + "]" },
                        _operator: " is null"),
                    new SqlWhere(
                        columnBrackets: new string[] { "[" + columnName + "]" },
                        _operator: "=''")))
                : null;
        }

        private SqlWhereCollection SearchWhere(SiteSettings ss, SqlWhereCollection where)
        {
            var words = Search.SearchIndexes();
            return words.Count() != 0
                ? SearchWhere(ss, where, words)
                : where;
        }

        private SqlWhereCollection SearchWhere(
            SiteSettings ss, SqlWhereCollection where, IEnumerable<string> words)
        {
            var results = SearchIndexUtilities.GetIdCollection(
                searchIndexes: words, siteId: ss.SiteId).Join();
            if (results != string.Empty)
            {
                switch (ss.ReferenceType)
                {
                    case "Issues":
                        where.Add(
                            columnBrackets: new string[] { "[IssueId]" },
                            name: "IssueId",
                            _operator: " in (" + results + ")");
                        break;
                    case "Results":
                        where.Add(
                            columnBrackets: new string[] { "[ResultId]" },
                            name: "ResultId",
                            _operator: " in (" + results + ")");
                        break;
                    case "Wikis":
                        where.Add(
                            columnBrackets: new string[] { "[WikiId]" },
                            name: "WikiId",
                            _operator: " in (" + results + ")");
                        break;
                }
            }
            else
            {
                where.Add(raw: "0=1");
            }
            return where;
        }

        public SqlOrderByCollection OrderBy(SiteSettings ss, SqlOrderByCollection orderBy)
        {
            if (ColumnSorterHash?.Any() == true)
            {
                orderBy = new SqlOrderByCollection();
                ColumnSorterHash?.ForEach(data =>
                {
                    switch (ss.ReferenceType + "_" + data.Key)
                    {
                        case "Depts_TenantId": orderBy.Depts_TenantId(data.Value); break;
                        case "Depts_DeptId": orderBy.Depts_DeptId(data.Value); break;
                        case "Depts_Ver": orderBy.Depts_Ver(data.Value); break;
                        case "Depts_DeptCode": orderBy.Depts_DeptCode(data.Value); break;
                        case "Depts_Dept": orderBy.Depts_Dept(data.Value); break;
                        case "Depts_DeptName": orderBy.Depts_DeptName(data.Value); break;
                        case "Depts_Body": orderBy.Depts_Body(data.Value); break;
                        case "Depts_Comments": orderBy.Depts_Comments(data.Value); break;
                        case "Depts_Creator": orderBy.Depts_Creator(data.Value); break;
                        case "Depts_Updator": orderBy.Depts_Updator(data.Value); break;
                        case "Depts_CreatedTime": orderBy.Depts_CreatedTime(data.Value); break;
                        case "Depts_UpdatedTime": orderBy.Depts_UpdatedTime(data.Value); break;                        case "Groups_TenantId": orderBy.Groups_TenantId(data.Value); break;
                        case "Groups_GroupId": orderBy.Groups_GroupId(data.Value); break;
                        case "Groups_Ver": orderBy.Groups_Ver(data.Value); break;
                        case "Groups_GroupName": orderBy.Groups_GroupName(data.Value); break;
                        case "Groups_Body": orderBy.Groups_Body(data.Value); break;
                        case "Groups_Comments": orderBy.Groups_Comments(data.Value); break;
                        case "Groups_Creator": orderBy.Groups_Creator(data.Value); break;
                        case "Groups_Updator": orderBy.Groups_Updator(data.Value); break;
                        case "Groups_CreatedTime": orderBy.Groups_CreatedTime(data.Value); break;
                        case "Groups_UpdatedTime": orderBy.Groups_UpdatedTime(data.Value); break;                        case "Users_TenantId": orderBy.Users_TenantId(data.Value); break;
                        case "Users_UserId": orderBy.Users_UserId(data.Value); break;
                        case "Users_Ver": orderBy.Users_Ver(data.Value); break;
                        case "Users_LoginId": orderBy.Users_LoginId(data.Value); break;
                        case "Users_Disabled": orderBy.Users_Disabled(data.Value); break;
                        case "Users_UserCode": orderBy.Users_UserCode(data.Value); break;
                        case "Users_Password": orderBy.Users_Password(data.Value); break;
                        case "Users_LastName": orderBy.Users_LastName(data.Value); break;
                        case "Users_FirstName": orderBy.Users_FirstName(data.Value); break;
                        case "Users_FullName1": orderBy.Users_FullName1(data.Value); break;
                        case "Users_FullName2": orderBy.Users_FullName2(data.Value); break;
                        case "Users_Birthday": orderBy.Users_Birthday(data.Value); break;
                        case "Users_Gender": orderBy.Users_Gender(data.Value); break;
                        case "Users_Language": orderBy.Users_Language(data.Value); break;
                        case "Users_TimeZone": orderBy.Users_TimeZone(data.Value); break;
                        case "Users_DeptId": orderBy.Users_DeptId(data.Value); break;
                        case "Users_FirstAndLastNameOrder": orderBy.Users_FirstAndLastNameOrder(data.Value); break;
                        case "Users_LastLoginTime": orderBy.Users_LastLoginTime(data.Value); break;
                        case "Users_PasswordExpirationTime": orderBy.Users_PasswordExpirationTime(data.Value); break;
                        case "Users_PasswordChangeTime": orderBy.Users_PasswordChangeTime(data.Value); break;
                        case "Users_NumberOfLogins": orderBy.Users_NumberOfLogins(data.Value); break;
                        case "Users_NumberOfDenial": orderBy.Users_NumberOfDenial(data.Value); break;
                        case "Users_TenantManager": orderBy.Users_TenantManager(data.Value); break;
                        case "Users_ServiceManager": orderBy.Users_ServiceManager(data.Value); break;
                        case "Users_Developer": orderBy.Users_Developer(data.Value); break;
                        case "Users_Comments": orderBy.Users_Comments(data.Value); break;
                        case "Users_Creator": orderBy.Users_Creator(data.Value); break;
                        case "Users_Updator": orderBy.Users_Updator(data.Value); break;
                        case "Users_CreatedTime": orderBy.Users_CreatedTime(data.Value); break;
                        case "Users_UpdatedTime": orderBy.Users_UpdatedTime(data.Value); break;                        case "Sites_TenantId": orderBy.Sites_TenantId(data.Value); break;
                        case "Sites_SiteId": orderBy.Sites_SiteId(data.Value); break;
                        case "Sites_UpdatedTime": orderBy.Sites_UpdatedTime(data.Value); break;
                        case "Sites_Ver": orderBy.Sites_Ver(data.Value); break;
                        case "Sites_Title": orderBy.Sites_Title(data.Value); break;
                        case "Sites_Body": orderBy.Sites_Body(data.Value); break;
                        case "Sites_TitleBody": orderBy.Sites_TitleBody(data.Value); break;
                        case "Sites_ReferenceType": orderBy.Sites_ReferenceType(data.Value); break;
                        case "Sites_ParentId": orderBy.Sites_ParentId(data.Value); break;
                        case "Sites_InheritPermission": orderBy.Sites_InheritPermission(data.Value); break;
                        case "Sites_PermissionType": orderBy.Sites_PermissionType(data.Value); break;
                        case "Sites_SiteSettings": orderBy.Sites_SiteSettings(data.Value); break;
                        case "Sites_Comments": orderBy.Sites_Comments(data.Value); break;
                        case "Sites_Creator": orderBy.Sites_Creator(data.Value); break;
                        case "Sites_Updator": orderBy.Sites_Updator(data.Value); break;
                        case "Sites_CreatedTime": orderBy.Sites_CreatedTime(data.Value); break;                        case "Issues_SiteId": orderBy.Issues_SiteId(data.Value); break;
                        case "Issues_UpdatedTime": orderBy.Issues_UpdatedTime(data.Value); break;
                        case "Issues_IssueId": orderBy.Issues_IssueId(data.Value); break;
                        case "Issues_Ver": orderBy.Issues_Ver(data.Value); break;
                        case "Issues_Title": orderBy.Issues_Title(data.Value); break;
                        case "Issues_Body": orderBy.Issues_Body(data.Value); break;
                        case "Issues_TitleBody": orderBy.Issues_TitleBody(data.Value); break;
                        case "Issues_StartTime": orderBy.Issues_StartTime(data.Value); break;
                        case "Issues_CompletionTime": orderBy.Issues_CompletionTime(data.Value); break;
                        case "Issues_WorkValue": orderBy.Issues_WorkValue(data.Value); break;
                        case "Issues_ProgressRate": orderBy.Issues_ProgressRate(data.Value); break;
                        case "Issues_RemainingWorkValue": orderBy.Issues_RemainingWorkValue(data.Value); break;
                        case "Issues_Status": orderBy.Issues_Status(data.Value); break;
                        case "Issues_Manager": orderBy.Issues_Manager(data.Value); break;
                        case "Issues_Owner": orderBy.Issues_Owner(data.Value); break;
                        case "Issues_ClassA": orderBy.Issues_ClassA(data.Value); break;
                        case "Issues_ClassB": orderBy.Issues_ClassB(data.Value); break;
                        case "Issues_ClassC": orderBy.Issues_ClassC(data.Value); break;
                        case "Issues_ClassD": orderBy.Issues_ClassD(data.Value); break;
                        case "Issues_ClassE": orderBy.Issues_ClassE(data.Value); break;
                        case "Issues_ClassF": orderBy.Issues_ClassF(data.Value); break;
                        case "Issues_ClassG": orderBy.Issues_ClassG(data.Value); break;
                        case "Issues_ClassH": orderBy.Issues_ClassH(data.Value); break;
                        case "Issues_ClassI": orderBy.Issues_ClassI(data.Value); break;
                        case "Issues_ClassJ": orderBy.Issues_ClassJ(data.Value); break;
                        case "Issues_ClassK": orderBy.Issues_ClassK(data.Value); break;
                        case "Issues_ClassL": orderBy.Issues_ClassL(data.Value); break;
                        case "Issues_ClassM": orderBy.Issues_ClassM(data.Value); break;
                        case "Issues_ClassN": orderBy.Issues_ClassN(data.Value); break;
                        case "Issues_ClassO": orderBy.Issues_ClassO(data.Value); break;
                        case "Issues_ClassP": orderBy.Issues_ClassP(data.Value); break;
                        case "Issues_ClassQ": orderBy.Issues_ClassQ(data.Value); break;
                        case "Issues_ClassR": orderBy.Issues_ClassR(data.Value); break;
                        case "Issues_ClassS": orderBy.Issues_ClassS(data.Value); break;
                        case "Issues_ClassT": orderBy.Issues_ClassT(data.Value); break;
                        case "Issues_ClassU": orderBy.Issues_ClassU(data.Value); break;
                        case "Issues_ClassV": orderBy.Issues_ClassV(data.Value); break;
                        case "Issues_ClassW": orderBy.Issues_ClassW(data.Value); break;
                        case "Issues_ClassX": orderBy.Issues_ClassX(data.Value); break;
                        case "Issues_ClassY": orderBy.Issues_ClassY(data.Value); break;
                        case "Issues_ClassZ": orderBy.Issues_ClassZ(data.Value); break;
                        case "Issues_NumA": orderBy.Issues_NumA(data.Value); break;
                        case "Issues_NumB": orderBy.Issues_NumB(data.Value); break;
                        case "Issues_NumC": orderBy.Issues_NumC(data.Value); break;
                        case "Issues_NumD": orderBy.Issues_NumD(data.Value); break;
                        case "Issues_NumE": orderBy.Issues_NumE(data.Value); break;
                        case "Issues_NumF": orderBy.Issues_NumF(data.Value); break;
                        case "Issues_NumG": orderBy.Issues_NumG(data.Value); break;
                        case "Issues_NumH": orderBy.Issues_NumH(data.Value); break;
                        case "Issues_NumI": orderBy.Issues_NumI(data.Value); break;
                        case "Issues_NumJ": orderBy.Issues_NumJ(data.Value); break;
                        case "Issues_NumK": orderBy.Issues_NumK(data.Value); break;
                        case "Issues_NumL": orderBy.Issues_NumL(data.Value); break;
                        case "Issues_NumM": orderBy.Issues_NumM(data.Value); break;
                        case "Issues_NumN": orderBy.Issues_NumN(data.Value); break;
                        case "Issues_NumO": orderBy.Issues_NumO(data.Value); break;
                        case "Issues_NumP": orderBy.Issues_NumP(data.Value); break;
                        case "Issues_NumQ": orderBy.Issues_NumQ(data.Value); break;
                        case "Issues_NumR": orderBy.Issues_NumR(data.Value); break;
                        case "Issues_NumS": orderBy.Issues_NumS(data.Value); break;
                        case "Issues_NumT": orderBy.Issues_NumT(data.Value); break;
                        case "Issues_NumU": orderBy.Issues_NumU(data.Value); break;
                        case "Issues_NumV": orderBy.Issues_NumV(data.Value); break;
                        case "Issues_NumW": orderBy.Issues_NumW(data.Value); break;
                        case "Issues_NumX": orderBy.Issues_NumX(data.Value); break;
                        case "Issues_NumY": orderBy.Issues_NumY(data.Value); break;
                        case "Issues_NumZ": orderBy.Issues_NumZ(data.Value); break;
                        case "Issues_DateA": orderBy.Issues_DateA(data.Value); break;
                        case "Issues_DateB": orderBy.Issues_DateB(data.Value); break;
                        case "Issues_DateC": orderBy.Issues_DateC(data.Value); break;
                        case "Issues_DateD": orderBy.Issues_DateD(data.Value); break;
                        case "Issues_DateE": orderBy.Issues_DateE(data.Value); break;
                        case "Issues_DateF": orderBy.Issues_DateF(data.Value); break;
                        case "Issues_DateG": orderBy.Issues_DateG(data.Value); break;
                        case "Issues_DateH": orderBy.Issues_DateH(data.Value); break;
                        case "Issues_DateI": orderBy.Issues_DateI(data.Value); break;
                        case "Issues_DateJ": orderBy.Issues_DateJ(data.Value); break;
                        case "Issues_DateK": orderBy.Issues_DateK(data.Value); break;
                        case "Issues_DateL": orderBy.Issues_DateL(data.Value); break;
                        case "Issues_DateM": orderBy.Issues_DateM(data.Value); break;
                        case "Issues_DateN": orderBy.Issues_DateN(data.Value); break;
                        case "Issues_DateO": orderBy.Issues_DateO(data.Value); break;
                        case "Issues_DateP": orderBy.Issues_DateP(data.Value); break;
                        case "Issues_DateQ": orderBy.Issues_DateQ(data.Value); break;
                        case "Issues_DateR": orderBy.Issues_DateR(data.Value); break;
                        case "Issues_DateS": orderBy.Issues_DateS(data.Value); break;
                        case "Issues_DateT": orderBy.Issues_DateT(data.Value); break;
                        case "Issues_DateU": orderBy.Issues_DateU(data.Value); break;
                        case "Issues_DateV": orderBy.Issues_DateV(data.Value); break;
                        case "Issues_DateW": orderBy.Issues_DateW(data.Value); break;
                        case "Issues_DateX": orderBy.Issues_DateX(data.Value); break;
                        case "Issues_DateY": orderBy.Issues_DateY(data.Value); break;
                        case "Issues_DateZ": orderBy.Issues_DateZ(data.Value); break;
                        case "Issues_DescriptionA": orderBy.Issues_DescriptionA(data.Value); break;
                        case "Issues_DescriptionB": orderBy.Issues_DescriptionB(data.Value); break;
                        case "Issues_DescriptionC": orderBy.Issues_DescriptionC(data.Value); break;
                        case "Issues_DescriptionD": orderBy.Issues_DescriptionD(data.Value); break;
                        case "Issues_DescriptionE": orderBy.Issues_DescriptionE(data.Value); break;
                        case "Issues_DescriptionF": orderBy.Issues_DescriptionF(data.Value); break;
                        case "Issues_DescriptionG": orderBy.Issues_DescriptionG(data.Value); break;
                        case "Issues_DescriptionH": orderBy.Issues_DescriptionH(data.Value); break;
                        case "Issues_DescriptionI": orderBy.Issues_DescriptionI(data.Value); break;
                        case "Issues_DescriptionJ": orderBy.Issues_DescriptionJ(data.Value); break;
                        case "Issues_DescriptionK": orderBy.Issues_DescriptionK(data.Value); break;
                        case "Issues_DescriptionL": orderBy.Issues_DescriptionL(data.Value); break;
                        case "Issues_DescriptionM": orderBy.Issues_DescriptionM(data.Value); break;
                        case "Issues_DescriptionN": orderBy.Issues_DescriptionN(data.Value); break;
                        case "Issues_DescriptionO": orderBy.Issues_DescriptionO(data.Value); break;
                        case "Issues_DescriptionP": orderBy.Issues_DescriptionP(data.Value); break;
                        case "Issues_DescriptionQ": orderBy.Issues_DescriptionQ(data.Value); break;
                        case "Issues_DescriptionR": orderBy.Issues_DescriptionR(data.Value); break;
                        case "Issues_DescriptionS": orderBy.Issues_DescriptionS(data.Value); break;
                        case "Issues_DescriptionT": orderBy.Issues_DescriptionT(data.Value); break;
                        case "Issues_DescriptionU": orderBy.Issues_DescriptionU(data.Value); break;
                        case "Issues_DescriptionV": orderBy.Issues_DescriptionV(data.Value); break;
                        case "Issues_DescriptionW": orderBy.Issues_DescriptionW(data.Value); break;
                        case "Issues_DescriptionX": orderBy.Issues_DescriptionX(data.Value); break;
                        case "Issues_DescriptionY": orderBy.Issues_DescriptionY(data.Value); break;
                        case "Issues_DescriptionZ": orderBy.Issues_DescriptionZ(data.Value); break;
                        case "Issues_CheckA": orderBy.Issues_CheckA(data.Value); break;
                        case "Issues_CheckB": orderBy.Issues_CheckB(data.Value); break;
                        case "Issues_CheckC": orderBy.Issues_CheckC(data.Value); break;
                        case "Issues_CheckD": orderBy.Issues_CheckD(data.Value); break;
                        case "Issues_CheckE": orderBy.Issues_CheckE(data.Value); break;
                        case "Issues_CheckF": orderBy.Issues_CheckF(data.Value); break;
                        case "Issues_CheckG": orderBy.Issues_CheckG(data.Value); break;
                        case "Issues_CheckH": orderBy.Issues_CheckH(data.Value); break;
                        case "Issues_CheckI": orderBy.Issues_CheckI(data.Value); break;
                        case "Issues_CheckJ": orderBy.Issues_CheckJ(data.Value); break;
                        case "Issues_CheckK": orderBy.Issues_CheckK(data.Value); break;
                        case "Issues_CheckL": orderBy.Issues_CheckL(data.Value); break;
                        case "Issues_CheckM": orderBy.Issues_CheckM(data.Value); break;
                        case "Issues_CheckN": orderBy.Issues_CheckN(data.Value); break;
                        case "Issues_CheckO": orderBy.Issues_CheckO(data.Value); break;
                        case "Issues_CheckP": orderBy.Issues_CheckP(data.Value); break;
                        case "Issues_CheckQ": orderBy.Issues_CheckQ(data.Value); break;
                        case "Issues_CheckR": orderBy.Issues_CheckR(data.Value); break;
                        case "Issues_CheckS": orderBy.Issues_CheckS(data.Value); break;
                        case "Issues_CheckT": orderBy.Issues_CheckT(data.Value); break;
                        case "Issues_CheckU": orderBy.Issues_CheckU(data.Value); break;
                        case "Issues_CheckV": orderBy.Issues_CheckV(data.Value); break;
                        case "Issues_CheckW": orderBy.Issues_CheckW(data.Value); break;
                        case "Issues_CheckX": orderBy.Issues_CheckX(data.Value); break;
                        case "Issues_CheckY": orderBy.Issues_CheckY(data.Value); break;
                        case "Issues_CheckZ": orderBy.Issues_CheckZ(data.Value); break;
                        case "Issues_Comments": orderBy.Issues_Comments(data.Value); break;
                        case "Issues_Creator": orderBy.Issues_Creator(data.Value); break;
                        case "Issues_Updator": orderBy.Issues_Updator(data.Value); break;
                        case "Issues_CreatedTime": orderBy.Issues_CreatedTime(data.Value); break;                        case "Results_SiteId": orderBy.Results_SiteId(data.Value); break;
                        case "Results_UpdatedTime": orderBy.Results_UpdatedTime(data.Value); break;
                        case "Results_ResultId": orderBy.Results_ResultId(data.Value); break;
                        case "Results_Ver": orderBy.Results_Ver(data.Value); break;
                        case "Results_Title": orderBy.Results_Title(data.Value); break;
                        case "Results_Body": orderBy.Results_Body(data.Value); break;
                        case "Results_TitleBody": orderBy.Results_TitleBody(data.Value); break;
                        case "Results_Status": orderBy.Results_Status(data.Value); break;
                        case "Results_Manager": orderBy.Results_Manager(data.Value); break;
                        case "Results_Owner": orderBy.Results_Owner(data.Value); break;
                        case "Results_ClassA": orderBy.Results_ClassA(data.Value); break;
                        case "Results_ClassB": orderBy.Results_ClassB(data.Value); break;
                        case "Results_ClassC": orderBy.Results_ClassC(data.Value); break;
                        case "Results_ClassD": orderBy.Results_ClassD(data.Value); break;
                        case "Results_ClassE": orderBy.Results_ClassE(data.Value); break;
                        case "Results_ClassF": orderBy.Results_ClassF(data.Value); break;
                        case "Results_ClassG": orderBy.Results_ClassG(data.Value); break;
                        case "Results_ClassH": orderBy.Results_ClassH(data.Value); break;
                        case "Results_ClassI": orderBy.Results_ClassI(data.Value); break;
                        case "Results_ClassJ": orderBy.Results_ClassJ(data.Value); break;
                        case "Results_ClassK": orderBy.Results_ClassK(data.Value); break;
                        case "Results_ClassL": orderBy.Results_ClassL(data.Value); break;
                        case "Results_ClassM": orderBy.Results_ClassM(data.Value); break;
                        case "Results_ClassN": orderBy.Results_ClassN(data.Value); break;
                        case "Results_ClassO": orderBy.Results_ClassO(data.Value); break;
                        case "Results_ClassP": orderBy.Results_ClassP(data.Value); break;
                        case "Results_ClassQ": orderBy.Results_ClassQ(data.Value); break;
                        case "Results_ClassR": orderBy.Results_ClassR(data.Value); break;
                        case "Results_ClassS": orderBy.Results_ClassS(data.Value); break;
                        case "Results_ClassT": orderBy.Results_ClassT(data.Value); break;
                        case "Results_ClassU": orderBy.Results_ClassU(data.Value); break;
                        case "Results_ClassV": orderBy.Results_ClassV(data.Value); break;
                        case "Results_ClassW": orderBy.Results_ClassW(data.Value); break;
                        case "Results_ClassX": orderBy.Results_ClassX(data.Value); break;
                        case "Results_ClassY": orderBy.Results_ClassY(data.Value); break;
                        case "Results_ClassZ": orderBy.Results_ClassZ(data.Value); break;
                        case "Results_NumA": orderBy.Results_NumA(data.Value); break;
                        case "Results_NumB": orderBy.Results_NumB(data.Value); break;
                        case "Results_NumC": orderBy.Results_NumC(data.Value); break;
                        case "Results_NumD": orderBy.Results_NumD(data.Value); break;
                        case "Results_NumE": orderBy.Results_NumE(data.Value); break;
                        case "Results_NumF": orderBy.Results_NumF(data.Value); break;
                        case "Results_NumG": orderBy.Results_NumG(data.Value); break;
                        case "Results_NumH": orderBy.Results_NumH(data.Value); break;
                        case "Results_NumI": orderBy.Results_NumI(data.Value); break;
                        case "Results_NumJ": orderBy.Results_NumJ(data.Value); break;
                        case "Results_NumK": orderBy.Results_NumK(data.Value); break;
                        case "Results_NumL": orderBy.Results_NumL(data.Value); break;
                        case "Results_NumM": orderBy.Results_NumM(data.Value); break;
                        case "Results_NumN": orderBy.Results_NumN(data.Value); break;
                        case "Results_NumO": orderBy.Results_NumO(data.Value); break;
                        case "Results_NumP": orderBy.Results_NumP(data.Value); break;
                        case "Results_NumQ": orderBy.Results_NumQ(data.Value); break;
                        case "Results_NumR": orderBy.Results_NumR(data.Value); break;
                        case "Results_NumS": orderBy.Results_NumS(data.Value); break;
                        case "Results_NumT": orderBy.Results_NumT(data.Value); break;
                        case "Results_NumU": orderBy.Results_NumU(data.Value); break;
                        case "Results_NumV": orderBy.Results_NumV(data.Value); break;
                        case "Results_NumW": orderBy.Results_NumW(data.Value); break;
                        case "Results_NumX": orderBy.Results_NumX(data.Value); break;
                        case "Results_NumY": orderBy.Results_NumY(data.Value); break;
                        case "Results_NumZ": orderBy.Results_NumZ(data.Value); break;
                        case "Results_DateA": orderBy.Results_DateA(data.Value); break;
                        case "Results_DateB": orderBy.Results_DateB(data.Value); break;
                        case "Results_DateC": orderBy.Results_DateC(data.Value); break;
                        case "Results_DateD": orderBy.Results_DateD(data.Value); break;
                        case "Results_DateE": orderBy.Results_DateE(data.Value); break;
                        case "Results_DateF": orderBy.Results_DateF(data.Value); break;
                        case "Results_DateG": orderBy.Results_DateG(data.Value); break;
                        case "Results_DateH": orderBy.Results_DateH(data.Value); break;
                        case "Results_DateI": orderBy.Results_DateI(data.Value); break;
                        case "Results_DateJ": orderBy.Results_DateJ(data.Value); break;
                        case "Results_DateK": orderBy.Results_DateK(data.Value); break;
                        case "Results_DateL": orderBy.Results_DateL(data.Value); break;
                        case "Results_DateM": orderBy.Results_DateM(data.Value); break;
                        case "Results_DateN": orderBy.Results_DateN(data.Value); break;
                        case "Results_DateO": orderBy.Results_DateO(data.Value); break;
                        case "Results_DateP": orderBy.Results_DateP(data.Value); break;
                        case "Results_DateQ": orderBy.Results_DateQ(data.Value); break;
                        case "Results_DateR": orderBy.Results_DateR(data.Value); break;
                        case "Results_DateS": orderBy.Results_DateS(data.Value); break;
                        case "Results_DateT": orderBy.Results_DateT(data.Value); break;
                        case "Results_DateU": orderBy.Results_DateU(data.Value); break;
                        case "Results_DateV": orderBy.Results_DateV(data.Value); break;
                        case "Results_DateW": orderBy.Results_DateW(data.Value); break;
                        case "Results_DateX": orderBy.Results_DateX(data.Value); break;
                        case "Results_DateY": orderBy.Results_DateY(data.Value); break;
                        case "Results_DateZ": orderBy.Results_DateZ(data.Value); break;
                        case "Results_DescriptionA": orderBy.Results_DescriptionA(data.Value); break;
                        case "Results_DescriptionB": orderBy.Results_DescriptionB(data.Value); break;
                        case "Results_DescriptionC": orderBy.Results_DescriptionC(data.Value); break;
                        case "Results_DescriptionD": orderBy.Results_DescriptionD(data.Value); break;
                        case "Results_DescriptionE": orderBy.Results_DescriptionE(data.Value); break;
                        case "Results_DescriptionF": orderBy.Results_DescriptionF(data.Value); break;
                        case "Results_DescriptionG": orderBy.Results_DescriptionG(data.Value); break;
                        case "Results_DescriptionH": orderBy.Results_DescriptionH(data.Value); break;
                        case "Results_DescriptionI": orderBy.Results_DescriptionI(data.Value); break;
                        case "Results_DescriptionJ": orderBy.Results_DescriptionJ(data.Value); break;
                        case "Results_DescriptionK": orderBy.Results_DescriptionK(data.Value); break;
                        case "Results_DescriptionL": orderBy.Results_DescriptionL(data.Value); break;
                        case "Results_DescriptionM": orderBy.Results_DescriptionM(data.Value); break;
                        case "Results_DescriptionN": orderBy.Results_DescriptionN(data.Value); break;
                        case "Results_DescriptionO": orderBy.Results_DescriptionO(data.Value); break;
                        case "Results_DescriptionP": orderBy.Results_DescriptionP(data.Value); break;
                        case "Results_DescriptionQ": orderBy.Results_DescriptionQ(data.Value); break;
                        case "Results_DescriptionR": orderBy.Results_DescriptionR(data.Value); break;
                        case "Results_DescriptionS": orderBy.Results_DescriptionS(data.Value); break;
                        case "Results_DescriptionT": orderBy.Results_DescriptionT(data.Value); break;
                        case "Results_DescriptionU": orderBy.Results_DescriptionU(data.Value); break;
                        case "Results_DescriptionV": orderBy.Results_DescriptionV(data.Value); break;
                        case "Results_DescriptionW": orderBy.Results_DescriptionW(data.Value); break;
                        case "Results_DescriptionX": orderBy.Results_DescriptionX(data.Value); break;
                        case "Results_DescriptionY": orderBy.Results_DescriptionY(data.Value); break;
                        case "Results_DescriptionZ": orderBy.Results_DescriptionZ(data.Value); break;
                        case "Results_CheckA": orderBy.Results_CheckA(data.Value); break;
                        case "Results_CheckB": orderBy.Results_CheckB(data.Value); break;
                        case "Results_CheckC": orderBy.Results_CheckC(data.Value); break;
                        case "Results_CheckD": orderBy.Results_CheckD(data.Value); break;
                        case "Results_CheckE": orderBy.Results_CheckE(data.Value); break;
                        case "Results_CheckF": orderBy.Results_CheckF(data.Value); break;
                        case "Results_CheckG": orderBy.Results_CheckG(data.Value); break;
                        case "Results_CheckH": orderBy.Results_CheckH(data.Value); break;
                        case "Results_CheckI": orderBy.Results_CheckI(data.Value); break;
                        case "Results_CheckJ": orderBy.Results_CheckJ(data.Value); break;
                        case "Results_CheckK": orderBy.Results_CheckK(data.Value); break;
                        case "Results_CheckL": orderBy.Results_CheckL(data.Value); break;
                        case "Results_CheckM": orderBy.Results_CheckM(data.Value); break;
                        case "Results_CheckN": orderBy.Results_CheckN(data.Value); break;
                        case "Results_CheckO": orderBy.Results_CheckO(data.Value); break;
                        case "Results_CheckP": orderBy.Results_CheckP(data.Value); break;
                        case "Results_CheckQ": orderBy.Results_CheckQ(data.Value); break;
                        case "Results_CheckR": orderBy.Results_CheckR(data.Value); break;
                        case "Results_CheckS": orderBy.Results_CheckS(data.Value); break;
                        case "Results_CheckT": orderBy.Results_CheckT(data.Value); break;
                        case "Results_CheckU": orderBy.Results_CheckU(data.Value); break;
                        case "Results_CheckV": orderBy.Results_CheckV(data.Value); break;
                        case "Results_CheckW": orderBy.Results_CheckW(data.Value); break;
                        case "Results_CheckX": orderBy.Results_CheckX(data.Value); break;
                        case "Results_CheckY": orderBy.Results_CheckY(data.Value); break;
                        case "Results_CheckZ": orderBy.Results_CheckZ(data.Value); break;
                        case "Results_Comments": orderBy.Results_Comments(data.Value); break;
                        case "Results_Creator": orderBy.Results_Creator(data.Value); break;
                        case "Results_Updator": orderBy.Results_Updator(data.Value); break;
                        case "Results_CreatedTime": orderBy.Results_CreatedTime(data.Value); break;                        case "Wikis_SiteId": orderBy.Wikis_SiteId(data.Value); break;
                        case "Wikis_UpdatedTime": orderBy.Wikis_UpdatedTime(data.Value); break;
                        case "Wikis_WikiId": orderBy.Wikis_WikiId(data.Value); break;
                        case "Wikis_Ver": orderBy.Wikis_Ver(data.Value); break;
                        case "Wikis_Title": orderBy.Wikis_Title(data.Value); break;
                        case "Wikis_Body": orderBy.Wikis_Body(data.Value); break;
                        case "Wikis_TitleBody": orderBy.Wikis_TitleBody(data.Value); break;
                        case "Wikis_Comments": orderBy.Wikis_Comments(data.Value); break;
                        case "Wikis_Creator": orderBy.Wikis_Creator(data.Value); break;
                        case "Wikis_Updator": orderBy.Wikis_Updator(data.Value); break;
                        case "Wikis_CreatedTime": orderBy.Wikis_CreatedTime(data.Value); break;
                    }
                });
            }
            return orderBy;
        }
    }
}
