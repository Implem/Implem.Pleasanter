using Implem.DisplayAccessor;
using Implem.ParameterAccessor.Parts;
using System;
using System.Collections.Generic;
namespace Implem.DefinitionAccessor
{
    public static class Parameters
    {
        public static License.License License = new License.License();
        public static List<string> SyntaxErrors = new List<string>();
        public static Api Api;
        public static Authentication Authentication;
        public static BackgroundTask BackgroundTask;
        public static BinaryStorage BinaryStorage;
        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> CustomDefinitions;
        public static Deleted Deleted;
        public static Dictionary<string, string> ExtendedColumnDefinitions;
        public static List<ExtendedColumns> ExtendedColumnsSet;
        public static List<ExtendedSql> ExtendedSqls;
        public static List<ExtendedStyle> ExtendedStyles;
        public static List<ExtendedScript> ExtendedScripts;
        public static Dictionary<string, List<DisplayElement>> ExtendedHtmls;
        public static Dictionary<string, string> ExtendedTags;
        public static General General;
        public static History History;
        public static ParameterAccessor.Parts.Version Version;
        public static Mail Mail;
        public static Migration Migration;
        public static Notification Notification;
        public static Parameter Parameter;
        public static Permissions Permissions;
        public static Rds Rds;
        public static Registration Registration;
        public static Reminder Reminder;
        public static Search Search;
        public static Security Security;
        public static Service Service;
        public static Session Session;
        public static Site Site;
        public static SitePackage SitePackage;
        public static SysLog SysLog;
        public static Locations Locations;
        public static Validation Validation;

        public static bool CommercialLicense()
        {
            return License.Check();
        }

        public static DateTime LicenseDeadline()
        {
            return License.Deadline;
        }

        public static string Licensee()
        {
            return License.Licensee;
        }
    }
}