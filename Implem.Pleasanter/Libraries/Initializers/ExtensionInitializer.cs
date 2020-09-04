using Implem.DisplayAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;
using Parameters = Implem.DefinitionAccessor.Parameters;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public class ExtensionInitializer
    {
        public static void Initialize(Context context)
        {
            var extensions = new ExtensionCollection(
                context: context,
                where: DataSources.Rds.ExtensionsWhere()
                    .Disabled(false));
            if (extensions.Any())
            {
                Parameters.ExtendedSqls = Parameters.ExtendedSqls ?? new List<ExtendedSql>();
                Parameters.ExtendedStyles = Parameters.ExtendedStyles ?? new List<ExtendedStyle>();
                Parameters.ExtendedScripts = Parameters.ExtendedScripts ?? new List<ExtendedScript>();
                Parameters.ExtendedHtmls = Parameters.ExtendedHtmls ?? new List<ExtendedHtml>();
                extensions.ForEach(extension =>
                {
                    switch (extension.ExtensionType)
                    {
                        case "Sql":
                            var extendedSql = extension.ExtensionSettings.Deserialize<ExtendedSql>();
                            if (extendedSql != null)
                            {
                                extendedSql.Name = Strings.CoalesceEmpty(
                                    extension.ExtensionName,
                                    extendedSql.Name);
                                if (!extension.Body.IsNullOrEmpty())
                                {
                                    extendedSql.CommandText = extension.Body;
                                }
                                Parameters.ExtendedSqls.Add(extendedSql);
                            }
                            break;
                        case "Style":
                            var extendedStyle = extension.ExtensionSettings.Deserialize<ExtendedStyle>();
                            if (extendedStyle != null)
                            {
                                extendedStyle.Name = Strings.CoalesceEmpty(
                                    extension.ExtensionName,
                                    extendedStyle.Name);
                                if (!extension.Body.IsNullOrEmpty())
                                {
                                    extendedStyle.Style = extension.Body;
                                }
                                Parameters.ExtendedStyles.Add(extendedStyle);
                            }
                            break;
                        case "Script":
                            var extendedScript = extension.ExtensionSettings.Deserialize<ExtendedScript>();
                            if (extendedScript != null)
                            {
                                extendedScript.Name = Strings.CoalesceEmpty(
                                    extension.ExtensionName,
                                    extendedScript.Name);
                                if (!extension.Body.IsNullOrEmpty())
                                {
                                    extendedScript.Script = extension.Body;
                                }
                                Parameters.ExtendedScripts.Add(extendedScript);
                            }
                            break;
                        case "Html":
                            var extendedHtml = extension.ExtensionSettings.Deserialize<ExtendedHtml>();
                            if (extendedHtml != null)
                            {
                                extendedHtml.Name = Strings.CoalesceEmpty(
                                    extension.ExtensionName,
                                    extendedHtml.Name);
                                if (!extension.Body.IsNullOrEmpty())
                                {
                                    var listDisplay = new Dictionary<string, List<DisplayElement>>();
                                    var name = extendedHtml.Name;
                                    var displayElement = new DisplayElement
                                    {
                                        Language = extendedHtml.Language,
                                        Body = extension.Body
                                    };
                                    listDisplay
                                        .AddIfNotConainsKey(
                                            key: name,
                                            value: new List<DisplayElement>())
                                        .Get(name)
                                        .Add(displayElement);
                                    extendedHtml.Html = listDisplay;
                                }
                                Parameters.ExtendedHtmls.Add(extendedHtml);
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }
    }
}