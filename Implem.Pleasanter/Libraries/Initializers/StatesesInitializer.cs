using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Models;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class StatusesInitializer
    {
        public static void Initialize()
        {
            if (!StatusUtilities.Initialized())
            {
                var hash = StatusUtilities.MonitorHash();
                Rds.ExecuteNonQuery(statements: StatusUtilities.MonitorHash()
                    .Select(o => StatusUtilities.UpdateStatus(o.Key))
                    .ToArray());
            }
        }
    }
}