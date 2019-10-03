using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class ParametersInitializer
    {
        public static string Initialize(Context context)
        {
            if (Permissions.CanManageTenant(context: context))
            {
                Initializer.ReloadParameters();
            }
            return "[]";
        }
    }
}