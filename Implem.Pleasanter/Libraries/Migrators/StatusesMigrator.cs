using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class StatusesMigrator
    {
        public static void Migrate(Context context)
        {
            if (LowerThan(context: context, version: "0.37.10.0"))
            {
                Statuses.Version00_037_010.Migrate(context: context);
                StatusUtilities.UpdateAssemblyVersion(
                    context: context,
                    version: "0.37.10.0");
            }
            if (LowerThan(context: context, version: "0.39.22.0"))
            {
                Statuses.Version00_039_022.Migrate(context: context);
                StatusUtilities.UpdateAssemblyVersion(
                    context: context,
                    version: "0.39.22.0");
            }
            if (LowerThan(context: context, version: "0.43.52.0"))
            {
                Statuses.Version00_043_052.Migrate(context: context);
                StatusUtilities.UpdateAssemblyVersion(
                    context: context,
                    version: "0.43.52.0");
            }
        }

        private static bool LowerThan(Context context, string version)
        {
            return new AssemblyVersion(context: context)
                .LowerThan(new AssemblyVersion(version));
        }
    }
}