using Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts;
using Implem.CodeDefiner.Utilities;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal static class Creators
    {
        internal static string Create(CodeDefinition codeDefinition, DataContainer dataContainer)
        {
            var code = codeDefinition.FormattedCode();
            foreach (var placeholder in code.RegexValues(CodePatterns.IdPlaceholder))
            {
                var id = placeholder.RegexFirst(CodePatterns.Id);
                var codeChildDefinition = Def.CodeDefinitionCollection
                    .FirstOrDefault(o => o.Id == id);
                if (codeChildDefinition != null)
                {
                    Def.SetCodeDefinitionOption(placeholder, codeChildDefinition);
                    var codeChildCollection = new List<string>();
                    switch (codeChildDefinition.RepeatType)
                    {
                        case "DefinitionFile":
                            DefinitionFile.SetCodeCollection(
                                codeChildDefinition, codeChildCollection, dataContainer);
                            break;
                        case "DefinitionRow":
                            DefinitionRow.SetCodeCollection(
                                codeChildDefinition, codeChildCollection, dataContainer);
                            break;
                        case "DefinitionColumn":
                            DefinitionColumn.SetCodeCollection(
                                codeChildDefinition, codeChildCollection, dataContainer);
                            break;
                        case "BaseModel":
                            BaseModel.SetCodeCollection(
                                codeChildDefinition, codeChildCollection, dataContainer);
                            break;
                        case "BaseItemModel":
                            BaseItemModel.SetCodeCollection(
                                codeChildDefinition, codeChildCollection, dataContainer);
                            break;
                        case "Table":
                            Table.SetCodeCollection(
                                codeChildDefinition, codeChildCollection, dataContainer);
                            break;
                        case "Column":
                            Column.SetCodeCollection(
                                codeChildDefinition, codeChildCollection, dataContainer);
                            break;
                        case "Join":
                            Join.SetCodeCollection(
                                codeChildDefinition, codeChildCollection, dataContainer);
                            break;
                        case "Form":
                            Form.SetCodeCollection(
                                codeChildDefinition, codeChildCollection, dataContainer);
                            break;
                        case "Display":
                            Display.SetCodeCollection(
                                codeChildDefinition, codeChildCollection, dataContainer);
                            break;
                        default:
                            switch (dataContainer.Type)
                            {
                                case "DefinitionFile":
                                    DefinitionFile.SetCodeCollection_Default(
                                        codeChildDefinition, codeChildCollection, dataContainer);
                                    break;
                                case "Table":
                                    Column.SetCodeCollection_Default(
                                        codeChildDefinition, codeChildCollection, dataContainer);
                                    break;
                                default:
                                    break;
                            }
                            break;
                    }
                    code = CodeChildCollection(
                        code, placeholder, codeChildDefinition, codeChildCollection);
                    codeChildDefinition.RestoreBySavedMemory();
                }
                else
                {
                    Consoles.Write(
                        DisplayAccessor.Displays.Get("DefinitionNotFound").Params(
                            id, DefinitionFiles.CodeDefinitionFileName),
                        Consoles.Types.Error,
                        abort: true);
                }
            }
            ReplaceCode(ref code, dataContainer.TableName);
            return code;
        }

        private static string CodeChildCollection(
            string code,
            string placeholder,
            CodeDefinition codeChildDefinition,
            List<string> codeChildCollection)
        {
            return code.Replace(
                placeholder,
                codeChildCollection.Join(codeChildDefinition.Separator.Replace("\\r\\n", "\r\n")));
        }

        internal static void SetCodeCollection(
            ref string code,
            List<string> codeCollection,
            CodeDefinition codeDefinition,
            DataContainer dataContainer,
            Action replaceCode)
        {
            code = Create(codeDefinition, dataContainer);
            replaceCode();
            codeCollection.Add(code);
        }

        private static void ReplaceCode(ref string code, string tableName)
        {
            foreach (var placeholder in code.RegexValues(CodePatterns.ReplacementPlaceholder))
            {
                switch (placeholder)
                {
                    case "IdType":
                        code = code.Replace("#IdType#", tableName.CsTypeIdColumn());
                        break;
                    case "IdTypeDefault":
                        code = code.Replace("#IdTypeDefault#", tableName.DefaultIdColumn());
                        break;
                    case "CastIdType":
                        code = code.Replace("#CastIdType#", tableName.CastTypeIdColumn());
                        break;
                }
            }
        }

        internal static string FormattedCode(this CodeDefinition codeRow)
        {
            string code = string.Empty;
            if (codeRow.Body.IndexOf("\r\n") == -1)
            {
                code += Strings.Tab(codeRow.Indent) + codeRow.Body;
            }
            else
            {
                codeRow.Body.SplitReturn().ForEach(line =>
                {
                    if (line.Trim().IsNullOrEmpty())
                    {
                        code += "\r\n";
                    }
                    else if (line.StartsWith("<!--"))
                    {
                        code += line + "\r\n";
                    }
                    else
                    {
                        code += Strings.Tab(codeRow.Indent) + line + "\r\n";
                    }
                });
            }
            code = code.NoSpace(codeRow.NoSpace);
            return code;
        }

        internal static string CsTypeIdColumn(this string tableName)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == tableName)
                .Where(o => o.ColumnName == o.ModelName + "Id")
                .Select(o => o.TypeName.CsType())
                .FirstOrDefault() ?? string.Empty;
        }

        internal static string DefaultIdColumn(this string tableName)
        {
            var tableColumnDefinition = Def.ColumnDefinitionCollection
                .Where(o => o.TableName == tableName)
                .Where(o => o.ColumnName == o.ModelName + "Id")
                .FirstOrDefault();
            if (tableColumnDefinition != null)
            {
                switch (tableColumnDefinition.TypeName)
                {
                    case "char":
                    case "nchar":
                    case "varchar":
                    case "nvarchar":
                        return "\"\"";
                    case "int":
                    case "bigint":
                        return "0";
                    default:
                        return string.Empty;
                }
            }
            return string.Empty;
        }

        internal static string CastTypeIdColumn(this string tableName)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == tableName)
                .Where(o => o.ColumnName == o.ModelName + "Id")
                .Select(o => o.TypeName.CastType())
                .FirstOrDefault() ?? string.Empty;
        }
    }
}
