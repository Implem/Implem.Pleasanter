using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class StatusesInitializer
    {
        public static void Initialize(int tenantId = 0)
        {
            StatusUtilities.Initialize(tenantId);
        }
    }
}