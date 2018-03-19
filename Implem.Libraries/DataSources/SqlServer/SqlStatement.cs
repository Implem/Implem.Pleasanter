using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlStatement
    {
        public string CommandText = string.Empty;
        public Sqls.TableTypes TableType = Sqls.TableTypes.Normal;
        public string TableBracket = string.Empty;
        public string HistoryTableBracket = string.Empty;
        public string DeletedTableBracket = string.Empty;
        public SqlColumnCollection SqlColumnCollection = new SqlColumnCollection();
        public SqlJoinCollection SqlJoinCollection = new SqlJoinCollection();
        public SqlWhereCollection SqlWhereCollection = new SqlWhereCollection();
        public SqlOrderByCollection SqlOrderByCollection = new SqlOrderByCollection();
        public SqlParamCollection SqlParamCollection = new SqlParamCollection();
        public SqlGroupByCollection SqlGroupByCollection = new SqlGroupByCollection();
        public SqlHavingCollection SqlHavingCollection = new SqlHavingCollection();
        public bool AddUpdatorParam = true;
        public bool AddUpdatedTimeParam = true;
        public bool SelectIdentity;
        public bool CountRecord;
        public string If;
        public bool Not = false;
        public bool Terminate = true;
        public bool Using = true;
        public Sqls.UnionTypes UnionType;

        public SqlStatement()
        {
        }

        public SqlStatement(
            string commandText,
            SqlParamCollection param)
        {
            CommandText = commandText;
            SqlParamCollection = param;
        }

        public SqlStatement(string commandText)
        {
            CommandText = commandText;
        }

        public virtual void BuildCommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount = null)
        {
            if (!Using) return;
            commandText.Append(CommandText);
            AddParams_Param(sqlCommand, commandCount);
        }

        public string GetCommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            string prefix = "",
            int? commandCount = null)
        {
            var commandText = new StringBuilder();
            Terminate = false;
            SqlWhereCollection?.Prefix(prefix);
            SqlParamCollection?.Prefix(prefix);
            BuildCommandText(sqlContainer, sqlCommand, commandText, commandCount);
            return commandText.ToString();
        }

        protected void AddParam(
            SqlCommand sqlCommand, string name, object value, int? commandCount)
        {
            var key = name + commandCount;
            if (!sqlCommand.Parameters.Contains(key))
            {
                sqlCommand.Parameters.Add(new SqlParameter(key, value));
            }
        }

        protected void AddParams_Where(SqlCommand sqlCommand, int? commandCount)
        {
            AddParams_Where(sqlCommand, SqlWhereCollection, commandCount);
        }

        private void AddParams_Where(
            SqlCommand sqlCommand, SqlWhereCollection sqlWhereCollection, int? commandCount)
        {
            sqlWhereCollection?
                .Where(o => o != null)
                .Where(o => o.Using)
                .ForEach(sqlWhere =>
                {
                    if (sqlWhere.Value != null)
                    {
                        if (sqlWhere.Value.IsCollection())
                        {
                            (sqlWhere.Value.ToObjectEnumerable())
                                .Select((o, i) => new { Value = o, Index = i })
                                .ForEach(data =>
                                    AddParam(
                                        sqlCommand,
                                        sqlWhere.Name + data.Index + "_",
                                        data.Value,
                                        commandCount));
                        }
                        else
                        {
                            AddParam(
                                sqlCommand,
                                sqlWhere.Name,
                                sqlWhere.Value,
                                commandCount);
                        }
                    }
                    AddParams_Where(sqlCommand, sqlWhere.Or, commandCount);
                });
        }

        protected void AddParams_Having(SqlCommand sqlCommand, int? commandCount)
        {
            AddParams_Having(sqlCommand, SqlHavingCollection, commandCount);
        }

        private void AddParams_Having(
            SqlCommand sqlCommand, SqlHavingCollection sqlHavingCollection, int? commandCount)
        {
            sqlHavingCollection?
                .Where(o => o.Using)
                .Where(o => o.Value != null)
                .ForEach(sqlHaving =>
                {
                    if (sqlHaving.Value.IsCollection())
                    {
                        (sqlHaving.Value.ToObjectEnumerable())
                            .Select((o, i) => new { Value = o, Index = i })
                            .ForEach(data =>
                                AddParam(
                                    sqlCommand,
                                    sqlHaving.TableName + data.Index + "_",
                                    data.Value,
                                    commandCount));
                    }
                    else
                    {
                        AddParam(
                            sqlCommand,
                            sqlHaving.TableName,
                            sqlHaving.Value,
                            commandCount);
                    }
                });
        }

        protected void AddParams_Param(SqlCommand sqlCommand, int? commandCount)
        {
            SqlParamCollection?
                .Where(o => o.Using)
                .Where(o => o.Raw.IsNullOrEmpty())
                .Where(o => o.Value != null)
                .ForEach(sqlParam =>
                    AddParam(sqlCommand, sqlParam.VariableName, sqlParam.Value, commandCount));
        }

        protected void AddUnion(StringBuilder commandText, Sqls.UnionTypes unionType)
        {
            if (unionType != Sqls.UnionTypes.None && commandText.ToString().EndsWith(";"))
            {
                commandText.Length -= 1;
            }
            switch (unionType)
            {
                case Sqls.UnionTypes.Union:
                    commandText.Append(" union ");
                    break;
                case Sqls.UnionTypes.UnionAll:
                    commandText.Append(" union all ");
                    break;
            }
        }

        protected void AddTermination(StringBuilder commandText)
        {
            if (Terminate && !commandText.ToString().EndsWith(";"))
            {
                commandText.Append(";");
            }
        }

        protected void Build_SelectIdentity(
            StringBuilder commandText, bool selectIdentity, int? commandCount)
        {
            if (selectIdentity)
            {
                commandText.Append("set @_I = @@identity select @_I;\n");
            }
        }

        protected void Build_CountRecord(StringBuilder commandText)
        {
            if (CountRecord)
            {
                commandText.Append("select @@rowcount;\n");
            }
        }

        protected void Build_If(StringBuilder commandText)
        {
            if (!If.IsNullOrEmpty())
            {
                commandText.Append("if (" + If + ") begin\n");
            }
        }

        protected void Build_EndIf(StringBuilder commandText)
        {
            if (!If.IsNullOrEmpty())
            {
                commandText.Append("end\n");
            }
        }

        public string GetTableBracket()
        {
            switch (TableType)
            {
                case Sqls.TableTypes.Normal: return TableBracket;
                case Sqls.TableTypes.History: return HistoryTableBracket;
                case Sqls.TableTypes.Deleted: return DeletedTableBracket;
                default: return null;
            }
        }
    }
}
