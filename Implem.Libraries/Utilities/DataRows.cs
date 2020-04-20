using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Libraries.Utilities
{
    public static class DataRows
    {
        public static List<string> Columns(this EnumerableRowCollection<DataRow> dataRows)
        {
            return dataRows?.FirstOrDefault().Columns();
        }

        public static List<string> Columns(this DataRow dataRow)
        {
            var columns = new List<string>();
            if (dataRow != null)
            {
                foreach (DataColumn column in dataRow.Table.Columns)
                {
                    columns.Add(column.ColumnName);
                }
            }
            return columns;
        }

        public static bool Bool(this DataRow dataRow, string name)
        {
            return dataRow?.Table.Columns.Contains(name) == true
                ? dataRow[name].ToBool()
                : false;
        }

        public static int Int(this DataRow dataRow, string name)
        {
            return dataRow?.Table.Columns.Contains(name) == true
                ? dataRow[name].ToInt()
                : 0;
        }

        public static long Long(this DataRow dataRow, string name)
        {
            return dataRow?.Table.Columns.Contains(name) == true
                ? dataRow[name].ToLong()
                : 0;
        }

        public static decimal Decimal(this DataRow dataRow, string name)
        {
            return dataRow?.Table.Columns.Contains(name) == true
                ? dataRow[name].ToDecimal()
                : 0;
        }

        public static DateTime DateTime(this DataRow dataRow, string name)
        {
            return dataRow?.Table.Columns.Contains(name) == true
                ? dataRow[name].ToDateTime()
                : System.DateTime.MinValue;
        }

        public static string String(this DataRow dataRow, string name)
        {
            if (dataRow?.Table.Columns.Contains(name) == true
                && dataRow[name] != DBNull.Value)
            {
                switch (dataRow.Table.Columns[name].DataType.Name)
                {
                    case "Boolean":
                        return dataRow.Field<bool>(name).ToString();
                    case "Int32":
                        return dataRow.Field<int>(name).ToString();
                    case "Int64":
                        return dataRow.Field<long>(name).ToString();
                    case "Decimal":
                        return dataRow.Field<decimal>(name).ToString();
                    case "DateTime":
                        return dataRow.Field<DateTime>(name).ToString();
                    case "Double":
                        return dataRow.Field<double>(name).ToString();
                    default:
                        return dataRow.Field<string>(name);
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static byte[] Bytes(this DataRow dataRow, string name)
        {
            return dataRow?.Table.Columns.Contains(name) == true
                ? dataRow[name] is DBNull 
                    ? null
                    : dataRow[name] as byte[]
                : null;
        }

        public static object Object(this DataRow dataRow, string name)
        {
            return dataRow?.Table.Columns.Contains(name) == true
                ? dataRow.Field<object>(name) ?? null
                : null;
        }
    }
}