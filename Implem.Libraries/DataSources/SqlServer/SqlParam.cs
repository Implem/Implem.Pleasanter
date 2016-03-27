using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlParam
    {
        public string ColumnBracket;
        public string Name;
        public string VariableName;
        public object Value;
        public SqlStatement Sub = null;
        public string Raw;
        public bool Using = true;
        public bool Updating = true;

        public SqlParam(
            string columnBracket,
            string name,
            object value,
            SqlStatement sub = null,
            string raw = null,
            bool _using = true,
            bool updating = true)
        {
            ColumnBracket = columnBracket;
            Name = name;
            VariableName = name + "_Param";
            Value = Correct(value);
            Sub = sub;
            Raw = raw;
            Using = _using;
            Updating = updating;
        }

        public SqlParam(
            string name,
            object value)
        {
            VariableName = name;
            Value = Correct(value);
        }

        private object Correct(object self)
        {
            return !(self is DateTime)
                ? self
                : self.ToDateTime().NotZero()
                    ? self
                    : DBNull.Value;
        }
    }

    public class SqlParamCollection : ListEx<SqlParam>
    {
        public SqlParamCollection(params SqlParam[] sqlParamCollection)
        {
            this.AddRange(sqlParamCollection);
        }

        public SqlParamCollection Add(
            string columnBracket,
            string name,
            object value,
            SqlStatement sub,
            string raw,
            bool _using)
        {
            base.Add(new SqlParam(columnBracket, name, value, sub, raw, _using: _using));
            return this;
        }

        public void Prefix(string prefix)
        {
            this.ForEach(o => o.VariableName += prefix);
        }
    }
}
