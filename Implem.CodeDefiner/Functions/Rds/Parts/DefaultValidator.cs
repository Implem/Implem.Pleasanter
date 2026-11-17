using Implem.CodeDefiner.Functions.Rds.Parts.MySql;
using Implem.DefinitionAccessor;
using Implem.Factory;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds.Parts
{
    internal static class DefaultValidator
    {
        internal static void Validate(IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            var errors = columnDefinitionCollection
                .Where(o => o.Default.IsNullOrEmpty() == false)
                .Where(o => o.NotUpdate == false)
                .Where(o => o.JoinTableName.IsNullOrEmpty())
                .Where(o => o.Calc.IsNullOrEmpty())
                .Select(o => new
                {
                    ColumnDefinition = o,
                    Error = Error(columnDefinition: o)
                })
                .Where(o => o.Error != null)
                .Select(o => $"{o.ColumnDefinition.TableName}.{o.ColumnDefinition.ColumnName}"
                    + $" ({o.ColumnDefinition.TypeName}, Default: \"{o.ColumnDefinition.Default}\")"
                    + $": {o.Error}")
                .ToList();
            if (errors.Count == 0)
            {
                return;
            }
            throw new InvalidOperationException(
                "Invalid default values in the column definitions.\r\n"
                + errors.Join("\r\n"));
        }

        private static string Error(ColumnDefinition columnDefinition)
        {
            var value = columnDefinition.Default;
            switch (columnDefinition.TypeName)
            {
                case "int":
                    return int.TryParse(
                        value,
                        NumberStyles.AllowLeadingSign,
                        CultureInfo.InvariantCulture,
                        out _)
                            ? null
                            : "Must be an integer within the range of int.";
                case "bigint":
                    return long.TryParse(
                        value,
                        NumberStyles.AllowLeadingSign,
                        CultureInfo.InvariantCulture,
                        out _)
                            ? null
                            : "Must be an integer within the range of bigint.";
                case "decimal":
                case "numeric":
                    return DecimalError(
                        value: value,
                        size: columnDefinition.Size);
                case "float":
                    return FloatError(value: value);
            }
            switch (columnDefinition.TypeName.CsTypeSummary())
            {
                case Types.CsBool:
                    return value == "0" || value == "1"
                        ? null
                        : "Must be 0 or 1.";
                case Types.CsDateTime:
                    return value.ToLower() == "now"
                        ? null
                        : "Only \"now\" is supported.";
                case Types.CsString:
                    return StringError(columnDefinition: columnDefinition);
                default:
                    return "Default values are not supported for this type.";
            }
        }

        private static string DecimalError(string value, string size)
        {
            if (decimal.TryParse(
                value,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var number) == false)
            {
                return "Must be a number.";
            }
            var precisionAndScale = size?.Split(',');
            if (precisionAndScale?.Length != 2
                || int.TryParse(precisionAndScale[0], out var precision) == false
                || int.TryParse(precisionAndScale[1], out var scale) == false)
            {
                return null;
            }
            var digits = Math.Abs(number)
                .ToString(Constraints.DecimalFormat, CultureInfo.InvariantCulture)
                .Split('.');
            var integerDigits = digits[0].TrimStart('0').Length;
            var fractionDigits = digits.Length == 2
                ? digits[1].Length
                : 0;
            return integerDigits <= precision - scale
                && fractionDigits <= scale
                    ? null
                    : $"Must fit in decimal({precision},{scale}).";
        }

        private static string FloatError(string value)
        {
            var styles = NumberStyles.AllowLeadingSign
                | NumberStyles.AllowDecimalPoint
                | NumberStyles.AllowExponent;
            return double.TryParse(
                value,
                styles,
                CultureInfo.InvariantCulture,
                out var number)
                && decimal.TryParse(
                    value,
                    styles,
                    CultureInfo.InvariantCulture,
                    out var decimalNumber)
                && (double)decimalNumber == number
                    ? null
                    : "Must be a number that converts to decimal without loss.";
        }

        private static string StringError(ColumnDefinition columnDefinition)
        {
            var value = columnDefinition.Default;
            if (columnDefinition.MaxLength == -1)
            {
                return "Default values are not supported for columns with unlimited length.";
            }
            var maxLength = MaxLength(columnDefinition: columnDefinition);
            if (maxLength > 0
                && value.Length > maxLength)
            {
                return $"Must be {maxLength} characters or less.";
            }
            return value.Any(char.IsControl)
                ? "Control characters are not allowed."
                : null;
        }

        private static int MaxLength(ColumnDefinition columnDefinition)
        {
            return MySqlColumns.IsLargeNvarchar(columnDefinition: columnDefinition)
                ? Math.Min(
                    columnDefinition.MaxLength,
                    RdsFactory.Create("MySQL").SqlDefinitionSetting.ReducedVarcharLength)
                : columnDefinition.MaxLength;
        }
    }
}
