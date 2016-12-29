using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System;
using System.IO;
namespace Implem.CodeDefiner.Functions.CodeDefiner
{
    internal static class BackupCreater
    {
        internal static void BackupSolutionFiles()
        {
            string tempPath = CopyToTemp();
            CreateZipFile(tempPath);
            DeleteTempDirectory(tempPath);
        }

        private static string CopyToTemp()
        {
            Consoles.Write(
                text: DisplayAccessor.Displays.Get("InCopying"), type: Consoles.Types.Info);
            var tempPath = TempPath();
            Files.CopyDirectory(
                sourcePath: Directories.ServicePath(),
                destinationPath: tempPath,
                excludePathCollection: Parameters.General.SolutionBackupExcludeDirectories
                    .Split(','));
            return tempPath;
        }

        private static void CreateZipFile(string tempPath)
        {
            Consoles.Write(
                text: DisplayAccessor.Displays.Get("InCompression"), type: Consoles.Types.Info);
            var zipFileName = ZipFileName();
            Archives.Zip(
                zipFilePath: tempPath,
                sourceFilePath: Path.Combine(Parameters.General.SolutionBackupPath, zipFileName),
                distinationFileName: zipFileName,
                tempPath: Directories.Temp());
        }

        private static void DeleteTempDirectory(string tempPath)
        {
            Directory.Delete(path: tempPath, recursive: true);
        }

        private static string TempPath()
        {
            return Path.Combine(Parameters.General.SolutionBackupPath, Strings.NewGuid());
        }

        private static string ZipFileName()
        {
            return 
                Environments.ServiceName + "_" + 
                DateTimes.Full() + "_" + 
                Environment.MachineName + ".zip";
        }
    }
}
