using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlSelect : SqlStatement
    {
        public string DataTableName;
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
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount = null)
        {
            switch (TableType)
            {
                case Sqls.TableTypes.History:
                    AddTableTypeColumn("History");
                    BuildHistoryWithoutFlag(
                        sqlContainer,
                        sqlCommand,
                        commandText,
                        commandCount,
                        Sqls.UnionTypes.None);
                    break;
                case Sqls.TableTypes.HistoryWithoutFlag:
                    BuildHistoryWithoutFlag(
                        sqlContainer,
                        sqlCommand,
                        commandText,
                        commandCount,
                        Sqls.UnionTypes.None);
                    break;
                case Sqls.TableTypes.NormalAndDeleted:
                    BuildNormalAndDeleted(
                        sqlContainer,
                        sqlCommand,
                        commandText,
                        commandCount);
                    break;
                case Sqls.TableTypes.NormalAndHistory:
                    BuildNormalAndHistory(
                        sqlContainer,
                        sqlCommand,
                        commandText,
                        commandCount);
                    break;
                case Sqls.TableTypes.Deleted:
                    BuildDeleted(
                        sqlContainer,
                        sqlCommand,
                        commandText,
                        commandCount,
                        Sqls.UnionTypes.None);
                    break;
                case Sqls.TableTypes.All:
                    AddTableTypeColumn("Normal");
                    BuildNormal(sqlContainer, sqlCommand, commandText, commandCount);
                    DataTableName = null;
                    BuildDeleted(
                        sqlContainer,
                        sqlCommand,
                        commandText,
                        commandCount,
                        Sqls.UnionTypes.Union);
                    AddTableTypeColumn("History");
                    BuildHistoryWithoutFlag(
                        sqlContainer,
                        sqlCommand,
                        commandText,
                        commandCount,
                        Sqls.UnionTypes.Union);
                    break;
                default:
                    BuildNormal(sqlContainer, sqlCommand, commandText, commandCount);
                    break;
            }
        }

        private void BuildHistoryWithoutFlag(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount,
            Sqls.UnionTypes unionType)
        {
            BuildCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.History,
                unionType: unionType,
                orderBy: true,
                countRecord: CountRecord,
                commandCount: commandCount);
        }

        private void BuildNormalAndDeleted(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount)
        {
            AddTableTypeColumn("Normal");
            BuildCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Normal,
                unionType: Sqls.UnionTypes.None,
                orderBy: false,
                countRecord: false,
                commandCount: commandCount);
            DataTableName = null;
            BuildDeleted(
                sqlContainer,
                sqlCommand,
                commandText,
                commandCount,
                Sqls.UnionTypes.Union);
        }

        private void BuildNormalAndHistory(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount)
        {
            AddTableTypeColumn("Normal");
            BuildCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Normal,
                unionType: Sqls.UnionTypes.None,
                orderBy: false,
                countRecord: false,
                commandCount: commandCount);
            DataTableName = null;
            AddTableTypeColumn("History");
            BuildHistoryWithoutFlag(
                sqlContainer,
                sqlCommand,
                commandText,
                commandCount,
                Sqls.UnionTypes.Union);
        }

        private void BuildDeleted(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount,
            Sqls.UnionTypes unionType)
        {
            AddTableTypeColumn("Deleted");
            BuildCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Deleted,
                unionType: unionType,
                orderBy: true,
                countRecord: CountRecord,
                commandCount: commandCount);
        }

        private void BuildNormal(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount)
        {
            BuildCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Normal,
                unionType: UnionType,
                orderBy: true,
                countRecord: CountRecord,
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
                        deleted + " as [IsDeleted]", adHoc: true));
                    break;
                case Sqls.TableTypes.NormalAndHistory:
                    SqlColumnCollection?.Add(new SqlColumn(
                        history + " as [IsHistory]", adHoc: true));
                    break;
                case Sqls.TableTypes.All:
                    SqlColumnCollection?.Add(new SqlColumn(
                        deleted + " as [IsDeleted]", adHoc: true));
                    SqlColumnCollection?.Add(new SqlColumn(
                        history + " as [IsHistory]", adHoc: true));
                    break;
                default:
                    break;
            }
        }

        private void BuildCommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            Sqls.TableTypes tableType,
            Sqls.UnionTypes unionType,
            bool orderBy,
            bool countRecord,
            int? commandCount)
        {
            if (!Using) return;
            if (!DataTableName.IsNullOrEmpty()) sqlContainer.DataTableNames.Add(DataTableName);
            AddUnion(commandText, unionType);
            SqlColumnCollection?.BuildCommandText(
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
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount,
                select: true);
            SqlGroupByCollection?.BuildCommandText(
                commandText: commandText);
            SqlHavingCollection?.BuildCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount);
            if (orderBy)
            {
                SqlOrderByCollection?.BuildCommandText(
                    commandText: commandText,
                    pageSize: PageSize,
                    tableType: TableType,
                    commandCount: commandCount);
            }
            AddTermination(commandText);
            if (countRecord)
            {
                sqlContainer.DataTableNames.Add("Count");
                commandText.Append("select count(*) from ( select 1 as [c] ");
                commandText.Append(from);
                SqlJoinCollection?.BuildCommandText(commandText: commandText);
                SqlWhereCollection?.BuildCommandText(
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandText: commandText,
                    commandCount: commandCount,
                    select: true);
                SqlGroupByCollection?.BuildCommandText(
                    commandText: commandText);
                SqlHavingCollection?.BuildCommandText(
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandText: commandText,
                    commandCount: commandCount);
                commandText.Append(") as [table_count]");
                AddTermination(commandText);
            }
            AddParams_Where(sqlCommand, commandCount);
            AddParams_Having(sqlCommand, commandCount);
            AddParams_Paging(sqlCommand, commandCount);
            AddParams_Param(sqlCommand, commandCount);
        }

        private void AddParams_Paging(SqlCommand sqlCommand, int? commandCount)
        {
            if (PageSize != 0)
            {
                AddParam(sqlCommand, "_Offset", Offset, commandCount);
                AddParam(sqlCommand, "_PageSize", PageSize, commandCount);
            }
        }

        private string From(Sqls.TableTypes tableType, string _as)
        {
            var tableBlacket = TableBracket;
            switch (tableType)
            {
                case Sqls.TableTypes.History:
                    tableBlacket = HistoryTableBracket;
                    break;
                case Sqls.TableTypes.Deleted:
                    tableBlacket = DeletedTableBracket;
                    break;
            }
            return "from " + tableBlacket + (!_as.IsNullOrEmpty()
                ? " as [" + _as + "]"
                : " as " + TableBracket) + "\n";
        }
    }
}
