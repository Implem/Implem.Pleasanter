using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace Implem.Libraries.Utilities
{
    public static class Types
    {
        public const string CsInt = "int";
        public const string CsLong = "long";
        public const string CsDecimal = "decimal";
        public const string CsSingle = "Single";
        public const string CsDouble = "double";
        public const string CsString = "string";
        public const string CsDateTime = "DateTime";
        public const string CsBool = "bool";
        public const string CsNumeric = "numeric";
        public const string CsBytes = "byte[]";

        public static string CsType(this string type) 
        {
            switch (type.ToLower().Trim())
            {
                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                case "string":
                    return CsString;
                case "int":
                case "smallint":
                    return CsInt;
                case "bigint":
                case "long":
                    return CsLong;
                case "decimal":
                    return CsDecimal;
                case "float":
                case "real":
                    return CsDouble;
                case "datetime":
                case "datetime2":
                case "date":
                case "smalldatetime":
                    return CsDateTime;
                case "bit":
                case "bool":
                    return CsBool;
                case "image":
                    return CsBytes;
                default:
                    return type;
            }
        }

        public static string CsTypeSummary(this string type) 
        {
            switch (type.ToLower().Trim())
            {
                case "string":
                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                    return CsString;
                case "int":
                case "long":
                case "smallint":
                case "bigint":
                case "decimal":
                case "float":
                case "real":
                    return CsNumeric;
                case "datetime":
                case "datetime2":
                case "date":
                case "smalldatetime":
                    return CsDateTime;
                case "bit":
                case "bool":
                    return CsBool;
                case "image":
                    return CsBytes;
                default:
                    return string.Empty;
            }
        }

        public static string ToString(this decimal self, string intFormat, string decimalFormat)
        {
            return self.ToString(decimalFormat).RegexExists(@"\.0+$")
                ? self.ToString(intFormat)
                : self.ToString(decimalFormat);
        }

        public static string ToStr(this object self) 
        {
            return (self ?? string.Empty).ToString();
        }

        public static int ToInt(this int? self)
        {
            return self != null
                ? Convert.ToInt32(self)
                : 0;
        }

        public static int ToInt(this long? self)
        {
            return self != null && self <= int.MaxValue && self >= int.MinValue
                ? Convert.ToInt32(self)
                : 0;
        }

        public static int ToInt(this long self)
        {
            return self <= int.MaxValue && self >= int.MinValue
                ? Convert.ToInt32(self)
                : 0;
        }

        public static int ToInt(this decimal self)
        {
            return self <= int.MaxValue && self >= int.MinValue
                ? Convert.ToInt32(self)
                : 0;
        }

        public static int ToInt(this decimal? self)
        {
            return self != null && self <= int.MaxValue && self >= int.MinValue
                ? Convert.ToInt32(self)
                : 0;
        }

        public static int ToInt(this float self)
        {
            return self <= int.MaxValue && self >= int.MinValue
                ? Convert.ToInt32(self)
                : 0;
        }

        public static int ToInt(this float? self)
        {
            return self != null && self <= int.MaxValue && self >= int.MinValue
                ? Convert.ToInt32(self)
                : 0;
        }

        public static int ToInt(this double self)
        {
            return self <= int.MaxValue && self >= int.MinValue
                ? Convert.ToInt32(self)
                : 0;
        }

        public static int ToInt(this double? self)
        {
            return self != null && self <= int.MaxValue && self >= int.MinValue
                ? Convert.ToInt32(self)
                : 0;
        }

        public static int ToInt(this object self) 
        {
            if (self != null)
            {
                var data = 0;
                if (int.TryParse(self.ToString(), out data))
                {
                    return data;
                }
                if (self is Enum)
                {
                    return Convert.ToInt32(self);
                }
            }
            return 0;
        }

        public static long ToLong(this int self)
        {
            return Convert.ToInt64(self);
        }

        public static long ToLong(this int? self)
        {
            return self != null
                ? Convert.ToInt64(self)
                : 0;
        }

        public static long ToLong(this long? self)
        {
            return self != null
                ? Convert.ToInt64(self)
                : 0;
        }

        public static long ToLong(this decimal self)
        {
            return self <= long.MaxValue && self >= long.MinValue
                ? Convert.ToInt64(self)
                : 0;
        }

        public static long ToLong(this decimal? self)
        {
            return self != null && self <= long.MaxValue && self >= long.MinValue
                ? Convert.ToInt64(self)
                : 0;
        }

        public static long ToLong(this float self)
        {
            return self <= long.MaxValue && self >= long.MinValue
                ? Convert.ToInt64(self)
                : 0;
        }

        public static long ToLong(this float? self)
        {
            return self != null && self <= long.MaxValue && self >= long.MinValue
                ? Convert.ToInt64(self)
                : 0;
        }

        public static long ToLong(this double self)
        {
            return self <= long.MaxValue && self >= long.MinValue
                ? Convert.ToInt64(self)
                : 0;
        }

        public static long ToLong(this double? self)
        {
            return self != null && self <= long.MaxValue && self >= long.MinValue
                ? Convert.ToInt64(self)
                : 0;
        }

        public static long ToLong(this object self) 
        {
            if (self != null)
            {
                long data;
                if (long.TryParse(self.ToString(), out data))
                {
                    return data;
                }
                if (self is Enum)
                {
                    return Convert.ToInt64(self);
                }
            }
            return 0;
        }

        public static decimal ToDecimal(this object self) 
        {
            decimal data;
            if (self != null && decimal.TryParse(self.ToString(), out data))
            {
                return data;
            }
            else
            {
                return 0;
            }
        }

        public static decimal ToDecimal(this object self, CultureInfo cultureInfo)
        {
            decimal data;
            if (self != null && decimal.TryParse(
                self.ToString(), NumberStyles.Any, cultureInfo, out data))
            {
                return data;
            }
            else
            {
                return 0;
            }
        }

        public static float ToSingle(this object self) 
        {
            float data;
            return self != null && float.TryParse(self.ToString(), out data) ? data : 0;
        }

        public static double ToDouble(this object obj) 
        {
            double data;
            return obj != null && double.TryParse(obj.ToString(), out data) ? data : 0;
        }

        public static float ToFloat(this object obj)
        {
            float data;
            return obj != null && float.TryParse(obj.ToString(), out data) ? data : 0;
        }

        public static DateTime ToDateTime(this object obj)
        {
            DateTime data;
            var str = obj.ToStr();
            return obj != null && DateTime.TryParse(str, out data)
                ? data
                : obj.ToDecimal() > 0
                    ? DateTime.FromOADate(double.Parse(str))
                    : DateTime.FromOADate(0);
        }

        public static bool ToBool(this object obj) 
        {
            switch (obj?.GetType().Name)
            {
                case "Boolean":
                    return Convert.ToBoolean(obj);
                case "String":
                    return
                        obj.ToString().Trim() == "1" ||
                        obj.ToString().Trim() == "-1" ||
                        obj.ToString().Trim().ToLower() == "true" ||
                        obj.ToString().Trim().ToLower() == "on";
                case "Int":
                case "Int32":
                case "Long":
                case "Single":
                case "Double":
                    return
                        Convert.ToInt32(obj) == 1 ||
                        Convert.ToInt32(obj) == -1;
                default:
                    return false;
            }
        }

        public static bool IsCollection(this object self)
        {
            return self != null && !(self is string) && self.GetType().GetInterfaces().Any(o =>
                o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }
    }
}
