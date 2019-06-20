using System.IO;
using System.IO.Compression;
namespace Implem.Libraries.Utilities
{
    public static class Archives
    {
        public static void Zip(
            string zipFilePath,
            string sourceFilePath,
            string distinationFileName,
            string tempPath)
        {
            if (File.Exists(zipFilePath))
            {
                if (!File.Exists(sourceFilePath))
                {
                    tempPath = Path.Combine(tempPath, Strings.NewGuid());
                    Directory.CreateDirectory(tempPath);
                    ZipFile.CreateFromDirectory(tempPath, sourceFilePath);
                    Files.DeleteDirectory(tempPath);
                }
                ZipWrite(zipFilePath, sourceFilePath, distinationFileName);
            }
            else
            {
                if (Directory.Exists(zipFilePath))
                {
                    if (!File.Exists(sourceFilePath))
                    {
                        ZipFile.CreateFromDirectory(zipFilePath, sourceFilePath);
                    }
                    else
                    {
                        ZipWrite(zipFilePath, sourceFilePath, distinationFileName);
                    }
                }
            }
        }

        private static void ZipWrite(
            string sourceFilePath,
            string zipFilePath,
            string distinationFileName)
        {
            using (var zipFile = new FileStream(zipFilePath, FileMode.Open))
            {
                ZipWrite(sourceFilePath, distinationFileName, zipFile);
            }
        }

        private static void ZipWrite(
            string sourceFilePath,
            string distinationFileName,
            FileStream zipFile)
        {
            using (var zipArchive = new ZipArchive(zipFile, ZipArchiveMode.Update))
            {
                zipArchive.CreateEntryFromFile(sourceFilePath, distinationFileName);
            }
        }
    }
}
