using Implem.CodeDefiner.Utilities;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class Column
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            var columnCollection = Def.ColumnDefinitionCollection
                .Where(o => o.TableName == dataContainer.TableName)
                .Where(o => !CheckExclude(codeDefinition, o))
                .OrderBy(o => o[Strings.CoalesceEmpty(codeDefinition.Order, "No")])
                .ToList();
            var count = columnCollection.Count();
            columnCollection.ForEach(columnDefinition =>
            {
                dataContainer.ColumnName = columnDefinition.ColumnName;
                Creators.SetCodeCollection(
                    ref code, 
                    codeCollection,
                    codeDefinition,
                    dataContainer,
                    () => ReplaceCode(
                        ref code,
                        codeDefinition,
                        columnDefinition,
                        count));
                dataContainer.ColumnName = string.Empty;
            });
        }

        internal static void SetCodeCollection_Default(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            if (!Table.CheckExclude(codeDefinition, dataContainer.TableName) ||
                dataContainer.TableName == string.Empty)
            {
                var code = Creators.Create(codeDefinition, dataContainer);
                if (dataContainer.ColumnName != string.Empty)
                {
                    var columnDefinition = Def.ColumnDefinitionCollection.First(o =>
                         o.TableName == dataContainer.TableName &&
                         o.ColumnName == dataContainer.ColumnName);
                    if (!CheckExclude(codeDefinition, columnDefinition))
                    {
                        ReplaceCode(ref code, codeDefinition, columnDefinition);
                        codeCollection.Add(code);
                    }
                }
                else
                {
                    codeCollection.Add(code);
                }
            }
        }

        internal static bool CheckExclude(
            CodeDefinition codeDefinition,
            ColumnDefinition columnDefinition)
        {
            if (codeDefinition.NotJoin && columnDefinition.JoinTableName != string.Empty) return true;
            if (codeDefinition.Join && columnDefinition.JoinTableName == string.Empty) return true;
            if (codeDefinition.JoinExpression && columnDefinition.JoinExpression == string.Empty) return true;
            if (codeDefinition.Pk && columnDefinition.Pk == 0) return true;
            if (codeDefinition.NotPk && columnDefinition.Pk != 0) return true;
            if (codeDefinition.Session && !columnDefinition.Session) return true;
            if (codeDefinition.Identity && !columnDefinition.Identity) return true;
            if (codeDefinition.NotIdentity && columnDefinition.Identity) return true;
            if (codeDefinition.Unique && !columnDefinition.Unique) return true;
            if (codeDefinition.NotUnique && columnDefinition.Unique) return true;
            if (codeDefinition.NotDefault && columnDefinition.Default != string.Empty) return true;
            if (codeDefinition.ItemId &&
                Def.ExistsModel(columnDefinition.ModelName, o => o.ItemId > 0) &&
                columnDefinition.ColumnName != columnDefinition.ModelName + "Id")
                return true;
            if (codeDefinition.NotItemId &&
                Def.ExistsModel(columnDefinition.ModelName, o => o.ItemId > 0) &&
                columnDefinition.ColumnName == columnDefinition.ModelName + "Id")
                return true;
            if (codeDefinition.Calc && columnDefinition.Calc == string.Empty) return true;
            if (codeDefinition.NotCalc && columnDefinition.Calc != string.Empty) return true;
            if (codeDefinition.NotWhereSpecial && columnDefinition.WhereSpecial) return true;
            if (codeDefinition.SearchIndex && columnDefinition.SearchIndexPriority == 0) return true;
            if (codeDefinition.NotByForm && columnDefinition.ByForm != string.Empty) return true;
            if (codeDefinition.Form && columnDefinition.NotForm) return true;
            if (codeDefinition.Select && columnDefinition.NotSelect) return true;
            if (codeDefinition.Update && columnDefinition.NotUpdate) return true;
            if (codeDefinition.SelectColumns && columnDefinition.SelectColumns == string.Empty) return true;
            if (codeDefinition.NotSelectColumn && columnDefinition.SelectColumns != string.Empty) return true;
            if (codeDefinition.ComputeColumn && columnDefinition.ComputeColumn == string.Empty) return true;
            if (codeDefinition.NotSelectColumn && columnDefinition.ComputeColumn != string.Empty) return true;
            if (codeDefinition.Aggregatable && !columnDefinition.Aggregatable) return true;
            if (codeDefinition.Computable && !columnDefinition.Computable) return true;
            if (codeDefinition.Include != string.Empty && !codeDefinition.Include.Split(',').Contains(columnDefinition.ColumnName)) return true;
            if (codeDefinition.Exclude != string.Empty && codeDefinition.Exclude.Split(',').Contains(columnDefinition.ColumnName)) return true;
            if (codeDefinition.IncludeTypeName != string.Empty && !codeDefinition.IncludeTypeName.Split(',').Contains(columnDefinition.TypeName)) return true;
            if (codeDefinition.ExcludeTypeName != string.Empty && codeDefinition.ExcludeTypeName.Split(',').Contains(columnDefinition.TypeName)) return true;
            if (codeDefinition.IncludeTypeCs != string.Empty && !codeDefinition.IncludeTypeCs.Split(',').Contains(columnDefinition.TypeCs)) return true;
            if (codeDefinition.ExcludeTypeCs != string.Empty && codeDefinition.ExcludeTypeCs.Split(',').Contains(columnDefinition.TypeCs)) return true;
            if (codeDefinition.IncludeDefaultCs != string.Empty && !codeDefinition.IncludeDefaultCs.Split(',').Contains(columnDefinition.DefaultCs)) return true;
            if (codeDefinition.ExcludeDefaultCs != string.Empty && codeDefinition.ExcludeDefaultCs.Split(',').Contains(columnDefinition.DefaultCs)) return true;
            if (codeDefinition.NotTypeCs && columnDefinition.TypeCs != string.Empty) return true;
            if (codeDefinition.History && columnDefinition.History == 0) return true;
            if (codeDefinition.PkHistory && columnDefinition.PkHistory == 0) return true;
            if (codeDefinition.GridColumn && columnDefinition.GridColumn == 0) return true;
            if (codeDefinition.FilterColumn && columnDefinition.FilterColumn == 0) return true;
            if (codeDefinition.EditorColumn && !columnDefinition.EditorColumn) return true;
            if (codeDefinition.TitleColumn && columnDefinition.TitleColumn == 0) return true;
            if (codeDefinition.UserColumn && !columnDefinition.UserColumn) return true;
            if (codeDefinition.NotUserColumn && columnDefinition.UserColumn) return true;
            if (codeDefinition.EnumColumn && !columnDefinition.EnumColumn) return true;
            if (codeDefinition.Exclude.Split(',').Contains(columnDefinition.ColumnName)) return true;
            if (codeDefinition.NotItem && Def.ExistsTable(columnDefinition.TableName, o => o.ItemId > 0)) return true;
            if (codeDefinition.ItemOnly && !Def.ExistsTable(columnDefinition.TableName, o => o.ItemId > 0)) return true;
            if (codeDefinition.GenericUi && !Def.ExistsTable(columnDefinition.TableName, o => o.GenericUi)) return true;
            if (codeDefinition.UpdateMonitor && !Def.ExistsTable(columnDefinition.TableName, o => o.UpdateMonitor)) return true;
            if (codeDefinition.ControlType != string.Empty && codeDefinition.ControlType != columnDefinition.ControlType) return true;
            if (codeDefinition.Null && !columnDefinition.Nullable) return true;
            if (codeDefinition.NotNull && columnDefinition.Nullable) return true;
            if (codeDefinition.Like && !columnDefinition.Like) return true;
            if (codeDefinition.NotBase)
            {
                if (Def.ItemModelNameCollection().Contains(columnDefinition.ModelName))
                {
                    if (Def.ExistsColumnBaseItem(o =>
                        o.ColumnName == columnDefinition.ColumnName &&
                        !o.EachModel))
                    {
                        return true;
                    }
                }
                else
                {
                    if (Def.ExistsColumnBase(o =>
                        o.ColumnName == columnDefinition.ColumnName &&
                        !o.EachModel))
                    {
                        return true;
                    }
                }
            }
            if (codeDefinition.IdentityOrPk)
            {
                if ((HasUniqueColumn(columnDefinition) && !(columnDefinition.Identity || columnDefinition.Unique)) ||
                    (!HasUniqueColumn(columnDefinition) && columnDefinition.Pk == 0))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HasUniqueColumn(ColumnDefinition columnDefinition)
        {
            return Def.ColumnDefinitionCollection.Any(o =>
                o.TableName == columnDefinition.TableName && (o.Identity || o.Unique));
        }

        internal static void ReplaceCode(
            ref string code,
            CodeDefinition codeDefinition,
            ColumnDefinition columnDefinition,
            int columnCount = 0)
        {
            foreach (var placeholder in code.RegexValues(CodePatterns.ReplacementPlaceholder))
            {
                switch (placeholder)
                {
                    case "ByForm":
                        code = code.ByForm(columnDefinition);
                        break;
                    case "ByApi":
                        code = code.ByApi(columnDefinition);
                        break;
                    case "ByDataRow":
                        code = code.ByDataRow(columnDefinition);
                        break;
                    case "BySession":
                        code = code.BySession(columnDefinition);
                        break;
                }
            }
            foreach (var placeholder in code.RegexValues(CodePatterns.ReplacementPlaceholder))
            {
                switch (placeholder)
                {
                    case "columnName":
                        code = code.Replace(
                            "#columnName#", columnDefinition.ColumnName.LocalalVariableName());
                        break;
                    case "ColumnName":
                        code = code.Replace(
                            "#ColumnName#", columnDefinition.ColumnName.PublicVariableName());
                        break;
                    case "ColumnCaption":
                        code = code.Replace("#ColumnCaption#", columnDefinition.ColumnLabel);
                        break;
                    case "Type":
                        code = code.Replace("#Type#", Strings.CoalesceEmpty(
                            columnDefinition.TypeCs, columnDefinition.TypeName.CsType()));
                        break;
                    case "RecordingType":
                        code = code.Replace("#RecordingType#", columnDefinition.TypeName.CsType());
                        break;
                    case "RecordingData":
                        code = code.Replace("#RecordingData#", columnDefinition.RecordingData);
                        break;
                    case "CastType":
                        code = code.Replace("#CastType#", columnDefinition.TypeName.CastType());
                        break;
                    case "DefaultData":
                        code = code.Replace(
                            "#DefaultData#", columnDefinition.DefaultData());
                        break;
                    case "InitialValue":
                        code = code.Replace(
                            "#InitialValue#",
                            columnDefinition.InitialValue());
                        break;
                    case "RecordingDefaultData":
                        code = code.Replace(
                            "#RecordingDefaultData#",
                            columnDefinition.DefaultData(recordingData: true));
                        break;
                    case "Hash":
                        code = code.Replace(
                            "#Hash#",
                            columnDefinition.Hash ? ".Sha512Cng()" : string.Empty);
                        break;
                    case "MaxLength":
                        code = code.Replace(
                            "#MaxLength#", CodeMaxLength(columnDefinition));
                        break;
                    case "Calc":
                        code = code.Replace("#Calc#", columnDefinition.Calc
                            .Replace("#TableName#", columnDefinition.TableName)
                            .Replace("#ModelName#", columnDefinition.ModelName));
                        break;
                    case "ColumnCount":
                        code = code.Replace("#ColumnCount#", columnCount.ToString());
                        break;
                    case "GridEnable":
                        code = code.Replace("#GridEnable#", columnDefinition.GridEnabled
                            .ToOneOrZeroString());
                        break;
                    default:
                        code = ReplaceCode(code, columnDefinition, placeholder);
                        break;
                }
            }
            if (codeDefinition.ReplaceOld != string.Empty)
            {
                code = code.Replace(codeDefinition.ReplaceOld, codeDefinition.ReplaceNew);
            }
        }

        private static string ReplaceCode(
            string code, ColumnDefinition columnDefinition, string placeholder)
        {
            if (Def.ColumnXls.XlsSheet.Columns.Contains(placeholder))
            {
                code = code.Replace(
                    "#" + placeholder + "#", columnDefinition[placeholder].ToString());
            }
            return code;
        }

        private static string InitialValue(this ColumnDefinition columnDefinition)
        {
            switch ((columnDefinition.TypeName).CsTypeSummary())
            {
                case Types.CsString:
                    switch (columnDefinition.RecordingData)
                    {
                        case ".ToJson()":
                        case ".RecordingJson()":
                            return "\"[]\"";
                        default:
                            return DefaultData(columnDefinition, "string.Empty", bracket: true);
                    }
                case Types.CsNumeric:
                    return "0";
                case Types.CsDateTime:
                    return "0.ToDateTime()";
                case Types.CsBool:
                    return "false";
                case Types.CsBytes:
                    return "null";
                default:
                    return string.Empty;
            }
        }
  
        private static string DefaultData(
            this ColumnDefinition columnDefinition, bool recordingData = false)
        {
            switch (Strings.CoalesceEmpty(
                recordingData ? columnDefinition.TypeName : string.Empty,
                columnDefinition.TypeCs,
                columnDefinition.TypeName).CsTypeSummary())
            {
                case Types.CsString:
                    switch (columnDefinition.RecordingData)
                    {
                        case ".ToJson()":
                        case ".RecordingJson()":
                            return "\"[]\"";
                        default:
                            return DefaultData(columnDefinition, "string.Empty", bracket: true);
                    }
                case Types.CsNumeric:
                    return DefaultData(columnDefinition, "0");
                case Types.CsDateTime:
                    return "0.ToDateTime()";
                case Types.CsBool:
                    if (columnDefinition.Default == "1")
                    {
                        return "true";
                    }
                    else
                    {
                        return "false";
                    }
                case Types.CsBytes:
                    return "null";
                default:
                    if (columnDefinition.EnumColumn)
                    {
                        if (!columnDefinition.Default.IsNullOrEmpty())
                        {
                            return "(" + columnDefinition.TypeCs + ")" +
                                columnDefinition.Default;
                        }
                        else
                        {
                            return "null";
                        }
                    }
                    else if (columnDefinition.DefaultCs != string.Empty)
                    {
                        return columnDefinition.DefaultCs;
                    }
                    else
                    {
                        return "new " + columnDefinition.TypeCs + "()";
                    }
            }
        }

        private static string DefaultData(
            ColumnDefinition columnDefinition, string _default, bool bracket = false)
        {
            if (!columnDefinition.DefaultCs.IsNullOrEmpty())
            {
                return bracket
                    ? "\"" + columnDefinition.DefaultCs + "\""
                    : columnDefinition.DefaultCs;
            }
            else if (!columnDefinition.Default.IsNullOrEmpty())
            {
                return bracket
                    ? "\"" + columnDefinition.Default + "\""
                    : columnDefinition.Default;
            }
            else
            {
                return _default;
            }
        }

        internal static string PublicVariableName(this string variableName)
        {
            return variableName.ToUpperFirstChar().EscapeReservedWord();
        }

        internal static string LocalalVariableName(this string variableName)
        {
            return variableName.ToLowerFirstChar().EscapeReservedWord();
        }

        internal static string CodeMaxLength(ColumnDefinition columnDefinition)
        {
            if (columnDefinition.TypeName.CsTypeSummary() == Types.CsString &&
                columnDefinition.MaxLength > 0)
            {
                return ".MaxLength({0})".Params(columnDefinition.MaxLength);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}