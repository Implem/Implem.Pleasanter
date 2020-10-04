using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Implem.Pleasanter.Libraries.Requests
{
    [Serializable]
    public class RecordSelector
    {
        public bool All { get; set; }
        public List<long> Selected { get; set; }
        public bool Nothing { get; set; }
        public View View { get; set; }

        public RecordSelector()
        {
        }

        public RecordSelector(Context context)
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

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            Nothing = !All && Selected?.Any() != true;
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

        public bool Checked(long id)
        {
            return All
                ? !Selected.Contains(id)
                : Selected.Contains(id);
        }
    }
}