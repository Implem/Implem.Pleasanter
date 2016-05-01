using Implem.CodeDefiner.Utilities;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class Display
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            Def.DisplayDefinitionCollection
                .Where(o => !CheckExclude(codeDefinition, o))
                .ForEach(screenDefinition =>
                    Creators.SetCodeCollection(
                        ref code,
                        codeCollection,
                        codeDefinition,
                        dataContainer,
                        () => ReplaceCode(ref code, screenDefinition)));
        }

        private static bool CheckExclude(
            CodeDefinition codeDefinition,
            DisplayDefinition screenDefinition)
        {
            if (!codeDefinition.DisplayLanguages && !screenDefinition.Language.IsNullOrEmpty()) return true;
            if (codeDefinition.DisplayType != string.Empty && !codeDefinition.DisplayType.Split(',').Contains(screenDefinition.Type)) return true;
            if (codeDefinition.ClientScript && !screenDefinition.ClientScript) return true;
            return false;
        }

        private static void ReplaceCode(ref string code, DisplayDefinition displayDefinition)
        {
            foreach (var placeholder in code.RegexValues(CodePatterns.ReplacementPlaceholder))
            {
                switch (placeholder)
                {
                    case "DisplayId":
                        code = code.Replace("#DisplayId#", displayDefinition.Id);
                        break;
                    case "DisplayContent":
                        code = code.Replace("#DisplayContent#", displayDefinition.Content);
                        break;
                    case "DisplayCssClass":
                        code = code.Replace("#DisplayCssClass#", displayDefinition.CssClass);
                        break;
                    case "DisplayContentEncoded":
                        code = code.Replace(
                            "#DisplayContentEncoded#",
                            HttpUtility.HtmlEncode(displayDefinition.Content));
                        break;
                }
            }
        }
    }
}
