using Microsoft.Extensions.Logging;
using System.IO.Compression;
using Zx;

namespace Implem.PleasanterSetup
{
    public class PleasanterSetup : ConsoleAppBase
    {
        private enum DBMS
        {
            SQLServer = 1,
            PostgreSQL = 2
        };
        private string defaultInstallDir = Path.Combine(Environment.GetEnvironmentVariable("SystemDrive"), "web", "pleasanter");
        private string defaultHostName = "localhost";
        private string defaultServiceName = "Implem.Pleasanter";
        private bool versionUp = false;
        private ILogger logger;

        public PleasanterSetup(ILogger<PleasanterSetup> logger)
        {
            this.logger = logger;
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
            logger.LogInformation("This is Setup Command.");
            var installDir = defaultInstallDir;
            if (string.IsNullOrEmpty(directory))
            {
                logger.LogInformation("Install Directory [Default: /web/pleasanter] : ");
                var userInputResourceDir = Console.ReadLine();
                if (!string.IsNullOrEmpty(userInputResourceDir))
                {
                    installDir = userInputResourceDir;
                }
            }
            logger.LogInformation("DBMS [1: SQL Server, 2: PostgreSQL] : ");
            var dbms = "";
            while (dbms != "1" && dbms != "2")
            {
                dbms = Console.ReadLine();
            }
            logger.LogInformation("Server [Default: localhost] : ");
            var userInputServer = Console.ReadLine();
            var server = string.IsNullOrEmpty(userInputServer)
                ? defaultHostName
                : userInputServer;
            logger.LogInformation("[Default: Implem.Pleasanter] : ");
            var userInputServiceName = Console.ReadLine();
            var serviceName = string.IsNullOrEmpty(userInputServiceName)
                ? defaultServiceName
                : userInputServiceName;
            logger.LogInformation("[Default: sa(SQL Server), postgres(PostgreSQL)] : ");
            var userId = Console.ReadLine();
            if (string.IsNullOrEmpty(userId))
            {
                switch ((DBMS)int.Parse(dbms))
                {
                    case DBMS.SQLServer:
                        userId = "sa";
                        break;
                    case DBMS.PostgreSQL:
                        userId = "postgres";
                        break;
                }
            }
            logger.LogInformation("Password : ");
            var password = "";
            while (string.IsNullOrEmpty(password))
            {
                password = Console.ReadLine();
            }
            logger.LogInformation("----- Summary -----");
            logger.LogInformation($"Install Directory : {installDir}");
            logger.LogInformation($"DBMS              : {Enum.GetName(typeof(DBMS), int.Parse(dbms))}");
            logger.LogInformation($"Server            : {server}");
            logger.LogInformation($"Service Name      : {serviceName}");
            logger.LogInformation($"User ID           : {userId}");
            logger.LogInformation($"Password          : {password}");
            logger.LogInformation("---------------------");
            logger.LogInformation("Shall I install Pleasanter with this content? Please enter ‘y(yes)' or 'n(no)’. : ");
            var doNext = Console.ReadLine();
            while (string.IsNullOrEmpty(doNext))
            {
                doNext = Console.ReadLine();
            }
            if (doNext.StartsWith("y"))
            {
                if (!Directory.Exists(installDir))
                {
                    Directory.CreateDirectory(installDir);
                }
                var parametersDir = Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "App_Data",
                    "Parameters");
                var backupDir = Path.Combine(
                    Path.GetDirectoryName(installDir),
                    $"{Path.GetFileName(installDir)}{DateTime.Now:_yyyyMMdd_HHmmss}");
                logger.LogInformation($"backupDir: {backupDir}");
                var backupParametersDir = Path.Combine(
                    backupDir,
                    "Implem.Pleasanter",
                    "App_Data",
                    "Parameters");
                if (Directory.Exists(parametersDir) && Directory.GetFiles(parametersDir).Length > 0)
                {
                    versionUp = true;
                    // 既存資源のバックアップ
                    Directory.CreateDirectory(backupDir);
                    foreach (var dir in Directory.GetDirectories(installDir))
                    {
                        Directory.Move(dir, dir.Replace(installDir, backupDir));
                    }
                }
                // 新しい資源の配置
                if (File.Exists(releasezip))
                {
                    var releaseZipDir = Path.GetDirectoryName(releasezip);
                    ZipFile.ExtractToDirectory(releasezip, releaseZipDir, true);
                    var unzipDir = Path.Combine(releaseZipDir, "pleasanter");
                    foreach (var dir in Directory.GetDirectories(unzipDir))
                    {
                        Directory.Move(dir, dir.Replace(unzipDir, installDir));
                    }
                    Directory.Delete(unzipDir);
                }
                else
                {
                    logger.LogError("プリザンターのリリースファイルが存在しません。");
                    return;
                }
                // パラメータファイルのマージ
                Merge(source: backupParametersDir, destination: parametersDir);
                // データベースの作成(スキーマ更新)
                await Rds(
                    directory: installDir,
                    force: force,
                    noinput: noinput,
                    license: license,
                    extendedcolumns: extendedcolumns);
            }
            else
            {
                logger.LogInformation("セットアップコマンドの処理を終了します。");
                return;
            }
        }

        [Command("merge")]
        public void Merge(
            [Option("s")] string source,
            [Option("d")] string destination,
            string[]? excludes = null)
        {
            logger.LogInformation("This is Merge Command.");
            if (!Directory.Exists(source))
            {
                logger.LogError($"\"{source}\" does not exist.");
                return;
            }
            if (!Directory.Exists(destination))
            {
                logger.LogError($"\"{destination}\" does not exist.");
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
            logger.LogInformation("This is Rds Command.");
            var resourceDir = string.IsNullOrEmpty(directory)
                ? defaultInstallDir
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
            var forceOption = force ? "-f" : "";
            var noInputOption = noinput ? "-n" : "";
            await $"cd {codeDefinerDir}";
            await $"dotnet Implem.CodeDefiner.dll _rds {forceOption} {noInputOption}";
            await $"cd -";
        }
    }
}
