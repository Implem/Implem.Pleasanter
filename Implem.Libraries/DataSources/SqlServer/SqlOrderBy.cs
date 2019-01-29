﻿namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlOrderBy
    {
        public string TableName;
        public string ColumnBracket;
        public Types OrderType;
        public Sqls.Functions Function;

        public enum Types
        {
            asc,
            desc,
            release
        }

        public SqlOrderBy(
            string columnBracket,
            Types orderType = Types.asc,
            string tableName = null,
            Sqls.Functions function = Sqls.Functions.None)
        {
            TableName = tableName;
            ColumnBracket = columnBracket;
            OrderType = orderType;
            Function = function;
        }

        public string Sql(string tableBracket, Sqls.TableTypes tableType)
        {
            string columnBracket = Sqls.TableAndColumnBracket(
                tableBracket: tableBracket,
                columnBracket: ColumnBracket);
            var orderType = " " + OrderType.ToString().ToLower();
            switch (Function)
            {
                case Sqls.Functions.Count:
                case Sqls.Functions.Sum:
                case Sqls.Functions.Min:
                case Sqls.Functions.Max:
                case Sqls.Functions.Avg:
                    return
                        Function.ToString().ToLower() +
                        "(" +
                        columnBracket +
                        ")" +
                        orderType;
                default:
                    return columnBracket + orderType;
            }
        }
    }
}
