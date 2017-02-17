using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlSelect : SqlStatement
    {
        public string DataTableName;
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
                    SqlColumnCollection?.Add(new SqlColumn("1 as [IsHistory]", adHoc: true));
                    goto case Sqls.TableTypes.HistoryWithoutFlag;
                case Sqls.TableTypes.HistoryWithoutFlag:
                    BuildCommandText(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        tableType: Sqls.TableTypes.History,
                        from:  "from " + HistoryTableBracket + "\n",
                        unionType: TableType == Sqls.TableTypes.NormalAndHistory
                            ? Sqls.UnionTypes.Union
                            : Sqls.UnionTypes.None,
                        orderBy: true,
                        countRecord: CountRecord,
                        commandCount: commandCount);
                    break;
                case Sqls.TableTypes.NormalAndDeleted:
                    SqlColumnCollection?.Add(new SqlColumn("0 as [IsDeleted]", adHoc: true));
                    BuildCommandText(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        tableType: Sqls.TableTypes.Normal,
                        from: "from " + TableBracket + "\n",
                        unionType: Sqls.UnionTypes.None,
                        orderBy: false,
                        countRecord: false,
                        commandCount: commandCount);
                    DataTableName = null;
                    goto case Sqls.TableTypes.Deleted;
                case Sqls.TableTypes.NormalAndHistory:
                    SqlColumnCollection?.Add(new SqlColumn("0 as [IsHistory]", adHoc: true));
                    BuildCommandText(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        tableType: Sqls.TableTypes.Normal,
                        from: "from " + TableBracket + "\n",
                        unionType: Sqls.UnionTypes.None,
                        orderBy: false,
                        countRecord: false,
                        commandCount: commandCount);
                    DataTableName = null;
                    goto case Sqls.TableTypes.History;
                case Sqls.TableTypes.Deleted:
                    SqlColumnCollection?.Add(new SqlColumn("1 as [IsDeleted]", adHoc: true));
                    BuildCommandText(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        tableType: Sqls.TableTypes.Deleted,
                        from: "from " + DeletedTableBracket + "\n",
                        unionType: TableType == Sqls.TableTypes.NormalAndDeleted
                            ? Sqls.UnionTypes.Union
                            : Sqls.UnionTypes.None,
                        orderBy: true,
                        countRecord: CountRecord,
                        commandCount: commandCount);
                    break;
                default:
                    BuildCommandText(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        tableType: Sqls.TableTypes.Normal,
                        from: "from " + TableBracket + "\n",
                        unionType: UnionType,
                        orderBy: true,
                        countRecord: CountRecord,
                        commandCount: commandCount);
                    break;
            }
        }

        private void BuildCommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            Sqls.TableTypes tableType,
            string from,
            Sqls.UnionTypes unionType,
            bool orderBy,
            bool countRecord,
            int? commandCount)
        {
            if (!Using) return;
            if (!DataTableName.IsNullOrEmpty()) sqlContainer.DataTableNames.Add(DataTableName);
            AddUnion(commandText, unionType);
            SqlColumnCollection?.BuildCommandText(
                sqlContainer,
                sqlCommand,
                commandText,
                tableType,
                commandCount,
                Distinct,
                Top);
            commandText.Append(from);
            SqlJoinCollection?.BuildCommandText(commandText);
            SqlWhereCollection?.BuildCommandText(
                sqlContainer, sqlCommand, commandText, TableType, commandCount, select: true);
            SqlGroupByCollection?.BuildCommandText(commandText, tableType);
            SqlHavingCollection?.BuildCommandText(
                sqlContainer, sqlCommand, commandText, tableType, commandCount);
            if (orderBy)
            {
                SqlOrderByCollection?.BuildCommandText(
                    commandText: commandText,
                    pageSize: PageSize,
                    tableType: tableType,
                    unionType: unionType,
                    commandCount: commandCount);
            }
            AddTermination(commandText);
            if (countRecord)
            {
                sqlContainer.DataTableNames.Add("Count");
                commandText.Append("select count(*) from ( select 1 as [c] ");
                commandText.Append(from);
                SqlJoinCollection?.BuildCommandText(commandText);
                SqlWhereCollection?.BuildCommandText(
                    sqlContainer, sqlCommand, commandText, TableType, commandCount, select: true);
                SqlGroupByCollection?.BuildCommandText(commandText, tableType);
                SqlHavingCollection?.BuildCommandText(
                    sqlContainer, sqlCommand, commandText, tableType, commandCount);
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
            if (SqlOrderByCollection?.Count > 0 && PageSize != 0)
            {
                AddParam(sqlCommand, "_Offset", Offset, commandCount);
                AddParam(sqlCommand, "_PageSize", PageSize, commandCount);
            }
        }
    }
}
