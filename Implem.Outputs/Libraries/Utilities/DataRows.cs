using Implem.Libraries.Utilities;
using System;
using System.Data;
namespace Implem.Pleasanter.Libraries.Utilities
{
    public static class DataRows
    {
        public static bool Bool(this DataRow dataRow, string name)
        {
            return dataRow.Table.Columns.Contains(name)
                ? dataRow[name].ToBool()
                : false;
        }

        public static int Int(this DataRow dataRow, string name)
        {
            return dataRow.Table.Columns.Contains(name)
                ? dataRow[name].ToInt()
                : 0;
        }

        public static long Long(this DataRow dataRow, string name)
        {
            return dataRow.Table.Columns.Contains(name)
                ? dataRow[name].ToLong()
                : 0;
        }

        public static Decimal Decimal(this DataRow dataRow, string name)
        {
            return dataRow.Table.Columns.Contains(name)
                ? dataRow[name].ToDecimal()
                : 0;
        }

        public static DateTime DateTime(this DataRow dataRow, string name)
        {
            return dataRow.Table.Columns.Contains(name)
                ? dataRow[name].ToDateTime()
                : System.DateTime.MinValue;
        }

        public static string String(this DataRow dataRow, string name)
        {
            return dataRow.Table.Columns.Contains(name)
                ? dataRow.Field<string>(name) ?? string.Empty
                : string.Empty;
        }

        public static byte[] Bytes(this DataRow dataRow, string name)
        {
            return dataRow.Table.Columns.Contains(name)
                ? dataRow[name] is DBNull 
                    ? dataRow[name] as byte[]
                    : null
                : null;
        }
    }
}