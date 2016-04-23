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
                return Path.Combine(Environments.CurrentDirectoryPath, pathes.Join("\\"));
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
            var path = Parameters.Path.Definitions;
            return path.IsNullOrEmpty()
                ? Outputs("App_Data", "Definitions") + fileName.IsNotEmpty("\\" + fileName)
                : path;
        }

        public static string Imports()
        {
            var path = Parameters.Path.Imports;
            return path.IsNullOrEmpty()
                ? Outputs("App_Data", "Imports")
                : path;
        }

        public static string Temp()
        {
            var path = Parameters.Path.Temp;
            return path.IsNullOrEmpty()
                ? Path.Combine(Environments.CurrentDirectoryPath, "App_Data", "Temp")
                : path;
        }

        public static string Logs()
        {
            var path = Parameters.Path.Logs;
            return path.IsNullOrEmpty()
                ? Path.Combine(Environments.CurrentDirectoryPath, "App_Data", "Logs")
                : path;
        }

        public static string Histories()
        {
            var path = Parameters.Path.Histories;
            return path.IsNullOrEmpty()
                ? Path.Combine(Environments.CurrentDirectoryPath, "App_Data", "Histories")
                : path;
        }

        public static string BinaryStorage()
        {
            var path = Parameters.BinaryStorage.Path;
            return path.IsNullOrEmpty()
                ? Path.Combine(Environments.CurrentDirectoryPath, "App_Data", "BinaryStorage")
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
