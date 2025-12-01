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
        public static readonly string EnterpriseUrl = "https://pleasanter.org/extensions-trial-ended-info/?utm_source=installer&utm_medium=app&utm_campaign=extension-trial&utm_content=route03";
        public static readonly string FirstSetupTrialUrl = "https://pleasanter.org/pleasanter-extensions-trial/?utm_source=installer&utm_medium=app&utm_campaign=extension-trial&utm_content=route01";
        public static readonly string VerUpTrialUrl = "https://pleasanter.org/pleasanter-extensions-trial/?utm_source=installer&utm_medium=app&utm_campaign=extension-trial&utm_content=route02\r\n";
        public static readonly string InstallUrlForWindows = "https://pleasanter.org/ja/manual/getting-started-installer-pleasanter-windows";
        public static readonly string InstallUrlForUbuntu = "https://pleasanter.org/ja/manual/getting-started-installer-pleasanter-ubuntu";
        public static readonly string InstallUrlForAlmaLinux = "https://pleasanter.org/ja/manual/getting-started-installer-pleasanter-almalinux";
        public static readonly string InstallUrlForRhel8 = "https://pleasanter.org/ja/manual/getting-started-installer-pleasanter-rhel-8";
        public static readonly string InstallUrlForRhel9 = "https://pleasanter.org/ja/manual/getting-started-installer-pleasanter-rhel9";
        public static readonly string InstallUrlForAzure = "https://pleasanter.org/ja/manual/getting-started-installer-pleasanter-azure";

    }
}
