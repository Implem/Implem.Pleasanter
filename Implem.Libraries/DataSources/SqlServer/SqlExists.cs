using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlExists : SqlStatement
    {
        public List<SqlStatement> SqlStatements { get; set; }

        public SqlExists()
        {
        }

        public override void BuildCommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount = null)
        {
            if (!Using) return;
            if (Not) commandText.Append("not ");
            if (SqlStatements?.Any() == true)
            {
                commandText.Append("exists(");
                SqlStatements.ForEach(statement =>
                    commandText.Append(statement.GetCommandText(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandCount: commandCount)));
                commandText.Append(")");
            }
            else
            {
                commandText.Append("exists(select * from ", TableBracket, " ");
                SqlJoinCollection?.BuildCommandText(commandText);
                SqlWhereCollection?.BuildCommandText(
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandText: commandText,
                    commandCount: commandCount,
                    select: true);
                commandText.Append(")");
                AddTermination(commandText);
                AddParams_Where(
                    sqlCommand: sqlCommand,
                    commandCount: commandCount);
                switch (TableType)
                {
                    case Sqls.TableTypes.History:
                        commandText = commandText.Replace(TableBracket, HistoryTableBracket);
                        break;
                    case Sqls.TableTypes.Deleted:
                        commandText = commandText.Replace(TableBracket, DeletedTableBracket);
                        break;
                }
            }
        }
    }
}