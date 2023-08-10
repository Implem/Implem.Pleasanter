namespace Implem.PleasanterSetup
{
    internal static class Merger
    {
        internal static void MergeParametersJson(string sourcePath, string destPath, string[] excludeFiles)
        {
            try
            {
                CopySourceOnlyFiles(sourcePath, destPath);
                MegreJsonFiles(sourcePath, destPath, excludeFiles);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void MegreJsonFiles(string sourcePath, string destPath, string[] excludeFiles)
        {
            foreach (var file in Directory.GetFiles(destPath, "*.json", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);
                var fileContent = File.ReadAllText(file);
                var sourceFile = file.Replace(destPath, sourcePath);
                var outputFile = Path.GetFullPath(file);
                if (File.Exists(sourceFile))
                {
                    if (excludeFiles?.Contains(fileName) == true)
                    {
                        continue;
                    }
                    var sourceFileContent = File.ReadAllText(sourceFile);
                    if (fileContent != sourceFileContent)
                    {
                        if (fileName.ToLower() == "navigationmenus.json")
                        {
                            File.WriteAllText(file, sourceFileContent);
                            continue;
                        }
                        var mergedJson = Jsons.Merge(sourceFileContent, fileContent);
                        File.WriteAllText(file, mergedJson);
                    }
                    else
                    {
                    }
                }
            }
        }

        private static void CopySourceOnlyFiles(string sourcePath, string destPath)
        {
            var onlySourceFiles = Directory
                .GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)
                .Where(file => !File.Exists(file.Replace(sourcePath, destPath)));
            foreach (var file in onlySourceFiles)
            {
                var replacedFilePath = file.Replace(sourcePath, destPath);
                var replacedDirectoryPath = Path.GetDirectoryName(replacedFilePath);
                if (!Directory.Exists(replacedDirectoryPath))
                {
                    Directory.CreateDirectory(replacedDirectoryPath);
                }
                File.Copy(file, replacedFilePath, overwrite: true);
            }
        }
    }
}
