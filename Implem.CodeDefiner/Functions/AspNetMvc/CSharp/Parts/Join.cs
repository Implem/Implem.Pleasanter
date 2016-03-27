using Implem.CodeDefiner.Utilities;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class Join
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            Def.ColumnDefinitionCollection
                .Where(o => !Column.CheckExclude(codeDefinition, o))
                .Where(o => o.TableName == dataContainer.TableName)
                .OrderBy(o => o.No)
                .ForEach(columnDefinition =>
                {
                    dataContainer.ColumnName = columnDefinition.ColumnName;
                    Creators.SetCodeCollection(
                        ref code, codeCollection, codeDefinition, dataContainer, () =>
                        {
                            var tableNameAlias = TableNameAlias(columnDefinition);
                            ReplaceCodeOfJoin(
                                ref code,
                                codeDefinition,
                                columnDefinition,
                                tableNameAlias: TableNameAlias(columnDefinition),
                                columnNameAlias: ColumnNameAlias(columnDefinition),
                                joinExpression: Epression(
                                    columnDefinition,
                                    tableNameAlias,
                                    dataContainer.TableName));
                        });
                    dataContainer.ColumnName = string.Empty;
                });
        }

        private static string ColumnNameAlias(
            ColumnDefinition columnDefinition, string prefix = "")
        {
            return columnDefinition.ComputeColumn != string.Empty || prefix != string.Empty
                ? " as " + "[" + columnDefinition.ColumnName + prefix + "]"
                : string.Empty;
        }

        internal static string TableNameAlias(ColumnDefinition columnDefinition)
        {
            if (columnDefinition.JoinTableName != string.Empty)
            {
                return "t" + Def.ColumnDefinitionCollection
                    .Where(o => o.TableName == columnDefinition.TableName)
                    .Where(o => o.JoinTableName != string.Empty)
                    .Select(o => new
                    {
                        JoinTableName = o.JoinTableName,
                        JoinType = o.JoinType,
                        JoinExpression = o.JoinExpression
                    })
                    .Distinct()
                    .Select((o, i) => new { Index = i + 1, Data = o })
                    .Where(o => o.Data.JoinTableName == columnDefinition.JoinTableName)
                    .Where(o => o.Data.JoinType == columnDefinition.JoinType)
                    .Where(o => o.Data.JoinExpression == columnDefinition.JoinExpression)
                    .FirstOrDefault().Index;
            }
            else
            {
                return "t0";
            }
        }

        internal static string Epression(
            ColumnDefinition columnDefinition, string tableNameAlias, string tableName)
        {
            if (columnDefinition.JoinTableName != string.Empty)
            {
                return columnDefinition.JoinExpression
                    .Replace("#TableName#", columnDefinition.TableName)
                    .Replace("#Alias#", TableName(columnDefinition, tableNameAlias))
                    .Replace(
                        "#BeforeTableNameAlias#",
                        Strings.CoalesceEmpty(tableNameAlias.ChangePrefixNumber(-1)));
            }
            else
            {
                return string.Empty;
            }
        }

        private static string ColumnName(
            ColumnDefinition columnDefinition, string tableNameAlias)
        {
            return
                "[" + TableName(columnDefinition, tableNameAlias) + "]." +
                "[" + columnDefinition.ColumnName + "]";
        }

        internal static void ReplaceCodeOfJoin(
            ref string code,
            CodeDefinition codeDefinition,
            ColumnDefinition columnDefinition,
            string tableNameAlias,
            string columnNameAlias,
            string joinExpression)
        {
            foreach (Match placeholder in code.RegexMatches(
                CodePatterns.ReplacementPlaceholder))
            {
                switch (placeholder.Value)
                {
                    case "TableName":
                        code = code.Replace("#TableName#", columnDefinition.TableName);
                        break;
                    case "ColumnName":
                        code = code.Replace("#ColumnName#", columnDefinition.ColumnName);
                        break;
                    case "Type":
                        code = code.Replace("#Type#", columnDefinition.TypeName.CsType());
                        break;
                    case "ColumnNameBracket":
                        code = code.Replace(
                            "#ColumnNameBracket#",
                            "\"[" + columnDefinition.ColumnName + "]\"");
                        break;
                    case "ColumnBracket":
                        code = code.Replace(
                            "#ColumnBracket#",
                            ColumnBracket(
                                columnDefinition,
                                tableNameAlias));
                        break;
                    case "ColumnBrackets":
                        code = code.Replace(
                            "#ColumnBrackets#",
                            ColumnBrackets(
                                columnDefinition, 
                                tableNameAlias));
                        break;
                    case "ColumnBracketsWithAlias":
                        code = code.Replace(
                            "#ColumnBracketsWithAlias#",
                            ColumnBrackets(
                                columnDefinition, 
                                tableNameAlias, 
                                columnNameAlias));
                        break;
                    case "ColumnTotal":
                        code = code.Replace(
                            "#ColumnTotal#",
                            ComputeColumn(
                                columnDefinition,
                                "sum",
                                tableNameAlias));
                        break;
                    case "ColumnTotalWithAlias":
                        code = code.Replace(
                            "#ColumnTotalWithAlias#",
                            ComputeColumn(
                                columnDefinition,
                                "sum",
                                tableNameAlias,
                                ColumnNameAlias(columnDefinition, "Total")));
                        break;
                    case "ColumnAverage":
                        code = code.Replace(
                            "#ColumnAverage#",
                            ComputeColumn(
                                columnDefinition,
                                "avg",
                                tableNameAlias));
                        break;
                    case "ColumnAverageWithAlias":
                        code = code.Replace(
                            "#ColumnAverageWithAlias#",
                            ComputeColumn(
                                columnDefinition,
                                "avg",
                                tableNameAlias,
                                ColumnNameAlias(columnDefinition, "Average")));
                        break;
                    case "ColumnMax":
                        code = code.Replace(
                            "#ColumnMax#",
                            ComputeColumn(
                                columnDefinition,
                                "max",
                                tableNameAlias));
                        break;
                    case "ColumnMaxWithAlias":
                        code = code.Replace(
                            "#ColumnMaxWithAlias#",
                            ComputeColumn(
                                columnDefinition,
                                "max",
                                tableNameAlias,
                                ColumnNameAlias(columnDefinition, "Max")));
                        break;
                    case "ColumnMin":
                        code = code.Replace(
                            "#ColumnMin#",
                            ComputeColumn(
                                columnDefinition,
                                "min",
                                tableNameAlias));
                        break;
                    case "ColumnMinWithAlias":
                        code = code.Replace(
                            "#ColumnMinWithAlias#",
                            ComputeColumn(
                                columnDefinition,
                                "min",
                                tableNameAlias,
                                ColumnNameAlias(columnDefinition, "Min")));
                        break;
                    case "OrderByColumns":
                        code = code.Replace(
                            "#OrderByColumns#", 
                            OrderByColumns(
                                columnDefinition,
                                tableNameAlias));
                        break;
                    case "Alias":
                        code = code
                            .Replace("#Alias#", columnNameAlias)
                            .Replace("#BeforeAlias#", columnNameAlias.ChangePrefixNumber(-1));
                        break;
                    case "JoinTableName":
                        code = code.Replace("#JoinTableName#", columnDefinition.JoinTableName);
                        break;
                    case "JoinType":
                        code = code.Replace("#JoinType#", columnDefinition.JoinType);
                        break;
                    case "TableNameAlias":
                        code = code.Replace("#TableNameAlias#", " as [" + tableNameAlias + "]");
                        break;
                    case "JoinExpression":
                        code = code.Replace("#JoinExpression#", joinExpression);
                        break;
                    case "RecordingData":
                        code = code.Replace("#RecordingData#", columnDefinition.RecordingData);
                        break;
                    case "MaxLength":
                        code = code.Replace(
                            "#MaxLength#", Column.CodeMaxLength(columnDefinition));
                        break;
                    default:
                        break;
                }
            }
            if (codeDefinition.ReplaceOld != string.Empty)
            {
                code = code.Replace(codeDefinition.ReplaceOld, codeDefinition.ReplaceNew);
            }
        }

        private static string OrderByColumns(
            ColumnDefinition columnDefinition,
            string tableNameAlias)
        {
            return columnDefinition.OrderByColumns == string.Empty
                ? columnDefinition.ComputeColumn == string.Empty
                    ? "\"" + ColumnName(columnDefinition, tableNameAlias) + "\""
                    : "\"" + columnDefinition.ComputeColumn
                        .ReplaceTableName(columnDefinition, tableNameAlias) + "\""
                : columnDefinition.OrderByColumns.Split(',')
                    .Select(o => "\"" + o
                        .ReplaceTableName(columnDefinition, tableNameAlias) + "\"")
                    .Join(", ");
        }

        private static string ColumnBrackets(
            ColumnDefinition columnDefinition,
            string tableNameAlias,
            string columnNameAlias = "")
        {
            return columnDefinition.SelectColumns == string.Empty
                ? columnDefinition.ComputeColumn == string.Empty
                    ? ColumnBracket(columnDefinition, tableNameAlias, columnNameAlias)
                    : ComputeColumn(columnDefinition, string.Empty, tableNameAlias, columnNameAlias)
                : SelectColumns(columnDefinition, tableNameAlias, columnNameAlias);
        }

        private static string SelectColumns(
            ColumnDefinition columnDefinition, 
            string tableNameAlias,
            string columnNameAlias)
        {
            return columnDefinition.SelectColumns
                .Split(',')
                .Select(o => "\"" +
                    o.ReplaceTableName(columnDefinition, tableNameAlias) +
                    columnNameAlias + "\"")
                .Join(", ");
        }

        private static string ColumnBracket(
            ColumnDefinition columnDefinition,
            string tableNameAlias,
            string columnNameAlias = "")
        {
            return "\"" + ColumnName(columnDefinition, tableNameAlias).ReplaceTableName(
                columnDefinition, tableNameAlias) + columnNameAlias + "\"";
        }

        private static string ComputeColumn(
            ColumnDefinition columnDefinition,
            string method,
            string tableNameAlias,
            string columnNameAlias = "")
        {
            var column = ComputeColumn(columnDefinition, tableNameAlias)
                .ReplaceTableName(columnDefinition, tableNameAlias);
            return method != string.Empty
                ? "\"" + method + "(" + column + ")" + columnNameAlias + "\""
                : "\"" + column + columnNameAlias + "\"";
        }

        internal static string ComputeColumn(
            ColumnDefinition columnDefinition, string tableNameAlias)
        {
            return columnDefinition.ComputeColumn != string.Empty
                ? columnDefinition.ComputeColumn
                    .ReplaceTableName(columnDefinition, tableNameAlias)
                : ColumnName(columnDefinition, tableNameAlias);
        }

        private static string ReplaceTableName(
            this string self, ColumnDefinition columnDefinition, string tableNameAlias)
        {
            return self.Replace("#TableName#", TableName(columnDefinition, tableNameAlias));
        }

        private static string TableName(ColumnDefinition columnDefinition, string tableNameAlias)
        {
            return Strings.CoalesceEmpty(tableNameAlias, columnDefinition.TableName);
        }
    }
}
