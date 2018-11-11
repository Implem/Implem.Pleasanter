using System.Linq;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class DataViewGrid
    {
        public static int Offset(Context context)
        {
            return
                context.Forms.ControlId().StartsWith("ViewFilters_") ||
                context.Forms.Keys.Any(o => o.StartsWith("ViewSorters_"))
                    ? 0
                    : context.Forms.Int("GridOffset");
        }
    }
}