using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Linq;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlParamCollection : ListEx<SqlParam>
    {
        public SqlParamCollection(params SqlParam[] sqlParamCollection)
        {
            AddRange(sqlParamCollection);
        }

        public SqlParamCollection Add(
            string columnBracket,
            string name,
            object value,
            SqlStatement sub,
            string raw,
            bool _using = true)
        {
            if (_using)
            {
                Add(new SqlParam(
                    columnBracket: columnBracket,
                    name: name,
                    value: value,
                    sub: sub,
                    raw: raw));
            }
            return this;
        }

        public SqlParamCollection Add(string variableName, object value, bool _using = true)
        {
            if (_using)
            {
                Add(new SqlParam()
                {
                    VariableName = variableName,
                    Value = value
                });
            }
            return this;
        }

        public void Prefix(string prefix)
        {
            this
                .Where(o => o.VariableName?.RegexExists("^[A-Z0-9]{32}$") != true)
                .ForEach(o =>
                    o.VariableName += prefix);
        }
    }
}
