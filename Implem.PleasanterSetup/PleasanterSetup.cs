using Cysharp.Diagnostics;
using Implem.DefinitionAccessor;
using Implem.ParameterAccessor.Parts;
using Implem.PleasanterSetup.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Zx;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;

namespace Implem.PleasanterSetup
{
    public class PleasanterSetup : ConsoleAppBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private static HttpClient client = new HttpClient();
        private static readonly string url = "https://api.github.com/repos/Implem/Implem.Pleasanter/releases/latest";
        private string installDir;
        private string unzipDirPath;
        private string CodeDefinerDirPath;
        private string temporaryPath;
        private string licenseDllPath;
        private string provider;
        private string userName;
        private string dbms;
        private string server;
        private string port;
        private string serviceName;
        private string defaultLanguage;
        private string defaultTimeZone;
        private string userId;
        private string connectionString;
        private string SaConnectionString;
        private string OwnerConnectionString;
        private string UserConnectionString;
        private string saPassword;
        private string ownerPassword;
        private string userPassword;
        private string mySqlConnectingHost;
        private bool isEnvironmentUser;
        private bool versionUp;
        private bool EnterpriseEdition;
        private bool isProviderAzure;
        private bool addMySqlConnectingHost;
        private ExtendedColumns extendedIssuesColumns;
        private ExtendedColumns extendedResultsColumns;
        private enum DBMS
        {
            SQLServer = 1,
            PostgreSQL = 2,
            MySQL = 3
        };

        public PleasanterSetup(
            IConfiguration configuration,
            ILogger<PleasanterSetup> logger)
        {
            this.provider = string.Empty;
            this.userName = string.Empty;
            this.configuration = configuration;
            this.logger = logger;
            this.installDir = string.Empty;
            this.unzipDirPath = string.Empty;
            this.CodeDefinerDirPath = string.Empty;
            this.licenseDllPath = string.Empty;
            this.temporaryPath = string.Empty;
            this.dbms = string.Empty;
            this.port = string.Empty;
            this.server = string.Empty;
            this.serviceName = string.Empty;
            this.defaultLanguage = string.Empty;
            this.defaultTimeZone = string.Empty;
            this.userId = string.Empty;
            this.connectionString = string.Empty;
            this.SaConnectionString = string.Empty;
            this.OwnerConnectionString = string.Empty;
            this.UserConnectionString = string.Empty;
            this.saPassword = string.Empty;
            this.ownerPassword = string.Empty;
            this.userPassword = string.Empty;
            this.isEnvironmentUser = false;
            this.versionUp = false;
            this.EnterpriseEdition = false;
            this.isProviderAzure = false;
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
            bool noinput = false)
        {
            try
            {
                await SetupMain(
                    releasezip,
                    directory,
                    patchPath,
                    setUpState,
                    force,
                    noinput);
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                Environment.Exit(1);
            }
        }

        private async Task SetupMain(
            string releasezip,
            string directory,
            string patchPath,
            bool setUpState,
            bool force,
            bool noinput)
        {
            PrepareEnvironment(
                releasezip,
                patchPath,
                setUpState);
            SetSummaryAndAsk(
                directory,
                releasezip,
                patchPath);
            var doNext = AskForInstallOrVersionUp();
            if (doNext)
            {
                await PrepareInstallDirectory();
                var (backupDir, guidDir) = await PrepareBackupAndGuidDir();
                await HandleVersionUpIfNeeded(
                    releasezip,
                    patchPath,
                    backupDir,
                    guidDir);
                await PlaceAndSetupResources(
                    releasezip,
                    patchPath,
                    backupDir,
                    guidDir,
                    setUpState);
                SetParameters();
                await SetupDatabase(
                    setUpState: setUpState,
                    force: force,
                    noinput: noinput);
            }
            else
            {
                logger.LogInformation("Finishes processing the setup command.");
                return;
            }
        }

        private void PrepareEnvironment(
            string releasezip,
            string patchPath,
            bool setUpState)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (!NetworkInterface.GetIsNetworkAvailable() && setUpState)
            {
                if (string.IsNullOrEmpty(releasezip))
                {
                    logger.LogError("\"-r\" is required");
                    Environment.Exit(0);
                }
            }
            if (versionUp && !NetworkInterface.GetIsNetworkAvailable() && setUpState)
            {
                if (string.IsNullOrEmpty(patchPath))
                {
                    logger.LogError("\"-patch\" is required");
                    Environment.Exit(0);
                }
            }
            if (!string.IsNullOrEmpty(releasezip))
            {
                releasezip = Path.GetFullPath(releasezip);
                if (!File.Exists(releasezip))
                {
                    logger.LogError($"{releasezip} does not exist.");
                    Environment.Exit(0);
                }
            }
            if (!string.IsNullOrEmpty(patchPath))
            {
                patchPath = Path.GetFullPath(patchPath);
                if (!File.Exists(patchPath))
                {
                    logger.LogError($"{patchPath} does not exist.");
                    Environment.Exit(0);
                }
            }
        }

        private void SetSummaryAndAsk(
            string directory,
            string releasezip,
            string patchPath)
        {
            SetSummary(
                directory: directory,
                releasezip: releasezip,
                patchPath: patchPath);
        }

        private async Task PrepareInstallDirectory()
        {
            if (!isProviderAzure && !Directory.Exists(installDir))
            {
                await CreateDirectoryCrossPlatform(installDir);
            }
        }

        private async Task<(string backupDir, string guidDir)> PrepareBackupAndGuidDir()
        {
            var backupDir = Path.Combine(
                Path.GetDirectoryName(installDir),
                $"{Path.GetFileName(installDir)}{DateTime.Now:_yyyyMMdd_HHmmss}");
            var guid = Guid.NewGuid();
            var guidDir = Path.Combine(
                Path.GetDirectoryName(installDir),
                guid.ToString());
            await CreateDirectoryCrossPlatform(guidDir);
            return (backupDir, guidDir);
        }

        private async Task HandleVersionUpIfNeeded(string releasezip, string patchPath, string backupDir, string guidDir)
        {
            if (versionUp)
            {
                CopyResourceDirectory(
                    installDir: installDir,
                    destDir: backupDir,
                    guidDir: guidDir);
            }
        }

        private async Task PlaceAndSetupResources(
            string releasezip,
            string patchPath,
            string backupDir,
            string guidDir,
            bool setUpState)
        {
            if (string.IsNullOrEmpty(releasezip))
            {
                releasezip = await DownloadNewResource(
                    guidDir,
                    "Pleasanter");
            }
            SetNewResource(
                installDir: installDir,
                releaseZip: releasezip,
                guidDir: guidDir);
            if (versionUp)
            {
                if (string.IsNullOrEmpty(patchPath))
                {
                    patchPath = await DownloadNewResource(
                        isProviderAzure
                            ? unzipDirPath
                            : installDir,
                        "ParametersPatch");
                }
                SetParametersPatch(
                    isProviderAzure
                        ? unzipDirPath
                        : installDir,
                    patchPath);
                if (isProviderAzure)
                {
                    await Merge(
                        releaseZip: unzipDirPath,
                        previous: temporaryPath,
                        patchPath: patchPath,
                        setUpState: setUpState);
                }
                else
                {
                    await Merge(
                        releaseZip: installDir,
                        previous: backupDir,
                        patchPath: patchPath,
                        setUpState: setUpState);
                }
            }
            if (isProviderAzure)
            {
                MoveResource(
                    installDir,
                    unzipDirPath,
                    guidDir);
            }
            if (EnterpriseEdition && versionUp)
            {
                ExistingCopyLicense(
                    installDir: installDir,
                    backupDir: backupDir);
            }
        }

        private async Task SetupDatabase(
            bool setUpState,
            bool force,
            bool noinput)
        {
            await Rds(
                directory: installDir,
                setUpState: setUpState,
                force: force,
                noinput: noinput);
        }

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
                var guid = Guid.NewGuid();
                var guidDir = Path.Combine(
                    Path.GetDirectoryName(installDir),
                    guid.ToString());
                // バージョンアップの場合は既存資源のバックアップを行う
                var destinationPath = previous;
                CopyResourceDirectory(
                    installDir: previous,
                    destDir: backupDir);
                SetNewResource(
                    installDir: previous,
                    releaseZip: releaseZip,
                    guidDir: guidDir);
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
            bool noinput = false)
        {
            var resourceDir = string.IsNullOrEmpty(directory)
                ? GetDefaultInstallDir()
                : directory;
            if (!setUpState)
            {
                // ユーザにセットアップに必要な情報を入力してもらう
                SetSummary(
                    directory: directory,
                    setUpState: setUpState);
            }
            var codeDefinerPath = isProviderAzure
                ? Path.Combine(
                    Path.GetDirectoryName(resourceDir),
                    Path.GetFileName(CodeDefinerDirPath))
                : Path.Combine(
                    resourceDir,
                    "Implem.CodeDefiner");
            await ExecuteCodeDefiner(
                codeDefinerDir: codeDefinerPath,
                force: force,
                noinput: noinput);
        }

        [Command("trial")]
        public async Task Trial(
            [Option("d")] string directory = "",
            bool trial = true,
            bool noinput = false)
        {
            try
            {
                SetSummary(
                    directory: directory,
                    trial: trial);
                var doNext = AskForInstallOrVersionUp(trial);
                if (!doNext)
                {
                    logger.LogInformation("Finishes processing the trial command.");
                    return;
                }
                var codeDefinerPath = isProviderAzure
                    ? CodeDefinerDirPath
                    : Path.Combine(
                        installDir,
                        "Implem.CodeDefiner");
                await ExecuteTrialCodeDefiner(
                    codeDefinerDir: codeDefinerPath,
                    noinput: noinput);
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                Environment.Exit(1);
            }
        }

        protected static void CancelHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Exiting gracefully...");
            args.Cancel = true;
            Environment.Exit(0);
        }

        private bool AskForInstallOrVersionUp(
            bool trial = false)
        {
            DisplaySummary();
            if (trial)
            {
                logger.LogInformation("Do you want to start the trial? Please enter ‘y(yes)' or 'n(no)’. : ");
            }
            else
            {
                logger.LogInformation("Shall I install Pleasanter with this content? Please enter ‘y(yes)' or 'n(no)’. : ");
            }
            var doNext = Console.ReadLine();
            while (string.IsNullOrEmpty(doNext))
            {
                doNext = Console.ReadLine();
            }
            return doNext.ToLower().StartsWith("y");
        }

        private void AskForInstallDir(string directory)
        {
            var defaultPath = GetDefaultInstallDir();
            if (!string.IsNullOrEmpty(directory))
            {
                installDir = Path.GetFullPath(directory);
            }
            else
            {
                logger.LogInformation($"Install Directory [Default: {defaultPath}] : ");
                var userInputResourceDir = Console.ReadLine();
                installDir = !string.IsNullOrEmpty(userInputResourceDir)
                    ? Path.GetFullPath(userInputResourceDir)
                    : defaultPath;
            }
            if (isProviderAzure)
            {
                logger.LogInformation($"CodeDefiner Directory [Default: {CodeDefinerDirPath}] : ");
                var userInputCodeDefinerDir = Console.ReadLine();
                CodeDefinerDirPath = !string.IsNullOrEmpty(userInputCodeDefinerDir)
                    ? Path.GetFullPath(userInputCodeDefinerDir)
                    : CodeDefinerDirPath;
            }
        }

        private void AskForDbms()
        {
            if (isProviderAzure)
            {
                dbms = "1";
            }
            else
            {
                logger.LogInformation("DBMS [1: SQL Server, 2: PostgreSQL, 3: MySQL] : ");
                while (dbms != "1" && dbms != "2" && dbms != "3")
                {
                    dbms = Console.ReadLine() ?? string.Empty;
                }
            }
        }

        private void AskForPort()
        {
            var defaultPort = string.Empty;
            switch (dbms)
            {
                case "1":
                    defaultPort = "1433";
                    break;
                case "2":
                    defaultPort = "5432";
                    break;
                case "3":
                    defaultPort = "3306";
                    break;
            }
            logger.LogInformation($"Please enter port number[Default: {defaultPort}] ");
            port = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(port))
            {
                if (dbms != "1")
                {
                    port = defaultPort;
                }
            }
        }

        private void AskForServer()
        {
            logger.LogInformation("ConnectionString Server [Default: localhost] : ");
            var userInputServer = Console.ReadLine();
            server = string.IsNullOrEmpty(userInputServer)
                ? DefaultParameters.HostName ?? string.Empty
                : userInputServer;
        }

        private void AskForMySqlConnectingHost()
        {
            logger.LogInformation("MySQL Connecting Host [Default: %] : ");
            var userInputMySqlConnectingHost = Console.ReadLine();
            mySqlConnectingHost = string.IsNullOrEmpty(userInputMySqlConnectingHost)
                ? "%"
                : userInputMySqlConnectingHost;
        }

        private void AskForServiceName()
        {
            logger.LogInformation("ServiceName [Default: Implem.Pleasanter] : ");
            var userInputServiceName = Console.ReadLine();
            serviceName = string.IsNullOrEmpty(userInputServiceName)
                ? DefaultParameters.ServiceName ?? string.Empty
                : userInputServiceName;
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
            var timeZoneString = Environment.OSVersion.Platform == PlatformID.Win32NT
                ? "Tokyo Standard Time"
                : "Asia/Tokyo";
            do
            {
                logger.LogInformation($"TimeZoneDefault [1: UTC(Default), 2: {timeZoneString}, 3: Other] : ");
                var choiceTimeZone = Console.ReadLine() ?? string.Empty;
                switch (choiceTimeZone)
                {
                    case "1":
                    case "":
                        defaultTimeZone = "UTC";
                        break;
                    case "2":
                        defaultTimeZone = timeZoneString;
                        break;
                    case "3":
                        do
                        {
                            logger.LogInformation("Please enter a valid time zone for your OS:");
                            defaultTimeZone = Console.ReadLine() ?? string.Empty;
                        } while (!TimeZoneInfo.GetSystemTimeZones().Any(o => o.Id == defaultTimeZone));
                        break;
                    default:
                        logger.LogInformation("Please enter the number of choices");
                        continue;
                }
                break;
            } while (true);
        }

        private void AskForUserId()
        {
            switch ((DBMS)int.Parse(dbms))
            {
                case DBMS.SQLServer:
                    userId = "sa";
                    break;
                case DBMS.PostgreSQL:
                    userId = "postgres";
                    break;
                case DBMS.MySQL:
                    userId = "root";
                    break;
            }
            logger.LogInformation($"SaConnectionString UID [Default: {userId}] : ");
            var userInputUserId = Console.ReadLine();
            if (!string.IsNullOrEmpty(userInputUserId))
            {
                userId = userInputUserId;
            }
        }

        private void AskForPassword()
        {
            logger.LogInformation("SaConnectionString PWD : ");
            while (string.IsNullOrEmpty(saPassword))
            {
                saPassword = ReadPassword() ?? "";
                Console.WriteLine();
            }
            logger.LogInformation("OwnerConnectionString PWD : ");
            while (string.IsNullOrEmpty(ownerPassword))
            {
                ownerPassword = ReadPassword() ?? "";
                Console.WriteLine();
            }
            logger.LogInformation("UserConnectionString PWD : ");
            while (string.IsNullOrEmpty(userPassword))
            {
                userPassword = ReadPassword() ?? "";
                Console.WriteLine();
            }
        }

        private string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo keyInfo;
            do
            {
                password = string.Empty;
                logger.LogInformation("Enter your password: ");
                do
                {
                    keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
                    {
                        password += keyInfo.KeyChar;
                        Console.Write("*");
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                } while (keyInfo.Key != ConsoleKey.Enter);
                Console.WriteLine();
                if (string.IsNullOrEmpty(password))
                {
                    logger.LogInformation("Password cannot be empty. Please try again.");
                }
            } while (string.IsNullOrEmpty(password));
            return password;
        }

        private string CalculateRangeOfColumns(
            string columnType,
            int count)
        {
            return count > 26
                ? $"{columnType}A - {columnType}{(count - 26).ToString("D3")}"
                : count > 0 || count == 26
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

        private void ExistingCopyLicense(string installDir, string backupDir)
        {
            if (isProviderAzure)
            {
                var license = Path.Combine(
                    backupDir,
                    "Implem.License.dll");
                File.Copy(
                    license,
                    Path.Combine(
                        Path.GetDirectoryName(installDir),
                        Path.GetFileName(CodeDefinerDirPath),
                        "Implem.License.dll"),
                    true);
                File.Copy(
                    license,
                    Path.Combine(
                        installDir,
                        "Implem.License.dll"),
                    true);
            }
            else
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
        }

        private void CopyResourceDirectory(
            string installDir,
            string destDir,
            string guidDir = "")
        {
            if (isProviderAzure)
            {
                var backupDirCodeDefiner = Path.Combine(
                    Path.GetDirectoryName(CodeDefinerDirPath),
                    $"{Path.GetFileName(CodeDefinerDirPath)}{DateTime.Now:_yyyyMMdd_HHmmss}");
                Directory.CreateDirectory(destDir);
                foreach (var file in Directory.GetFiles(installDir))
                {
                    var destFile = Path.Combine(destDir, Path.GetFileName(file));
                    File.Move(file, destFile);
                }
                foreach (var dir in Directory.GetDirectories(installDir))
                {
                    var destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                    Directory.Move(dir, destSubDir);
                }
                Directory.Move(
                    Path.Combine(
                        Path.GetDirectoryName(installDir),
                        $"{Path.GetFileName(CodeDefinerDirPath)}"),
                    backupDirCodeDefiner);
                temporaryPath = Path.Combine(
                     guidDir,
                    "Implem.Pleasanter");
                CopyDirectory(destDir, temporaryPath, true);
            }
            else
            {
                Directory.CreateDirectory(destDir);
                foreach (var dir in Directory.GetDirectories(installDir))
                {
                    Directory.Move(dir, dir.Replace(installDir, destDir));
                }
            }
        }

        private void DisplaySummary(bool trial = false)
        {
            logger.LogInformation("------ Summary ------");
            logger.LogInformation($"Install Directory         : {installDir}");
            logger.LogInformation($"DBMS                      : {Enum.GetName(typeof(DBMS), int.Parse(dbms))}");
            logger.LogInformation($"SaConnectionString PWD    : **********");
            logger.LogInformation($"OwnerConnectionString PWD : **********");
            logger.LogInformation($"UserConnectionString PWD  : **********");
            if (!string.IsNullOrEmpty(port))
            {
                logger.LogInformation($"Port                      : {port}");
            }
            logger.LogInformation($"Server                    : {server}");
            logger.LogInformation($"Service Name              : {serviceName}");
            if (dbms.Equals("3"))
            {
                logger.LogInformation($"MySqlConnectingHost       : {mySqlConnectingHost}");
            }
            if (!versionUp)
            {
                logger.LogInformation($"DefaultLanguage           : {defaultLanguage}");
                logger.LogInformation($"DefaultTimeZone           : {defaultTimeZone}");
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
            var arguments = "";
            var fileName = "";
            var pOtion = "";
            var forceOption = force ? "/f" : "";
            var noInputOption = noinput ? "/n" : "";
            var language = "";
            var timeZone = "";
            if (isEnvironmentUser)
            {
                var permissionCommand = $"chown -R {userName} {Path.GetDirectoryName(installDir)}";
                await $"sudo chown -R {userName} {Path.GetDirectoryName(installDir)}";
            }
            await $"cd {codeDefinerDir}";
            if (isProviderAzure)
            {
                pOtion = "/p " + installDir;
            }
            //初回インストール時にのみ既定の言語、タイムゾーンを引数に追加する。
            if (!versionUp)
            {
                language = "/l " + defaultLanguage;
                timeZone = "/z " + "\"" + defaultTimeZone + "\"";
            }
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                fileName = "dotnet";
                arguments = $"Implem.CodeDefiner.dll _rds {pOtion} {forceOption} {noInputOption} {language} {timeZone}";
            }
            else
            {
                var dotnet_root = Environment.GetEnvironmentVariable("DOTNET_ROOT");
                fileName = $"sudo";
                arguments = Environment.ExpandEnvironmentVariables($"-u {userName} -E {dotnet_root}/dotnet Implem.CodeDefiner.dll _rds {forceOption} {noInputOption} {language} {timeZone}");
            }
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments
            };
            var userInput = string.Empty;
            var trialActive = false;
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
                        userInput = input;
                        process.StandardInput.WriteLine(input);
                    }
                    if (!EnterpriseEdition
                    && item is "<ERROR> Configurator.CheckColumnsShrinkage: The columns will be shrinked."
                    && !force)
                    {
                        trialActive = true;
                    }
                    if (!EnterpriseEdition
                    && item is "<SUCCESS> Starter.Main: All of the processes have been completed.")
                    {
                        var trialUrl = DefaultParameters.FirstSetupTrialUrl;
                        if (versionUp) trialUrl = DefaultParameters.VerUpTrialUrl;
                        Implem.PleasanterSetup.Utilities.UrlOpener.Open(
                            url: trialUrl,
                            message: $"To enable the trial, please run `pleasanter-setup trial`. " +
                                $"\nFor reference, visit {StripQueryParameters(trialUrl)}.",
                            isAzure: isProviderAzure);
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
            //上の処理結果で表示非表示
            if (trialActive) {
                //トライアルで実行する。
                logger.LogWarning("Column shrinkage was detected during execution of the '_rds' command. The trial command will be executed.");
                await ExecuteTrialCodeDefiner(
                    codeDefinerDir: codeDefinerDir,
                    noinput: noinput);
                return;
            }
            if (userInput.Equals("y"))
            {
                logger.LogInformation("Setup is complete.");
            }
        }

        private async Task ExecuteTrialCodeDefiner(
            string codeDefinerDir,
            bool noinput)
        {
            var arguments = "";
            var fileName = "";
            var pOtion = "";
            var noInputOption = noinput ? "/n" : "";
            if (isEnvironmentUser)
            {
                var permissionCommand = $"chown -R {userName} {Path.GetDirectoryName(installDir)}";
                await $"sudo chown -R {userName} {Path.GetDirectoryName(installDir)}";
            }
            await $"cd {codeDefinerDir}";
            if (isProviderAzure)
            {
                pOtion = "/p " + installDir;
            }
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                fileName = "dotnet";
                arguments = $"Implem.CodeDefiner.dll trial {pOtion} {noInputOption}";
            }
            else
            {
                var dotnet_root = Environment.GetEnvironmentVariable("DOTNET_ROOT");
                fileName = $"sudo";
                arguments = Environment.ExpandEnvironmentVariables($"-u {userName} -E {dotnet_root}/dotnet Implem.CodeDefiner.dll trial {noInputOption}");
            }
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments
            };
            var userInput = string.Empty;
            var trialExpired = false;
            var trialColumnsShrinked = false;
            var (process, stdOut, stdError) = ProcessX.GetDualAsyncEnumerable(processStartInfo);
            // 標準出力と標準エラー出力を処理するタスク
            var outputTask = Task.Run(async () =>
            {
                await foreach (var item in stdOut)
                {
                    Console.WriteLine(item);
                    if (item is "Type \"y\" (yes) to continue the Pleasanter Extensions Trial process, otherwise type \"n\" (no).")
                    {
                        var input = Console.ReadLine();
                        userInput = input;
                        process.StandardInput.WriteLine(input);
                    }
                    //まずカラムの縮小を検知する。Comunity Editionかつカラムの縮小を検知する。EnterpriseEditionがfalseかつ、文言一致で比較？
                    if (!EnterpriseEdition
                    && item is "<ERROR> Configurator.TrialConfigure: Trial period has expired. You can not rerun.")
                    {
                        trialExpired = true;
                    }
                    else if (!EnterpriseEdition
                    && item is "<ERROR> Configurator.CheckColumnsShrinkage: The columns will be shrinked.")
                    {
                        trialColumnsShrinked = true;
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
            //上の処理結果で表示非表示
            if (trialExpired)
            {
                // エンタープライズ案内ページを開く（失敗時は指定メッセージを表示）
                Implem.PleasanterSetup.Utilities.UrlOpener.Open(
                     url: DefaultParameters.EnterpriseUrl,
                     message: $"If you are using the Enterprise Edition, please visit {StripQueryParameters(DefaultParameters.EnterpriseUrl)}.",
                     isAzure: isProviderAzure);
            }
            if (userInput.Equals("y"))
            {
                if (trialColumnsShrinked)
                {
                    Implem.PleasanterSetup.Utilities.UrlOpener.Open(
                         url: DefaultParameters.VerUpTrialUrl,
                         message: $"To enable the Extended Column settings, please refer to the manual at {StripQueryParameters(DefaultParameters.VerUpTrialUrl)}.",
                         isAzure: isProviderAzure);
                }
                logger.LogInformation("Setup is complete.");
            }
        }

        private async Task ExecuteMerge(
            string newResource,
            string preResource)
        {
            try
            {
                logger.LogInformation("Start the merge process.");
                //プリザンター実行ユーザとインストーラの実行ユーザが異なるインストール先のディレクトリの権限を戻す
                if (isEnvironmentUser)
                {
                    await $"sudo chown -R {userName} {Path.GetDirectoryName(installDir)}";
                }
                var codeDefinerDir = isProviderAzure
                ? CodeDefinerDirPath
                : Path.Combine(
                    newResource,
                    "Implem.CodeDefiner");
                var pOtion = string.Empty;
                //Azureのみ/pを指定
                if (isProviderAzure)
                {
                    preResource = Directory.GetParent(preResource).ToString();
                    pOtion = "/p " + Path.Combine(newResource, "Implem.Pleasanter");
                }
                await $"cd {codeDefinerDir}";

                var fileName = string.Empty;
                var arguments = string.Empty;
                //Widows,Linuxでコマンドが異なる
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    fileName = "dotnet";
                    arguments = $"Implem.CodeDefiner.dll merge /b {preResource} /i {newResource} {pOtion}";
                }
                else
                {
                    fileName = "sudo";
                    var dotnet_root = Environment.GetEnvironmentVariable("DOTNET_ROOT");
                    arguments = $"-u {userName} {dotnet_root}/dotnet Implem.CodeDefiner.dll merge /b {preResource} /i {newResource} ";
                }
                bool errorFlag = false;
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments
                };
                var (process, stdOut, stdError) = ProcessX.GetDualAsyncEnumerable(processStartInfo);
                // 標準出力と標準エラー出力を処理するタスク
                var outputTask = Task.Run(async () =>
                {
                    await foreach (var item in stdOut)
                    {
                        Console.WriteLine(item);
                        if (item.Contains("<ERROR>"))
                        {
                            errorFlag = true;
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
                if (errorFlag)
                {
                    logger.LogError("Merge process has been interrupted.");
                    Environment.Exit(0);
                }
                logger.LogInformation("The merge process has finished.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                logger.LogError("Merge process has been interrupted.");
                Environment.Exit(0);
            }
        }

        private async Task ExecuteCreateDirectory(string dir)
        {
            try
            {
                var createCommand = $"mkdir -p {dir}";
                await ExecuteCommands(createCommand);
                var permissionDirectory = Directory.GetParent(dir);
                if (permissionDirectory == null)
                {
                    logger.LogError($"Failed to get parent directory for '{dir}'. It may be a root directory.");
                    Environment.Exit(0);
                }
                var permissionCommand = $"chown -R {userName} {permissionDirectory}";
                await ExecuteCommands(permissionCommand);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                Environment.Exit(0);
            }
        }

        // OSごとにディレクトリ作成を分岐する共通メソッド
        private async Task CreateDirectoryCrossPlatform(string dir)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Directory.CreateDirectory(dir);
            }
            else
            {
                await ExecuteCreateDirectory(dir);
            }
        }

        private async Task ExecuteCommands(string command)
        {
            Console.WriteLine($"sudo {command}");
            await $"sudo {command}";
        }


        private string GetDefaultInstallDir()
        {
            string instanceId = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
            if (!string.IsNullOrEmpty(instanceId))
            {
                isProviderAzure = true;
                CodeDefinerDirPath = DefaultParameters.CodeDefinerDirForAzure;
                return installDir = DefaultParameters.InstallDirForAzure;
            }
            else
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT
                    ? installDir = DefaultParameters.InstallDirForWindows
                    : installDir = DefaultParameters.InstallDirForLinux;
            }
        }

        private bool MeetsVersionUpRequirements()
        {
            var pleasanterDll = string.Empty;
            //dllがある場合はバージョンアップ
            if (isProviderAzure)
            {
                pleasanterDll = Path.Combine(
                    installDir,
                    "Implem.Pleasanter.dll");
            }
            else
            {
                pleasanterDll = Path.Combine(
                    installDir,
                    "Implem.Pleasanter",
                    "Implem.Pleasanter.dll");
            }
            return File.Exists(pleasanterDll);
        }

        private void SetParameters()
        {
            try
            {
                logger.LogInformation("Start setting parameters");
                var parametersDir = string.Empty;
                if (isProviderAzure)
                {
                    parametersDir = Path.Combine(
                        installDir,
                        "App_Data",
                        "Parameters");
                }
                else
                {
                    parametersDir = Path.Combine(
                        installDir,
                        "Implem.Pleasanter",
                        "App_Data",
                        "Parameters");
                }
                if (!versionUp)
                {
                    //Rds.json,Service.json内容をユーザの入力値をもとに書き込む
                    SetRdsParameters(parametersDir);
                    SetServiceParameters(parametersDir);
                }
                else
                {
                    // バージョンアップ時にMySqlConnectingHostをRds.jsonに新規追加する場合
                    if (addMySqlConnectingHost)
                    {
                        AddMySqlConnectingHostRds(parametersDir);
                    }
                }
                logger.LogInformation("Finish setting parameters");
            }
            catch (Exception e)
            {
                logger.LogInformation(e.Message);
                Environment.Exit(0);
            }
        }

        private void SetRdsParameters(string parametersDir)
        {
            var file = Path.Combine(parametersDir, "Rds.json");
            var json = File.ReadAllText(file);
            JObject inputData = (JObject)JsonConvert.DeserializeObject(json);

            var data = json.Deserialize<Rds>();
            data.Dbms = Enum.GetName(typeof(DBMS), int.Parse(dbms));
            var database = dbms == "1"
                    ? "master"
                    : dbms == "2"
                        ? "postgres"
                            : dbms == "3"
                                ? "mysql"
                        : string.Empty;
            if (provider.Equals("Local"))
            {
                switch (dbms)
                {
                    case "1":
                        if (provider.Equals("Azure"))
                        {
                            data.SaConnectionString = connectionString;
                            data.OwnerConnectionString = connectionString;
                            data.UserConnectionString = connectionString;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(port))
                            {
                                server = string.Join(",", server, port);
                            }
                            data.SaConnectionString = $"Server={server};Database={database};UID={userId};PWD={saPassword};";
                            data.OwnerConnectionString = $"Server={server};Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={ownerPassword};";
                            data.UserConnectionString = $"Server={server};Database=#ServiceName#;UID=#ServiceName#_User;PWD={userPassword};";
                        }
                        break;
                    case "2":
                    case "3":
                        data.SaConnectionString = $"Server={server};Port={port};Database={database};UID={userId};PWD={saPassword};";
                        data.OwnerConnectionString = $"Server={server};Port={port};Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={ownerPassword};";
                        data.UserConnectionString = $"Server={server};Port={port};Database=#ServiceName#;UID=#ServiceName#_User;PWD={userPassword};";
                        break;
                }
            }
            else
            {
                data.Provider = provider;
                data.SaConnectionString = connectionString;
                data.OwnerConnectionString = connectionString;
                data.UserConnectionString = connectionString;
            }
            if (data.Dbms.Equals("MySQL"))
            {
                data.MySqlConnectingHost = mySqlConnectingHost;
            }
            JObject dataAsJObject = JObject.FromObject(data);
            //読み込んだファイルに存在するパラメータ
            foreach (var prop in dataAsJObject.Properties())
            {
                if (inputData.ContainsKey(prop.Name))
                {
                    inputData[prop.Name] = prop.Value;
                }
            }
            json = Jsons.ToJson(inputData);
            File.WriteAllText(
                file,
                json);
        }

        private void SetServiceParameters(string parametersDir)
        {
            var file = Path.Combine(parametersDir, "Service.json");
            var json = File.ReadAllText(file);
            JObject inputData = (JObject)JsonConvert.DeserializeObject(json);
            var data = json.Deserialize<Service>();
            data.Name = serviceName;
            JObject dataAsJObject = JObject.FromObject(data);
            foreach (var prop in dataAsJObject.Properties())
            {
                if (inputData.ContainsKey(prop.Name))
                {
                    inputData[prop.Name] = prop.Value;
                }
            }
            json = Jsons.ToJson(inputData);
            File.WriteAllText(
                file,
                json);
        }

        private void AddMySqlConnectingHostRds(string parametersDir)
        {
            var file = Path.Combine(parametersDir, "Rds.json");
            var json = File.ReadAllText(file);
            JObject inputData = (JObject)JsonConvert.DeserializeObject(json);
            if (inputData.ContainsKey("MySqlConnectingHost"))
            {
                inputData["MySqlConnectingHost"] = mySqlConnectingHost;
            }
            File.WriteAllText(
                file,
                Jsons.ToJson(inputData));
        }

        private void SetSummary(
            string directory,
            string releasezip = "",
            string patchPath = "",
            bool setUpState = true,
            bool trial = false)
        {
            //オフライン環境での必須オプションの確認
            AskForInstallDir(directory);
            //Implem.Pleasanter.dllの有無でバージョンアップか判断
            AskForUserName();
            versionUp = MeetsVersionUpRequirements();
            if (trial)
            {
                CheckTrialAvailability();
            }
            if (!setUpState && !Directory.Exists(installDir))
            {
                logger.LogError($"{installDir} does not exist.");
                Environment.Exit(1);
            }
            if (versionUp)
            {
                EnterpriseEdition = CheckLicense();
                GetUserData();
            }
            else
            {
                AskForProvider();
                AskForServiceName();
                AskForDbms();
                if (provider.Equals("Local"))
                {
                    AskForPort();
                    AskForServer();
                    AskForUserId();
                    AskForPassword();
                }
                if (provider.Equals("Azure"))
                {
                    AskForConnectionString();
                }
                if (dbms.Equals("3"))
                {
                    AskForMySqlConnectingHost();
                }
                AskForDefaultLanguage();
                AskForDefaultTimeZone();
            }
        }
        private void AskForUserName()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                do
                {
                    logger.LogInformation("Please enter the user who will execute Pleasanter.");
                    var userInputUserName = Console.ReadLine();
                    if (!string.IsNullOrEmpty(userInputUserName))
                    {
                        userName = userInputUserName;
                        if (Environment.UserName != userName)
                        {
                            isEnvironmentUser = true;
                        }
                        break;
                    }
                } while (true);
            }
        }
        private void AskForConnectionString()
        {
            do
            {
                logger.LogInformation("Enter the connection string for your Azure SQL Database.");
                var userInputConnectionString = Console.ReadLine();
                if (!string.IsNullOrEmpty(userInputConnectionString))
                {
                    connectionString = userInputConnectionString;
                    break;
                }
            } while (true);
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;
            if (builder.ContainsKey("Server"))
            {
                server = builder["Server"].ToString();
            }
            else
            {
                server = builder["Data Source"].ToString();
            }
        }
        private void AskForProvider()
        {
            if (isProviderAzure)
            {
                provider = "Azure";
            }
            else
            {
                provider = "Local";
            }
        }

        private void GetUserData()
        {
            var rdsFile = string.Empty;
            var serviceFile = string.Empty;
            try
            {
                if (isProviderAzure)
                {
                    rdsFile = Path.Combine(
                        installDir,
                        "App_Data",
                        "Parameters",
                        "Rds.json");
                    serviceFile = Path.Combine(
                        installDir,
                        "App_Data",
                        "Parameters",
                        "Service.json");
                }
                else
                {
                    rdsFile = Path.Combine(
                        installDir,
                        "Implem.Pleasanter",
                        "App_Data",
                        "Parameters",
                        "Rds.json");
                    serviceFile = Path.Combine(
                        installDir,
                        "Implem.Pleasanter",
                        "App_Data",
                        "Parameters",
                        "Service.json");
                }
                if (!File.Exists(rdsFile))
                {
                    logger.LogError($"The file {rdsFile} was not found.");
                    Environment.Exit(0);
                }
                if (!File.Exists(serviceFile))
                {
                    logger.LogError($"The file {serviceFile} was not found.");
                    Environment.Exit(0);
                }
                var rdsJson = File.ReadAllText(rdsFile);
                var rdsData = rdsJson.Deserialize<Rds>();
                var serviceJson = File.ReadAllText(serviceFile);
                var serviceData = serviceJson.Deserialize<Service>();
                provider = rdsData.Provider;
                serviceName = serviceData.Name;
                SaConnectionString = CoalesceEmpty(
                    rdsData.SaConnectionString,
                    Environment.GetEnvironmentVariable($"{serviceData.EnvironmentName}_Rds_SaConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceData.EnvironmentName}_Rds_ConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_{rdsData.Dbms}_SaConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_{rdsData.Dbms}_ConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_SaConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_ConnectionString"));
                OwnerConnectionString = CoalesceEmpty(
                    rdsData.OwnerConnectionString,
                    Environment.GetEnvironmentVariable($"{serviceData.EnvironmentName}_Rds_OwnerConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceData.EnvironmentName}_Rds_ConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_{rdsData.Dbms}_OwnerConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_{rdsData.Dbms}_ConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_OwnerConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_ConnectionString"));
                UserConnectionString = CoalesceEmpty(
                    rdsData.UserConnectionString,
                    Environment.GetEnvironmentVariable($"{serviceData.EnvironmentName}_Rds_UserConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceData.EnvironmentName}_Rds_ConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_{rdsData.Dbms}_UserConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_{rdsData.Dbms}_ConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_UserConnectionString"),
                    Environment.GetEnvironmentVariable($"{serviceName}_Rds_ConnectionString"));
                // MySQLかつRds.jsonにMySqlConnectingHostの追加を要する更新であるか否かの判定。
                if (rdsData.Dbms.Equals("MySQL") && rdsData.MySqlConnectingHost == null)
                {
                    AskForMySqlConnectingHost();
                    addMySqlConnectingHost = true;
                }
                else
                {
                    mySqlConnectingHost = CoalesceEmpty(
                        rdsData.MySqlConnectingHost,
                        Environment.GetEnvironmentVariable($"{serviceData.EnvironmentName}_Rds_MySqlConnectingHost"),
                        Environment.GetEnvironmentVariable($"{serviceName}_Rds_MySqlConnectingHost"));
                }
                GetDbms(rdsData);
                GetServerAndPort(rdsData.Dbms);
                GetUserId();
                GetPassword();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                Environment.Exit(0);
            }
        }

        private void GetDbms(Rds data)
        {
            switch (data.Dbms)
            {
                case "SQLServer":
                    dbms = "1";
                    break;
                case "PostgreSQL":
                    dbms = "2";
                    break;
                case "MySQL":
                    dbms = "3";
                    break;
            }
        }

        private void GetServerAndPort(string dbms)
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = SaConnectionString;
            switch (dbms)
            {
                case "SQLServer":
                    if (builder.ContainsKey("server"))
                    {
                        server = builder["server"].ToString();
                    }
                    else
                    {
                        server = builder["Data Source"].ToString();
                    }
                    int commaIndex = server.IndexOf(',');
                    port = server.IndexOf(',') >= 0
                        ? server.Substring(commaIndex + 1)
                        : string.Empty;
                    break;
                default:
                    server = builder["server"].ToString();
                    if (builder.ContainsKey("port"))
                    {
                        port = builder["port"].ToString();
                    }
                    break;
            }
        }

        private void GetUserId()
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = SaConnectionString;
            if (!provider.Equals("Azure"))
            {
                userId = builder["UID"].ToString();
            }
        }

        private void GetPassword()
        {
            if (!provider.Equals("Azure"))
            {
                DbConnectionStringBuilder saBuilder = new DbConnectionStringBuilder();
                saBuilder.ConnectionString = SaConnectionString;
                saPassword = saBuilder["PWD"].ToString();
                DbConnectionStringBuilder ownerBuilder = new DbConnectionStringBuilder();
                ownerBuilder.ConnectionString = OwnerConnectionString;
                ownerPassword = ownerBuilder["PWD"].ToString();
                DbConnectionStringBuilder userBuilder = new DbConnectionStringBuilder();
                userBuilder.ConnectionString = UserConnectionString;
                userPassword = userBuilder["PWD"].ToString();
            }
        }

        public static string CoalesceEmpty(params string[] args)
        {
            return args
                .Where(o => !string.IsNullOrEmpty(o))
                .FirstOrDefault() ?? string.Empty;
        }

        private bool CheckLicense(string licenseZip)
        {
            var absolutePath = Path.GetFullPath(licenseZip);
            var baseFileName = Path.GetFileNameWithoutExtension(absolutePath);
            var unzipDir = Path.Combine(
                        Path.GetDirectoryName(absolutePath),
                        baseFileName);
            ZipFile.ExtractToDirectory(
                licenseZip,
                unzipDir,
                true);
            var license = Path.Combine(
                unzipDir,
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
                Environment.Exit(0);
            }
            Assembly assembly = Assembly.LoadFile(license);
            Type type = assembly.GetType("Implem.License.License");
            object instance = Activator.CreateInstance(type);
            MethodInfo checkMethod = type.GetMethod("Check");
            return Convert.ToBoolean(checkMethod?.Invoke(instance, null));
        }

        private void MoveResource(
            string installDir,
            string unzipDir,
            string guidDir = "")
        {
            try
            {
                if (unzipDir != installDir)
                {
                    foreach (var dir in Directory.GetDirectories(unzipDir))
                    {
                        var path = string.Empty;
                        switch (Path.GetFileName(dir))
                        {
                            case "Implem.Pleasanter":
                                if (Directory.Exists(dir))
                                {
                                    CopyDirectory(
                                        dir,
                                        installDir,
                                        true);
                                }
                                else
                                {
                                    path = installDir;
                                    Directory.Move(dir, path);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    Directory.Delete(unzipDir, true);
                }
                if (Directory.Exists(guidDir))
                {
                    Directory.Delete(guidDir, true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                Environment.Exit(0);
            }
        }

        private void SetNewResource(
            string installDir,
            string releaseZip,
            string guidDir)
        {
            try
            {
                ZipFile.ExtractToDirectory(
                    releaseZip,
                    guidDir,
                    true);
                var unzipDir = Path.Combine(
                    guidDir,
                    "pleasanter");
                if (!isProviderAzure)
                {
                    logger.LogInformation($"Start placing release resources to {installDir}.");
                    if (unzipDir != installDir)
                    {
                        foreach (var dir in Directory.GetDirectories(unzipDir))
                        {
                            Directory.Move(dir, dir.Replace(unzipDir, installDir));
                        }
                        Directory.Delete(guidDir, true);
                    }
                    logger.LogInformation($"The release resource was placed in {installDir}.");
                }
                else
                {
                    foreach (var dir in Directory.GetDirectories(unzipDir))
                    {
                        var path = string.Empty;
                        switch (Path.GetFileName(dir))
                        {
                            case "Implem.CodeDefiner":
                                path = CodeDefinerDirPath;
                                Directory.Move(dir, path);
                                break;
                            default:
                                break;
                        }
                    }
                    unzipDirPath = unzipDir;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString(), ex);
                Environment.Exit(0);
            }
        }

        private void SetParametersPatch(string previous, string patchPath)
        {
            try
            {
                logger.LogInformation($"Start placing ParametersPatch.zip to {previous}.");
                var destPath = Path.Combine(
                    previous,
                    "ParametersPatch.zip");
                File.Move(
                    patchPath,
                    destPath,
                    true);
                logger.LogInformation($"Finish placing ParametersPatch.zip to {previous}.");
            }
            catch (Exception e)
            {
                logger.LogError($"{e.Message}");
                Environment.Exit(0);
            }
        }
        private void CheckTrialAvailability()
        {
            if (!versionUp)
            {
                logger.LogInformation(
                    "To install the Pleasanter Community Edition, please follow the installer guide below." + Environment.NewLine +
                    "After installation, to enable the trial run: pleasanter-setup trial" + Environment.NewLine +
                    "Windows:   " + DefaultParameters.InstallUrlForWindows + Environment.NewLine +
                    "Ubuntu:    " + DefaultParameters.InstallUrlForUbuntu + Environment.NewLine +
                    "AlmaLinux: " + DefaultParameters.InstallUrlForAlmaLinux + Environment.NewLine +
                    "RHEL 8:    " + DefaultParameters.InstallUrlForRhel8 + Environment.NewLine +
                    "RHEL 9:    " + DefaultParameters.InstallUrlForRhel9 + Environment.NewLine +
                    "Azure:     " + DefaultParameters.InstallUrlForAzure
                );
                Environment.Exit(1);
            }
            var enterpriseEdition = CheckLicense();
            if (enterpriseEdition)
            {
                logger.LogInformation("The trial option is only available for the Community Edition.");
                Environment.Exit(1);
            }
            else
            {
                var pleasanterDllPath = string.Empty;
                if (isProviderAzure)
                {
                    pleasanterDllPath = Path.Combine(
                       installDir,
                       "Implem.Pleasanter.dll");
                }
                else
                {
                    pleasanterDllPath = Path.Combine(
                        installDir,
                        "Implem.Pleasanter",
                        "Implem.Pleasanter.dll");
                }
                var assemblyName = AssemblyName.GetAssemblyName(pleasanterDllPath);
                var version = assemblyName.Version;
                var minVersion = new System.Version(1, 4, 15, 0);
                if (version < minVersion)
                {
                    logger.LogError($"The trial feature is available from version 1.4.15.0 or later. Please install version 1.4.15.0 or newer.");
                    Environment.Exit(1);
                }
            }
        }

        private bool CheckLicense()
        {
            try
            {
                if (string.IsNullOrEmpty(installDir))
                {
                    logger.LogError("installDir is not set.");
                    Environment.Exit(0);
                }
                var license = string.Empty;
                if (isProviderAzure)
                {
                    license = Path.Combine(
                        installDir,
                        "Implem.License.dll");
                }
                else
                {
                    license = Path.Combine(
                        installDir,
                        "Implem.Pleasanter",
                        "Implem.License.dll");
                }
                if (!File.Exists(license))
                {
                    logger.LogError($"Implem.License.dll not found: {license}");
                    Environment.Exit(0);
                }
                licenseDllPath = license;
                return CheckMethodExecute(license);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                Environment.Exit(0);
            }
            return false;
        }

        private async Task<string> DownloadNewResource(string installDir, string fileName)
        {
            logger.LogInformation($"Download {fileName}");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject release = JObject.Parse(responseBody);
            var destinationDir = string.Empty;
            foreach (var asset in release["assets"])
            {
                if (asset["name"].ToString().Contains(fileName))
                {
                    try
                    {
                        string assetUrl = asset["browser_download_url"].ToString();
                        HttpResponseMessage assetResponse = await client.GetAsync(assetUrl);
                        assetResponse.EnsureSuccessStatusCode();
                        destinationDir = Path.Combine(installDir, asset["name"].ToString());
                        byte[] fileBytes = await assetResponse.Content.ReadAsByteArrayAsync();
                        await File.WriteAllBytesAsync(destinationDir, fileBytes);
                        logger.LogInformation($"Downloaded {asset["name"]} to {destinationDir}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                        Environment.Exit(0);
                    }
                }
            }
            return destinationDir;
        }
        private static string StripQueryParameters(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            int idx = url.IndexOf('?');
            return idx >= 0 ? url.Substring(0, idx) : url;
        }
    }
}
