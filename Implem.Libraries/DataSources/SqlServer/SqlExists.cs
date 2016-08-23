using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlExists : SqlStatement
    {
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
            commandText.Append("exists(select * from ", TableBracket, " ");
            SqlJoinCollection?.BuildCommandText(commandText);
            SqlWhereCollection?.BuildCommandText(
                sqlContainer, sqlCommand, commandText, commandCount);
            commandText.Append(")");
            AddTermination(commandText, UnionType);
            AddParams_Where(sqlCommand, commandCount);
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