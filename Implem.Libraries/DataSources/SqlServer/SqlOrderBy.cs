﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Implem.IRds;
using Implem.Libraries.Utilities;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlOrderBy
    {
        public string TableName;
        public string ColumnBracket;
        public Types OrderType;
        public string IsNullValue;
        public Sqls.Functions Function;
        public SqlStatement Sub;
        public string Raw;

        [JsonConverter(typeof(StringEnumConverter))]
        public enum Types
        {
            asc,
            desc,
            release
        }

        public SqlOrderBy(
            string columnBracket = null,
            Types orderType = Types.asc,
            string tableName = null,
            string isNullValue = null,
            Sqls.Functions function = Sqls.Functions.None,
            SqlStatement sub = null,
            string raw = null)
        {
            ColumnBracket = columnBracket;
            OrderType = orderType;
            TableName = tableName;
            IsNullValue = isNullValue;
            Function = function;
            Sub = sub;
            Raw = raw;
        }

        public string Sql(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            string tableBracket,
            Sqls.TableTypes tableType)
        {
            var orderType = " " + OrderType.ToString().ToLower();
            if (!Raw.IsNullOrEmpty())
            {
                return Raw;
            }
            else if (Sub != null)
            {
                return Sql_Sub(
                    factory: factory,
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    orderType: orderType);
            }
            else
            {
                string columnBracket = Sqls.TableAndColumnBracket(
                    tableBracket: tableType == Sqls.TableTypes.NormalAndHistory
                        ? string.Empty
                        : tableBracket,
                    columnBracket: ColumnBracket);
                switch (Function)
                {
                    case Sqls.Functions.Count:
                    case Sqls.Functions.Sum:
                    case Sqls.Functions.Min:
                    case Sqls.Functions.Max:
                        return
                            Function.ToString().ToLower() +
                            "(" +
                            columnBracket +
                            ")" +
                            orderType;
                    case Sqls.Functions.Avg:
                        return $"avg({factory.Sqls.IsNull}({columnBracket}, 0)) {orderType}";
                    default:
                        return IsNullValue.IsNullOrEmpty()
                            ? columnBracket + orderType
                            : $"{factory.Sqls.IsNull}({columnBracket}, {IsNullValue}) {orderType}";
                }
            }
        }

        private string Sql_Sub(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            string orderType)
        {
            return "(" + Sub.GetCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand) + ")"
                    + orderType;
        }
    }
}
