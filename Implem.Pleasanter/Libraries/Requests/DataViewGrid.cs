using System.Linq;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class DataViewGrid
    {
        public static int Offset()
        {
            return
                Forms.Data("ControlId").StartsWith("DataViewFilters_") ||
                Forms.Keys().Any(o => o.StartsWith("GridSorters_"))
                    ? 0
                    : Forms.Int("GridOffset");
        }
    }
}