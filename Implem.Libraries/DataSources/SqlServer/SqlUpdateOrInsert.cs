using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlUpdateOrInsert : SqlStatement
    {
        public SqlUpdateOrInsert()
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
            Build_UpdateOrInsertStatement(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount);
            Build_SelectIdentity(factory: factory, commandText: commandText);
            AddParams_Where(
                factory: factory,
                sqlCommand: sqlCommand,
                commandCount: commandCount);
            AddParams_Param(
                factory: factory,
                sqlCommand: sqlCommand,
                commandCount: commandCount);
            AddTermination(commandText: commandText);
            Build_EndIf(commandText: commandText);
        }

        private void Build_UpdateOrInsertStatement(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer, 
            ISqlCommand sqlCommand, 
            StringBuilder commandText, 
            int? commandCount)
        {
            var tableBracket = TableBracket;
            switch (TableType)
            {
                case Sqls.TableTypes.History: tableBracket = HistoryTableBracket; break;
                case Sqls.TableTypes.Deleted: tableBracket = DeletedTableBracket; break;
            }
            var updateColumnNameCollection = new List<string>();
            if (AddUpdatorParam) updateColumnNameCollection.Add($"\"Updator\" = {Parameters.Parameter.SqlParameterPrefix}U");
            if (AddUpdatedTimeParam) updateColumnNameCollection.Add($"\"UpdatedTime\" = {factory.Sqls.CurrentDateTime} ");
            var insertColumnNameCollection = new List<string>
            {
                "\"Creator\"",
                "\"Updator\""
            };
            var valueCollection = new List<string> { $"{Parameters.Parameter.SqlParameterPrefix}U", $"{Parameters.Parameter.SqlParameterPrefix}U" };
            SqlParamCollection
                .Where(o => (o as SqlParam).Using)
                .ForEach(sqlParam =>
                {
                    insertColumnNameCollection.Add(sqlParam.ColumnBracket);
                    if (!sqlParam.Raw.IsNullOrEmpty())
                    {
                        switch (sqlParam.Raw?.ToString())
                        {
                            case "@@identity":
                                if (sqlParam.Updating)
                                {
                                    updateColumnNameCollection.Add(
                                        sqlParam.ColumnBracket + $"={Parameters.Parameter.SqlParameterPrefix}I");
                                }
                                valueCollection.Add(
                                    sqlParam.ColumnBracket + $"{Parameters.Parameter.SqlParameterPrefix}I");
                                break;
                            default:
                                if (sqlParam.Updating)
                                {
                                    updateColumnNameCollection.Add(
                                        sqlParam.ColumnBracket + "=" + sqlParam.Raw
                                            .Replace("#CommandCount#", commandCount.ToString()));
                                }
                                valueCollection.Add(sqlParam.Raw
                                    .Replace("#CommandCount#", commandCount.ToString()));
                                break;
                        }
                    }
                    else if (sqlParam.Sub != null)
                    {
                        var sub = sqlParam.Sub.GetCommandText(
                            factory: factory,
                            sqlContainer: sqlContainer,
                            sqlCommand: sqlCommand,
                            prefix: "_sub",
                            commandCount: commandCount);
                        if (sqlParam.Updating)
                        {
                            updateColumnNameCollection.Add(sqlParam.ColumnBracket +
                                "=(" + sub + ")");
                        }
                        valueCollection.Add("(" + sub + ")");
                    }
                    else
                    {
                        if (sqlParam.Updating)
                        {
                            updateColumnNameCollection.Add(sqlParam.ColumnBracket +
                                "=@" + sqlParam.VariableName + commandCount.ToStr());
                        }
                        valueCollection.Add("@" + sqlParam.VariableName + commandCount.ToStr());
                    }
                });
            commandText.Append(factory.SqlCommandText.CreateUpdateOrInsert(
                tableBracket: tableBracket,
                setClause: $" set {updateColumnNameCollection.Join()} ",
                sqlWhereAppender: commandText_ =>
                    SqlWhereCollection.BuildCommandText(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText_,
                        commandCount: commandCount),
                intoClause: insertColumnNameCollection.Join(),
                valueClause: valueCollection.Join()));
        }
    }
}
