using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class GridSelector
    {
        public bool All;
        public List<long> Selected;
        public bool Nothing;

        public GridSelector(Context context)
        {
            All = context.RequestData("GridCheckAll").ToBool();
            Selected = All
                ? Get(
                    context: context,
                    name: "GridUnCheckedItems")
                : Get(
                    context: context,
                    name: "GridCheckedItems");
            Nothing = !All && !Selected.Any();
        }

        private static List<long> Get(Context context, string name)
        {
            return context.RequestData(name)
                .Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .Distinct()
                .ToList();
        }
    }
}