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
                        default:
                            break;
                    }
                });
            }
        }
    }
}