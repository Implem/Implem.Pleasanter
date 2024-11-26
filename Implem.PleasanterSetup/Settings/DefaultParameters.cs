using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.PleasanterSetup.Settings
{
    internal static class DefaultParameters
    {
        public static readonly string InstallDirForWindows = "C:\\web\\pleasanter";
        public static readonly string InstallDirForLinux = "/web/pleasanter";
        public static readonly string InstallDirForAzure = "C:\\home\\site\\wwwroot";
        public static readonly string CodeDefinerDirForAzure = "C:\\home\\site\\CodeDefiner";
        public static  readonly string HostName = "localhost";
        public static readonly string ServiceName = "Implem.Pleasanter";
        public static readonly string url = "https://api.github.com/repos/Implem/Implem.Pleasanter/releases/latest";
    }
}
