using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal static class Spids
    {
        internal static void Kill(string uid)
        {
            Get(uid).AsEnumerable().ForEach(spidDataRow =>
                Def.SqlIoBySa().ExecuteNonQuery(
                    Def.Sql.KillSpid.Replace("#Spid#", spidDataRow["spid"].ToString())));
        }

        private static DataTable Get(string uid)
        {
            return Def.SqlIoBySa().ExecuteTable(
                Def.Sql.SpWho.Replace("#Uid#", uid));
        }
    }
}
