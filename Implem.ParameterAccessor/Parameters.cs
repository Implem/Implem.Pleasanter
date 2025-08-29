using Implem.ParameterAccessor.Parts;
using System;
using System.Collections.Generic;
namespace Implem.DefinitionAccessor
{
    public static class Parameters
    {
        public static List<string> SyntaxErrors = new List<string>();
        public static Api Api;
        public static Authentication Authentication;
        public static BackgroundService BackgroundService;
        public static BackgroundTask BackgroundTask;
        public static BinaryStorage BinaryStorage;
        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> CustomDefinitions;
        public static Deleted Deleted;
        public static Dictionary<string, string> ExtendedColumnDefinitions;
        public static Env Env;
        public static AutoTestSettings ExtendedAutoTestSettings;
        public static List<AutoTestScenario> ExtendedAutoTestScenarios;
        public static List<AutoTestOperation> ExtendedAutoTestOperations;
        public static List<ExtendedColumns> ExtendedColumnsSet;
        public static List<ExtendedField> ExtendedFields;
        public static List<ExtendedHtml> ExtendedHtmls;
        public static List<ExtendedNavigationMenu> ExtendedNavigationMenus;
        public static List<ExtendedScript> ExtendedScripts;
        public static List<ExtendedServerScript> ExtendedServerScripts;
        public static List<ExtendedSql> ExtendedSqls;
        public static List<ExtendedStyle> ExtendedStyles;
        public static List<ExtendedHeadLink> ExtendedHeadLinks;
        public static List<ExtendedPlugin> ExtendedPlugins;
        public static Dictionary<string, string> ExtendedTags;
        public static General General;
        public static GroupMembers GroupMembers;
        public static History History;
        public static License.License License = new License.License();
        public static Locations Locations;
        public static Mail Mail;
        public static Mobile Mobile;
        public static List<NavigationMenu> NavigationMenus;
        public static Migration Migration;
        public static Notification Notification;
        public static Parameter Parameter;
        public static Permissions Permissions;
        public static Rds Rds;
        public static Kvs Kvs;
        public static Registration Registration;
        public static Reminder Reminder;
        public static Script Script;
        public static Search Search;
        public static Security Security;
        public static Service Service;
        public static Session Session;
        public static Site Site;
        public static SitePackage SitePackage;
        public static SysLog SysLog;
        public static User User;
        public static TextEditorUI TextEditorUI;
        public static CustomApps UserTemplate;
        public static ParameterAccessor.Parts.Version Version;
        public static Validation Validation;
        public static Dashboard Dashboard;
        public static GroupChildren GroupChildren;
        public static OutputCache OutputCache;
        public static TrialLicense TrialLicense;

        public static bool CommercialLicense()
        {
            return TrialLicense?.Check() ?? License.Check();
        }

        public static int LicensedUsers()
        {
            return TrialLicense?.Users ?? License.Users;
        }

        public static DateTime LicenseDeadline()
        {
            return TrialLicense?.Deadline ?? License.Deadline;
        }

        public static string Licensee()
        {
            return TrialLicense?.Licensee ?? License.Licensee;
        }

        public static int GetLicenseType()
        {
            if (License.Deadline == DateTime.MinValue
                && (License.Licensee == null || License.Licensee == "")
                && License.Users == 0)
            {
                if (TrialLicense == null) return 0x02;
                return TrialLicense.Check() ? 0x08 : 0x09;
            }
            return License.Check() ? 0x04 : 0x05;
        }

        public static string Copyright()
        {
            var prop = License.GetType().GetProperty("Copyright");
            return (string)(prop?.GetValue(License))
                ?? $"Copyright &copy; Implem Inc. 2014 - {DateTime.Now.Year}";
        }

        public static string CopyrightUrl()
        {
            var prop = License.GetType().GetProperty("CopyrightUrl");
            return (string)(prop?.GetValue(License))
                ?? "https://implem.co.jp";
        }

        public static bool DisableAds()
        {
            var prop = License.GetType().GetProperty("DisableAds");
            return (bool?)(prop?.GetValue(License)) ?? false;
        }

        public static int Environment()
        {
            //1:共用環境、2:DEMO環境、3:トライアル(期限関係なし) 0:その他
            var prop = License.GetType().GetProperty("Environment");
            var retVal = (int?)(prop?.GetValue(License)) ?? 0;
            return retVal != 0
                ? retVal
                : TrialLicense != null
                    ? 3
                    : 0;
        }

        public static Dictionary<string, object> GetLicenseInfo()
        {
            var ret = new Dictionary<string, object>
            {
                { "CommercialLicense", CommercialLicense() },
                { "LicensedUsers", LicensedUsers() },
                { "LicenseDeadline", LicenseDeadline() },
                { "Licensee", Licensee() }
            };
            return ret;
        }
    }
}