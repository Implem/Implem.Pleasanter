using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class StatusesInitializer
    {
        public static void Initialize(Context context)
        {
            StatusUtilities.Initialize(context: context);
        }
    }
}