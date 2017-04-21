using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class StatusesMigrator
    {
        public static void Migrate()
        {
            if (LowerThan("0.37.10.0"))
            {
                Statuses.Version00_037_010.Migrate();
                StatusUtilities.UpdateAssemblyVersion("0.37.10.0");
            }
        }

        private static bool LowerThan(string version)
        {
            return new AssemblyVersion().LowerThan(new AssemblyVersion(version));
        }
    }
}