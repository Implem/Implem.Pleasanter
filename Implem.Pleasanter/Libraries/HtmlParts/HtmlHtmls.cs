using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlHtmls
    {
        public static string ExtendedHtmls(
            Context context,
            string id,
            string columnName = null)
        {
            return ExtensionUtilities.ExtensionWhere<ExtendedHtml>(
                context: context,
                extensions: Parameters.ExtendedHtmls,
                columnName: columnName)
                .Select(o => o.Html.Display(
                    context: context,
                    id: id))
                .Join("\n");
        }
    }
}
