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
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Models
{
    public class LinkModel : BaseModel
    {
        public long DestinationId = 0;
        public long SourceId = 0;
        public string ReferenceType = string.Empty;
        public long SiteId = 0;
        public string Title = string.Empty;
        public string Subset = string.Empty;
        public string SiteTitle = string.Empty;
        public long SavedDestinationId = 0;
        public long SavedSourceId = 0;
        public string SavedReferenceType = string.Empty;
        public long SavedSiteId = 0;
        public string SavedTitle = string.Empty;
        public string SavedSubset = string.Empty;
        public string SavedSiteTitle = string.Empty;
        public bool DestinationId_Updated { get { return DestinationId != SavedDestinationId; } }
        public bool SourceId_Updated { get { return SourceId != SavedSourceId; } }

        public LinkModel(
            DataRow dataRow)
        {
            OnConstructing();
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

        public LinkModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectLinks(
                tableType: tableType,
                column: column ?? Rds.LinksColumnDefault(),
                join: join ??  Rds.LinksJoinDefault(),
                where: where ?? Rds.LinksWhereDefault(this),
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
                    case "DestinationId": if (dataRow[name] != DBNull.Value) { DestinationId = dataRow[name].ToLong(); SavedDestinationId = DestinationId; } break;
                    case "SourceId": if (dataRow[name] != DBNull.Value) { SourceId = dataRow[name].ToLong(); SavedSourceId = SourceId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "ReferenceType": ReferenceType = dataRow[name].ToString(); SavedReferenceType = ReferenceType; break;
                    case "SiteId": SiteId = dataRow[name].ToLong(); SavedSiteId = SiteId; break;
                    case "Title": Title = dataRow[name].ToString(); SavedTitle = Title; break;
                    case "Subset": Subset = dataRow[name].ToString(); SavedSubset = Subset; break;
                    case "SiteTitle": SiteTitle = dataRow[name].ToString(); SavedSiteTitle = SiteTitle; break;
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

    public class LinkCollection : List<LinkModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public LinkCollection(
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
                Set(Get(
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

        public LinkCollection(
            DataTable dataTable)
        {
            Set(dataTable);
        }

        private LinkCollection Set(
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new LinkModel(dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public LinkCollection(
            string commandText,
            SqlParamCollection param = null)
        {
            Set(Get(commandText, param));
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
                Rds.SelectLinks(
                    dataTableName: "Main",
                    column: column ?? Rds.LinksColumnDefault(),
                    join: join ??  Rds.LinksJoinDefault(),
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
                statements.AddRange(Rds.LinksAggregations(aggregationCollection, where));
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
                statements: Rds.LinksStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class LinksUtility
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, LinkModel linkModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: linkModel.Ver);
                case "Comments": return hb.Td(column: column, value: linkModel.Comments);
                case "Creator": return hb.Td(column: column, value: linkModel.Creator);
                case "Updator": return hb.Td(column: column, value: linkModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: linkModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: linkModel.UpdatedTime);
                default: return hb;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlInsert Insert(
            Dictionary<long, long> link, bool selectIdentity = false)
        {
            return Rds.InsertLinks(
                param: Rds.LinksParam()
                    .DestinationId()
                    .SourceId(),
                select: Rds.Raw(link.Select(o => "select @_U,@_U,{0},{1} "
                    .Params(
                        o.Key.ToString(),
                        selectIdentity
                            ? Def.Sql.Identity
                            : o.Value.ToString()))
                                .Join("union ") + ";\n"),
                _using: link.Count > 0);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        /// <returns></returns>
        public static SqlJoinCollection JoinByDestination()
        {
            return Rds.LinksJoin()
                .Add("inner join [Items] as [t1] on [t0].[DestinationId]=[t1].[ReferenceId]")
                .Add("inner join [Sites] as [t2] on [t1].[SiteId]=[t2].[SiteId]");
        }
    }
}
