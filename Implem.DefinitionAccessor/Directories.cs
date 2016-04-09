using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Implem.DefinitionAccessor
{
    public static class Directories
    {
        private const string CodeDefinerDirectoryName = "Implem.CodeDefiner";
        private const string DefinitionAccessorDirectoryName = "Implem.DefinitionAccessor";
        private const string LibrariesDirectoryName = "Implem.Libraries";

        public static string ServicePath()
        {
            return new DirectoryInfo(Environments.CurrentDirectoryPath).Parent.FullName;
        }

        public static string CodeDefiner(params string[] pathes)
        {
            return CurrentPath(CodeDefinerDirectoryName, pathes.ToList());
        }

        public static string DefinitionAccessor(params string[] pathes)
        {
            return CurrentPath(DefinitionAccessorDirectoryName, pathes.ToList());
        }

        public static string Libraries(params string[] pathes)
        {
            return CurrentPath(LibrariesDirectoryName, pathes.ToList());
        }

        public static string Outputs(params string[] pathes)
        {
            if (Environments.CodeDefiner)
            {
                return CurrentPath("Implem." + Environments.ServiceName, pathes.ToList());
            }
            else
            {
                var list = pathes.ToList();
                list.Insert(0, Environments.CurrentDirectoryPath);
                return Path.Combine(list.ToArray());
            }
        }

        public static string Definitions(string fileName = "")
        {
            var path = Def.Parameters.DefinitionsPath;
            return path.IsNullOrEmpty()
                ? Outputs("App_Data", "Definitions") + fileName.IsNotEmpty("\\" + fileName)
                : path;
        }

        public static string Imports()
        {
            var path = Def.Parameters.ImportsPath;
            return path.IsNullOrEmpty()
                ? Outputs("App_Data", "Imports")
                : path;
        }

        public static string Temp()
        {
            var path = Def.Parameters.TempPath;
            return path.IsNullOrEmpty()
                ? Path.Combine(Environments.CurrentDirectoryPath, "App_Data", "Temp")
                : path;
        }

        public static string Logs()
        {
            var path = Def.Parameters.LogsPath;
            return path.IsNullOrEmpty()
                ? Path.Combine(Environments.CurrentDirectoryPath, "App_Data", "Logs")
                : path;
        }

        public static string Histories()
        {
            var path = Def.Parameters.HistoriesPath;
            return path.IsNullOrEmpty()
                ? Path.Combine(Environments.CurrentDirectoryPath, "App_Data", "Histories")
                : path;
        }

        public static string Data()
        {
            var path = Def.Parameters.DataPath;
            return path.IsNullOrEmpty()
                ? Path.Combine(Environments.CurrentDirectoryPath, "App_Data", "Data")
                : path;
        }

        internal static string CurrentPath(string directoryName, List<string> pathes)
        {
            pathes.Insert(0, ServicePath());
            pathes.Insert(1, directoryName);
            return Path.Combine(pathes.ToArray());
        }
    }
}
