﻿using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Linq;
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
            SetMainQueryInfoForSub();
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
            commandText.Append(factory.SqlCommandText.CreateRestore(
                template: CommandText
                .Params(SqlWhereCollection.Sql(
                    factory: factory,
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandCount: commandCount))));
        }

        private string GetTableBracketText()
        {
            switch (TableType)
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
            SqlWhereCollection
                .Where(o => o.Sub != null)
                .ForEach(o => o.Sub.SetMainQueryInfo(
                    sqlClass: GetType().ToString(),
                    allTableBrackets: GetAllTableBrackets()));
        }
    }
}
