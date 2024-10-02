using Implem.ParameterAccessor.Parts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;
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
        private string defaultLanguage;
        private string defaultTimeZone;
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
            ILogger<PleasanterSetup> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.installDir = string.Empty;
            this.dbms = string.Empty;
            this.server = string.Empty;
            this.serviceName = string.Empty;
            this.defaultLanguage = string.Empty;
            this.defaultTimeZone = string.Empty;
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
            bool setUpState = true,
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
                //新規インストールの時エラーになるかも後で確認
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
                // 新しい資源を配置する Gitから落としてくるように変更する必要あり
                SetNewResource(
                    installDir: installDir,
                    releaseZip: releasezip);
                // ユーザがコンソール上で入力した値を各パラメータファイルへ書き込む
                SetParameters(backupDir);
                //ユーザがライセンスファイルを指定した場合
                if (File.Exists(license))
                {
                    CopyLicense(
                        resourceDir: installDir,
                        licenseDir: license);
                }
                if (!File.Exists(license) && versionUp)
                {
                    ExistingCopyLicense(
                        installDir: installDir,
                        backupDir: backupDir);
                }
                // バージョンアップの場合はパラメータファイルのマージを行う
                if (versionUp)
                {
                    var currentVersion = FileVersionInfo.GetVersionInfo(
                        Path.Combine(
                            backupDir,
                            "Implem.Pleasanter",
                            "Implem.Pleasanter.dll")).FileVersion;
                    var newVersion = FileVersionInfo.GetVersionInfo(
                        Path.Combine(
                            installDir,
                            "Implem.Pleasanter",
                            "Implem.Pleasanter.dll")).FileVersion;
                    //1.4以前の場合にエラー処理

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
                    setUpState: setUpState,
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
        private void Merge(
            [Option("s")] string source,
            [Option("d")] string destination)
            //string currentVersion = "",
            //string newVersion = "",
            //string[] excludes = null)
        {
            var currentVersion = FileVersionInfo.GetVersionInfo(
                Path.Combine(
                        source,
                        "Implem.Pleasanter",
                        "Implem.Pleasanter.dll")).FileVersion;
            var newVersion = FileVersionInfo.GetVersionInfo(
                Path.Combine(
                    destination,
                    "Implem.Pleasanter",
                    "Implem.Pleasanter.dll")).FileVersion;
            //1.4以前の場合にエラー処理
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
                installDir:installDir,
                destPath: destination,
                currentVersion: currentVersion,
                newVersion: newVersion);
        }

        [Command("rds")]
        public async Task Rds(
            [Option("d")] string directory = "",
            bool setUpState = false,
            bool force = false,
            bool noinput = false,
            string license = "",
            string extendedcolumns = "")
        {
            if (!setUpState)
            {
                // ユーザにセットアップに必要な情報を入力してもらう
                SetSummary(
                    directory: directory,
                    licenseZip: license,
                    extendedColumnsDir: extendedcolumns);
            }
            var resourceDir = string.IsNullOrEmpty(directory)
                ? GetDefaultInstallDir()
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

        [Command("patch")]
        public void Patch(
            [Option("p")] string previous,
            [Option("r")] string release)
        {
            var patchFolderPath = Path.Combine(
                GetSourcePath(),
                "Implem.PleasanterSetup",
                "Patch");
            var preVerionPath = Path.Combine(
                previous,
                "pleasanter",
                "Implem.Pleasanter",
                "App_Data",
                "Parameters");
            //パッチファイル格納用フォルダの作成処理
            var newVersionName = Path.GetFileName(release).Replace("Pleasanter_", "");
            var modifiedVersion = ReplaceVersion(newVersionName);
            Directory.CreateDirectory(
                Path.Combine(
                    patchFolderPath,
                    modifiedVersion));
            var newVersionPath = Path.Combine(
                release,
                "pleasanter",
                "Implem.Pleasanter",
                "App_Data",
                "Parameters");
            var createPath = Path.Combine(
                patchFolderPath,
                modifiedVersion);
            CreatePatch(preVerionPath, newVersionPath, createPath);
        }

        public static string ReplaceVersion(string versionInfo)
        {
            var pattern = @"(\d+)\.(\d+)\.(\d+)\.(\d+)";
            return Regex.Replace(versionInfo, pattern, "0$1.0$2.0$3.0$4");
        }

        private void CreatePatch(string preVersionPath, string newVersionPath, string createPath, bool createFlag = false)
        {
            foreach (var file in Directory.GetFiles(preVersionPath, "*.json", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);
                var fileContent = File.ReadAllText(file);
                var sourceFile = file.Replace(preVersionPath, newVersionPath);
                var outputFile = Path.GetFullPath(file);
                if (File.Exists(sourceFile))
                {
                    var releaseFileContent = File.ReadAllText(sourceFile);
                    if (fileContent != releaseFileContent)
                    {
                        Console.WriteLine(fileName);
                        var patchJson = PatchJson.CreatePatch(releaseFileContent, fileContent, createPath);
                        File.WriteAllText(Path.Combine(
                                createPath,
                                fileName),
                                patchJson);
                        createFlag = true;
                    }
                }
            }
        }

        //最終的にはComitterに移動
        public static string GetSourcePath()
        {
            var parts = new DirectoryInfo(
                Assembly.GetEntryAssembly().Location).FullName.Split(Path.DirectorySeparatorChar);
            var truncateParts = parts.TakeWhile(part => !part.Contains("Implem.PleasanterSetup"));
            string sourcePath = Path.Combine(truncateParts.ToArray());
            return sourcePath;
        }

        private bool AskForInstallOrVersionUp()
        {
            DisplaySummary();
            logger.LogInformation("Shall I install Pleasanter with this content? Please enter ‘y(yes)' or 'n(no)’. : ");
            var doNext = Console.ReadLine();
            while (string.IsNullOrEmpty(doNext))
            {
                doNext = Console.ReadLine();
            }
            return doNext.ToLower().StartsWith("y");
        }
        
        private void AskForInstallDir(string directory)
        {
            if (!string.IsNullOrEmpty(directory))
            {
                installDir = directory;
            }
            else
            {
                logger.LogInformation("Install Directory [Default: /web/pleasanter] : ");
                var userInputResourceDir = Console.ReadLine();
                installDir = !string.IsNullOrEmpty(userInputResourceDir)
                    ? userInputResourceDir
                    : GetDefaultInstallDir();
            }
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
                    switch (data.Dbms)
                    {
                        case "SQLServer":
                            dbms = "1";
                            break;
                        case "PostgreSQL":
                            dbms = "2";
                            break;
                    }
                }
                else
                {
                    logger.LogError($"The file {file} was not found.");
                    Environment.Exit(0);
                }
            }
            else
            {
                logger.LogInformation("DBMS [1: SQL Server, 2: PostgreSQL] : ");
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
                    if (data?.SaConnectionString != null)
                    {
                        DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                        builder.ConnectionString = data?.SaConnectionString;
                        server = builder["Server"].ToString();
                    }
                }
                else
                {
                    logger.LogError($"The file {file} was not found.");
                    Environment.Exit(0);
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
                    logger.LogError($"The file {file} was not found.");
                    Environment.Exit(0);
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
        }

        private void AskForDefaultLanguage()
        {
            do
            {
                logger.LogInformation("DefaultLanguage [1: English(Default), 2: Chinese, 3: Japanese, 4: German, 5: Korean, 6: Spanish, 7: Vietnamese] : ");
                var choiceLanguage = Console.ReadLine() ?? string.Empty;
                switch (choiceLanguage)
                {
                    case "1":
                    case "":
                        defaultLanguage = "en";
                        break;
                    case "2":
                        defaultLanguage = "zh";
                        break;
                    case "3":
                        defaultLanguage = "ja";
                        break;
                    case "4":
                        defaultLanguage = "de";
                        break;
                    case "5":
                        defaultLanguage = "ko";
                        break;
                    case "6":
                        defaultLanguage = "es";
                        break;
                    case "7":
                        defaultLanguage = "vn";
                        break;
                    default:
                        logger.LogInformation("Please enter the number of choices");
                        continue;
                }
                break;
            } while (true);
        }

        private void AskForDefaultTimeZone()
        {
            logger.LogInformation("Set the default time zone.");
            do
            {
                logger.LogInformation("Please enter a valid time zone for your OS:");
                defaultTimeZone = Console.ReadLine() ?? string.Empty;
            } while (!TimeZoneInfo.GetSystemTimeZones().Any(o => o.Id == defaultTimeZone));
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
                    if (data.SaConnectionString != null)
                    {
                        DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                        builder.ConnectionString = data?.SaConnectionString;
                        userId = builder["UID"].ToString();
                    }
                }
                else
                {
                    logger.LogError($"The file {file} was not found.");
                    Environment.Exit(0);
                }
            }
            else
            {
                logger.LogInformation("[Default: sa(SQL Server), postgres(PostgreSQL)] : ");
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
                    "Rds.json");
                if (File.Exists(file))
                {
                    var json = File.ReadAllText(file);
                    var data = json.Deserialize<Rds>();
                    if (data.SaConnectionString != null)
                    {
                        DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                        builder.ConnectionString = data?.SaConnectionString;
                        password = builder["PWD"].ToString();
                    }
                }
                else
                {
                    logger.LogError($"The file {file} was not found.");
                    Environment.Exit(0);
                }
            }
            else
            {
                logger.LogInformation("Password : ");
                while (string.IsNullOrEmpty(password))
                {
                    password = Console.ReadLine() ?? "";
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
                var json = File.ReadAllText(file);
                extendedColumns = json.Deserialize<ExtendedColumns>();
            }
            else
            {
                logger.LogInformation($"Please enter the number for extended \"{referenceType}\" columns.");
                extendedColumns.Class = AskForItemCount("Class");
                extendedColumns.Num = AskForItemCount("Num");
                extendedColumns.Date = AskForItemCount("Date");
                extendedColumns.Description = AskForItemCount("Description");
                extendedColumns.Check = AskForItemCount("Check");
                extendedColumns.Attachments = AskForItemCount("Attachments");
            }
            switch (referenceType)
            {
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
                logger.LogInformation($"{itemName} [Default 26] : ");
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
                file.CopyTo(targetFilePath, true);
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

        static void ExistingCopyLicense(string installDir, string backupDir)
        {
            var license = Path.Combine(
                backupDir,
                "Implem.Pleasanter",
                "Implem.License.dll");
            File.Copy(
                license,
                Path.Combine(
                    installDir,
                    "Implem.CodeDefiner",
                    "Implem.License.dll"),
                true);
            File.Copy(
                license,
                Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "Implem.License.dll"),
                true);
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
            logger.LogInformation("------ Summary ------");
            logger.LogInformation($"Install Directory : {installDir}");
            logger.LogInformation($"DBMS              : {Enum.GetName(typeof(DBMS), int.Parse(dbms))}");
            logger.LogInformation($"Server            : {server}");
            logger.LogInformation($"Service Name      : {serviceName}");
            if (!versionUp)
            {
                logger.LogInformation($"DefaultLanguage   : {defaultLanguage}");
                logger.LogInformation($"DefaultTimeZone   : {defaultTimeZone}");
            }
            if (!extendedIssuesColumns.Equals(new ExtendedColumns())
                || !extendedResultsColumns.Equals(new ExtendedColumns()))
            {
                logger.LogInformation("[Issues]");
                logger.LogInformation($"    Class       : {CalculateRangeOfColumns("Class", extendedIssuesColumns.Class)}");
                logger.LogInformation($"    Num         : {CalculateRangeOfColumns("Num", extendedIssuesColumns.Num)}");
                logger.LogInformation($"    Date        : {CalculateRangeOfColumns("Date", extendedIssuesColumns.Date)}");
                logger.LogInformation($"    Description : {CalculateRangeOfColumns("Description", extendedIssuesColumns.Description)}");
                logger.LogInformation($"    Check       : {CalculateRangeOfColumns("Check", extendedIssuesColumns.Check)}");
                logger.LogInformation($"    Attachments : {CalculateRangeOfColumns("Attachments", extendedIssuesColumns.Attachments)}");
                logger.LogInformation("[Results]");
                logger.LogInformation($"    Class       : {CalculateRangeOfColumns("Class", extendedResultsColumns.Class)}");
                logger.LogInformation($"    Num         : {CalculateRangeOfColumns("Num", extendedResultsColumns.Num)}");
                logger.LogInformation($"    Date        : {CalculateRangeOfColumns("Date", extendedResultsColumns.Date)}");
                logger.LogInformation($"    Description : {CalculateRangeOfColumns("Description", extendedResultsColumns.Description)}");
                logger.LogInformation($"    Check       : {CalculateRangeOfColumns("Check", extendedResultsColumns.Check)}");
                logger.LogInformation($"    Attachments : {CalculateRangeOfColumns("Attachments", extendedResultsColumns.Attachments)}");
            }
            logger.LogInformation("---------------------");
        }

        private async Task ExecuteCodeDefiner(
            string codeDefinerDir,
            bool force,
            bool noinput)
        {
            var forceOption = force ? "/f" : "";
            var noInputOption = noinput ? "/n" : "";
            var language = "";
            var timeZone = "";
            //初回インストール時にのみ既定の言語、タイムゾーンを引数に追加する。
            if (!versionUp)
            {
                language = "/l " + defaultLanguage;
                timeZone = "/z " + "\"" + defaultTimeZone + "\"";
            }
            await $"cd {codeDefinerDir}";
            await $"dotnet Implem.CodeDefiner.dll _rds {forceOption} {noInputOption} {language} {timeZone}";
            await $"cd -";
        }

        private string GetDefaultInstallDir()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                ? installDir = configuration["DefaultParameters:InstallDirForWindows"]
                : installDir = configuration["InstallDirForLinux"];
        }

        private void MeetsVersionUpRequirements(string parametersDir)
        {
            versionUp = Directory.Exists(parametersDir) && Directory.GetFiles(parametersDir).Length > 0;
        }

        private void SetParameters(string backupDir)
        {
            var parametersDir = Path.Combine(
                installDir,
                "Implem.Pleasanter",
                "App_Data",
                "Parameters");
            if (!versionUp)
            {
                //Rds.json,Service.json内容をユーザの入力値をもとに書き込む
                SetRdsParameters(parametersDir);
                SetServiceParameters(parametersDir);
                SetExtendedColumns(parametersDir);
            }
            else
            {
                //バージョンアップ時は旧資源のパラメータファイルを新資源にコピーする
                var backUpParameterDir = Path.Combine(
                backupDir,
                "Implem.Pleasanter",
                "App_Data",
                "Parameters");
                CopyDirectory(backUpParameterDir, parametersDir, true);
            }
        }

        private void SetRdsParameters(string parametersDir)
        {
            var file = Path.Combine(parametersDir, "Rds.json");
            var json = File.ReadAllText(file);
            var data = json.Deserialize<Rds>();
            data.Dbms = Enum.GetName(typeof(DBMS), int.Parse(dbms));
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
            var parametersPath =Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "App_Data",
                    "Parameters");
            MeetsVersionUpRequirements(parametersPath);
            AskForDbms(versionUp);
            AskForServer(versionUp);
            AskForServiceName(versionUp);
            if (!versionUp)
            {
                AskForDefaultLanguage();
                AskForDefaultTimeZone();
            }
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
                logger.LogError("プリザンターのリリースファイルが存在しません。");
                return;
            }
        }

    }
}
