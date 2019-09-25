using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlWhereCollection : ListEx<SqlWhere>, IJoin
    {
        public string Clause = "where ";
        public string MultiClauseOperator = " and ";

        public SqlWhereCollection(params SqlWhere[] sqlWhereCollection)
        {
            AddRange(sqlWhereCollection);
        }

        public SqlWhereCollection Add(
            string tableName = null,
            string[] columnBrackets = null,
            string name = null,
            object value = null,
            string _operator = "=",
            string multiColumnOperator = " or ",
            string multiParamOperator = " and ",
            SqlStatement subLeft = null,
            SqlStatement sub = null,
            bool subPrefix = true,
            string raw = null,
            bool _using = true)
        {
            Add(new SqlWhere(
                columnBrackets: columnBrackets,
                tableName: tableName,
                name: name,
                value: value,
                _operator: _operator,
                multiColumnOperator: multiColumnOperator,
                multiParamOperator: multiParamOperator,
                subLeft: subLeft,
                sub: sub,
                subPrefix: subPrefix,
                raw: raw,
                _using: _using));
            return this;
        }

        public SqlWhereCollection Add(SqlWhereCollection or = null, bool _using = true)
        {
            Add(new SqlWhere(or: or, _using: _using));
            return this;
        }

        public void BuildCommandText(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount,
            bool select = false)
        {
            commandText.Append(Sql(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandCount: commandCount,
                select: select));
        }

        public string Sql(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            int? commandCount,
            bool select = false)
        {
            if (!select)
            {
                this.Where(o => o?.ColumnBrackets != null)
                    .ForEach(where => where.ColumnBrackets =
                        where.ColumnBrackets.Select(o => o.Split('.').Last()).ToArray());
            }
            return this.Where(o => o != null).Any(o => o.Using)
                ? Clause + this
                    .Where(o => o != null)
                    .Where(o => o.Using)
                    .Select(o => o.Sql(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        tableBracket: Sqls.GetTableBracket(o.TableName),
                        commandCount: commandCount,
                        select: select))
                    .Join(MultiClauseOperator) + " "
                : string.Empty;
        }

        public void Prefix(string prefix)
        {
            this
                .Where(o => o.Name?.RegexExists("^[A-Z0-9]{32}$") != true)
                .ForEach(o =>
                    o.Name += prefix);
        }

        public List<string> JoinTableNames()
        {
            var data = this
                .Where(o => o != null)
                .Select(o => o.TableName.CutEnd("_Items"))
                .Where(o => o?.Contains("~") == true)
                .ToList();
            this
                .Where(o => o != null)
                .Select(o => o.Or)
                .Where(o => o != null)
                .ForEach(o => data.AddRange(o.JoinTableNames()));
            return data
                .Distinct()
                .ToList();
        }
    }
}