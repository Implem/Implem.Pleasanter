﻿using Implem.Libraries.Classes;
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
                Add(new SqlParam(columnBracket, name, value, sub, raw));
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
