using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Security;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class ParametersInitializer
    {
        public static string Initialize()
        {
            if (Permissions.CanManageTenant())
            {
                Initializer.SetParameters();
            }
            return "[]";
        }
    }
}