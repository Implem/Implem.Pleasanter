using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class GridSelector
    {
        public bool All;
        public List<long> Selected;

        public GridSelector()
        {
            All = Forms.Bool("GridCheckAll");
            Selected = All
                ? Get("GridUnCheckedItems")
                : Get("GridCheckedItems");
        }

        private static List<long> Get(string name)
        {
            return Forms.Data(name)
                .Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .Distinct()
                .ToList();
        }
    }
}