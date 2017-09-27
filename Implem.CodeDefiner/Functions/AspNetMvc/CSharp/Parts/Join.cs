using Implem.CodeDefiner.Utilities;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
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
                                joinType: JoinType(columnDefinition),
                                joinExpression: columnDefinition.JoinExpression);
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
            return columnDefinition.JoinTableName != string.Empty
                ? columnDefinition.JoinTableName
                : columnDefinition.TableName;
        }

        internal static string JoinType(ColumnDefinition columnDefinition)
        {
            switch (columnDefinition.JoinType)
            {
                case "inner join": return "SqlJoin.JoinTypes.Inner";
                case "left outer join": return "SqlJoin.JoinTypes.LeftOuter";
                case "right outer join": return "SqlJoin.JoinTypes.RightOuter";
                default: return string.Empty;
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
            string joinType,
            string joinExpression)
        {
            foreach (var placeholder in code.RegexValues(CodePatterns.ReplacementPlaceholder))
            {
                switch (placeholder)
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
                    case "ColumnBracket":
                        code = code.Replace("#ColumnBracket#", ColumnBracket(columnDefinition));
                        break;
                    case "ColumnBrackets":
                        code = code.Replace(
                            "#ColumnBrackets#",
                            ColumnBrackets(
                                columnDefinition, 
                                tableNameAlias, 
                                columnNameAlias,
                                selectColumnAlias: true));
                        break;
                    case "Columns":
                        code = code.Replace(
                            "#Columns#",
                            Columns(
                                columnDefinition,
                                tableNameAlias,
                                columnNameAlias,
                                selectColumnAlias: true));
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
                    case "As":
                        code = code.Replace(
                            "#As#",
                            columnDefinition.ComputeColumn != string.Empty
                                ? "\"" + columnDefinition.ColumnName + "\""
                                : "null");
                        break;
                    case "JoinTableName":
                        code = code.Replace(
                            "#JoinTableName#",
                            columnDefinition.JoinTableName != string.Empty
                                ? columnDefinition.JoinTableName
                                : columnDefinition.TableName);
                        break;
                    case "JoinType":
                        code = code.Replace("#JoinType#", joinType);
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
                ? "\"" + ColumnBracket(columnDefinition) + "\""
                : columnDefinition.OrderByColumns.Split(',')
                    .Select(o => "\"" + o
                        .ReplaceTableName(columnDefinition, tableNameAlias) + "\"")
                    .Join(", ");
        }

        private static string ColumnBrackets(
            ColumnDefinition columnDefinition,
            string tableNameAlias,
            string columnNameAlias = "",
            bool selectColumnAlias = true)
        {
            return columnDefinition.SelectColumns == string.Empty
                ? columnDefinition.ComputeColumn == string.Empty
                    ? ColumnBracket(columnDefinition, tableNameAlias, columnNameAlias)
                    : ComputeColumn(columnDefinition, string.Empty, tableNameAlias, columnNameAlias)
                : SelectColumns(columnDefinition, tableNameAlias, selectColumnAlias);
        }

        private static string Columns(
            ColumnDefinition columnDefinition,
            string tableNameAlias,
            string columnNameAlias = "",
            bool selectColumnAlias = true)
        {
            return columnDefinition.SelectColumns
                .Split(',')
                .Select(o => "\"" + o.RegexFirst("[a-zA-Z0-9]+") + "\"")
                .Join(", ");
        }

        private static string SelectColumns(
            ColumnDefinition columnDefinition, string tableNameAlias, bool selectColumnAlias)
        {
            return columnDefinition.SelectColumns
                .Split(',')
                .Select(o => o.Trim())
                .Select(o => "\"" + o.ReplaceTableName(columnDefinition, tableNameAlias) + "\"")
                .Join(", ");
        }

        private static string ColumnBracket(
            ColumnDefinition columnDefinition,
            string tableNameAlias,
            string columnNameAlias = "")
        {
            return "\"" + ColumnName(columnDefinition, tableNameAlias).ReplaceTableName(
                columnDefinition, tableNameAlias) + "\"" + ColumnNameAliasCode(columnNameAlias);
        }

        private static string ColumnBracket(ColumnDefinition columnDefinition)
        {
            return columnDefinition.ComputeColumn != string.Empty
                ? columnDefinition.ComputeColumn
                : "[" + columnDefinition.ColumnName + "]";
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
                ? "\"" + method + "(" + column + ")\"" + ColumnNameAliasCode(columnNameAlias)
                : "\"" + column + "\"" + ColumnNameAliasCode(columnNameAlias);
        }

        private static string ColumnNameAliasCode(string columnNameAlias)
        {
            return columnNameAlias != string.Empty
                ? " + (!_as.IsNullOrEmpty() ? \" as [\" + _as + \"]\" : \"" + columnNameAlias + "\")"
                : string.Empty;
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
