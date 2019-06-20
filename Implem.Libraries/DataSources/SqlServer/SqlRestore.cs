using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlRestore : SqlStatement
    {
        public SqlRestore()
        {
        }

        public override void BuildCommandText(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount = null)
        {
            if (!Using) return;
            Build_If(commandText);
            Build_RestoreStatement(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount);
            AddParams_Where(
                factory: factory,
                sqlCommand: sqlCommand,
                commandCount: commandCount);
            AddParams_Param(
                factory: factory,
                sqlCommand: sqlCommand,
                commandCount: commandCount);
            AddTermination(commandText);
            Build_EndIf(commandText);
        }

        private void Build_RestoreStatement(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer, 
            ISqlCommand sqlCommand, 
            StringBuilder commandText, 
            int? commandCount)
        {
            commandText.Append(CommandText
                .Params(SqlWhereCollection.Sql(
                    factory: factory,
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandCount: commandCount)));
        }
    }
}
