using Implem.CodeDefiner.Utilities;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class DefinitionColumn
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            dataContainer
                .XlsIoCollection[dataContainer.DefinitionName]
                .XlsSheet
                .Columns
                .ForEach(definitionColumnName =>
                    Creators.SetCodeCollection(
                        ref code,
                        codeCollection,
                        codeDefinition,
                        dataContainer,
                        () =>
                        {
                            var definitionColumnNameAlternate = definitionColumnName
                                .EscapeReservedWord()
                                .Replace("-", "_");
                            var definitionColumnType = dataContainer
                                .XlsIoCollection[dataContainer.DefinitionName]
                                .XlsSheet[0][definitionColumnName]
                                .ToString();
                            ReplaceCode(
                                ref code,
                                codeDefinition,
                                dataContainer.DefinitionName,
                                definitionColumnName,
                                definitionColumnNameAlternate,
                                definitionColumnType);
                        }));
        }

        private static void ReplaceCode(
            ref string code,
            CodeDefinition codeDefinition,
            string definitionName,
            string definitionColumnName,
            string definitionColumnNameAlternate,
            string definitionColumnType)
        {
            foreach (var placeholder in code.RegexValues(CodePatterns.ReplacementPlaceholder))
            {
                switch (placeholder)
                {
                    case "DefColumnName":
                        code = code.Replace("#DefColumnName#", definitionColumnNameAlternate);
                        break;
                    case "DefColumnNameOriginal":
                        code = code.Replace("#DefColumnNameOriginal#", definitionColumnName);
                        break;
                    case "Type":
                        code = code.Replace("#Type#", definitionColumnType);
                        break;
                    case "CastType":
                        code = code.Replace("#CastType#", definitionColumnType.CastType());
                        break;
                    case "File":
                        code = code.Replace("#File#", definitionName);
                        break;
                    case "file":
                        code = code.Replace("#file#", definitionName.ToLowerFirstChar());
                        break;
                    case "SetDefault":
                        code = code.Replace("#SetDefault#", definitionColumnType
                            .DefaultSetter());
                        break;
                }
            }
            if (codeDefinition.ReplaceOld != string.Empty)
            {
                code = code.Replace(codeDefinition.ReplaceOld, codeDefinition.ReplaceNew);
            }
        }

        private static string DefaultSetter(this string csType)
        {
            switch (csType.CsTypeSummary())
            {
                case Types.CsString: return " = string.Empty";
                case Types.CsNumeric: return " = 0";
                case Types.CsDateTime: return " = 0.ToDateTime()";
                case Types.CsBool: return " = false";
                default: return " = null";
            }
        }
    }
}
