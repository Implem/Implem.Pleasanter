using Implem.ParameterAccessor.Parts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Zx;

namespace Implem.PleasanterSetup
{
    public class PleasanterSetup : ConsoleAppBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<PleasanterSetup> logger;
        private string installDir;
        private string dbms;
        private string server;
        private string serviceName;
        private string userId;
        private string password;
        private bool versionUp;
        private enum DBMS
        {
            SQLServer = 1,
            PostgreSQL = 2
        };

        public PleasanterSetup(
            IConfiguration configuration,
            ILogger<PleasanterSetup> logger)
        {
            this.configuration = configuration;
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
            // ユーザにセットアップに必要な情報を入力してもらう
            SetSummary(directory);
            // 新規インストールまたはバージョンアップを行うかをユーザに確認する
            var doNext = AskForInstallOrVersionUp(
                installDir: installDir,
                dbms: dbms,
                server: server,
                serviceName: serviceName,
                userId: userId,
                password: password);
            if (doNext)
            {
                var backupDir = Path.Combine(
                    Path.GetDirectoryName(installDir),
                    $"{Path.GetFileName(installDir)}{DateTime.Now:_yyyyMMdd_HHmmss}");
                // バージョンアップの場合は既存資源のバックアップを行う
                if (versionUp)
                {
                    CopyResourceDirectory(
                        installDir: installDir,
                        destDir: backupDir);
                }
                // 新しい資源を配置する
                SetNewResource(
                    installDir: installDir,
                    releaseZip: releasezip);
                // ユーザがコンソール上で入力した値を各パラメータファイルへ書き込む
                SetParameters();
                // バージョンアップの場合はパラメータファイルのマージを行う
                if (versionUp)
                {
                    Merge(
                        source: Path.Combine(
                            backupDir,
                            "Implem.Pleasanter",
                            "App_Data",
                            "Parameters"),
                        destination: Path.Combine(
                            installDir,
                            "Implem.Pleasanter",
                            "App_Data",
                            "Parameters"));
                }
                // データベースの作成(スキーマ更新)を行う
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
            var resourceDir = string.IsNullOrEmpty(directory)
                ? GetDefaultInstallDir(directory)
                : directory;
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

        private bool MeetsVersionUpRequirements(string parametersDir)
        {
            return Directory.Exists(parametersDir) && Directory.GetFiles(parametersDir).Length > 0;
        }

        private void CopyResourceDirectory(string installDir, string destDir)
        {
            Directory.CreateDirectory(destDir);
            foreach (var dir in Directory.GetDirectories(installDir))
            {
                Directory.Move(dir, dir.Replace(installDir, destDir));
            }
        }

        private void SetNewResource(string installDir, string releaseZip)
        {
            if (!Directory.Exists(installDir))
            {
                Directory.CreateDirectory(installDir);
            }
            if (File.Exists(releaseZip))
            {
                var releaseZipDir = Path.GetDirectoryName(releaseZip);
                ZipFile.ExtractToDirectory(releaseZip, releaseZipDir, true);
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
        }

        private void DisplaySummary(
            string installDir,
            string dbms,
            string server,
            string serviceName,
            string userId,
            string password)
        {
            logger.LogInformation("------ Summary ------");
            logger.LogInformation($"Install Directory : {installDir}");
            logger.LogInformation($"DBMS              : {Enum.GetName(typeof(DBMS), int.Parse(dbms))}");
            logger.LogInformation($"Server            : {server}");
            logger.LogInformation($"Service Name      : {serviceName}");
            logger.LogInformation($"User ID           : {userId}");
            logger.LogInformation($"Password          : {password}");
            logger.LogInformation("---------------------");
        }

        private bool AskForInstallOrVersionUp(
            string installDir,
            string dbms,
            string server,
            string serviceName,
            string userId,
            string password)
        {
            DisplaySummary(installDir, dbms, server, serviceName, userId, password);
            logger.LogInformation("Shall I install Pleasanter with this content? Please enter ‘y(yes)' or 'n(no)’. : ");
            var doNext = Console.ReadLine();
            while (string.IsNullOrEmpty(doNext))
            {
                doNext = Console.ReadLine();
            }
            return doNext.ToLower().StartsWith("y");
        }

        private void SetSummary(string directory)
        {
            installDir = AskForInstallDir(directory);
            versionUp = MeetsVersionUpRequirements(
                Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "App_Data",
                    "Parameters"));
            dbms = AskForDbms(versionUp);
            server = AskForServer(versionUp);
            serviceName = AskForServiceName(versionUp);
            userId = AskForUserId(versionUp);
            password = AskForPassword(versionUp);
        }

        private string AskForInstallDir(string directory)
        {
            var installDir = GetDefaultInstallDir(directory);
            if (string.IsNullOrEmpty(installDir))
            {
                logger.LogInformation("Install Directory [Default: /web/pleasanter] : ");
                var userInputResourceDir = Console.ReadLine();
                if (!string.IsNullOrEmpty(userInputResourceDir))
                {
                    installDir = userInputResourceDir;
                }
            }
            return installDir;
        }

        private string AskForDbms(bool versionUp)
        {
            if (versionUp)
            {
                var file = Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "App_Data",
                    "Parameters",
                    "Rds.json");
                if (File.Exists(file))
                {
                    var json = File.ReadAllText(file);
                    var data = json.Deserialize<Rds>();
                    dbms = data.Dbms;
                }
            }
            else
            {
                logger.LogInformation("DBMS [1: SQL Server, 2: PostgreSQL] : ");
                while (dbms != "1" && dbms != "2")
                {
                    dbms = Console.ReadLine();
                }

            }
            return dbms;
        }

        private string AskForServer(bool versionUp)
        {
            var server = string.Empty;
            if (versionUp)
            {
                var file = Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "App_Data",
                    "Parameters",
                    "Rds.json");
                if (File.Exists(file))
                {
                    var json = File.ReadAllText(file);
                    var data = json.Deserialize<Rds>();
                    dbms = data.Dbms;
                }
            }
            else
            {
                logger.LogInformation("Server [Default: localhost] : ");
                var userInputServer = Console.ReadLine();
                server = string.IsNullOrEmpty(userInputServer)
                    ? configuration["DefaultParameters:HostName"] ?? string.Empty
                    : userInputServer;

            }
            return server;
        }

        private string AskForServiceName(bool versionUp)
        {
            if (versionUp)
            {
                var file = Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "App_Data",
                    "Parameters",
                    "Service.json");
                if (File.Exists(file))
                {
                    var json = File.ReadAllText(file);
                    var data = json.Deserialize<Service>();
                    serviceName = data.Name;
                }
            }
            else
            {
                logger.LogInformation("[Default: Implem.Pleasanter] : ");
                var userInputServiceName = Console.ReadLine();
                serviceName = string.IsNullOrEmpty(userInputServiceName)
                    ? configuration["DefaultParameters:ServiceName"] ?? string.Empty
                    : userInputServiceName;

            }
            return serviceName;
        }

        private string AskForUserId(bool versionUp)
        {
            if (versionUp)
            {
                var file = Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "App_Data",
                    "Parameters",
                    "Service.json");
                if (File.Exists(file))
                {
                    var json = File.ReadAllText(file);
                    var data = json.Deserialize<Rds>();
                    userId = data.SaConnectionString
                        .Split(";")
                        .Where(o => o.StartsWith("UID="))
                        .FirstOrDefault()
                        .Split("=")
                        .Last();
                }
            }
            else
            {
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
            }
            return userId;
        }

        private string AskForPassword(bool versionUp)
        {
            if (versionUp)
            {
                var file = Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "App_Data",
                    "Parameters",
                    "Service.json");
                if (File.Exists(file))
                {
                    var json = File.ReadAllText(file);
                    var data = json.Deserialize<Rds>();
                    password = data.SaConnectionString
                        .Split(";")
                        .Where(o => o.StartsWith("PWD="))
                        .FirstOrDefault()
                        .Split("=")
                        .LastOrDefault();
                }
            }
            else
            {
                logger.LogInformation("Password : ");
                var password = "";
                while (string.IsNullOrEmpty(password))
                {
                    password = Console.ReadLine();
                }
            }
            return password;
        }

        private string GetDefaultInstallDir(string directory)
        {
            var installDir = directory;
            switch (RuntimeInformation.OSDescription)
            {
                case "Windows":
                    installDir = configuration["DefaultParameters:InstallDirForWindows"];
                    break;
                default:
                    installDir = configuration["InstallDirForLinux"];
                    break;
            }
            return installDir;
        }

        private void SetParameters()
        {
            var parametersDir = Path.Combine(
                installDir,
                "Implem.Pleasanter",
                "App_Data",
                "Parameters");
            SetRdsParameters(parametersDir);
            SetServiceParameters(parametersDir);
        }

        private void SetRdsParameters(string parametersDir)
        {
            var file = Path.Combine(parametersDir, "Rds.json");
            var json = File.ReadAllText(file);
            var data = json.Deserialize<Rds>();
            data.Dbms = dbms;
            var database = dbms == "1"
                ? "master"
                : dbms == "2"
                    ? "postgres"
                    : string.Empty;
            data.SaConnectionString = $"Server={server};Database={database};UID={userId};PWD={password};";
            data.OwnerConnectionString = $"Server={server};Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={password};";
            data.UserConnectionString = $"Server={server};Database=#ServiceName#;UID=#ServiceName#_User;PWD={password};";
            File.WriteAllText(file, data.ToJson());
        }

        private void SetServiceParameters(string parametersDir)
        {
            var file = Path.Combine(parametersDir, "Service.json");
            var json = File.ReadAllText(file);
            var data = json.Deserialize<Service>();
            data.Name = serviceName;
            File.WriteAllText(file, data.ToJson());
        }
    }
}
