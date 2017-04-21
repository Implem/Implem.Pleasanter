using Implem.Libraries.Utilities;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.General
{
    public class AssemblyVersion
    {
        public int Major;
        public int Minor;
        public int Build;
        public int Revision;

        public AssemblyVersion()
        {
            SetByString(StatusUtilities.AssemblyVersion());
        }

        public AssemblyVersion(string version)
        {
            SetByString(version);
        }

        private void SetByString(string version)
        {
            Major = version.Split_1st('.').ToInt();
            Minor = version.Split_2nd('.').ToInt();
            Build = version.Split_3rd('.').ToInt();
            Revision = version.Split_4th('.').ToInt();
        }

        public bool LowerThan(AssemblyVersion target)
        {
            if (Major < target.Major) return true;
            if (Minor < target.Minor) return true;
            if (Build < target.Build) return true;
            return false;
        }
    }
}