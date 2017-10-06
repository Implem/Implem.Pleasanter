using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlRestore : SqlStatement
    {
        public SqlRestore()
        {
        }

        public override void BuildCommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount = null)
        {
            if (!Using) return;
            Build_If(commandText);
            Build_RestoreStatement(sqlContainer, sqlCommand, commandText, commandCount);
            AddParams_Where(sqlCommand, commandCount);
            AddParams_Param(sqlCommand, commandCount);
            AddTermination(commandText);
            Build_CountRecord(commandText);
            Build_EndIf(commandText);
        }

        private void Build_RestoreStatement(
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
