using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount = null)
        {
            if (!Using) return;
            Build_If(commandText);
            Build_UpdateOrInsertStatement(
                sqlContainer, sqlCommand, commandText, commandCount);
            Build_SelectIdentity(commandText, SelectIdentity, commandCount);
            AddParams_Where(sqlCommand, commandCount);
            AddParams_Param(sqlCommand, commandCount);
            AddTermination(commandText);
            Build_EndIf(commandText);
        }

        private void Build_UpdateOrInsertStatement(
            SqlContainer sqlContainer, 
            SqlCommand sqlCommand, 
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
            if (AddUpdatorParam) updateColumnNameCollection.Add("[Updator] = @_U");
            if (AddUpdatedTimeParam) updateColumnNameCollection.Add("[UpdatedTime] = getdate()");
            var insertColumnNameCollection = new List<string>
            {
                "[Creator]",
                "[Updator]"
            };
            var valueCollection = new List<string> { "@_U", "@_U" };
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
                                        sqlParam.ColumnBracket + "=@_I");
                                }
                                valueCollection.Add(
                                    sqlParam.ColumnBracket + "@_I");
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
            commandText.Append(
                "update ", tableBracket,
                " set ", updateColumnNameCollection.Join(), " ");
            SqlWhereCollection.BuildCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount);
            commandText.Append(
                " if @@rowcount = 0 insert into ",
                tableBracket,
                "(", insertColumnNameCollection.Join(), ") values(", valueCollection.Join(), ")");
        }
    }
}
