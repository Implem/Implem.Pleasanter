using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class ParametersInitializer
    {
        public static string Initialize(Context context)
        {
            if (context.HasPrivilege)
            {
                Initializer.ReloadParameters();
                ExtensionInitializer.Initialize(context: context);
            }
            return string.Empty;
        }
    }
}