using System.IO.Compression;
using Zx;

namespace Implem.PleasanterSetup
{
    public class PleasanterSetup : ConsoleAppBase
    {
        public string DefaultRecourceDir = Path.Combine(Environment.GetEnvironmentVariable("SystemDrive"), "web", "pleasanter");
        public bool VersionUp = false;

        public PleasanterSetup()
        {
        }

        [RootCommand]
        public async Task Setup(
            [Option(0)] string releasezip,
            [Option("d")] string directory = "",
            bool force = false,
            bool noinput = false,
            string license = "",
            string extendedcolumns = "")
        {
            var resourceDir = string.IsNullOrEmpty(directory)
                ? DefaultRecourceDir
                : Path.Combine(directory);
            var parametersDir = "";
            var backupParametersDir = "";
            if (Directory.Exists(resourceDir))
            {
                parametersDir = Path.Combine(resourceDir, "Implem.Pleasanter", "App_Data", "Parameters");
                if (Directory.Exists(parametersDir) && Directory.GetFiles(parametersDir).Length > 0)
                {
                    VersionUp = true;
                    // 既存資源のバックアップ
                    var backupDir = Path.Combine(
                        Path.GetDirectoryName(resourceDir),
                        $"{Path.GetFileName(resourceDir)}{DateTime.Now:_yyyyMMdd_HHmmss}");
                    Console.WriteLine($"backupDir: {backupDir}");
                    backupParametersDir = Path.Combine(backupDir, "Implem.Pleasanter", "App_Data", "Parameters");
                    Directory.CreateDirectory(backupDir);
                    foreach(var dir in Directory.GetDirectories(resourceDir))
                    {
                        Directory.Move(dir, dir.Replace(resourceDir, backupDir));
                    }
                }
                // 新しい資源の配置
                if (File.Exists(releasezip))
                {
                    var releaseZipDir = Path.GetDirectoryName(releasezip);
                    ZipFile.ExtractToDirectory(releasezip, releaseZipDir, true);
                    var unzipDir = Path.Combine(releaseZipDir, "pleasanter");
                    foreach(var dir in Directory.GetDirectories(unzipDir))
                    {
                        Directory.Move(dir, dir.Replace(unzipDir, resourceDir));
                    }
                    Directory.Delete(unzipDir);
                }
                else
                {
                    Console.WriteLine("プリザンターのリリースファイルが存在しません。");
                    return;
                }
                // パラメータファイルのマージ
                Merge(source: backupParametersDir, destination: parametersDir);
                // データベースの作成(スキーマ更新)
                await Rds(
                    directory: resourceDir,
                    force: force,
                    noinput: noinput,
                    license: license,
                    extendedcolumns: extendedcolumns);
            }
            else
            {
                Console.WriteLine("プリザンターの資源を配置するディレクトリが存在しません。");
                return;
            }
        }

        [Command("merge")]
        public void Merge(
            [Option("s")] string source,
            [Option("d")] string destination,
            string[]? excludes = null)
        {
            Console.WriteLine("This is Merge Command.");
            if (!Directory.Exists(source))
            {
                Console.WriteLine($"\"{source}\" does not exist.");
                return;
            }
            if (!Directory.Exists(destination))
            {
                Console.WriteLine($"\"{destination}\" does not exist.");
                return;
            }
            Merger.MergeParametersJson(
                sourcePath: source,
                destPath: destination,
                excludeFiles: excludes);
        }

        [Command("rds")]
        public async Task Rds(
            [Option("d")] string directory = "",
            bool force = false,
            bool noinput = false,
            string license = "",
            string extendedcolumns = "")
        {
            Console.WriteLine("This is Rds Command.");
            var resourceDir = string.IsNullOrEmpty(directory)
                ? DefaultRecourceDir
                : Path.Combine(directory);
            if (File.Exists(license))
            {
                var licenseDir = Path.GetDirectoryName(license);
                var unzipDir = Path.Combine(licenseDir, Path.GetFileNameWithoutExtension(license));
                ZipFile.ExtractToDirectory(license, unzipDir, true);
                CopyLicense(resourceDir, unzipDir);
            }
            await ExecuteCodeDefiner(
                codeDefinerDir: Path.Combine(resourceDir, "Implem.CodeDefiner"),
                force: force,
                noinput: noinput);
        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        static void CopyLicense(string resourceDir, string licenseDir)
        {
            var license = Path.Combine(licenseDir, "forNetCore(MultiPlatform)", "Implem.License.dll");
            if (File.Exists(license))
            {
                if (File.Exists(resourceDir))
                {
                    if (File.Exists(Path.Combine(resourceDir, "Implem.CodeDefiner")))
                    {
                        File.Copy(
                            license,
                            Path.Combine(resourceDir, "Implem.CodeDefiner", "Implem.License.dll"),
                            true);
                    }
                    if (File.Exists(Path.Combine(resourceDir, "Implem.Pleasanter")))
                    {
                        File.Copy(
                            license,
                            Path.Combine(resourceDir, "Implem.Pleasanter", "Implem.License.dll"),
                            true);
                    }
                }
            }
        }

        private async Task ExecuteCodeDefiner(string codeDefinerDir, bool force, bool noinput)
        {
            var forceOption = force ? "-f": "";
            var noInputOption = noinput ? "-n" : "";
            await $"cd {codeDefinerDir}";
            await $"dotnet Implem.CodeDefiner.dll _rds {forceOption} {noInputOption}";
          
        } 
    }
}
