using Cysharp.Diagnostics;
using Implem.DefinitionAccessor;
using Implem.ParameterAccessor.Parts;
using Implem.PleasanterSetup.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Zx;

namespace Implem.PleasanterSetup
{
    public class PleasanterSetup : ConsoleAppBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private string installDir;
        private string licenseDllPath;
        private string dbms;
        private string server;
        private string serviceName;
        private string defaultLanguage;
        private string defaultTimeZone;
        private string userId;
        private string saPassword;
        private string ownerPassword;
        private string userPassword;
        private bool versionUp;
        private bool enterpriseEdition;
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
            this.licenseDllPath = string.Empty;
            this.dbms = string.Empty;
            this.server = string.Empty;
            this.serviceName = string.Empty;
            this.defaultLanguage = string.Empty;
            this.defaultTimeZone = string.Empty;
            this.userId = string.Empty;
            this.saPassword = string.Empty;
            this.ownerPassword = string.Empty;
            this.userPassword = string.Empty;
            this.versionUp = false;
            this.enterpriseEdition = false;
            this.extendedIssuesColumns = new ExtendedColumns();
            this.extendedResultsColumns = new ExtendedColumns();
        }

        [RootCommand]
        public async Task Setup(
            [Option("r")] string releasezip = "",
            [Option("d")] string directory = "",
            [Option("patch")] string patchPath = "",
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

                if (string.IsNullOrEmpty(releasezip))
                {
                    releasezip = await DownloadNewResource(
                        installDir,
                        "Pleasanter");
                }

                SetNewResource(
                    installDir: installDir,
                    releaseZip: releasezip);

                //バージョンアップ時マージ
                if (versionUp)
                {
                    if (string.IsNullOrEmpty(patchPath))
                    {
                        patchPath = await DownloadNewResource(
                        installDir,
                        "ParametersPatch");
                    }

                    //ParametersPatchの配置処理
                    SetParametersPatch(
                        installDir,
                        patchPath);

                    //backupの絶対パスと新資源の絶対パスが必要
                    await Merge(
                        releaseZip: installDir,
                        previous: backupDir,
                        patchPath: patchPath,
                        setUpState: setUpState);
                }
                // ユーザがコンソール上で入力した値を各パラメータファイルへ書き込む
                SetParameters();
                //ユーザがライセンスファイルを指定した場合
                if (File.Exists(license))
                {
                    CopyLicense(
                        resourceDir: installDir,
                        licenseDir: licenseDllPath);
                }
                if (!File.Exists(license) && versionUp)
                {
                    ExistingCopyLicense(
                        installDir: installDir,
                        backupDir: backupDir);
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
                //ログメッセージを英語に変更
                logger.LogInformation("セットアップコマンドの処理を終了します。");
                return;
            }
        }

        //一旦保留
        [Command("merge")]
        public async Task Merge(
            [Option("p")] string previous,
            [Option("r")] string releaseZip = "",
            [Option("patch")] string patchPath = "",
            bool setUpState = false)
        {
            if (!setUpState)
            {
                if (!string.IsNullOrEmpty(previous))
                {
                    if (!Directory.Exists(previous))
                    {
                        logger.LogError($"\"{previous}\" does not exist.");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(releaseZip))
                {
                    if (!File.Exists(releaseZip))
                    {
                        logger.LogError($"\"{releaseZip}\" does not exist.");
                        return;
                    }
                }
                else
                {
                    releaseZip = await DownloadNewResource(
                        previous,
                        "Pleasanter");
                }

                if (!string.IsNullOrEmpty(patchPath))
                {
                    if (!File.Exists(patchPath))
                    {
                        logger.LogError($"\"{patchPath}\" does not exist.");
                        return;
                    }
                }
                else
                {
                    patchPath = await DownloadNewResource(
                        previous,
                        "ParametersPatch");
                }

                //previousを退避
                var backupDir = Path.Combine(
                    Path.GetDirectoryName(previous),
                    $"{Path.GetFileName(previous)}{DateTime.Now:_yyyyMMdd_HHmmss}");
                // バージョンアップの場合は既存資源のバックアップを行う
                var destinationPath = previous;
                CopyResourceDirectory(
                    installDir: previous,
                    destDir: backupDir);

                SetNewResource(
                    installDir: previous,
                    releaseZip: releaseZip);
                SetParametersPatch(
                    previous,
                    patchPath);
                //ライセンスの移動
                ExistingCopyLicense(
                    installDir: previous,
                    backupDir: backupDir);

                await ExecuteMerge(destinationPath, backupDir);
            }
            else
            {
                await ExecuteMerge(releaseZip, previous);
            }
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
                //setUpStateがtrueじゃないなら
                SetParameters();
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
                else
                {
                    userId = userInputUserId;
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
                        saPassword = builder["PWD"].ToString();
                    }
                    if (data.OwnerConnectionString != null)
                    {
                        DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                        builder.ConnectionString = data?.OwnerConnectionString;
                        ownerPassword = builder["PWD"].ToString();
                    }
                    if (data.UserConnectionString != null)
                    {
                        DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                        builder.ConnectionString = data?.UserConnectionString;
                        userPassword = builder["PWD"].ToString();
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
                logger.LogInformation("Sa Password : ");
                while (string.IsNullOrEmpty(saPassword))
                {
                    saPassword = Console.ReadLine() ?? "";
                }
                logger.LogInformation("Owner Password : ");
                while (string.IsNullOrEmpty(ownerPassword))
                {
                    ownerPassword = Console.ReadLine() ?? "";
                }
                logger.LogInformation("User Password : ");
                while (string.IsNullOrEmpty(userPassword))
                {
                    userPassword = Console.ReadLine() ?? "";
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
                logger.LogInformation($"Please enter the number for extended \"{referenceType}\" columns to add.");
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
                    extendedIssuesColumns.TableName = referenceType;
                    extendedIssuesColumns.ReferenceType = referenceType;
                    break;
                case "Results":
                    extendedResultsColumns = extendedColumns;
                    extendedResultsColumns.TableName = referenceType;
                    extendedResultsColumns.ReferenceType = referenceType;
                    break;
            }
        }

        private int AskForItemCount(string itemName)
        {
            int count;
            do
            {
                logger.LogInformation($"{itemName} [Default value: 0] : ");
                var userInput = Console.ReadLine();
                if (int.TryParse(userInput, out count) && int.Parse(userInput) > 0)
                {
                    count = count + 26;
                    Console.WriteLine(count);
                    break;
                }else if (userInput.Length == 1 && char.IsLetter(userInput[0]))
                {
                    // アルファベットが入力された場合
                    char letter = char.ToUpper(userInput[0]);
                    int position = letter - 'A' + 1;
                    count = position;
                    Console.WriteLine(count);
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
            var license = licenseDir;
            if (File.Exists(license))
            {
                if (Directory.Exists(resourceDir))
                {
                    if (Directory.Exists(Path.Combine(
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
                    if (Directory.Exists(Path.Combine(
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

            var arguments = $"Implem.CodeDefiner.dll _rds {forceOption} {noInputOption} {language} {timeZone}";
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments
            };

            var (process, stdOut, stdError) = ProcessX.GetDualAsyncEnumerable(processStartInfo);

            // 標準出力と標準エラー出力を処理するタスク
            var outputTask = Task.Run(async () =>
            {
                await foreach (var item in stdOut)
                {
                    Console.WriteLine(item);
                    if (item is "Type \"y\" (yes) if the license is correct, otherwise type \"n\" (no).")
                    {
                        var input = Console.ReadLine();
                        process.StandardInput.WriteLine(input);
                    }
                }
            });

            var errorTask = Task.Run(async () =>
            {
                await foreach (var item in stdError)
                {
                    Console.WriteLine(item);
                }
            });

            await Task.WhenAll(outputTask, errorTask);
            logger.LogInformation("Setup is complete.");
        }

        private async Task ExecuteMerge(
            string newResource,
            string preResource)
        {
            var codeDefinerDir = Path.Combine(
                newResource,
                "Implem.CodeDefiner");
            await $"cd {codeDefinerDir}";
            await $"dotnet Implem.CodeDefiner.dll merge /b {preResource} /i {newResource} ";
            logger.LogInformation("The merge process has finished.");
        }

        private string GetDefaultInstallDir()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                ? installDir = configuration["DefaultParameters:InstallDirForWindows"]
                : installDir = configuration["InstallDirForLinux"];
        }

        private bool MeetsVersionUpRequirements()
        {
            //dllがある場合はバージョンアップ
            var pleasanterDll = Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "Implem.Pleasanter.dll");
            return File.Exists(pleasanterDll);
        }

        private void SetParameters()
        {
            var parametersDir = Path.Combine(
                installDir,
                "Implem.Pleasanter",
                "App_Data",
                "Parameters");

            //ここの分岐は検討
            if (!versionUp)
            {
                //Rds.json,Service.json内容をユーザの入力値をもとに書き込む
                SetRdsParameters(parametersDir);
                SetServiceParameters(parametersDir);
                SetExtendedColumns(parametersDir);
            }
            else if (enterpriseEdition)
            {
                SetExtendedColumns(parametersDir);
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
            data.SaConnectionString = $"Server={server};Database={database};UID={userId};PWD={saPassword};";
            data.OwnerConnectionString = $"Server={server};Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={ownerPassword};";
            data.UserConnectionString = $"Server={server};Database=#ServiceName#;UID=#ServiceName#_User;PWD={userPassword};";
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
                string json = JsonConvert.SerializeObject(extendedIssuesColumns, Formatting.Indented);
                File.WriteAllText(
                    issuesFile,
                    json);
            }
            if (extendedResultsColumns != null)
            {
                SetDisabledColumns(extendedResultsColumns);
                string json = JsonConvert.SerializeObject(extendedResultsColumns, Formatting.Indented);
                File.WriteAllText(
                    resultsFile,
                    json);
            }
        }

        private void SetDisabledColumns(ExtendedColumns extendedColumns)
        {
            var disabledColumns = new List<string>();

            foreach (var p in extendedColumns.GetType().GetFields())
            {
                var disabledColumn = new List<string>();
                switch (p.Name)
                {
                    case "Class":
                        disabledColumn = SetDisabledColumnsList("Class", extendedColumns.Class);
                        extendedColumns.Class = CalcColumnsCount(extendedColumns.Class);
                        break;
                    case "Num":
                        disabledColumn = SetDisabledColumnsList("Num", extendedColumns.Num);
                        extendedColumns.Num = CalcColumnsCount(extendedColumns.Num);
                        break;
                    case "Date":
                        disabledColumn = SetDisabledColumnsList("Date", extendedColumns.Date);
                        extendedColumns.Date = CalcColumnsCount(extendedColumns.Date);
                        break;
                    case "Description":
                        disabledColumn = SetDisabledColumnsList("Description", extendedColumns.Description);
                        extendedColumns.Description = CalcColumnsCount(extendedColumns.Description);
                        break;
                    case "Check":
                        disabledColumn = SetDisabledColumnsList("Check", extendedColumns.Check);
                        extendedColumns.Check = CalcColumnsCount(extendedColumns.Check);
                        break;
                    case "Attachments":
                        disabledColumn = SetDisabledColumnsList("Attachments", extendedColumns.Attachments);
                        extendedColumns.Attachments = CalcColumnsCount(extendedColumns.Attachments);
                        break;
                    default:
                        break;
                }
                foreach (var item in disabledColumn)
                {
                    disabledColumns.Add(item);
                }
            }
            extendedColumns.DisabledColumns = disabledColumns;
        }

        private int CalcColumnsCount(int count)
        {
            return count > 26
                ? count - 26
                : 0;
        }

        private List<string> SetDisabledColumnsList(string columnType, int count)
        {
            var disabeledColumnsList = new List<string>();
            var deleteCount = 26 - count;
            if (deleteCount > 0)
            {
                for (var i = 1; i <= deleteCount; i++)
                {
                    var alphabet = ((char)('A' + 26 - i)).ToString();
                    disabeledColumnsList.Add($"{columnType}{alphabet}");
                }
            }

            disabeledColumnsList.Sort();
            return disabeledColumnsList;
        }

        private void SetSummary(
            string directory,
            string licenseZip,
            string extendedColumnsDir,
            string releasezip = "",
            string patchPath = "")
        {
            AskForInstallDir(directory);
            //Implem.Pleasanter.dllの有無でバージョンアップか判断
            versionUp = MeetsVersionUpRequirements();
            //オフライン環境での必須オプションの確認
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                if (string.IsNullOrEmpty(releasezip))
                {
                    logger.LogInformation("-r is required");
                    return;
                }
            }
            if (versionUp && !NetworkInterface.GetIsNetworkAvailable())
            {
                if (string.IsNullOrEmpty(patchPath))
                {
                    logger.LogInformation("--patch is required");
                    return;
                }
            }
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
                //ライセンスがEnterpriseEditionか判定処理
                enterpriseEdition = CheckLicense(licenseZip);
                if (enterpriseEdition)
                {
                    AskForExtendedColums(
                        extendedColumnsDir,
                        "Issues");
                    AskForExtendedColums(
                        extendedColumnsDir,
                        "Results");
                }
            }
        }

        private bool CheckLicense(string licenseZip)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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
                baseFileName,
                "forNetCore(MultiPlatform)",
                "Implem.License.dll");
            licenseDllPath = license;
            return CheckMethodExecute(license);
        }

        private bool CheckMethodExecute(string license)
        {
            if (!File.Exists(license))
            {
                logger.LogError("Implem.License.dll does not exist.");

            }
            Assembly assembly = Assembly.LoadFile(license);
            Type type = assembly.GetType("Implem.License.License");
            object instance = Activator.CreateInstance(type);
            MethodInfo checkMethod = type.GetMethod("Check");
            return Convert.ToBoolean(checkMethod?.Invoke(instance, null));
        }

        private void SetNewResource(
            string installDir,
            string releaseZip)
        {
            logger.LogError($"Start placing release resources to {installDir}.");
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
                //同じフォルダで実行した場合にフォルダの移動時にエラーが起きるので条件追加
                if (unzipDir != installDir)
                {
                    foreach (var dir in Directory.GetDirectories(unzipDir))
                    {
                        Directory.Move(dir, dir.Replace(unzipDir, installDir));
                    }
                    Directory.Delete(unzipDir);
                }
                logger.LogError($"The release resource was placed in {installDir}.");
            }
            else
            {
                logger.LogError("The release zip file for Pleasanter does not exist.");
                return;
            }
        }
        private void SetParametersPatch(string previous, string patchPath)
        {
            var destPath = Path.Combine(
                previous,
                "ParametersPatch.zip");
            File.Copy(
                patchPath,
                destPath,
                true);
        }

        //作成途中
        private async Task<string?> DownloadNewResource(string installDir, string fileName)
        {
            //先頭がPleasanterのzipをダウンロードしてinstallDirに配置
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.github.com/repos/Implem/Implem.Pleasanter/releases/latest";
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject release = JObject.Parse(responseBody);

                string assetUrl = string.Empty;
                var destinationDir = string.Empty;
                foreach (var asset in release["assets"])
                {
                    if (asset["name"].ToString().Contains(fileName))
                    {
                        assetUrl = asset["browser_download_url"].ToString();
                        HttpResponseMessage assetResponse = await client.GetAsync(assetUrl);
                        assetResponse.EnsureSuccessStatusCode();
                        destinationDir = Path.Combine(Directory.GetParent(installDir).FullName, asset["name"].ToString());
                        byte[] fileBytes = await assetResponse.Content.ReadAsByteArrayAsync();
                        await File.WriteAllBytesAsync(destinationDir, fileBytes);
                        Console.WriteLine($"Downloaded {asset["name"]} to {destinationDir}");
                        break;
                    }
                }
                return destinationDir;
            }
        }
    }
}
