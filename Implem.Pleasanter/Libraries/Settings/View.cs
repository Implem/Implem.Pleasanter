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
        public string KambanGroupBy;
        public string KambanAggregateType;
        public string KambanValue;

        public View()
        {
        }

        public View(SiteSettings ss)
        {
            SetByForm(ss);
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
                    case "KambanGroupBy":
                        KambanGroupBy = String(controlId);
                        break;
                    case "KambanAggregateType":
                        KambanAggregateType = String(controlId);
                        break;
                    case "KambanValue":
                        KambanValue = String(controlId);
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
            SetRecordingData();
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
            if (ss.ColumnCollection.Any(o => o.ColumnName == columnName))
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
            if (ss.ColumnCollection.Any(o => o.ColumnName == columnName))
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
            Forms.Data("ViewSorters").Split(';').ForEach(data =>
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

        private void SetRecordingData()
        {
            if (!Incomplete.ToBool()) Incomplete = null;
            if (!Own.ToBool()) Own = null;
            if (!NearCompletionTime.ToBool()) NearCompletionTime = null;
            if (!Delay.ToBool()) Delay = null;
            if (!Overdue.ToBool()) Overdue = null;
            if (!ColumnFilterHash?.Any() == true) ColumnFilterHash = null;
            if (!ColumnSorterHash?.Any() == true) ColumnSorterHash = null;
            if (Search == string.Empty) Search = null;
            if (GanttGroupBy == string.Empty) GanttGroupBy = null;
            if (TimeSeriesGroupBy == string.Empty) TimeSeriesGroupBy = null;
            if (TimeSeriesAggregateType == string.Empty) TimeSeriesAggregateType = null;
            if (TimeSeriesValue == string.Empty) TimeSeriesValue = null;
            if (KambanGroupBy == string.Empty) KambanGroupBy = null;
            if (KambanAggregateType == string.Empty) KambanAggregateType = null;
            if (KambanValue == string.Empty) KambanValue = null;
        }

        public SqlWhereCollection Where(SiteSettings ss, SqlWhereCollection where)
        {
            GeneralsWhere(ss, where);
            ColumnsWhere(ss, where);
            SearchWhere(ss, where);
            return where;
        }

        private SqlWhereCollection GeneralsWhere(SiteSettings ss, SqlWhereCollection where)
        {
            var tableBracket = "[" + ss.ReferenceType + "]";
            if (Incomplete == true)
            {
                where.Add(
                    columnBrackets: new string[] { "[t0].[Status]" },
                    name: "_U",
                    _operator: "<{0}".Params(Parameters.General.CompletionCode));
            }
            if (Own == true)
            {
                where.Add(
                    columnBrackets: new string[] { "[t0].[Manager]", "[t0].[Owner]" },
                    name: "_U",
                    value: Sessions.UserId());
            }
            if (NearCompletionTime == true)
            {
                where.Add(
                    columnBrackets: new string[] { "[t0].[CompletionTime]" },
                    _operator: " between '{0}' and '{1}'".Params(
                        DateTime.Now.ToLocal().Date
                            .AddDays(ss.NearCompletionTimeBeforeDays.ToInt() * (-1)),
                        DateTime.Now.ToLocal().Date
                            .AddDays(ss.NearCompletionTimeAfterDays.ToInt() + 1)
                            .AddMilliseconds(-1)
                            .ToString("yyyy/M/d H:m:s.fff")));
            }
            if (Delay == true)
            {
                where
                    .Add(
                        columnBrackets: new string[] { "[t0].[Status]" },
                        name: "_U",
                        _operator: "<{0}".Params(Parameters.General.CompletionCode))
                    .Add(
                        columnBrackets: new string[] { "[t0].[ProgressRate]" },
                        _operator: "<",
                        raw: Def.Sql.ProgressRateDelay
                            .Replace("#TableName#", ss.ReferenceType));
            }
            if (Overdue == true)
            {
                where
                    .Add(
                        columnBrackets: new string[] { "[t0].[Status]" },
                        name: "_U",
                        _operator: "<{0}".Params(Parameters.General.CompletionCode))
                    .Add(
                        columnBrackets: new string[] { "[t0].[CompletionTime]" },
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
                            CsBoolColumns(data.ColumnName, data.Value, where);
                            break;
                        case Types.CsNumeric:
                            if (data.Value.RegexExists("[0-9]*,[0-9]*"))
                            {
                                CsNumericRangeColumns(
                                    data.Column, data.ColumnName, data.Value, where);
                            }
                            else
                            {
                                CsNumericColumns(
                                    data.Column, data.ColumnName, data.Value, where);
                            }
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

        private void CsBoolColumns(string columnName, string value, SqlWhereCollection where)
        {
            if (value.ToBool())
            {
                where.Add(raw: "[t0].[{0}] = 1".Params(columnName));
            }
        }

        private void CsNumericColumns(
            Column column, string columnName, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
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
                    columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                    name: columnName,
                    _operator: " in ({0})".Params(param
                        .Where(o => o != "\t")
                        .Select(o => o.ToLong())
                        .Join()))
                : null;
        }

        private SqlWhere CsNumericColumnsWhereNull(
            Column column, string columnName, List<string> param)
        {
            return param.Any(o => o == "\t")
                ? new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: " is null"),
                    new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: "={0}".Params(column.UserColumn
                            ? User.UserTypes.Anonymous.ToInt()
                            : 0))))
                : null;
        }

        private void CsNumericRangeColumns(
            Column column, string columnName, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
            var parts = new SqlWhereCollection();
            param.ForEach(data =>
            {
                var from = data.Split_1st();
                var to = data.Split_2nd();
                if (from == string.Empty)
                {
                    parts.Add(new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: " <{0}".Params(to)));
                }
                else if (to == string.Empty)
                {
                    parts.Add(new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: " >={0}".Params(from)));
                }
                else
                {
                    parts.Add(new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: " between {0} and {1}".Params(from, to)));
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
                    "[t0].[{0}] between '{1}' and '{2}'".Params(
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
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: " is null"),
                    new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: " not between '{0}' and '{1}'".Params(
                            Parameters.General.MinTime.ToUniversal()
                                .ToString("yyyy/M/d H:m:s"),
                            Parameters.General.MaxTime.ToUniversal()
                                .ToString("yyyy/M/d H:m:s")))))
                : null;
        }

        private void CsStringColumns(
            string columnName, string value, SqlWhereCollection where)
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
                    columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
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
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: " is null"),
                    new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
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
                    var type = data.Value;
                    switch (ss.ReferenceType + "_" + data.Key)
                    {
                        case "Depts_TenantId": orderBy.Depts_TenantId(type); break;
                        case "Depts_DeptId": orderBy.Depts_DeptId(type); break;
                        case "Depts_Ver": orderBy.Depts_Ver(type); break;
                        case "Depts_DeptCode": orderBy.Depts_DeptCode(type); break;
                        case "Depts_Dept": orderBy.Depts_Dept(type); break;
                        case "Depts_DeptName": orderBy.Depts_DeptName(type); break;
                        case "Depts_Body": orderBy.Depts_Body(type); break;
                        case "Depts_Comments": orderBy.Depts_Comments(type); break;
                        case "Depts_Creator": orderBy.Depts_Creator(type); break;
                        case "Depts_Updator": orderBy.Depts_Updator(type); break;
                        case "Depts_CreatedTime": orderBy.Depts_CreatedTime(type); break;
                        case "Depts_UpdatedTime": orderBy.Depts_UpdatedTime(type); break;                        case "Users_TenantId": orderBy.Users_TenantId(type); break;
                        case "Users_UserId": orderBy.Users_UserId(type); break;
                        case "Users_Ver": orderBy.Users_Ver(type); break;
                        case "Users_LoginId": orderBy.Users_LoginId(type); break;
                        case "Users_Disabled": orderBy.Users_Disabled(type); break;
                        case "Users_UserCode": orderBy.Users_UserCode(type); break;
                        case "Users_Password": orderBy.Users_Password(type); break;
                        case "Users_LastName": orderBy.Users_LastName(type); break;
                        case "Users_FirstName": orderBy.Users_FirstName(type); break;
                        case "Users_Birthday": orderBy.Users_Birthday(type); break;
                        case "Users_Gender": orderBy.Users_Gender(type); break;
                        case "Users_Language": orderBy.Users_Language(type); break;
                        case "Users_TimeZone": orderBy.Users_TimeZone(type); break;
                        case "Users_DeptId": orderBy.Users_DeptId(type); break;
                        case "Users_Dept": orderBy.Users_Dept(type); break;
                        case "Users_FirstAndLastNameOrder": orderBy.Users_FirstAndLastNameOrder(type); break;
                        case "Users_LastLoginTime": orderBy.Users_LastLoginTime(type); break;
                        case "Users_PasswordExpirationTime": orderBy.Users_PasswordExpirationTime(type); break;
                        case "Users_PasswordChangeTime": orderBy.Users_PasswordChangeTime(type); break;
                        case "Users_NumberOfLogins": orderBy.Users_NumberOfLogins(type); break;
                        case "Users_NumberOfDenial": orderBy.Users_NumberOfDenial(type); break;
                        case "Users_TenantAdmin": orderBy.Users_TenantAdmin(type); break;
                        case "Users_ServiceAdmin": orderBy.Users_ServiceAdmin(type); break;
                        case "Users_Developer": orderBy.Users_Developer(type); break;
                        case "Users_Comments": orderBy.Users_Comments(type); break;
                        case "Users_Creator": orderBy.Users_Creator(type); break;
                        case "Users_Updator": orderBy.Users_Updator(type); break;
                        case "Users_CreatedTime": orderBy.Users_CreatedTime(type); break;
                        case "Users_UpdatedTime": orderBy.Users_UpdatedTime(type); break;                        case "Sites_TenantId": orderBy.Sites_TenantId(type); break;
                        case "Sites_SiteId": orderBy.Sites_SiteId(type); break;
                        case "Sites_UpdatedTime": orderBy.Sites_UpdatedTime(type); break;
                        case "Sites_Ver": orderBy.Sites_Ver(type); break;
                        case "Sites_Title": orderBy.Sites_Title(type); break;
                        case "Sites_Body": orderBy.Sites_Body(type); break;
                        case "Sites_TitleBody": orderBy.Sites_TitleBody(type); break;
                        case "Sites_ReferenceType": orderBy.Sites_ReferenceType(type); break;
                        case "Sites_ParentId": orderBy.Sites_ParentId(type); break;
                        case "Sites_InheritPermission": orderBy.Sites_InheritPermission(type); break;
                        case "Sites_PermissionType": orderBy.Sites_PermissionType(type); break;
                        case "Sites_SiteSettings": orderBy.Sites_SiteSettings(type); break;
                        case "Sites_Comments": orderBy.Sites_Comments(type); break;
                        case "Sites_Creator": orderBy.Sites_Creator(type); break;
                        case "Sites_Updator": orderBy.Sites_Updator(type); break;
                        case "Sites_CreatedTime": orderBy.Sites_CreatedTime(type); break;                        case "Issues_SiteId": orderBy.Issues_SiteId(type); break;
                        case "Issues_UpdatedTime": orderBy.Issues_UpdatedTime(type); break;
                        case "Issues_IssueId": orderBy.Issues_IssueId(type); break;
                        case "Issues_Ver": orderBy.Issues_Ver(type); break;
                        case "Issues_Title": orderBy.Issues_Title(type); break;
                        case "Issues_Body": orderBy.Issues_Body(type); break;
                        case "Issues_TitleBody": orderBy.Issues_TitleBody(type); break;
                        case "Issues_StartTime": orderBy.Issues_StartTime(type); break;
                        case "Issues_CompletionTime": orderBy.Issues_CompletionTime(type); break;
                        case "Issues_WorkValue": orderBy.Issues_WorkValue(type); break;
                        case "Issues_ProgressRate": orderBy.Issues_ProgressRate(type); break;
                        case "Issues_RemainingWorkValue": orderBy.Issues_RemainingWorkValue(type); break;
                        case "Issues_Status": orderBy.Issues_Status(type); break;
                        case "Issues_Manager": orderBy.Issues_Manager(type); break;
                        case "Issues_Owner": orderBy.Issues_Owner(type); break;
                        case "Issues_ClassA": orderBy.Issues_ClassA(type); break;
                        case "Issues_ClassB": orderBy.Issues_ClassB(type); break;
                        case "Issues_ClassC": orderBy.Issues_ClassC(type); break;
                        case "Issues_ClassD": orderBy.Issues_ClassD(type); break;
                        case "Issues_ClassE": orderBy.Issues_ClassE(type); break;
                        case "Issues_ClassF": orderBy.Issues_ClassF(type); break;
                        case "Issues_ClassG": orderBy.Issues_ClassG(type); break;
                        case "Issues_ClassH": orderBy.Issues_ClassH(type); break;
                        case "Issues_ClassI": orderBy.Issues_ClassI(type); break;
                        case "Issues_ClassJ": orderBy.Issues_ClassJ(type); break;
                        case "Issues_ClassK": orderBy.Issues_ClassK(type); break;
                        case "Issues_ClassL": orderBy.Issues_ClassL(type); break;
                        case "Issues_ClassM": orderBy.Issues_ClassM(type); break;
                        case "Issues_ClassN": orderBy.Issues_ClassN(type); break;
                        case "Issues_ClassO": orderBy.Issues_ClassO(type); break;
                        case "Issues_ClassP": orderBy.Issues_ClassP(type); break;
                        case "Issues_ClassQ": orderBy.Issues_ClassQ(type); break;
                        case "Issues_ClassR": orderBy.Issues_ClassR(type); break;
                        case "Issues_ClassS": orderBy.Issues_ClassS(type); break;
                        case "Issues_ClassT": orderBy.Issues_ClassT(type); break;
                        case "Issues_ClassU": orderBy.Issues_ClassU(type); break;
                        case "Issues_ClassV": orderBy.Issues_ClassV(type); break;
                        case "Issues_ClassW": orderBy.Issues_ClassW(type); break;
                        case "Issues_ClassX": orderBy.Issues_ClassX(type); break;
                        case "Issues_ClassY": orderBy.Issues_ClassY(type); break;
                        case "Issues_ClassZ": orderBy.Issues_ClassZ(type); break;
                        case "Issues_NumA": orderBy.Issues_NumA(type); break;
                        case "Issues_NumB": orderBy.Issues_NumB(type); break;
                        case "Issues_NumC": orderBy.Issues_NumC(type); break;
                        case "Issues_NumD": orderBy.Issues_NumD(type); break;
                        case "Issues_NumE": orderBy.Issues_NumE(type); break;
                        case "Issues_NumF": orderBy.Issues_NumF(type); break;
                        case "Issues_NumG": orderBy.Issues_NumG(type); break;
                        case "Issues_NumH": orderBy.Issues_NumH(type); break;
                        case "Issues_NumI": orderBy.Issues_NumI(type); break;
                        case "Issues_NumJ": orderBy.Issues_NumJ(type); break;
                        case "Issues_NumK": orderBy.Issues_NumK(type); break;
                        case "Issues_NumL": orderBy.Issues_NumL(type); break;
                        case "Issues_NumM": orderBy.Issues_NumM(type); break;
                        case "Issues_NumN": orderBy.Issues_NumN(type); break;
                        case "Issues_NumO": orderBy.Issues_NumO(type); break;
                        case "Issues_NumP": orderBy.Issues_NumP(type); break;
                        case "Issues_NumQ": orderBy.Issues_NumQ(type); break;
                        case "Issues_NumR": orderBy.Issues_NumR(type); break;
                        case "Issues_NumS": orderBy.Issues_NumS(type); break;
                        case "Issues_NumT": orderBy.Issues_NumT(type); break;
                        case "Issues_NumU": orderBy.Issues_NumU(type); break;
                        case "Issues_NumV": orderBy.Issues_NumV(type); break;
                        case "Issues_NumW": orderBy.Issues_NumW(type); break;
                        case "Issues_NumX": orderBy.Issues_NumX(type); break;
                        case "Issues_NumY": orderBy.Issues_NumY(type); break;
                        case "Issues_NumZ": orderBy.Issues_NumZ(type); break;
                        case "Issues_DateA": orderBy.Issues_DateA(type); break;
                        case "Issues_DateB": orderBy.Issues_DateB(type); break;
                        case "Issues_DateC": orderBy.Issues_DateC(type); break;
                        case "Issues_DateD": orderBy.Issues_DateD(type); break;
                        case "Issues_DateE": orderBy.Issues_DateE(type); break;
                        case "Issues_DateF": orderBy.Issues_DateF(type); break;
                        case "Issues_DateG": orderBy.Issues_DateG(type); break;
                        case "Issues_DateH": orderBy.Issues_DateH(type); break;
                        case "Issues_DateI": orderBy.Issues_DateI(type); break;
                        case "Issues_DateJ": orderBy.Issues_DateJ(type); break;
                        case "Issues_DateK": orderBy.Issues_DateK(type); break;
                        case "Issues_DateL": orderBy.Issues_DateL(type); break;
                        case "Issues_DateM": orderBy.Issues_DateM(type); break;
                        case "Issues_DateN": orderBy.Issues_DateN(type); break;
                        case "Issues_DateO": orderBy.Issues_DateO(type); break;
                        case "Issues_DateP": orderBy.Issues_DateP(type); break;
                        case "Issues_DateQ": orderBy.Issues_DateQ(type); break;
                        case "Issues_DateR": orderBy.Issues_DateR(type); break;
                        case "Issues_DateS": orderBy.Issues_DateS(type); break;
                        case "Issues_DateT": orderBy.Issues_DateT(type); break;
                        case "Issues_DateU": orderBy.Issues_DateU(type); break;
                        case "Issues_DateV": orderBy.Issues_DateV(type); break;
                        case "Issues_DateW": orderBy.Issues_DateW(type); break;
                        case "Issues_DateX": orderBy.Issues_DateX(type); break;
                        case "Issues_DateY": orderBy.Issues_DateY(type); break;
                        case "Issues_DateZ": orderBy.Issues_DateZ(type); break;
                        case "Issues_DescriptionA": orderBy.Issues_DescriptionA(type); break;
                        case "Issues_DescriptionB": orderBy.Issues_DescriptionB(type); break;
                        case "Issues_DescriptionC": orderBy.Issues_DescriptionC(type); break;
                        case "Issues_DescriptionD": orderBy.Issues_DescriptionD(type); break;
                        case "Issues_DescriptionE": orderBy.Issues_DescriptionE(type); break;
                        case "Issues_DescriptionF": orderBy.Issues_DescriptionF(type); break;
                        case "Issues_DescriptionG": orderBy.Issues_DescriptionG(type); break;
                        case "Issues_DescriptionH": orderBy.Issues_DescriptionH(type); break;
                        case "Issues_DescriptionI": orderBy.Issues_DescriptionI(type); break;
                        case "Issues_DescriptionJ": orderBy.Issues_DescriptionJ(type); break;
                        case "Issues_DescriptionK": orderBy.Issues_DescriptionK(type); break;
                        case "Issues_DescriptionL": orderBy.Issues_DescriptionL(type); break;
                        case "Issues_DescriptionM": orderBy.Issues_DescriptionM(type); break;
                        case "Issues_DescriptionN": orderBy.Issues_DescriptionN(type); break;
                        case "Issues_DescriptionO": orderBy.Issues_DescriptionO(type); break;
                        case "Issues_DescriptionP": orderBy.Issues_DescriptionP(type); break;
                        case "Issues_DescriptionQ": orderBy.Issues_DescriptionQ(type); break;
                        case "Issues_DescriptionR": orderBy.Issues_DescriptionR(type); break;
                        case "Issues_DescriptionS": orderBy.Issues_DescriptionS(type); break;
                        case "Issues_DescriptionT": orderBy.Issues_DescriptionT(type); break;
                        case "Issues_DescriptionU": orderBy.Issues_DescriptionU(type); break;
                        case "Issues_DescriptionV": orderBy.Issues_DescriptionV(type); break;
                        case "Issues_DescriptionW": orderBy.Issues_DescriptionW(type); break;
                        case "Issues_DescriptionX": orderBy.Issues_DescriptionX(type); break;
                        case "Issues_DescriptionY": orderBy.Issues_DescriptionY(type); break;
                        case "Issues_DescriptionZ": orderBy.Issues_DescriptionZ(type); break;
                        case "Issues_CheckA": orderBy.Issues_CheckA(type); break;
                        case "Issues_CheckB": orderBy.Issues_CheckB(type); break;
                        case "Issues_CheckC": orderBy.Issues_CheckC(type); break;
                        case "Issues_CheckD": orderBy.Issues_CheckD(type); break;
                        case "Issues_CheckE": orderBy.Issues_CheckE(type); break;
                        case "Issues_CheckF": orderBy.Issues_CheckF(type); break;
                        case "Issues_CheckG": orderBy.Issues_CheckG(type); break;
                        case "Issues_CheckH": orderBy.Issues_CheckH(type); break;
                        case "Issues_CheckI": orderBy.Issues_CheckI(type); break;
                        case "Issues_CheckJ": orderBy.Issues_CheckJ(type); break;
                        case "Issues_CheckK": orderBy.Issues_CheckK(type); break;
                        case "Issues_CheckL": orderBy.Issues_CheckL(type); break;
                        case "Issues_CheckM": orderBy.Issues_CheckM(type); break;
                        case "Issues_CheckN": orderBy.Issues_CheckN(type); break;
                        case "Issues_CheckO": orderBy.Issues_CheckO(type); break;
                        case "Issues_CheckP": orderBy.Issues_CheckP(type); break;
                        case "Issues_CheckQ": orderBy.Issues_CheckQ(type); break;
                        case "Issues_CheckR": orderBy.Issues_CheckR(type); break;
                        case "Issues_CheckS": orderBy.Issues_CheckS(type); break;
                        case "Issues_CheckT": orderBy.Issues_CheckT(type); break;
                        case "Issues_CheckU": orderBy.Issues_CheckU(type); break;
                        case "Issues_CheckV": orderBy.Issues_CheckV(type); break;
                        case "Issues_CheckW": orderBy.Issues_CheckW(type); break;
                        case "Issues_CheckX": orderBy.Issues_CheckX(type); break;
                        case "Issues_CheckY": orderBy.Issues_CheckY(type); break;
                        case "Issues_CheckZ": orderBy.Issues_CheckZ(type); break;
                        case "Issues_Comments": orderBy.Issues_Comments(type); break;
                        case "Issues_Creator": orderBy.Issues_Creator(type); break;
                        case "Issues_Updator": orderBy.Issues_Updator(type); break;
                        case "Issues_CreatedTime": orderBy.Issues_CreatedTime(type); break;                        case "Results_SiteId": orderBy.Results_SiteId(type); break;
                        case "Results_UpdatedTime": orderBy.Results_UpdatedTime(type); break;
                        case "Results_ResultId": orderBy.Results_ResultId(type); break;
                        case "Results_Ver": orderBy.Results_Ver(type); break;
                        case "Results_Title": orderBy.Results_Title(type); break;
                        case "Results_Body": orderBy.Results_Body(type); break;
                        case "Results_TitleBody": orderBy.Results_TitleBody(type); break;
                        case "Results_Status": orderBy.Results_Status(type); break;
                        case "Results_Manager": orderBy.Results_Manager(type); break;
                        case "Results_Owner": orderBy.Results_Owner(type); break;
                        case "Results_ClassA": orderBy.Results_ClassA(type); break;
                        case "Results_ClassB": orderBy.Results_ClassB(type); break;
                        case "Results_ClassC": orderBy.Results_ClassC(type); break;
                        case "Results_ClassD": orderBy.Results_ClassD(type); break;
                        case "Results_ClassE": orderBy.Results_ClassE(type); break;
                        case "Results_ClassF": orderBy.Results_ClassF(type); break;
                        case "Results_ClassG": orderBy.Results_ClassG(type); break;
                        case "Results_ClassH": orderBy.Results_ClassH(type); break;
                        case "Results_ClassI": orderBy.Results_ClassI(type); break;
                        case "Results_ClassJ": orderBy.Results_ClassJ(type); break;
                        case "Results_ClassK": orderBy.Results_ClassK(type); break;
                        case "Results_ClassL": orderBy.Results_ClassL(type); break;
                        case "Results_ClassM": orderBy.Results_ClassM(type); break;
                        case "Results_ClassN": orderBy.Results_ClassN(type); break;
                        case "Results_ClassO": orderBy.Results_ClassO(type); break;
                        case "Results_ClassP": orderBy.Results_ClassP(type); break;
                        case "Results_ClassQ": orderBy.Results_ClassQ(type); break;
                        case "Results_ClassR": orderBy.Results_ClassR(type); break;
                        case "Results_ClassS": orderBy.Results_ClassS(type); break;
                        case "Results_ClassT": orderBy.Results_ClassT(type); break;
                        case "Results_ClassU": orderBy.Results_ClassU(type); break;
                        case "Results_ClassV": orderBy.Results_ClassV(type); break;
                        case "Results_ClassW": orderBy.Results_ClassW(type); break;
                        case "Results_ClassX": orderBy.Results_ClassX(type); break;
                        case "Results_ClassY": orderBy.Results_ClassY(type); break;
                        case "Results_ClassZ": orderBy.Results_ClassZ(type); break;
                        case "Results_NumA": orderBy.Results_NumA(type); break;
                        case "Results_NumB": orderBy.Results_NumB(type); break;
                        case "Results_NumC": orderBy.Results_NumC(type); break;
                        case "Results_NumD": orderBy.Results_NumD(type); break;
                        case "Results_NumE": orderBy.Results_NumE(type); break;
                        case "Results_NumF": orderBy.Results_NumF(type); break;
                        case "Results_NumG": orderBy.Results_NumG(type); break;
                        case "Results_NumH": orderBy.Results_NumH(type); break;
                        case "Results_NumI": orderBy.Results_NumI(type); break;
                        case "Results_NumJ": orderBy.Results_NumJ(type); break;
                        case "Results_NumK": orderBy.Results_NumK(type); break;
                        case "Results_NumL": orderBy.Results_NumL(type); break;
                        case "Results_NumM": orderBy.Results_NumM(type); break;
                        case "Results_NumN": orderBy.Results_NumN(type); break;
                        case "Results_NumO": orderBy.Results_NumO(type); break;
                        case "Results_NumP": orderBy.Results_NumP(type); break;
                        case "Results_NumQ": orderBy.Results_NumQ(type); break;
                        case "Results_NumR": orderBy.Results_NumR(type); break;
                        case "Results_NumS": orderBy.Results_NumS(type); break;
                        case "Results_NumT": orderBy.Results_NumT(type); break;
                        case "Results_NumU": orderBy.Results_NumU(type); break;
                        case "Results_NumV": orderBy.Results_NumV(type); break;
                        case "Results_NumW": orderBy.Results_NumW(type); break;
                        case "Results_NumX": orderBy.Results_NumX(type); break;
                        case "Results_NumY": orderBy.Results_NumY(type); break;
                        case "Results_NumZ": orderBy.Results_NumZ(type); break;
                        case "Results_DateA": orderBy.Results_DateA(type); break;
                        case "Results_DateB": orderBy.Results_DateB(type); break;
                        case "Results_DateC": orderBy.Results_DateC(type); break;
                        case "Results_DateD": orderBy.Results_DateD(type); break;
                        case "Results_DateE": orderBy.Results_DateE(type); break;
                        case "Results_DateF": orderBy.Results_DateF(type); break;
                        case "Results_DateG": orderBy.Results_DateG(type); break;
                        case "Results_DateH": orderBy.Results_DateH(type); break;
                        case "Results_DateI": orderBy.Results_DateI(type); break;
                        case "Results_DateJ": orderBy.Results_DateJ(type); break;
                        case "Results_DateK": orderBy.Results_DateK(type); break;
                        case "Results_DateL": orderBy.Results_DateL(type); break;
                        case "Results_DateM": orderBy.Results_DateM(type); break;
                        case "Results_DateN": orderBy.Results_DateN(type); break;
                        case "Results_DateO": orderBy.Results_DateO(type); break;
                        case "Results_DateP": orderBy.Results_DateP(type); break;
                        case "Results_DateQ": orderBy.Results_DateQ(type); break;
                        case "Results_DateR": orderBy.Results_DateR(type); break;
                        case "Results_DateS": orderBy.Results_DateS(type); break;
                        case "Results_DateT": orderBy.Results_DateT(type); break;
                        case "Results_DateU": orderBy.Results_DateU(type); break;
                        case "Results_DateV": orderBy.Results_DateV(type); break;
                        case "Results_DateW": orderBy.Results_DateW(type); break;
                        case "Results_DateX": orderBy.Results_DateX(type); break;
                        case "Results_DateY": orderBy.Results_DateY(type); break;
                        case "Results_DateZ": orderBy.Results_DateZ(type); break;
                        case "Results_DescriptionA": orderBy.Results_DescriptionA(type); break;
                        case "Results_DescriptionB": orderBy.Results_DescriptionB(type); break;
                        case "Results_DescriptionC": orderBy.Results_DescriptionC(type); break;
                        case "Results_DescriptionD": orderBy.Results_DescriptionD(type); break;
                        case "Results_DescriptionE": orderBy.Results_DescriptionE(type); break;
                        case "Results_DescriptionF": orderBy.Results_DescriptionF(type); break;
                        case "Results_DescriptionG": orderBy.Results_DescriptionG(type); break;
                        case "Results_DescriptionH": orderBy.Results_DescriptionH(type); break;
                        case "Results_DescriptionI": orderBy.Results_DescriptionI(type); break;
                        case "Results_DescriptionJ": orderBy.Results_DescriptionJ(type); break;
                        case "Results_DescriptionK": orderBy.Results_DescriptionK(type); break;
                        case "Results_DescriptionL": orderBy.Results_DescriptionL(type); break;
                        case "Results_DescriptionM": orderBy.Results_DescriptionM(type); break;
                        case "Results_DescriptionN": orderBy.Results_DescriptionN(type); break;
                        case "Results_DescriptionO": orderBy.Results_DescriptionO(type); break;
                        case "Results_DescriptionP": orderBy.Results_DescriptionP(type); break;
                        case "Results_DescriptionQ": orderBy.Results_DescriptionQ(type); break;
                        case "Results_DescriptionR": orderBy.Results_DescriptionR(type); break;
                        case "Results_DescriptionS": orderBy.Results_DescriptionS(type); break;
                        case "Results_DescriptionT": orderBy.Results_DescriptionT(type); break;
                        case "Results_DescriptionU": orderBy.Results_DescriptionU(type); break;
                        case "Results_DescriptionV": orderBy.Results_DescriptionV(type); break;
                        case "Results_DescriptionW": orderBy.Results_DescriptionW(type); break;
                        case "Results_DescriptionX": orderBy.Results_DescriptionX(type); break;
                        case "Results_DescriptionY": orderBy.Results_DescriptionY(type); break;
                        case "Results_DescriptionZ": orderBy.Results_DescriptionZ(type); break;
                        case "Results_CheckA": orderBy.Results_CheckA(type); break;
                        case "Results_CheckB": orderBy.Results_CheckB(type); break;
                        case "Results_CheckC": orderBy.Results_CheckC(type); break;
                        case "Results_CheckD": orderBy.Results_CheckD(type); break;
                        case "Results_CheckE": orderBy.Results_CheckE(type); break;
                        case "Results_CheckF": orderBy.Results_CheckF(type); break;
                        case "Results_CheckG": orderBy.Results_CheckG(type); break;
                        case "Results_CheckH": orderBy.Results_CheckH(type); break;
                        case "Results_CheckI": orderBy.Results_CheckI(type); break;
                        case "Results_CheckJ": orderBy.Results_CheckJ(type); break;
                        case "Results_CheckK": orderBy.Results_CheckK(type); break;
                        case "Results_CheckL": orderBy.Results_CheckL(type); break;
                        case "Results_CheckM": orderBy.Results_CheckM(type); break;
                        case "Results_CheckN": orderBy.Results_CheckN(type); break;
                        case "Results_CheckO": orderBy.Results_CheckO(type); break;
                        case "Results_CheckP": orderBy.Results_CheckP(type); break;
                        case "Results_CheckQ": orderBy.Results_CheckQ(type); break;
                        case "Results_CheckR": orderBy.Results_CheckR(type); break;
                        case "Results_CheckS": orderBy.Results_CheckS(type); break;
                        case "Results_CheckT": orderBy.Results_CheckT(type); break;
                        case "Results_CheckU": orderBy.Results_CheckU(type); break;
                        case "Results_CheckV": orderBy.Results_CheckV(type); break;
                        case "Results_CheckW": orderBy.Results_CheckW(type); break;
                        case "Results_CheckX": orderBy.Results_CheckX(type); break;
                        case "Results_CheckY": orderBy.Results_CheckY(type); break;
                        case "Results_CheckZ": orderBy.Results_CheckZ(type); break;
                        case "Results_Comments": orderBy.Results_Comments(type); break;
                        case "Results_Creator": orderBy.Results_Creator(type); break;
                        case "Results_Updator": orderBy.Results_Updator(type); break;
                        case "Results_CreatedTime": orderBy.Results_CreatedTime(type); break;                        case "Wikis_SiteId": orderBy.Wikis_SiteId(type); break;
                        case "Wikis_UpdatedTime": orderBy.Wikis_UpdatedTime(type); break;
                        case "Wikis_WikiId": orderBy.Wikis_WikiId(type); break;
                        case "Wikis_Ver": orderBy.Wikis_Ver(type); break;
                        case "Wikis_Title": orderBy.Wikis_Title(type); break;
                        case "Wikis_Body": orderBy.Wikis_Body(type); break;
                        case "Wikis_TitleBody": orderBy.Wikis_TitleBody(type); break;
                        case "Wikis_Comments": orderBy.Wikis_Comments(type); break;
                        case "Wikis_Creator": orderBy.Wikis_Creator(type); break;
                        case "Wikis_Updator": orderBy.Wikis_Updator(type); break;
                        case "Wikis_CreatedTime": orderBy.Wikis_CreatedTime(type); break;
                    }
                });
            }
            return orderBy;
        }
    }
}
