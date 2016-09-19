using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlHavingCollection : ListEx<SqlHaving>
    {
        public string Clause = "having ";
        public string MultiClauseOperator = " and ";

        public SqlHavingCollection(params SqlHaving[] sqlHavingCollection)
        {
            this.AddRange(sqlHavingCollection);
        }

        public SqlHavingCollection Add(
            string columnBracket = null,
            string name = "",
            object value = null,
            string _operator = "=",
            string multiParamOperator = " and ",
            SqlStatement sub = null,
            string raw = "",
            SqlHavingCollection or = null,
            bool _using = true)
        {
            base.Add(new SqlHaving(
                columnBracket: columnBracket,
                name: name,
                value: value,
                _operator: _operator,
                multiParamOperator: multiParamOperator,
                sub: sub,
                raw: raw,
                _using: _using));
            return this;
        }

        public void BuildCommandText(
            SqlContainer sqlContainer, 
            SqlCommand sqlCommand, 
            StringBuilder commandText, 
            int? commandCount)
        {
            commandText.Append(Sql(sqlContainer, sqlCommand, commandCount));
        }

        public string Sql(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            int? commandCount)
        {
            return this.Any(o => o.Using)
                ? Clause + this
                    .Where(o => o.Using)
                    .Select(o => o.Sql(sqlContainer, sqlCommand, commandCount))
                    .Join(MultiClauseOperator) + " "
                : string.Empty;
        }

        public void Prefix(string prefix)
        {
            this.ForEach(o => o.Name += prefix);
        }
    }
}