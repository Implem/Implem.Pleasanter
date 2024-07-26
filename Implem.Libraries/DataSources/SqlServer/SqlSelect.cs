using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlSelect : SqlStatement
    {
        public string As;
        public bool Distinct;
        public int Top;
        public int PageSize;
        public int Offset;
        public bool Union;

        public SqlSelect()
        {
        }

        public override void BuildCommandText(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount = null)
        {
            if (!MainQueryInfo.sqlClass.IsNullOrEmpty())
            {
                SetMainQueryInfoForSub();
            }
            switch (TableType)
            {
                case Sqls.TableTypes.History:
                    AddTableTypeColumn("History");
                    BuildHistoryWithoutFlag(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount,
                        unionType: Sqls.UnionTypes.None);
                    break;
                case Sqls.TableTypes.HistoryWithoutFlag:
                    BuildHistoryWithoutFlag(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount,
                        unionType: Sqls.UnionTypes.None);
                    break;
                case Sqls.TableTypes.NormalAndDeleted:
                    BuildNormalAndDeleted(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount);
                    break;
                case Sqls.TableTypes.NormalAndHistory:
                    BuildNormalAndHistory(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount);
                    break;
                case Sqls.TableTypes.Deleted:
                    BuildDeleted(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount,
                        unionType: Sqls.UnionTypes.None);
                    break;
                case Sqls.TableTypes.All:
                    AddTableTypeColumn("Normal");
                    BuildNormal(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount);
                    BuildDeleted(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount,
                        unionType: Sqls.UnionTypes.Union);
                    AddTableTypeColumn("History");
                    BuildHistoryWithoutFlag(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount,
                        unionType: Sqls.UnionTypes.Union);
                    break;
                default:
                    BuildNormal(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount);
                    break;
            }
        }

        private void BuildHistoryWithoutFlag(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount,
            Sqls.UnionTypes unionType)
        {
            BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.History,
                unionType: unionType,
                orderBy: true,
                commandCount: commandCount);
        }

        private void BuildNormalAndDeleted(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount)
        {
            AddTableTypeColumn("Normal");
            BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Normal,
                unionType: Sqls.UnionTypes.None,
                orderBy: false,
                commandCount: commandCount);
            BuildDeleted(
                factory,
                sqlContainer,
                sqlCommand,
                commandText,
                commandCount,
                Sqls.UnionTypes.Union);
        }

        private void BuildNormalAndHistory(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount)
        {
            AddTableTypeColumn("Normal");
            BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Normal,
                unionType: Sqls.UnionTypes.None,
                orderBy: false,
                commandCount: commandCount);
            AddTableTypeColumn("History");
            BuildHistoryWithoutFlag(
                factory,
                sqlContainer,
                sqlCommand,
                commandText,
                commandCount,
                Sqls.UnionTypes.Union);
        }

        private void BuildDeleted(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount,
            Sqls.UnionTypes unionType)
        {
            AddTableTypeColumn("Deleted");
            BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Deleted,
                unionType: unionType,
                orderBy: true,
                commandCount: commandCount);
        }

        private void BuildNormal(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount)
        {
            BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Normal,
                unionType: UnionType,
                orderBy: true,
                commandCount: commandCount);
        }

        private void AddTableTypeColumn(string type)
        {
            var history = type == "History" ? "1" : "0";
            var deleted = type == "Deleted" ? "1" : "0";
            switch (TableType)
            {
                case Sqls.TableTypes.NormalAndDeleted:
                    SqlColumnCollection?.Add(new SqlColumn(
                        deleted + " as \"IsDeleted\"", adHoc: true));
                    break;
                case Sqls.TableTypes.NormalAndHistory:
                    SqlColumnCollection?.Add(new SqlColumn(
                        history + " as \"IsHistory\"", adHoc: true));
                    break;
                case Sqls.TableTypes.All:
                    SqlColumnCollection?.Add(new SqlColumn(
                        deleted + " as \"IsDeleted\"", adHoc: true));
                    SqlColumnCollection?.Add(new SqlColumn(
                        history + " as \"IsHistory\"", adHoc: true));
                    break;
                default:
                    break;
            }
        }

        private void BuildCommandText(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            Sqls.TableTypes tableType,
            Sqls.UnionTypes unionType,
            bool orderBy,
            int? commandCount)
        {
            if (!Using) return;
            var tableBrackets = new List<string> { GetTableBracketText(tableType).ToLower() };
            SqlJoinCollection?.ForEach(o => tableBrackets.Add(o.TableBracket.ToLower()));
            if (Parameters.Rds.Dbms == "MySQL" &&
                !MainQueryInfo.sqlClass.IsNullOrEmpty() &&
                tableBrackets.Contains(MainQueryInfo.tableBracket.ToLower()))
            {
                //MySQLにおいて副問い合わせのselect文の生成時、メインのクエリと同一テーブルを参照する場合は、
                //select ... from (select ... from ...) as "仮テーブル名" 形式のコマンドを生成する。
                GetSelectFromSelectCommand(
                    factory: factory,
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandText: commandText,
                    tableType: tableType,
                    unionType: unionType,
                    orderBy: orderBy,
                    commandCount: commandCount);
            }
            else
            {
                GetSelectFromTableCommand(
                    factory: factory,
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandText: commandText,
                    tableType: tableType,
                    unionType: unionType,
                    orderBy: orderBy,
                    commandCount: commandCount);
            }
        }

        private void GetSelectFromSelectCommand(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            Sqls.TableTypes tableType,
            Sqls.UnionTypes unionType,
            bool orderBy,
            int? commandCount)
        {
            var subQueryStart = new StringBuilder("select ");
            var temporaryTableBracket = As.IsNullOrEmpty()
                ? TableBracket.Remove(TableBracket.Length - 1) + "Temp\""
                : "\"" + As + "Temp\"";
            subQueryStart.Append(temporaryTableBracket);
            subQueryStart.Append('.');
            var columnAsBracket = SqlColumnCollection.FirstOrDefault().AsBracket();
            if (columnAsBracket.IsNullOrEmpty())
            {
                subQueryStart.Append("\"");
                subQueryStart.Append(SqlColumnCollection.FirstOrDefault().ColumnName);
                subQueryStart.Append("\"");
            }
            else
            {
                subQueryStart.Append(columnAsBracket.Replace(" as ", string.Empty));
            }
            subQueryStart.Append(" from (");
            var subQueryEnd = new StringBuilder(") as ");
            subQueryEnd.Append(temporaryTableBracket);
            commandText.Append(subQueryStart);
            GetSelectFromTableCommand(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: tableType,
                unionType: unionType,
                orderBy: orderBy,
                commandCount: commandCount);
            commandText.Append(subQueryEnd);
        }

        private void GetSelectFromTableCommand(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            Sqls.TableTypes tableType,
            Sqls.UnionTypes unionType,
            bool orderBy,
            int? commandCount)
        {
            AddUnion(commandText, unionType);
            SqlColumnCollection?.BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount,
                distinct: Distinct,
                top: Top);
            var from = From(tableType, As);
            commandText.Append(from);
            SqlJoinCollection?.BuildCommandText(commandText: commandText);
            SqlWhereCollection?.BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount,
                select: true);
            SqlGroupByCollection?.BuildCommandText(
                commandText: commandText);
            SqlHavingCollection?.BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount);
            if (orderBy)
            {
                SqlOrderByCollection?.BuildCommandText(
                    factory: factory,
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandText: commandText,
                    pageSize: PageSize,
                    tableType: TableType,
                    commandCount: commandCount);
            }
            commandText.Append(
                factory.SqlCommandText.CreateLimitClause(limit: Top));
            AddTermination(commandText);
            AddParams_Where(factory, sqlCommand, commandCount);
            AddParams_Having(factory, sqlCommand, commandCount);
            AddParams_Paging(factory, sqlCommand, commandCount);
            AddParams_Param(factory, sqlCommand, commandCount);
        }

        private void AddParams_Paging(ISqlObjectFactory factory, ISqlCommand sqlCommand, int? commandCount)
        {
            if (PageSize != 0)
            {
                AddParam(
                    factory: factory,
                    sqlCommand: sqlCommand,
                    name: $"{Parameters.Parameter.SqlParameterPrefix}Offset",
                    value: Offset,
                    commandCount: commandCount);
                AddParam(
                    factory: factory,
                    sqlCommand: sqlCommand,
                    name: $"{Parameters.Parameter.SqlParameterPrefix}PageSize",
                    value: PageSize,
                    commandCount: commandCount);
            }
        }

        private string From(Sqls.TableTypes tableType, string _as)
        {
            return "from " + GetTableBracketText(tableType) + (!_as.IsNullOrEmpty()
                ? " as \"" + _as + "\""
                : " as " + TableBracket) + "\n";
        }

        private string GetTableBracketText(Sqls.TableTypes tableType)
        {
            switch (tableType)
            {
                case Sqls.TableTypes.History:
                    return HistoryTableBracket;
                case Sqls.TableTypes.Deleted:
                    return DeletedTableBracket;
                default:
                    return TableBracket;
            }
        }

        private void SetMainQueryInfoForSub()
        {
            //もし現在処理中のselect文がサブクエリである場合（メインのupdateやdeleteのクエリの情報が設定済みの場合）、かつ、
            //現在のselect文のさらに配下にサブクエリが存在する場合に、配下のサブクエリにメインの情報を設定する
            SqlWhereCollection
                .Where(o => o.Sub != null)
                .ForEach(o => o.Sub.SetMainQueryInfo(
                    sqlClass: MainQueryInfo.sqlClass,
                    tableBracket: MainQueryInfo.tableBracket));
        }
    }
}
