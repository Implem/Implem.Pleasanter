using System.Linq;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class DataViewGrid
    {
        public static int Offset()
        {
            return
                Forms.ControlId().StartsWith("ViewFilters_") ||
                Forms.Keys().Any(o => o.StartsWith("ViewSorters_"))
                    ? 0
                    : Forms.Int("GridOffset");
        }
    }
}