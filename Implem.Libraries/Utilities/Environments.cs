using System;
namespace Implem.Libraries.Utilities
{
    public static class Environments
    {
        public static bool CodeDefiner;
        public static string CurrentDirectoryPath;
        public static string ServiceName;
        public static string Guid = Strings.NewGuid();
        public static string MachineName;
        public static string Application;
        public static string AssemblyVersion;
        public static string RdsProvider;
        public static TimeZoneInfo RdsTimeZoneInfo;
        public static TimeZoneInfo TimeZoneInfoDefault;
        public static int DeadlockRetryCount;
        public static int DeadlockRetryInterval;
    }
}
