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
            DisplayAccessor.Displays.DisplayHash
                .Select(o => o.Value)
                .ForEach(display => display.Languages
                    .Where(element => !CheckExclude(codeDefinition, display, element))
                    .ForEach(element =>
                        Creators.SetCodeCollection(
                            ref code,
                            codeCollection,
                            codeDefinition,
                            dataContainer,
                            () => ReplaceCode(ref code, display, element))));
        }

        private static bool CheckExclude(
            CodeDefinition codeDefinition,
            DisplayAccessor.Display display,
            DisplayAccessor.DisplayElement element)
        {
            if (!codeDefinition.DisplayLanguages && !element.Language.IsNullOrEmpty()) return true;
            if (codeDefinition.DisplayType != string.Empty && !codeDefinition.DisplayType.Split(',').Contains(display.Type.ToString())) return true;
            if (codeDefinition.ClientScript && display.ClientScript != true) return true;
            return false;
        }

        private static void ReplaceCode(
            ref string code,
            DisplayAccessor.Display display,
            DisplayAccessor.DisplayElement element)
        {
            foreach (var placeholder in code.RegexValues(CodePatterns.ReplacementPlaceholder))
            {
                switch (placeholder)
                {
                    case "DisplayId":
                        code = code.Replace("#DisplayId#", display.Id + (!element.Language.IsNullOrEmpty()
                            ? "_" + element.Language
                            : string.Empty));
                        break;
                    case "DisplayContent":
                        code = code.Replace("#DisplayContent#", element.Body);
                        break;
                    case "DisplayCssClass":
                        code = code.Replace("#DisplayCssClass#", CssClass(display.Type));
                        break;
                    case "DisplayContentEncoded":
                        code = code.Replace(
                            "#DisplayContentEncoded#",
                            HttpUtility.HtmlEncode(element.Body));
                        break;
                }
            }
        }

        private static string CssClass(DisplayAccessor.Displays.Types type)
        {
            switch (type)
            {
                case DisplayAccessor.Displays.Types.Success:
                    return "alert-success";
                case DisplayAccessor.Displays.Types.Information:
                    return "alert-info";
                case DisplayAccessor.Displays.Types.Warning:
                    return "alert-warning";
                case DisplayAccessor.Displays.Types.Error:
                    return "alert-error";
                case DisplayAccessor.Displays.Types.Confirmation:
                    return "alert-confirm";
                default:
                    return null;
            }
        }
    }
}
