using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.IO;
namespace Implem.CodeDefiner.Utilities
{
    internal static class CodeHistories
    {
        internal static void Create(string filePath)
        {
            var zipFilePath = ZipFilePath();
            var sourceFilePath = SourceFilePath(filePath);
            var sourceFileName = SourceFileName(sourceFilePath);
            Archives.Zip(filePath, zipFilePath, sourceFileName, Directories.Temp());
        }

        private static string ZipFilePath()
        {
            return Path.Combine(Directories.Histories(), HistoryFileName());
        }

        private static string SourceFilePath(string filePath)
        {
            return filePath.DirectoryName() + @"\" +
                filePath.FileNameOnly() + "_" +
                DateTimes.Full() +
                filePath.FileExtension();
        }

        private static string HistoryFileName()
        {
            return "History_" + DateTimes.Full() + Environments.Guid + ".zip";
        }

        private static string SourceFileName(string sourceFilePath)
        {
            return sourceFilePath.Replace(@":", "").Replace(@"\", "_");
        }
    }
}
