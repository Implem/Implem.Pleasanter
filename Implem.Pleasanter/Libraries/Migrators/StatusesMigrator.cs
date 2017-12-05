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
            if (LowerThan("0.39.22.0"))
            {
                Statuses.Version00_039_022.Migrate();
                StatusUtilities.UpdateAssemblyVersion("0.39.22.0");
            }
            if (LowerThan("0.43.52.0"))
            {
                Statuses.Version00_043_052.Migrate();
                StatusUtilities.UpdateAssemblyVersion("0.43.52.0");
            }
        }

        private static bool LowerThan(string version)
        {
            return new AssemblyVersion().LowerThan(new AssemblyVersion(version));
        }
    }
}