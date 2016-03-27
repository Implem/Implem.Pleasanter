using Implem.CodeDefiner.Utilities;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal class Validation
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            Def.ColumnDefinitionCollection
                .Where(o => o.TableName == dataContainer.TableName)
                .Where(o => o.FormName == dataContainer.FormName)
                .Where(o => o.Validations != string.Empty)
                .ForEach(columnDefinition =>
            {
                Creators.SetCodeCollection(
                    ref code,
                    codeCollection,
                    codeDefinition,
                    dataContainer,
                    () => ReplaceCode(ref code, columnDefinition));
            });
        }

        private static void ReplaceCode(ref string code, ColumnDefinition columnDefinition)
        {
            foreach (Match placeholder in code.RegexMatches(CodePatterns.ReplacementPlaceholder))
            {
                switch (placeholder.Value)
                {
                    case "ColumnName":
                        code = code.Replace(
                            "#ColumnName#", columnDefinition.ColumnName.PublicVariableName());
                        break;
                    case "Validations":
                        code = code.Replace(
                            "#Validations#", columnDefinition.Validations);
                        break;
                    case "ValidationMessages":
                        code = code.Replace(
                            "#ValidationMessages#", ValidationMessage(columnDefinition));
                        break;
                    default:
                        break;
                }
            }
        }

        private static string ValidationMessage(ColumnDefinition columnDefinition)
        {
            return columnDefinition.Validations
                .Split(',')
                .Select(o => o.Split(':')._1st() + ": $('#" + columnDefinition.Id + "').attr('data-validate-" + o.Split(':')._1st() + "')")
                .Join(",");
        }
    }
}
