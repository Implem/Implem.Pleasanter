using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Analysis;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
namespace Implem.Pleasanter.Models
{
    public class SearchIndexModel : BaseModel
    {
        public string Word = string.Empty;
        public long ReferenceId = 0;
        public int Priority = 0;
        public string ReferenceType = string.Empty;
        public string Title = string.Empty;
        public string Subset = string.Empty;
        public string SavedWord = string.Empty;
        public long SavedReferenceId = 0;
        public int SavedPriority = 0;
        public string SavedReferenceType = string.Empty;
        public string SavedTitle = string.Empty;
        public string SavedSubset = string.Empty;
        public long SavedPermissionType = 31;
        public bool Word_Updated { get { return Word != SavedWord && Word != null; } }
        public bool ReferenceId_Updated { get { return ReferenceId != SavedReferenceId; } }
        public bool Priority_Updated { get { return Priority != SavedPriority; } }

        public SearchIndexModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataRow dataRow)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            Set(dataRow);
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

        public SearchIndexModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectSearchIndexes(
                tableType: tableType,
                column: column ?? Rds.SearchIndexesColumnDefault(),
                join: join ??  Rds.SearchIndexesJoinDefault(),
                where: where ?? Rds.SearchIndexesWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
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

        private void Set(DataRow dataRow)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var name = dataColumn.ColumnName;
                switch(name)
                {
                    case "Word": if (dataRow[name] != DBNull.Value) { Word = dataRow[name].ToString(); SavedWord = Word; } break;
                    case "ReferenceId": if (dataRow[name] != DBNull.Value) { ReferenceId = dataRow[name].ToLong(); SavedReferenceId = ReferenceId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "Priority": Priority = dataRow[name].ToInt(); SavedPriority = Priority; break;
                    case "ReferenceType": ReferenceType = dataRow[name].ToString(); SavedReferenceType = ReferenceType; break;
                    case "Title": Title = dataRow[name].ToString(); SavedTitle = Title; break;
                    case "Subset": Subset = dataRow[name].ToString(); SavedSubset = Subset; break;
                    case "PermissionType": PermissionType = (Permissions.Types)dataRow[name].ToLong(); SavedPermissionType = PermissionType.ToLong(); break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }

        private string ResponseConflicts()
        {
            Get();
            return AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(Updator.FullName).ToJson()
                : Messages.ResponseDeleteConflicts().ToJson();
        }
    }

    public class SearchIndexCollection : List<SearchIndexModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public SearchIndexCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregationCollection = null,
            bool get = true)
        {
            if (get)
            {
                Set(siteSettings, permissionType, Get(
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord,
                    aggregationCollection: aggregationCollection));
            }
        }

        public SearchIndexCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            Set(siteSettings, permissionType, dataTable);
        }

        private SearchIndexCollection Set(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new SearchIndexModel(siteSettings, permissionType, dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public SearchIndexCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            string commandText,
            SqlParamCollection param = null)
        {
            Set(siteSettings, permissionType, Get(commandText, param));
        }

        private DataTable Get(
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool history = false,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregationCollection = null)
        {
            var statements = new List<SqlStatement>
            {
                Rds.SelectSearchIndexes(
                    dataTableName: "Main",
                    column: column ?? Rds.SearchIndexesColumnDefault(),
                    join: join ??  Rds.SearchIndexesJoinDefault(),
                    where: where ?? null,
                    orderBy: orderBy ?? null,
                    param: param ?? null,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord)
            };
            if (aggregationCollection != null)
            {
                statements.AddRange(Rds.SearchIndexesAggregations(aggregationCollection, where));
            }
            var dataSet = Rds.ExecuteDataSet(
                transactional: false,
                statements: statements.ToArray());
            Aggregations.Set(dataSet, aggregationCollection);
            return dataSet.Tables["Main"];
        }

        private DataTable Get(string commandText, SqlParamCollection param = null)
        {
            return Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SearchIndexesStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class SearchIndexesUtility
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, SearchIndexModel searchIndexModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: searchIndexModel.Ver);
                case "Comments": return hb.Td(column: column, value: searchIndexModel.Comments);
                case "Creator": return hb.Td(column: column, value: searchIndexModel.Creator);
                case "Updator": return hb.Td(column: column, value: searchIndexModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: searchIndexModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: searchIndexModel.UpdatedTime);
                default: return hb;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Search()
        {
            var dataSet = DataRows(
                searchIndexes: QueryStrings.Data("text").SearchIndexes(),
                offset: QueryStrings.Int("offset"));
            return MainContainer(
                text: QueryStrings.Data("text"),
                offset: 0,
                results: dataSet?.Tables["Main"].AsEnumerable(),
                count: Rds.Count(dataSet)).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string AjaxSearch()
        {
            var offset = QueryStrings.Int("offset");
            var text = QueryStrings.Data("text");
            var dataSet = DataRows(text.SearchIndexes(), offset);
            var results = dataSet?.Tables["Main"].AsEnumerable();
            var responseCollection = new ResponseCollection();
            if (offset == 0)
            {
                responseCollection
                    .PushState(
                        "Search",
                        Navigations.Get("Items", "Search?text=" + Url.Encode(text)),
                        _using: !QueryStrings.Bool("reload"))
                    .Html(
                        "#MainContainer",
                        MainContainer(
                            text: text,
                            offset: 0,
                            results: results,
                            count: Rds.Count(dataSet)))
                    .Focus("#Search");
            }
            else
            {
                responseCollection
                    .Append(
                        "#SearchResults",
                        new HtmlBuilder().Results(text: text, offset: offset, results: results))
                    .Val(
                        "#SearchOffset",
                        (results != null &&
                        results.Count() > 0 &&
                        results.Count() == Parameters.General.SearchPageSize
                            ? offset + Parameters.General.SearchPageSize
                            : -1).ToString());
            }
            return responseCollection.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainContainer(
            string text, int offset, EnumerableRowCollection<DataRow> results, int count)
        {
            var hb = new HtmlBuilder();
            var searchIndexes = text.SearchIndexes();
            return hb.Template(
                siteId: 0,
                referenceId: "SearchIndexes",
                title: string.Empty,
                permissionType: Permissions.Types.Read,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.NotSet,
                allowAccess: true,
                useNavigationButtons: false,
                useTitle: false,
                useSearch: false,
                useBreadCrumbs: false,
                action: () => hb
                    .Div(id: "SearchResults", css: "search-results", action: () => hb
                        .Command(text: text)
                        .Count(count: count)
                        .Results(text: text, offset: offset, results: results))
                    .Hidden(
                        controlId: "SearchOffset",
                        value: Parameters.General.SearchPageSize.ToString()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Command(this HtmlBuilder hb, string text)
        {
            return hb.Div(css: "command-center", action: () => hb
                .TextBox(
                    controlId: "Search",
                    controlCss: " w600",
                    text: text,
                    placeholder: Displays.Search()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Count(
            this HtmlBuilder hb, int count)
        {
            return hb.Div(css: "count", action: () => hb
                .Span(css: "label", action: () => hb
                    .Text(text: Displays.Quantity()))
                .Span(css: "data", action: () => hb
                    .Text(text: count.ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Results(
            this HtmlBuilder hb, string text, int offset, EnumerableRowCollection<DataRow> results)
        {
            results?.ForEach(dataRow => Libraries.Search.Responses.Get(hb, dataRow, text));
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DataSet DataRows(IEnumerable<string> searchIndexes, int offset)
        {
            if (searchIndexes.Count() == 0) return null;
            var concordance = Math.Ceiling(
                searchIndexes.Count() * Parameters.General.SearchConcordanceRate);
            return Rds.ExecuteDataSet(statements:
                Rds.SelectSearchIndexes(
                    dataTableName: "Main",
                    column: Rds.SearchIndexesColumn()
                        .ReferenceId()
                        .ReferenceType()
                        .Title()
                        .Subset()
                        .PriorityTotal()
                        .SearchIndexesCount(),
                    join: Rds.SearchIndexesJoinDefault(),
                    where: Rds.SearchIndexesWhere()
                        .Word(searchIndexes, multiParamOperator: " or ")
                        .PermissionType(0, _operator: "<>"),
                    groupBy: Rds.SearchIndexesGroupBy()
                        .ReferenceId()
                        .ReferenceType()
                        .Title()
                        .Subset(),
                    having: Rds.SearchIndexesHaving()
                        .SearchIndexesCount(concordance, _operator: ">="),
                    orderBy: Rds.SearchIndexesOrderBy()
                        .SearchIndexesCount(SqlOrderBy.Types.desc)
                        .PriorityTotal()
                        .UpdatedTimeMax(SqlOrderBy.Types.desc),
                    offset: offset,
                    pageSize: Parameters.General.SearchPageSize,
                    countRecord: offset == 0));
        }
    }
}
