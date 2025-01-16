using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class Link
    {
        public string TableName;
        public string ColumnName;
        public long SiteId;
        public int? Priority;
        public bool? NoAddButton;
        public bool? AddSource;
        public bool? NotReturnParentRecord;
        public bool? ExcludeMe;
        public bool? MembersOnly;
        public bool? SelectNewLink;
        public string SearchFormat;
        public View View;
        public Lookups Lookups;
        public LinkActions LinkActions;
        public bool? JsonFormat;
        [NonSerialized]
        public string SiteTitle;
        [NonSerialized]
        public long SourceId;

        public Link()
        {
        }

        public Link(string columnName, long siteId)
        {
            ColumnName = columnName;
            SiteId = siteId;
        }

        public Link(string columnName, string settings)
        {
            ColumnName = columnName;
            settings?
                .RegexFirst(@"(?<=\[\[).+(?=\]\])")?
                .Split(',')
                .Select((o, i) => new { Index = i, Setting = o })
                .ForEach(data =>
                {
                    if (data.Index == 0)
                    {
                        SiteId = data.Setting.ToLong();
                    }
                    else
                    {
                        switch (data.Setting)
                        {
                            case "NoAddButton":
                                NoAddButton = true;
                                break;
                            case "AddSource":
                                AddSource = true;
                                break;
                            case "NotReturnParentRecord":
                                NotReturnParentRecord = true;
                                break;
                        }
                    }
                });
        }

        public string LinkedTableName()
        {
            return $"{ColumnName}~{SiteId}";
        }

        public Link GetRecordingData(Context context, SiteSettings ss)
        {
            var link = new Link();
            if (!TableName.IsNullOrEmpty()) link.TableName = TableName;
            if (!ColumnName.IsNullOrEmpty()) link.ColumnName = ColumnName;
            link.SiteId = SiteId;
            if (Priority > 0) link.Priority = Priority;
            if (NoAddButton == true) link.NoAddButton = true;
            if (AddSource == true) link.AddSource = true;
            if (NotReturnParentRecord == true) link.NotReturnParentRecord = true;
            if (ExcludeMe == true) link.ExcludeMe = true;
            if (MembersOnly == true) link.MembersOnly = true;
            if (SelectNewLink == true) link.SelectNewLink = true;
            if (!SearchFormat.IsNullOrEmpty()) link.SearchFormat = SearchFormat;
            var currentSs = GetSiteSettings(
                context: context,
                ss: ss);
            link.View = View?.GetRecordingData(
                context: context,
                ss: currentSs);
            link.Lookups = Lookups?.GetRecordingData();
            link.LinkActions = LinkActions?.GetRecordingData(
                context: context,
                ss: currentSs);
            if (JsonFormat == true) link.JsonFormat = true;
            return link;
        }

        private SiteSettings GetSiteSettings(Context context, SiteSettings ss)
        {
            switch (TableName)
            {
                case "Depts":
                    return SiteSettingsUtilities.DeptsSiteSettings(context: context);
                case "Groups":
                    return SiteSettingsUtilities.GroupsSiteSettings(context: context);
                case "Users":
                    return SiteSettingsUtilities.UsersSiteSettings(context: context);
                default:
                    return ss;
            }
        }

        public void SetChoiceHash(
            Context context,
            SiteSettings ss,
            Column column,
            string searchText,
            IEnumerable<string> selectedValues,
            Column parentColumn,
            List<long> parentIds,
            int offset,
            bool search,
            bool searchFormat,
            bool setAllChoices,
            bool setChoices)
        {
            switch (TableName)
            {
                case "Depts":
                    column.Type = Column.Types.Dept;
                    if (setChoices)
                    {
                        if (column.UseSearch != true || search || setAllChoices)
                        {
                            var currentSs = SiteSettingsUtilities.DeptsSiteSettings(context: context);
                            Set(
                                context: context,
                                ss: currentSs,
                                column: column,
                                dataRows: Depts(
                                    context: context,
                                    ss: currentSs,
                                    inheritPermission: ss.InheritPermission,
                                    searchText: searchText,
                                    setAllChoices: setAllChoices,
                                    offset: offset),
                                searchFormat: searchFormat);
                        }
                    }
                    break;
                case "Groups":
                    column.Type = Column.Types.Group;
                    if (setChoices)
                    {
                        if (column.UseSearch != true || search || setAllChoices)
                        {
                            var currentSs = SiteSettingsUtilities.GroupsSiteSettings(context: context);
                            Set(
                                context: context,
                                ss: currentSs,
                                column: column,
                                dataRows: Groups(
                                    context: context,
                                    ss: currentSs,
                                    inheritPermission: ss.InheritPermission,
                                    searchText: searchText,
                                    setAllChoices: setAllChoices,
                                    offset: offset),
                                searchFormat: searchFormat);
                        }
                    }
                    break;
                case "Users":
                    column.Type = Column.Types.User;
                    if (setChoices)
                    {
                        if (column.UseSearch != true || search || setAllChoices)
                        {
                            var currentSs = SiteSettingsUtilities.UsersSiteSettings(context: context);
                            Set(
                                context: context,
                                ss: currentSs,
                                column: column,
                                dataRows: Users(
                                    context: context,
                                    ss: currentSs,
                                    inheritPermission: ss.InheritPermission,
                                    searchText: searchText,
                                    setAllChoices: setAllChoices,
                                    offset: offset),
                                searchFormat: searchFormat);
                        }
                    }
                    break;
                default:
                    if (setChoices)
                    {
                        if (SiteId > 0)
                        {
                            if (column.UseSearch != true
                                || search
                                || setAllChoices
                                || selectedValues?.Any() == true)
                            {
                                // 項目利用時かつ項目候補が無い場合に強制的に作成を行う。実装を見直しする事が望ましい
                                // 現状のコードではSiteSettings.Init()で全部作る想定だがネストが深い場合は作られない。
                                if (ss.Destinations == null && search)
                                {
                                    ss.SetLinkedSiteSettings(context: context, sources: false);
                                    ss.SetPermissions(context: context, referenceId: ss.ReferenceId);
                                }
                                var currentSs = ss.Destinations?.Get(SiteId);
                                if (currentSs != null)
                                {
                                    Set(
                                        context: context,
                                        ss: currentSs,
                                        column: column,
                                        dataRows: Items(
                                            context: context,
                                            ss: currentSs,
                                            searchText: searchText,
                                            selectedValues: selectedValues,
                                            column: column,
                                            parentColumn: parentColumn,
                                            parentIds: parentIds,
                                            offset: offset,
                                            setAllChoices: setAllChoices),
                                        searchFormat: searchFormat);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void Set(
            Context context,
            SiteSettings ss,
            Column column,
            EnumerableRowCollection<DataRow> dataRows,
            bool searchFormat)
        {
            var choiceHash = dataRows.ToDictionary(
                dataRow => dataRow.String("Key"),
                dataRow => new Choice(
                    value: dataRow.String("Key"),
                    text: searchFormat && !SearchFormat.IsNullOrEmpty()
                        ? SearchFormatText(
                            context: context,
                            ss: ss,
                            dataRow: dataRow)
                        : dataRow.String("Text")));
            column.ChoiceHash.AddRange(choiceHash);
        }

        private string SearchFormatText(Context context, SiteSettings ss, DataRow dataRow)
        {
            var text = SearchFormat;
            switch (ss.ReferenceType)
            {
                case "Depts":
                    var deptModel = new DeptModel(
                        context: context,
                        ss: ss,
                        dataRow: dataRow);
                    var deptMine = deptModel.Mine(context: context);
                    ss.IncludedColumns(SearchFormat)
                        .Where(column => column.CanRead(
                            context: context,
                            ss: ss,
                            mine: null))
                        .ForEach(column =>
                            text = text.Replace(
                                $@"[{column.ColumnName}]",
                                $@"{deptModel.ToDisplay(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: deptMine)}"));
                    break;
                case "Groups":
                    var groupModel = new GroupModel(
                        context: context,
                        ss: ss,
                        dataRow: dataRow);
                    var groupMine = groupModel.Mine(context: context);
                    ss.IncludedColumns(SearchFormat)
                        .Where(column => column.CanRead(
                            context: context,
                            ss: ss,
                            mine: null))
                        .ForEach(column =>
                            text = text.Replace(
                                $@"[{column.ColumnName}]",
                                $@"{groupModel.ToDisplay(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: groupMine)}"));
                    break;
                case "Users":
                    var userModel = new UserModel(
                        context: context,
                        ss: ss,
                        dataRow: dataRow);
                    if (SearchFormat.Contains("[MailAddresses]"))
                    {
                        text = text.Replace(
                            "[MailAddresses]",
                            userModel.GetMailAddresses(context: context).Join());
                    }
                    var userMine = userModel.Mine(context: context);
                    ss.IncludedColumns(SearchFormat)
                        .Where(column => column.CanRead(
                            context: context,
                            ss: ss,
                            mine: null))
                        .ForEach(column =>
                            text = text.Replace(
                                $@"[{column.ColumnName}]",
                                $@"{userModel.ToDisplay(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: userMine)}"));
                    break;
                case "Issues":
                    var issueModel = new IssueModel(
                        context: context,
                        ss: ss,
                        dataRow: dataRow);
                    var issueMine = issueModel.Mine(context: context);
                    ss.IncludedColumns(SearchFormat)
                        .Where(column => column.CanRead(
                            context: context,
                            ss: ss,
                            mine: null))
                        .ForEach(column =>
                            text = text.Replace(
                                $@"[{column.ColumnName}]",
                                $@"{issueModel.ToDisplay(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: issueMine)}"));
                    break;
                case "Results":
                    var resultModel = new ResultModel(
                        context: context,
                        ss: ss,
                        dataRow: dataRow);
                    var resultMine = resultModel.Mine(context: context);
                    ss.IncludedColumns(SearchFormat)
                        .Where(column => column.CanRead(
                            context: context,
                            ss: ss,
                            mine: null))
                        .ForEach(column =>
                            text = text.Replace(
                                $@"[{column.ColumnName}]",
                                $@"{resultModel.ToDisplay(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: resultMine)}"));
                    break;
            }
            return text;
        }

        private EnumerableRowCollection<DataRow> Depts(
            Context context,
            SiteSettings ss,
            long inheritPermission,
            string searchText,
            bool setAllChoices,
            int offset)
        {
            var view = GetView(
                context: context,
                ss: ss);
            var sqlColumn = SetSqlColum(
                context: context,
                ss: ss,
                keyColumnName: "DeptId",
                textColumnName: "DeptName");
            var dataRows = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectDepts(
                    column: sqlColumn,
                    where: view.Where(
                        context: context,
                        ss: ss,
                        where: Rds.DeptsWhere()
                            .TenantId(context.TenantId)
                            .DeptId(
                                value: context.DeptId,
                                _operator: "<>",
                                _using: ExcludeMe == true)
                            .SiteDeptWhere(
                                context: context,
                                siteId: inheritPermission,
                                _using: MembersOnly == true)
                            .SqlWhereLike(
                                tableName: "Depts",
                                name: "SearchText",
                                searchText: searchText,
                                clauseCollection: new List<string>()
                                {
                                    Rds.Depts_DeptId_WhereLike(factory: context),
                                    Rds.Depts_DeptName_WhereLike(factory: context),
                                    Rds.Depts_Body_WhereLike(factory: context)
                                })),
                    orderBy: view.OrderBy(
                        context: context,
                        ss: ss,
                        orderBy: view.ColumnSorterHash?.Any() != true
                            ? Rds.DeptsOrderBy().DeptName()
                            : null),
                    offset: offset,
                    pageSize: !setAllChoices
                        ? Parameters.General.DropDownSearchPageSize
                        : 0))
                            .AsEnumerable();
            return dataRows;
        }

        private EnumerableRowCollection<DataRow> Groups(
            Context context,
            SiteSettings ss,
            long inheritPermission,
            string searchText,
            bool setAllChoices,
            int offset)
        {
            var view = GetView(
                context: context,
                ss: ss);
            var sqlColumn = SetSqlColum(
                context: context,
                ss: ss,
                keyColumnName: "GroupId",
                textColumnName: "GroupName");
            var dataRows = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectGroups(
                    column: sqlColumn,
                    where: view.Where(
                        context: context,
                        ss: ss,
                        where: Rds.GroupsWhere()
                            .TenantId(context.TenantId)
                            .SiteGroupWhere(
                                context: context,
                                siteId: inheritPermission,
                                _using: MembersOnly == true)
                            .SqlWhereLike(
                                tableName: "Depts",
                                name: "SearchText",
                                searchText: searchText,
                                clauseCollection: new List<string>()
                                {
                                    Rds.Groups_GroupId_WhereLike(factory: context),
                                    Rds.Groups_GroupName_WhereLike(factory: context),
                                    Rds.Groups_Body_WhereLike(factory: context)
                                })),
                    orderBy: view.OrderBy(
                        context: context,
                        ss: ss,
                        orderBy: view.ColumnSorterHash?.Any() != true
                            ? Rds.GroupsOrderBy().GroupName()
                            : null),
                    offset: offset,
                    pageSize: !setAllChoices
                        ? Parameters.General.DropDownSearchPageSize
                        : 0))
                            .AsEnumerable();
            return dataRows;
        }

        private EnumerableRowCollection<DataRow> Users(
            Context context,
            SiteSettings ss,
            long inheritPermission,
            string searchText,
            bool setAllChoices,
            int offset)
        {
            var view = GetView(
                context: context,
                ss: ss);
            var sqlColumn = SetSqlColum(
                context: context,
                ss: ss,
                keyColumnName: "UserId",
                textColumnName: "Name");
            var dataRows = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectUsers(
                    column: sqlColumn,
                    join: Rds.UsersJoin()
                        .Add(new SqlJoin(
                            tableBracket: "\"Depts\"",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "\"Users\".\"DeptId\"=\"Depts\".\"DeptId\"")),
                    where: view.Where(
                        context: context,
                        ss: ss,
                        where: Rds.UsersWhere()
                            .TenantId(context.TenantId)
                            .UserId(
                                value: context.UserId,
                                _operator: "<>",
                                _using: ExcludeMe == true)
                            .SiteUserWhere(
                                context: context,
                                siteId: inheritPermission,
                                _using: MembersOnly == true)
                            .SqlWhereLike(
                                tableName: "Depts",
                                name: "SearchText",
                                searchText: searchText,
                                clauseCollection: new List<string>()
                                {
                                    Rds.Users_LoginId_WhereLike(factory: context),
                                    Rds.Users_Name_WhereLike(factory: context),
                                    Rds.Users_UserCode_WhereLike(factory: context),
                                    Rds.Users_Body_WhereLike(factory: context),
                                    Rds.Depts_DeptCode_WhereLike(factory: context),
                                    Rds.Depts_DeptName_WhereLike(factory: context),
                                    Rds.Depts_Body_WhereLike(factory: context)
                                })),
                    orderBy: view.OrderBy(
                        context: context,
                        ss: ss,
                        orderBy: view.ColumnSorterHash?.Any() != true
                            ? Rds.UsersOrderBy().Name()
                            : null),
                    offset: offset,
                    pageSize: !setAllChoices
                        ? Parameters.General.DropDownSearchPageSize
                        : 0))
                            .AsEnumerable();
            return dataRows;
        }

        private EnumerableRowCollection<DataRow> Items(
            Context context,
            SiteSettings ss,
            string searchText,
            IEnumerable<string> selectedValues,
            Column column,
            Column parentColumn,
            List<long> parentIds,
            int offset,
            bool setAllChoices)
        {
            var tableName = $"{ss.ReferenceType}_Items";
            var view = GetView(
                context: context,
                ss: ss);
            view.Search = searchText;
            var sqlColumn = SetSqlColum(
                context: context,
                ss: ss,
                keyColumnName: Rds.IdColumn(tableName: ss.ReferenceType),
                textColumnName: "Title");
            var where = view.Where(
                context: context,
                ss: ss,
                where: Rds.ItemsWhere()
                    .ReferenceId_In(
                        tableName: tableName,
                        value: selectedValues?.Select(o => o.ToLong()),
                        _using: selectedValues?.Any() == true)
                    .ReferenceId_In(
                        tableName: tableName,
                        sub: new SqlStatement(ss.LinkHashRelatingColumnsSubQuery(
                            context: context,
                            referenceType: ss.ReferenceType,
                            parentColumn: parentColumn,
                            parentIds: parentIds)),
                        _using: (ss.ReferenceType == "Results"
                            || ss.ReferenceType == "Issues")
                            && (parentIds?.Any() ?? false)
                            && parentColumn != null));
            var orderBy = view.OrderBy(
                context: context,
                ss: ss,
                orderBy: view.ColumnSorterHash?.Any() != true
                    ? new Rds.ItemsOrderByCollection().Title(tableName: tableName)
                    : null);
            var join = ss.Join(
                context: context,
                join: new IJoin[]
                {
                    sqlColumn,
                    where,
                    orderBy
                });
            var dataSet = Rds.ExecuteDataSet(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.Select(
                        tableName: ss.ReferenceType,
                        dataTableName: "Main",
                        column: sqlColumn,
                        join: join,
                        where: where,
                        orderBy: orderBy,
                        offset: offset,
                        pageSize: !setAllChoices
                            ? Parameters.General.DropDownSearchPageSize
                            : 0),
                    Rds.SelectCount(
                        tableName: ss.ReferenceType,
                        join: join,
                        where: where)
                });
            column.TotalCount = Rds.Count(dataSet);
            return dataSet.Tables["Main"].AsEnumerable();
        }

        private SqlColumnCollection SetSqlColum(
            Context context,
            SiteSettings ss,
            string keyColumnName,
            string textColumnName)
        {
            var sqlColumn = new SqlColumnCollection();
            var keyColumn = ss.GetColumn(
                context: context,
                columnName: keyColumnName);
            sqlColumn.Add(
                context: context,
                column: keyColumn);
            sqlColumn.Add(
                context: context,
                column: keyColumn,
                _as: "Key");
            if (textColumnName == "Title")
            {
                sqlColumn.ItemTitle(tableName: ss.ReferenceType);
                sqlColumn.ItemTitle(
                    tableName: ss.ReferenceType,
                    _as: "Text");
            }
            else
            {
                var textColumn = ss.GetColumn(
                    context: context,
                    columnName: textColumnName);
                sqlColumn.Add(
                    context: context,
                    column: textColumn,
                    _as: "Text");
                sqlColumn.Add(
                    context: context,
                    column: textColumn);
            }
            if (!SearchFormat.IsNullOrEmpty())
            {
                var columns = new List<Column>();
                ss.IncludedColumns(SearchFormat)
                    .Where(column => column.CanRead(
                        context: context,
                        ss: ss,
                        mine: null))
                    .ForEach(column => columns.Add(column));
                sqlColumn.AddRange(columns.SelectMany(column => column.SqlColumnCollection())
                    .GroupBy(sql => sql.ColumnBracket + sql.As)
                    .Select(sql => sql.First())
                    .ToArray());
            }
            return sqlColumn;
        }

        private View GetView(Context context, SiteSettings ss)
        {
            var view = View ?? new View();
            if (ss.GetColumn(
                context: context,
                columnName: "Disabled") != null)
            {
                view.ColumnFilterHash = view.ColumnFilterHash ?? new Dictionary<string, string>();
                view.ColumnFilterHash.AddIfNotConainsKey("Disabled", "false");
            }
            return view;
        }

        public void Action(
            Context context,
            long siteId,
            string type,
            Dictionary<string, string> data,
            SqlSelect sub)
        {
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteId: siteId);
            LinkActions?.ForEach(action =>
            {
                if (action.Type == type)
                {
                    switch (type)
                    {
                        case "CopyWithLinks":
                            action.CopyWithLinks(
                                context: context,
                                ss: ss,
                                columnName: ColumnName,
                                from: data.Get("From").ToLong(),
                                to: data.Get("To").ToLong());
                            break;
                        case "DeleteWithLinks":
                            action.DeleteWithLinks(
                                context: context,
                                ss: ss,
                                columnName: ColumnName,
                                sub: sub);
                            break;
                    }
                }
            });
        }
    }
}