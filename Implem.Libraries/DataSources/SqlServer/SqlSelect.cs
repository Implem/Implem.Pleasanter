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
                        from:  "from " + HistoryTableBracket + " as [t0]\n",
                        unionType: UnionType,
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
                        from: "from " + TableBracket + " as [t0]\n",
                        unionType: Sqls.UnionTypes.Union,
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
                        from: "from " + TableBracket + " as [t0]\n",
                        unionType: Sqls.UnionTypes.Union,
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
                        from:  "from " + DeletedTableBracket + " as [t0]\n",
                        unionType: UnionType,
                        orderBy: true,
                        countRecord: CountRecord,
                        commandCount: commandCount);
                    break;
                default:
                    BuildCommandText(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        from: "from " + TableBracket + " as [t0]\n",
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
            string from,
            Sqls.UnionTypes unionType,
            bool orderBy,
            bool countRecord,
            int? commandCount)
        {
            if (!Using) return;
            if (!DataTableName.IsNullOrEmpty()) sqlContainer.DataTableNames.Add(DataTableName);
            SqlColumnCollection?.BuildCommandText(
                sqlContainer,
                sqlCommand,
                commandText,
                commandCount,
                Distinct,
                Top);
            commandText.Append(from);
            SqlJoinCollection?.BuildCommandText(commandText);
            SqlWhereCollection?.BuildCommandText(
                sqlContainer, sqlCommand, commandText, commandCount, select: true);
            SqlGroupByCollection?.BuildCommandText(commandText);
            SqlHavingCollection?.BuildCommandText(
                sqlContainer, sqlCommand, commandText, commandCount);
            if (orderBy)
            {
                SqlOrderByCollection?.BuildCommandText(commandText, PageSize, commandCount);
            }
            AddTermination(commandText, unionType);
            if (countRecord)
            {
                sqlContainer.DataTableNames.Add("Count");
                commandText.Append("select count(*) from ( select 1 as [c] ");
                commandText.Append(from);
                SqlJoinCollection?.BuildCommandText(commandText);
                SqlWhereCollection?.BuildCommandText(
                    sqlContainer, sqlCommand, commandText, commandCount, select: true);
                SqlGroupByCollection?.BuildCommandText(commandText);
                SqlHavingCollection?.BuildCommandText(
                    sqlContainer, sqlCommand, commandText, commandCount);
                commandText.Append(") as [table_count]");
                AddTermination(commandText, unionType);
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
