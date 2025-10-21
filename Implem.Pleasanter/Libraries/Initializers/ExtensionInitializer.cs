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
                    .Disabled(false),
                orderBy: DataSources.Rds.ExtensionsOrderBy()
                    .Extensions_ExtensionName());
            if (extensions.Any())
            {
                Parameters.ExtendedFields = Parameters.ExtendedFields ?? new List<ExtendedField>();
                Parameters.ExtendedHtmls = Parameters.ExtendedHtmls ?? new List<ExtendedHtml>();
                Parameters.ExtendedNavigationMenus = Parameters.ExtendedNavigationMenus ?? new List<ExtendedNavigationMenu>();
                Parameters.ExtendedScripts = Parameters.ExtendedScripts ?? new List<ExtendedScript>();
                Parameters.ExtendedServerScripts = Parameters.ExtendedServerScripts ?? new List<ExtendedServerScript>();
                Parameters.ExtendedSqls = Parameters.ExtendedSqls ?? new List<ExtendedSql>();
                Parameters.ExtendedStyles = Parameters.ExtendedStyles ?? new List<ExtendedStyle>();
                Parameters.ExtendedPlugins = Parameters.ExtendedPlugins ?? new List<ExtendedPlugin>();
                extensions.ForEach(extension =>
                {
                    switch (extension.ExtensionType)
                    {
                        case "Fields":
                            var extendedField = extension.ExtensionSettings.Deserialize<ExtendedField>();
                            if (extendedField != null)
                            {
                                extendedField.Name = Strings.CoalesceEmpty(
                                    extension.ExtensionName,
                                    extendedField.Name);
                                Parameters.ExtendedFields.Add(extendedField);
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
                        case "NavigationMenu":
                            var extendedNavigationMenu = extension.ExtensionSettings.Deserialize<ExtendedNavigationMenu>();
                            if (extendedNavigationMenu != null)
                            {
                                extendedNavigationMenu.Name = Strings.CoalesceEmpty(
                                    extension.ExtensionName,
                                    extendedNavigationMenu.Name);
                                Parameters.ExtendedNavigationMenus.Add(extendedNavigationMenu);
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
                        case "ServerScript":
                            var extendedServerScript = extension.ExtensionSettings.Deserialize<ExtendedServerScript>();
                            if (extendedServerScript != null)
                            {
                                extendedServerScript.Name = Strings.CoalesceEmpty(
                                    extension.ExtensionName,
                                    extendedServerScript.Name);
                                if (!extension.Body.IsNullOrEmpty())
                                {
                                    extendedServerScript.Body = extension.Body;
                                }
                                Parameters.ExtendedServerScripts.Add(extendedServerScript);
                            }
                            break;
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
                        case "Plugin":
                            var extendedPlugin = extension.ExtensionSettings.Deserialize<ExtendedPlugin>();
                            if (extendedPlugin != null)
                            {
                                extendedPlugin.Name = Strings.CoalesceEmpty(
                                    extension.ExtensionName,
                                    extendedPlugin.Name);
                                Parameters.ExtendedPlugins.Add(extendedPlugin);
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