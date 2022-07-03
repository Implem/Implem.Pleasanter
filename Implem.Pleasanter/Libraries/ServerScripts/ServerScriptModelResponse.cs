using Implem.Pleasanter.Libraries.Responses;

namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelResponse
    {
        public enum Types
        {
            Filter
        }

        public Types Type { get; set; }
        public string ColumnName { get; set; }
    }
}
