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
        public static ParameterAccessor.Parts.Version Version;
        public static Validation Validation;
        public static Dashboard Dashboard;
        public static OutputCache OutputCache;

        public static bool CommercialLicense()
        {
            return License.Check();
        }

        public static int LicensedUsers()
        {
            return License.Users;
        }

        public static DateTime LicenseDeadline()
        {
            return License.Deadline;
        }

        public static string Licensee()
        {
            return License.Licensee;
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