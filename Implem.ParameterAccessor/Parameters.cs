using Implem.ParameterAccessor.Parts;
using System.Collections.Generic;
namespace Implem.DefinitionAccessor
{
    public static class Parameters
    {
        public static bool Enterprise = false;
        public static List<string> SyntaxErrors = new List<string>();
        public static Api Api;
        public static Asset Asset;
        public static Authentication Authentication;
        public static BackgroundTask BackgroundTask;
        public static BinaryStorage BinaryStorage;
        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> CustomDefinitions;
        public static Deleted Deleted;
        public static List<ExtendedColumns> ExtendedColumnsSet;
        public static List<ExtendedSql> ExtendedSqls;
        public static List<string> ExtendedStyles;
        public static List<string> ExtendedScripts;
        public static Formats Formats;
        public static General General;
        public static Health Health;
        public static History History;
        public static Mail Mail;
        public static Notification Notification;
        public static Path Path;
        public static Permissions Permissions;
        public static Rds Rds;
        public static Reminder Reminder;
        public static Search Search;
        public static Security Security;
        public static Service Service;
        public static Session Session;
        public static SysLog SysLog;
    }
}