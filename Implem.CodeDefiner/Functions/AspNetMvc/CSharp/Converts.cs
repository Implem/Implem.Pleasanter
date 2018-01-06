using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Text.RegularExpressions;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal static class Converts
    {
        internal static string ConvertType(
            this string code,
            string convertType,
            ColumnDefinition columnDefinition)
        {
            var placeholder = Placeholder(code, convertType);
            string variable = Variable(placeholder);
            return columnDefinition.EnumColumn
                ? code.Replace(placeholder, ToEnum(columnDefinition, variable))
                : columnDefinition.TypeCs.IsNullOrEmpty()
                    ? code.Replace(placeholder, variable + columnDefinition.TypeName.CastType())
                    : TypeCs(code, convertType, columnDefinition, placeholder, variable);
        }

        private static string Placeholder(string code, string convertType)
        {
            return Strings.EnclosedString(code.Substring(code.IndexOf(convertType)));
        }

        private static string Variable(string placeholder)
        {
            return placeholder.RegexFirst(@"(?<=#.*?#\().*(?=\)$)", RegexOptions.Multiline);
        }

        private static string ToEnum(ColumnDefinition columnDefinition, string codeVariable)
        {
            return "(" + columnDefinition.TypeCs + ")" +
                codeVariable + columnDefinition.RecordingData;
        }

        internal static string ByForm(this string code, ColumnDefinition columnDefinition)
        {
            var placeholder = Placeholder(code, "#ByForm#");
            return columnDefinition.ByForm != string.Empty
                ? code.Replace(placeholder, columnDefinition.ByForm)
                : code.ConvertType("#ByForm#", columnDefinition);
        }

        internal static string ByApi(this string code, ColumnDefinition columnDefinition)
        {
            var placeholder = Placeholder(code, "#ByApi#");
            return columnDefinition.ByApi != string.Empty
                ? code.Replace(placeholder, columnDefinition.ByApi)
                : code.ConvertType("#ByApi#", columnDefinition);
        }

        internal static string ByDataRow(this string code, ColumnDefinition columnDefinition)
        {
            var placeholder = Placeholder(code, "#ByDataRow#");
            return columnDefinition.ByDataRow != string.Empty
                ? code.Replace(placeholder, columnDefinition.ByDataRow)
                : code.ConvertType("#ByDataRow#", columnDefinition);
        }

        internal static string BySession(this string code, ColumnDefinition columnDefinition)
        {
            var placeholder = Placeholder(code, "#BySession#");
            return columnDefinition.BySession != string.Empty
                ? code.Replace(placeholder, columnDefinition.BySession)
                : code.ConvertType("#BySession#", columnDefinition);
        }

        private static string TypeCs(
            string code, 
            string convertFrom, 
            ColumnDefinition columnDefinition, 
            string placeholder, 
            string codeVariable)
        {
            switch (columnDefinition.TypeCs + convertFrom)
            {
                case "Title#ByForm#":
                case "Body#ByForm#":
                case "Status#ByForm#":
                    return code.Replace(
                        placeholder,
                        CreateObjectByForm(columnDefinition));
                case "Time#ByForm#":
                case "CompletionTime#ByForm#":
                    return code.Replace(
                        placeholder,
                        CreateObjectByForm(columnDefinition, ", byForm: true"));
                case "Title#ByApi#":
                case "Body#ByApi#":
                case "Status#ByApi#":
                    return code.Replace(
                        placeholder,
                        CreateObjectByApi(columnDefinition));
                case "Time#ByApi#":
                case "CompletionTime#ByApi#":
                    return code.Replace(
                        placeholder,
                        CreateObjectByApi(columnDefinition, ", byForm: true"));
                case "Title#ByDataRow#":
                case "Body#ByDataRow#":
                case "Status#ByDataRow#":
                case "CompletionTime#ByDataRow#":
                case "ProgressRate#ByDataRow#":
                case "WorkValue#ByDataRow#":
                    return code.Replace(
                        placeholder,
                        CreateObjectByDataRow(columnDefinition, "column"));
                case "Time#ByDataRow#":
                    return code.Replace(
                        placeholder,
                        CreateObjectByDataRow(columnDefinition, "column.ColumnName"));
                default:
                    return code.Replace(
                        placeholder, AsPrefix(columnDefinition, codeVariable));
            }
        }

        private static string CreateObjectByForm(
            ColumnDefinition columnDefinition, string additionalArguments = "")
        {
            return "new {0}(Forms.Data(controlId){1}{2})".Params(
                columnDefinition.TypeCs,
                columnDefinition.TypeName.CastType(),
                additionalArguments);
        }

        private static string CreateObjectByApi(
            ColumnDefinition columnDefinition, string additionalArguments = "")
        {
            return "new {0}(data.{1}{2}{3})".Params(
                columnDefinition.TypeCs,
                columnDefinition.ColumnName,
                columnDefinition.TypeName.CastType(),
                additionalArguments);
        }

        private static string CreateObjectByDataRow(
            ColumnDefinition columnDefinition, string param)
        {
            return "new {0}(dataRow, {1})".Params(columnDefinition.TypeCs, param);
        }

        internal static string CastType(this string type)
        {
            switch (type.CsType())
            {
                case Types.CsString: return ".ToString()";
                case Types.CsInt: return ".ToInt()";
                case Types.CsLong: return ".ToLong()";
                case Types.CsDecimal: return ".ToDecimal()";
                case Types.CsSingle: return ".ToSingle()";
                case Types.CsDouble: return ".ToDouble()";
                case Types.CsDateTime: return ".ToDateTime()";
                case Types.CsBool: return ".ToBool()";
                default: return string.Empty;
            }
        }

        private static string AsPrefix(
            ColumnDefinition columnDefinition, string codeVariable)
        {
            return codeVariable + " as " + columnDefinition.TypeCs +
                (columnDefinition.DefaultCs != string.Empty && columnDefinition.DefaultCs != "null"
                    ? " ?? " + columnDefinition.DefaultCs
                    : string.Empty);
        }
    }
}
