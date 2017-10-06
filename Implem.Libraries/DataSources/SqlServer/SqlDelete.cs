using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlDelete : SqlStatement
    {
        public SqlDelete()
        {
        }

        public override void BuildCommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount = null)
        {
            if (!Using) return;
            Build_If(commandText: commandText);
            Build_DeleteStatement(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount);
            AddParams_Where(
                sqlCommand: sqlCommand,
                commandCount: commandCount);
            AddParams_Param(
                sqlCommand: sqlCommand,
                commandCount: commandCount);
            AddTermination(commandText: commandText);
            Build_CountRecord(commandText: commandText);
            Build_EndIf(commandText: commandText);
        }

        private void Build_DeleteStatement(
            SqlContainer sqlContainer, 
            SqlCommand sqlCommand, 
            StringBuilder commandText, 
            int? commandCount)
        {
            commandText.Append(CommandText
                .Params(SqlWhereCollection.Sql(
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandCount: commandCount)));
        }
    }
}
