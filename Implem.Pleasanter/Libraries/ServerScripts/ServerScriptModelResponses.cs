using Implem.Libraries.Utilities;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelResponses : List<ServerScriptModelResponse>
    {
        public void Reload(string type, string id)
        {
            Add(new ServerScriptModelResponse()
            {
                Type = type.ToEnum<ServerScriptModelResponse.Types>(),
                ColumnName = id
            });
        }
    }
}