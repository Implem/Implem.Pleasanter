using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal static class Spids
    {
        internal static void Kill(ISqlObjectFactory factory, string uid)
        {
            Get(factory: factory, uid: uid)
                .AsEnumerable()
                .ForEach(spidDataRow =>
                Def.SqlIoBySa(factory: factory).ExecuteNonQuery(
                    factory: factory,
                    commandText: Def.Sql.KillSpid.Replace("#Spid#", spidDataRow["spid"].ToString())));
        }

        private static DataTable Get(ISqlObjectFactory factory, string uid)
        {
            return Def.SqlIoBySa(factory: factory).ExecuteTable(
                factory: factory,
                commandText: Def.Sql.SpWho.Replace("#Uid#", uid));
        }
    }
}
