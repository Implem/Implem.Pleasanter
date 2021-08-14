using System.Collections.Generic;
using System.Dynamic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelView
    {
        public List<string> AlwaysGetColumns = new List<string>();
        public string OnSelectingWhere;
        public readonly ExpandoObject Filters = new ExpandoObject();
        public readonly ExpandoObject Sorters = new ExpandoObject();
    }
}