using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlInsert : SqlStatement
    {
        public SqlStatement Select;

        public SqlInsert()
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
            Build_InsertStatement(sqlContainer, sqlCommand, commandText, commandCount);
            Build_SelectIdentity(commandText, SelectIdentity, commandCount);
            AddParams_Param(sqlCommand, commandCount);
            AddTermination(commandText);
            Build_EndIf(commandText);
        }

        private void Build_InsertStatement(
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
            var columnNameCollection = new List<string>();
            var valueCollection = new List<string>();
            if (AddUpdatorParam)
            {
                columnNameCollection.Add("[Creator]");
                columnNameCollection.Add("[Updator]");
                valueCollection.Add("@_U");
                valueCollection.Add("@_U");
            }
            SqlParamCollection?
                .Where(o => o.Using)
                .ForEach(sqlParam =>
                {
                    columnNameCollection.Add(sqlParam.ColumnBracket);
                    if (!sqlParam.Raw.IsNullOrEmpty())
                    {
                        switch (sqlParam.Raw?.ToString())
                        {
                            case "@@identity":
                                valueCollection.Add("@_I");
                                break;
                            default:
                                valueCollection.Add(sqlParam.Raw);
                                break;
                        }
                    }
                    else if (sqlParam.Sub != null)
                    {
                        valueCollection.Add("(" + sqlParam.Sub.GetCommandText(
                            sqlContainer: sqlContainer,
                            sqlCommand: sqlCommand,
                            prefix: "_sub",
                            commandCount: commandCount) + ")");
                    }
                    else
                    {
                        valueCollection.Add("@" + sqlParam.VariableName + commandCount
                            .ToString());
                    }
                });
            commandText.Append(
                "insert into ",
                tableBracket,
                "(", columnNameCollection.Join(), ") ",
                Values(
                    valueCollection,
                    sqlContainer,
                    sqlCommand,
                    commandCount));
        }

        private string Values(
            IEnumerable<string> valueCollection,
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            int? commandCount)
        {
            if (Select == null)
            {
                return "values(" + valueCollection.Join() + "); ";
            }
            else
            {
                if (AddUpdatorParam)
                {
                    Select.SqlColumnCollection.InsertRange(0, new List<SqlColumn>
                    {
                        new SqlColumn("@_U as [Creator]"),
                        new SqlColumn("@_U as [Updator]")
                    });
                }
                return Select.GetCommandText(
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    prefix: "_sub",
                    commandCount: commandCount);
            }
        }
    }
}