using Implem.DefinitionAccessor;

namespace Implem.Pleasanter.Libraries.Scim
{
    internal static class ScimFeatureUtilities
    {
        internal static bool Enabled() => Parameters.Scim?.Enabled == true
                && Parameters.CommercialLicense() && Parameters.HasScim();

        internal static bool SwaggerEnabled() => Enabled()
                && Parameters.Scim?.SwaggerEnabled == true;
    }
}
