using Implem.ParameterAccessor.Parts;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Zx;

namespace Implem.PleasanterSetup
{
    public class PleasanterSetup : ConsoleAppBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private string installDir;
        private string dbms;
        private string server;
        private string serviceName;
        private string userId;
        private string password;
        private bool versionUp;
        private ExtendedColumns extendedIssuesColumns;
        private ExtendedColumns extendedResultsColumns;
        private enum DBMS
        {
            SQLServer = 1,
            PostgreSQL = 2
        };

        public PleasanterSetup(
            IConfiguration configuration,
            ILogger logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.installDir = string.Empty;
            this.dbms = string.Empty;
            this.server = string.Empty;
            this.serviceName = string.Empty;
            this.userId = string.Empty;
            this.password = string.Empty;
            this.versionUp = false;
            this.extendedIssuesColumns = new ExtendedColumns();
            this.extendedResultsColumns = new ExtendedColumns();
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
            SetSummary(
                directory: directory,
                licenseZip: license,
                extendedColumnsDir: extendedcolumns);
            // 新規インストールまたはバージョンアップを行うかをユーザに確認する
            var doNext = AskForInstallOrVersionUp();
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
                logger.Info("セットアップコマンドの処理を終了します。");
                return;
            }
        }

        [Command("merge")]
        public void Merge(
            [Option("s")] string source,
            [Option("d")] string destination,
            string[] excludes = null)
        {
            if (!Directory.Exists(source))
            {
                logger.Error($"\"{source}\" does not exist.");
                return;
            }
            if (!Directory.Exists(destination))
            {
                logger.Error($"\"{destination}\" does not exist.");
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
            if (GetCallingMethodName() != "Setup")
            {
                // ユーザにセットアップに必要な情報を入力してもらう
                SetSummary(
                    directory: directory,
                    licenseZip: license,
                    extendedColumnsDir: extendedcolumns);
            }
            var resourceDir = string.IsNullOrEmpty(directory)
                ? GetDefaultInstallDir(directory)
                : directory;
            // ライセンスファイルを配置する
            SetLicense(
                resourceDir: resourceDir,
                licenseZip: license);
            await ExecuteCodeDefiner(
                codeDefinerDir: Path.Combine(
                    resourceDir,
                    "Implem.CodeDefiner"),
                force: force,
                noinput: noinput);
        }

        private bool AskForInstallOrVersionUp()
        {
            DisplaySummary();
            logger.Info("Shall I install Pleasanter with this content? Please enter ‘y(yes)' or 'n(no)’. : ");
            var doNext = Console.ReadLine();
            while (string.IsNullOrEmpty(doNext))
            {
                doNext = Console.ReadLine();
            }
            return doNext.ToLower().StartsWith("y");
        }

        private void AskForInstallDir(string directory)
        {
            logger.Info("Install Directory [Default: /web/pleasanter] : ");
            var userInputResourceDir = Console.ReadLine();
            installDir = !string.IsNullOrEmpty(userInputResourceDir)
                ? userInputResourceDir
                : GetDefaultInstallDir(directory);
        }

        private void AskForDbms(bool versionUp)
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
                else
                {
                    logger.Error($"The file {file} was not found.");
                    Environment.Exit(0);
                }
            }
            else
            {
                logger.Info("DBMS [1: SQL Server, 2: PostgreSQL] : ");
                while (dbms != "1" && dbms != "2")
                {
                    dbms = Console.ReadLine() ?? string.Empty;
                }

            }
        }

        private void AskForServer(bool versionUp)
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
                    server = data.SaConnectionString
                        .Split(";")
                        .Where(o => o.StartsWith("Server="))
                        .FirstOrDefault()?
                        .Split("=")
                        .Last();
                }
                else
                {
                    logger.Error($"The file {file} was not found.");
                    Environment.Exit(0);
                }
            }
            else
            {
                logger.Info("Server [Default: localhost] : ");
                var userInputServer = Console.ReadLine();
                server = string.IsNullOrEmpty(userInputServer)
                    ? configuration["DefaultParameters:HostName"] ?? string.Empty
                    : userInputServer;

            }
        }

        private void AskForServiceName(bool versionUp)
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
                else
                {
                    logger.Error($"The file {file} was not found.");
                    Environment.Exit(0);
                }
            }
            else
            {
                logger.Info("[Default: Implem.Pleasanter] : ");
                var userInputServiceName = Console.ReadLine();
                serviceName = string.IsNullOrEmpty(userInputServiceName)
                    ? configuration["DefaultParameters:ServiceName"] ?? string.Empty
                    : userInputServiceName;

            }
        }

        private void AskForUserId(bool versionUp)
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
                    userId = data.SaConnectionString
                        .Split(";")
                        .Where(o => o.StartsWith("UID="))
                        .FirstOrDefault()?
                        .Split("=")
                        .Last();
                }
                else
                {
                    logger.Error($"The file {file} was not found.");
                    Environment.Exit(0);
                }
            }
            else
            {
                logger.Info("[Default: sa(SQL Server), postgres(PostgreSQL)] : ");
                var userInputUserId = Console.ReadLine();
                if (string.IsNullOrEmpty(userInputUserId))
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
        }

        private void AskForPassword(bool versionUp)
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
                        .FirstOrDefault()?
                        .Split("=")
                        .LastOrDefault();
                }
                else
                {
                    logger.Error($"The file {file} was not found.");
                    Environment.Exit(0);
                }
            }
            else
            {
                logger.Info("Password : ");
                while (string.IsNullOrEmpty(password))
                {
                    password = Console.ReadLine();
                }
            }
        }

        private void AskForExtendedColums(
            string extendedColumnsDir,
            string referenceType)
        {
            var extendedColumns = new ExtendedColumns();
            if (Directory.Exists(extendedColumnsDir))
            {
                var file = Path.Combine(
                    extendedColumnsDir,
                    $"{referenceType}.json");
                extendedColumns = file.Deserialize<ExtendedColumns>();
            }
            else
            {
                logger.Info($"Please enter the number for extended \"{referenceType}\" columns.");
                extendedColumns.Class = AskForItemCount("Class");
                extendedColumns.Num = AskForItemCount("Num");
                extendedColumns.Date = AskForItemCount("Date");
                extendedColumns.Description = AskForItemCount("Description");
                extendedColumns.Check = AskForItemCount("Check");
                extendedColumns.Attachments = AskForItemCount("Attachments");
            }
            switch(referenceType) {
                case "Issues":
                    extendedIssuesColumns = extendedColumns;
                    extendedIssuesColumns.ReferenceType = referenceType;
                    break;
                case "Results":
                    extendedResultsColumns = extendedColumns;
                    extendedResultsColumns.ReferenceType = referenceType;
                    break;
            }
        }

        private int AskForItemCount(string itemName)
        {
            int count;
            do
            {
                logger.Info($"{itemName} [Default 26] : ");
                var userInputCount = Console.ReadLine();
                if (int.TryParse(userInputCount, out count))
                {
                    break;
                }
            }
            while (true);
            return count;
        }

        private string CalculateRangeOfColumns(
            string columnType,
            int count)
        {
            return count > 26
                ? $"{columnType}A - {columnType}{(count - 26).ToString("D3")}"
                : count > 0
                    ? $"{columnType}A - {columnType}{((char)('A' + count - 1)).ToString()}"
                    : string.Empty;
        }

        static void CopyDirectory(
            string sourceDir,
            string destinationDir,
            bool recursive)
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
                string targetFilePath = Path.Combine(
                    destinationDir,
                    file.Name);
                file.CopyTo(targetFilePath);
            }
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(
                        destinationDir,
                        subDir.Name);
                    CopyDirectory(
                        subDir.FullName,
                        newDestinationDir,
                        true);
                }
            }
        }

        static void CopyLicense(
            string resourceDir,
            string licenseDir)
        {
            var license = Path.Combine(
                licenseDir,
                "forNetCore(MultiPlatform)",
                "Implem.License.dll");
            if (File.Exists(license))
            {
                if (File.Exists(resourceDir))
                {
                    if (File.Exists(Path.Combine(
                        resourceDir,
                        "Implem.CodeDefiner")))
                    {
                        File.Copy(
                            license,
                            Path.Combine(
                                resourceDir,
                                "Implem.CodeDefiner",
                                "Implem.License.dll"),
                            true);
                    }
                    if (File.Exists(Path.Combine(
                        resourceDir,
                        "Implem.Pleasanter")))
                    {
                        File.Copy(
                            license,
                            Path.Combine(
                                resourceDir,
                                "Implem.Pleasanter",
                                "Implem.License.dll"),
                            true);
                    }
                }
            }
        }

        private void CopyResourceDirectory(
            string installDir,
            string destDir)
        {
            Directory.CreateDirectory(destDir);
            foreach (var dir in Directory.GetDirectories(installDir))
            {
                Directory.Move(dir, dir.Replace(installDir, destDir));
            }
        }

        private void DisplaySummary()
        {
            logger.Info("------ Summary ------");
            logger.Info($"Install Directory : {installDir}");
            logger.Info($"DBMS              : {Enum.GetName(typeof(DBMS), int.Parse(dbms))}");
            logger.Info($"Server            : {server}");
            logger.Info($"Service Name      : {serviceName}");
            logger.Info($"User ID           : {userId}");
            logger.Info($"Password          : {password}");
            if (!extendedIssuesColumns.Equals(new ExtendedColumns())
                || !extendedResultsColumns.Equals(new ExtendedColumns()))
            {
                logger.Info("[Issues]");
                logger.Info($"    Class       : {CalculateRangeOfColumns("Class", extendedIssuesColumns.Class)}");
                logger.Info($"    Num         : {CalculateRangeOfColumns("Num", extendedIssuesColumns.Num)}");
                logger.Info($"    Date        : {CalculateRangeOfColumns("Date", extendedIssuesColumns.Date)}");
                logger.Info($"    Description :  {CalculateRangeOfColumns("Description", extendedIssuesColumns.Description)}");
                logger.Info($"    Check       :  {CalculateRangeOfColumns("Check", extendedIssuesColumns.Check)}");
                logger.Info($"    Attachments :  {CalculateRangeOfColumns("Attachments", extendedIssuesColumns.Attachments)}");
                logger.Info("[Results]");
                logger.Info($"    Class       :  {CalculateRangeOfColumns("Class", extendedResultsColumns.Class)}");
                logger.Info($"    Num         :  {CalculateRangeOfColumns("Num", extendedResultsColumns.Num)}");
                logger.Info($"    Date        :  {CalculateRangeOfColumns("Date", extendedResultsColumns.Date)}");
                logger.Info($"    Description :  {CalculateRangeOfColumns("Description", extendedResultsColumns.Description)}");
                logger.Info($"    Check       :  {CalculateRangeOfColumns("Check", extendedResultsColumns.Check)}");
                logger.Info($"    Attachments :  {CalculateRangeOfColumns("Attachments", extendedResultsColumns.Attachments)}");
            }
            logger.Info("---------------------");
        }

        private async Task ExecuteCodeDefiner(
            string codeDefinerDir,
            bool force,
            bool noinput)
        {
            var forceOption = force ? "-f" : "";
            var noInputOption = noinput ? "-n" : "";
            await $"cd {codeDefinerDir}";
            await $"dotnet Implem.CodeDefiner.dll _rds {forceOption} {noInputOption}";
            await $"cd -";
        }

        private string GetDefaultInstallDir(string directory)
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                ? installDir = configuration["DefaultParameters:InstallDirForWindows"]
                : installDir = configuration["InstallDirForLinux"];
        }

        private string GetCallingMethodName()
        {
            var callingMethodName = string.Empty;
            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();
            if (stackFrames != null && stackFrames.Length > 1)
            {
                var callingFrame = stackFrames[1];
                var callingMethod = callingFrame.GetMethod();
                callingMethodName = callingMethod?.Name ?? string.Empty;
            }
            return callingMethodName;
        }

        private void MeetsVersionUpRequirements(string parametersDir)
        {
            versionUp = Directory.Exists(parametersDir) && Directory.GetFiles(parametersDir).Length > 0;
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
            SetExtendedColumns(parametersDir);
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

        private void SetLicense(
            string resourceDir,
            string licenseZip)
        {
            if (string.IsNullOrEmpty(licenseZip) && File.Exists(licenseZip))
            {
                var baseFileName = Path.GetFileNameWithoutExtension(licenseZip);
                var unzipDir = Path.Combine(
                    Path.GetDirectoryName(licenseZip),
                    baseFileName);
                ZipFile.ExtractToDirectory(
                    licenseZip,
                    unzipDir,
                    true);
                var license = Path.Combine(
                    unzipDir,
                    "forNetCore(MultiPlatform)",
                    "Implem.License.dll");
                if (File.Exists(license))
                {
                    if (File.Exists(resourceDir))
                    {
                        if (File.Exists(Path.Combine(
                            resourceDir,
                            "Implem.CodeDefiner")))
                        {
                            File.Copy(
                                license,
                                Path.Combine(
                                    resourceDir,
                                    "Implem.CodeDefiner",
                                    "Implem.License.dll"),
                                true);
                        }
                        if (File.Exists(Path.Combine(
                            resourceDir,
                            "Implem.Pleasanter")))
                        {
                            File.Copy(
                                license,
                                Path.Combine(
                                    resourceDir,
                                    "Implem.Pleasanter",
                                    "Implem.License.dll"),
                                true);
                        }
                    }
                }
                Directory.Delete(unzipDir);
            }
        }

        private void SetExtendedColumns(string parametersDir)
        {
            var issuesFile = Path.Combine(
                parametersDir,
                "ExtendedColumns",
                "Issues.json");
            var resultsFile = Path.Combine(
                parametersDir,
                "ExtendedColumns",
                "Results.json");
            if (extendedIssuesColumns != null)
            {
                SetDisabledColumns(extendedIssuesColumns);
                File.WriteAllText(
                    issuesFile,
                    extendedIssuesColumns.ToJson());
            }
            if (extendedResultsColumns != null)
            {
                SetDisabledColumns(extendedResultsColumns);
                File.WriteAllText(
                    resultsFile,
                    extendedResultsColumns.ToJson());
            }
        }

        private void SetDisabledColumns(ExtendedColumns extendedColumns)
        {
            var disabledColumns = new List<string>();
            foreach (var p in extendedColumns.GetType().GetProperties())
            {
                switch (p.Name)
                {
                    case "Class":
                        SetDisabledColumnsList("Class", extendedColumns.Class);
                        break;
                    case "Num":
                        SetDisabledColumnsList("Num", extendedColumns.Num);
                        break;
                    case "Date":
                        SetDisabledColumnsList("Date", extendedColumns.Date);
                        break;
                    case "Description":
                        SetDisabledColumnsList("Description", extendedColumns.Description);
                        break;
                    case "Check":
                        SetDisabledColumnsList("Check", extendedColumns.Check);
                        break;
                    case "Attachments":
                        SetDisabledColumnsList("Attachments", extendedColumns.Attachments);
                        break;
                    default:
                        break;
                }
            }
            extendedColumns.DisabledColumns = disabledColumns;
        }

        private List<string> SetDisabledColumnsList(string columnType, int count)
        {
            var disabeledColumnsList = new List<string>();
            var deleteCount = 26 - count;
            if (deleteCount > 0)
            {
                for (var i = 1; i <= deleteCount; i++)
                {
                    var alphabet = ((char)('A' + 26 - 1 - i)).ToString();
                    disabeledColumnsList.Add($"{columnType}{alphabet}");
                }
            }
            disabeledColumnsList.Sort();
            return disabeledColumnsList;
        }

        private void SetSummary(
            string directory,
            string licenseZip,
            string extendedColumnsDir)
        {
            AskForInstallDir(directory);
            MeetsVersionUpRequirements(
                Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "App_Data",
                    "Parameters"));
            AskForDbms(versionUp);
            AskForServer(versionUp);
            AskForServiceName(versionUp);
            AskForUserId(versionUp);
            AskForPassword(versionUp);
            if (File.Exists(licenseZip))
            {
                AskForExtendedColums(
                    extendedColumnsDir,
                    "Issues");
                AskForExtendedColums(
                    extendedColumnsDir,
                    "Results");
            }
        }

        private void SetNewResource(
            string installDir,
            string releaseZip)
        {
            if (!Directory.Exists(installDir))
            {
                Directory.CreateDirectory(installDir);
            }
            if (File.Exists(releaseZip))
            {
                var releaseZipDir = Path.GetDirectoryName(releaseZip);
                ZipFile.ExtractToDirectory(
                    releaseZip,
                    releaseZipDir,
                    true);
                var unzipDir = Path.Combine(
                    releaseZipDir,
                    "pleasanter");
                foreach (var dir in Directory.GetDirectories(unzipDir))
                {
                    Directory.Move(dir, dir.Replace(unzipDir, installDir));
                }
                Directory.Delete(unzipDir);
            }
            else
            {
                logger.Error("プリザンターのリリースファイルが存在しません。");
                return;
            }
        }

    }
}
