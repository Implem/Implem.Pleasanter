using Implem.ParameterAccessor.Parts;
using System.Collections.Generic;
namespace Implem.DefinitionAccessor
{
    public static class Parameters
    {
        public static Asset Asset;
        public static Authentication Authentication;
        public static BackgroundTask BackgroundTask;
        public static BinaryStorage BinaryStorage;
        public static List<ContractType> ContractTypes;
        public static Dictionary<string, IEnumerable<string>> ExcludeColumns;
        public static List<Format> Formats;
        public static General General;
        public static Health Health;
        public static Mail Mail;
        public static Path Path;
        public static Permissions Permissions;
        public static Rds Rds;
        public static Search Search;
        public static Security Security;
        public static Service Service;
        public static SysLog SysLog;
    }
}
