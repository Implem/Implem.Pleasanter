using System.Dynamic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelView
    {
        public readonly ExpandoObject Filters = new ExpandoObject();
        public readonly ExpandoObject Sorters = new ExpandoObject();
    }
}