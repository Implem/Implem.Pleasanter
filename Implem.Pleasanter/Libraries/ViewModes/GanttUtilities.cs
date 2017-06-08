using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public static class GanttUtilities
    {
        public static IOrderedEnumerable<GanttElement> Sort(
            this IOrderedEnumerable<GanttElement> self, Column column)
        {
            if (column != null)
            {
                switch (column.TypeName.CsTypeSummary())
                {
                    case "numeric": return self.ThenBy(o => o.SortBy.ToDecimal());
                    case "DateTime": return self.ThenBy(o => o.SortBy.ToDateTime());
                    case "string": return self.ThenBy(o => o.SortBy.ToString());
                    default: return self;
                }
            }
            else
            {
                return self;
            }
        }
    }
}