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
        public bool NoCount;
        public SqlStatement Sub = null;
        public string Raw;
        public bool Using = true;
        public bool Updating = true;

        public SqlParam()
        {
        }

        public SqlParam(
            string columnBracket,
            string name,
            object value,
            bool noCount = false,
            SqlStatement sub = null,
            string raw = null,
            bool _using = true,
            bool updating = true)
        {
            ColumnBracket = columnBracket;
            Name = name;
            VariableName = name + "_";
            Value = Correct(value);
            NoCount = noCount;
            Sub = sub;
            Raw = raw;
            Using = _using;
            Updating = updating;
        }

        private object Correct(object self)
        {
            return !(self is DateTime)
                ? self
                : self.ToDateTime().InRange()
                    ? self
                    : DBNull.Value;
        }
    }
}
