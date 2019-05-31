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
        public static List<ExtendedColumns> ExtendedColumnsSet;
        public static List<ExtendedSql> ExtendedSqls;
        public static List<string> ExtendedStyles;
        public static List<string> ExtendedScripts;
        public static General General;
        public static History History;
        public static ParameterAccessor.Parts.Version Version;
        public static Mail Mail;
        public static Notification Notification;
        public static Permissions Permissions;
        public static Rds Rds;
        public static Reminder Reminder;
        public static Search Search;
        public static Security Security;
        public static Service Service;
        public static Session Session;
        public static SysLog SysLog;

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