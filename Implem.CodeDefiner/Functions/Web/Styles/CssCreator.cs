using Implem.CodeDefiner.Utilities;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Implem.CodeDefiner.Functions.Web.Styles
{
    internal class CssCreator
    {
        internal static void Create()
        {
            var code = new StringBuilder(Def.Code.Css);
            Def.CssDefinitionCollection.ForEach(cssDefinition =>
            {
                var cssCollection = new List<string>();
                PropertyNameCollection(cssDefinition).ForEach(PropertyName =>
                    SetCssCollection(cssDefinition, cssCollection, PropertyName));
                code.Append(Section(cssDefinition.Id, cssCollection));
            });
            CodeWriter.Write(
                Directories.Outputs("Styles", "site.css"),
                code.ToString());
        }

        private static IEnumerable<string> PropertyNameCollection(CssDefinition cssDefinition)
        {
            Consoles.Write(cssDefinition.Id, Consoles.Types.Info);
            return typeof(CssDefinition).GetMembers()
                .Where(o => o.Name != "Id")
                .Where(o => o.Name != string.Empty)
                .Where(o => cssDefinition[o.Name].ToStr() != string.Empty)
                .Select(o => o.Name);
        }

        private static void SetCssCollection(
            CssDefinition cssDefinition,
            List<string> cssCollection,
            string PropertyName)
        {
            var propertyNameOriginal = PropertyNameOriginal(PropertyName);
            var value = cssDefinition[PropertyName].ToString();
            if (propertyNameOriginal != "Specific")
            {
                cssCollection.Add("    {0}:{1};".Params(propertyNameOriginal, value));
            }
            else
            {
                cssDefinition[propertyNameOriginal].ToString().SplitReturn()
                    .Where(o => o.Trim() != string.Empty)
                    .ForEach(line =>
                        cssCollection.Add("    {0}".Params(line)));
            }
        }

        private static string PropertyNameOriginal(string PropertyName)
        {
            var propertyNameOriginal = PropertyName.Replace("_", "-");
            if (propertyNameOriginal.StartsWith("-"))
            {
                return propertyNameOriginal = propertyNameOriginal.Substring(1);
            }
            else
            {
                return propertyNameOriginal;
            }
        }

        private static string Section(string selector, IEnumerable<string> cssCollection)
        {
            return "{0}{{\r\n{1}\r\n}}\r\n\r\n".Params(selector, cssCollection.JoinReturn());
        }
    }
}
