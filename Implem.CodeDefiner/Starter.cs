using Implem.CodeDefiner.Settings;
using Implem.DefinitionAccessor;
using Implem.Factory;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Exceptions;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Implem.CodeDefiner
{
    public class Starter
    {
        private static ISqlObjectFactory factory;
        private static bool CompleteConfigureDatabase = false;
        private static string LogFolderName;
        private static string LogNameText;

        public static void Main(string[] args)
        {
            LogFolderName = "logs";
            LogNameText = $"Implem.CodeDefiner_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";
            Directory.CreateDirectory(LogFolderName);
            var logName = Path.Combine(LogFolderName, $"{LogNameText}.log");
            Trace.Listeners.Add(new TextWriterTraceListener(logName));
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                if (e.ExceptionObject is DbException dbException)
                {
                    Consoles.Write($"UnhandledException: [{factory.SqlErrors.ErrorCode(dbException)}] {dbException.Message}", Consoles.Types.Error, true);
                }
                else
                {
                    Consoles.Write("UnhandledException: " + e.ExceptionObject, Consoles.Types.Error, true);
                }
            };
            ValidateArgs(args);
            var argHash = ArgsType(args);
            var action = args[0];
            var path = argHash.Get("p")?.Replace('\\', System.IO.Path.DirectorySeparatorChar);
            var target = argHash.Get("t");
            try
            {
                Initializer.Initialize(
                    path,
                    assemblyVersion: Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    setLanguage: argHash.Get("l"),
                    setTimeZone: argHash.Get("z"),
                    codeDefiner: true,
                    setSaPassword: argHash.ContainsKey("s"),
                    setRandomPassword: argHash.ContainsKey("r"));
                Consoles.Write(
                    text: $"Implem.CodeDefiner {Environments.AssemblyVersion}",
                    type: Consoles.Types.Info);
                // CodeDefinerではSqlCommandTimeOutを無制限とする。
                Parameters.Rds.SqlCommandTimeOut = 0;
                factory = RdsFactory.Create(Parameters.Rds.Dbms);
                switch (action)
                {
                    case "_rds":
                        ConfigureDatabase(
                            factory: factory,
                            force: argHash.ContainsKey("f"),
                            noInput: argHash.ContainsKey("y"),
                            checkMigration: argHash.ContainsKey("c"));
                        break;
                    case "rds":
                        ConfigureDatabase(
                            factory: factory,
                            force: argHash.ContainsKey("f"),
                            noInput: argHash.ContainsKey("y"));
                        CreateDefinitionAccessorCode();
                        CreateMvcCode(target);
                        break;
                    case "_def":
                        CreateDefinitionAccessorCode();
                        break;
                    case "def":
                        CreateDefinitionAccessorCode();
                        CreateMvcCode(target);
                        break;
                    case "mvc":
                        CreateMvcCode(target);
                        break;
                    case "backup":
                        CreateSolutionBackup();
                        break;
                    case "migrate":
                        CanMigrate();
                        ConfigureDatabase(
                            factory: factory,
                            force: argHash.ContainsKey("f"),
                            noInput: argHash.ContainsKey("y"));
                        if (CompleteConfigureDatabase) MigrateDatabase(factoryTo: factory);
                        break;
                    case "ConvertTime":
                        ConvertTime(
                            factory: factory,
                            hourOffset: int.Parse(argHash.Get("h") ?? "-9"));
                        break;
                    case "merge":
                        MergeParameters(
                            backUpPath: argHash.Get("b"),
                            installPath: argHash.Get("i"));
                        break;
                    case "trial":
                        TrialConfigureDatabase(
                            factory: factory,
                            noInput: argHash.ContainsKey("y"),
                            useExColumnsFile: argHash.ContainsKey("e"));
                        break;
                    default:
                        WriteErrorToConsole(args);
                        break;
                }
                if (Consoles.ErrorCount > 0)
                {
                    Consoles.Write(
                        string.Format(DisplayAccessor.Displays.Get("CodeDefinerErrorCount"),
                            Consoles.ErrorCount,
                            Path.GetFullPath(logName)),
                        Consoles.Types.Error);
                }
                else
                {
                    Consoles.Write(
                        DisplayAccessor.Displays.Get("CodeDefinerCompleted"),
                        Consoles.Types.Success);
                }
            }
            catch (ParametersNotFoundException e)
            {
                Consoles.Write(
                    "ParametersNotFoundException : " + e.Message,
                    Consoles.Types.Error);
            }
            catch (ParametersIllegalSyntaxException e)
            {
                Consoles.Write(
                    "ParametersIllegalSyntaxException : " + e.Message,
                    Consoles.Types.Error);
            }
            catch (InvalidTimeZoneException e)
            {
                Consoles.Write(
                    "InvalidTimeZoneException : " + e.Message,
                    Consoles.Types.Error);
            }
            catch (InvalidLanguageException e)
            {
                Consoles.Write(
                    "InvalidLanguageException : " + e.Message,
                    Consoles.Types.Error);
            }
            catch (InvalidVersionException e)
            {
                Consoles.Write(
                    "InvalidVersionException : " + e.Message,
                    Consoles.Types.Error);
            }
            catch (ArgumentNullException e)
            {
                Consoles.Write(
                    "ArgumentNullException : " + e.Message,
                    Consoles.Types.Error);
            }
            catch (FileNotFoundException e)
            {
                Consoles.Write(
                    "FileNotFoundException : " + e.Message,
                    Consoles.Types.Error);
            }
            catch (DirectoryNotFoundException e)
            {
                Consoles.Write(
                    "DirectoryNotFoundException : " + e.Message,
                    Consoles.Types.Error);
            }
            catch (JsonException e)
            {
                Consoles.Write(
                    "JsonException : " + e.Message + "\n" + e.StackTrace,
                    Consoles.Types.Error);
            }
            catch (FormatException e)
            {
                Consoles.Write(
                    "FormatException : " + e.Message,
                    Consoles.Types.Error);
            }
            catch (Exception e)
            {
                Consoles.Write(
                    "UnhandledException : " + e.Message + "\n" + e.StackTrace,
                    Consoles.Types.Error);
            }
            WaitConsole(args);
        }

        public static Dictionary<string, string> ArgsType(string[] args)
        {
            var argsDictionary = new Dictionary<string, string>();
            string[] pathParameters = ["p", "b", "i"];
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].StartsWith("/"))
                {
                    string key = args[i].Replace("/", "");
                    string value = "";
                    if (pathParameters.Any(o => o == key) && i + 1 < args.Length)
                    {
                        value = args[i + 1];
                        argsDictionary[key] = value;
                        i++;
                        continue;
                    }
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("/"))
                    {
                        value = args[i + 1];
                        i++;
                    }
                    argsDictionary[key] = value;
                }
            }
            return argsDictionary;
        }

        private static void MergeParameters(
            string installPath = "",
            string backUpPath = "",
            string patchSourceZip = "")
        {
            if (backUpPath.IsNullOrEmpty())
            {
                throw new ArgumentNullException("/b");
            }
            if (installPath.IsNullOrEmpty())
            {
                installPath = GetDefaultInstallDir();
                patchSourceZip = GetDefaultPatchDir();
            }
            else
            {
                patchSourceZip = Path.Combine(
                    installPath,
                    "ParametersPatch.zip");
            }
            if (!File.Exists(patchSourceZip))
            {
                throw new FileNotFoundException($"The file '{patchSourceZip}' does not exist.");
            }
            var parametersDir = Path.Combine(
                installPath,
                "Implem.Pleasanter",
                "App_Data",
                "Parameters");
            var backUpParameterDir = Path.Combine(
                backUpPath,
                "Implem.Pleasanter",
                "App_Data",
                "Parameters");
            var newVersion = ReplaceVersion(
                FileVersionInfo.GetVersionInfo(
                Path.Combine(
                    installPath,
                    "Implem.Pleasanter",
                    "Implem.Pleasanter.dll")).FileVersion);
            var currentVersion = ReplaceVersion(
                FileVersionInfo.GetVersionInfo(
                Path.Combine(
                    backUpPath,
                    "Implem.Pleasanter",
                    "Implem.Pleasanter.dll")).FileVersion);
            ZipFile.ExtractToDirectory(
                    patchSourceZip,
                    installPath,
                    true);
            var patchSourcePath = Path.Combine(
                installPath,
                "ParametersPatch");
            CheckVersion(
                newVersion,
                currentVersion,
                patchSourcePath);
            CopyDirectory(
                backUpParameterDir,
                parametersDir,
                true);
            Functions.Patch.PatchParameters.ApplyToPatch(
                patchSourcePath,
                parametersDir,
                newVersion,
                currentVersion);
        }

        private static string GetDefaultInstallDir()
        {
            var defaultPath = new DefaultParameters();
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                ? defaultPath.InstallDirForWindows
                : defaultPath.InstallDirForLinux;
        }

        private static string GetDefaultPatchDir()
        {
            var defaultPath = new DefaultParameters();
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                ? defaultPath.PatchZipPathForWindows
                : defaultPath.PatchZIpPathForLinux;
        }

        private static string ReplaceVersion(string versionInfo)
        {
            return string.Join(".", versionInfo.Split(".").Select(s => ("00" + s)[^2..]));
        }

        private static void CheckVersion(string newVersion, string currentVersion, string patchSourcePath)
        {
            var newVersionObj = new System.Version(newVersion);
            var currentVersionObj = new System.Version(currentVersion);
            var patchDir = new DirectoryInfo(patchSourcePath);
            DirectoryInfo[] dirs = patchDir.GetDirectories();
            if (newVersionObj < currentVersionObj)
            {
                throw new InvalidVersionException("Invalid Version" + $" From:{currentVersionObj}" + $" To:{newVersionObj}");
            }
            if (!dirs.Any(o => o.Name == currentVersion))
            {
                throw new InvalidVersionException("Invalid Version:" + currentVersionObj);
            }
            if (!dirs.Any(o => o.Name == newVersion))
            {
                throw new InvalidVersionException("Invalid Version:" + newVersionObj);
            }
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

        private static void ValidateArgs(IEnumerable<string> args)
        {
            if (args.Count() == 0)
            {
                WriteErrorToConsole(args);
            }
            var argNames = args.Skip(1)
                .Where(o => o.StartsWith('/'))
                .Where(o => o.Length == 2);
            if (argNames.Count() != argNames.Distinct().Count())
            {
                WriteErrorToConsole(args);
            }
        }

        private static void ConfigureDatabase(
            ISqlObjectFactory factory,
            bool force = false,
            bool noInput = false,
            bool checkMigration = false)
        {
            try
            {
                TryOpenConnections(factory);
                var completed = Functions.Rds.Configurator.Configure(
                    factory: factory,
                    force: force,
                    noInput: noInput,
                    checkMigration: checkMigration);
                if (completed)
                {
                    Consoles.Write(
                        text: DisplayAccessor.Displays.Get("CodeDefinerRdsCompleted"),
                        type: Consoles.Types.Success);
                    CompleteConfigureDatabase = true;
                }
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                Consoles.Write(
                    text: $"[{e.Number}] {e.Message}",
                    type: Consoles.Types.Error,
                    abort: true);
            }
            catch (Exception e)
            {
                Consoles.Write(
                    text: e.ToString(),
                    type: Consoles.Types.Error,
                    abort: true);
            }
        }

        private static bool TryOpenConnections(ISqlObjectFactory factory)
        {
            int number;
            string message;
            if (!Sqls.TryOpenConnections(
                factory,
                out number, out message, Parameters.Rds.SaConnectionString))
            {
                Consoles.Write($"[{number}] {message}", Consoles.Types.Error, true);
                return false;
            }
            return true;
        }

        private static void CreateDefinitionAccessorCode()
        {
            Functions.AspNetMvc.CSharp.DefinitionAccessorCreator.Create();
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerDefCompleted"),
                Consoles.Types.Success);
        }

        private static void CreateMvcCode(string target)
        {
            Functions.AspNetMvc.CSharp.MvcCreator.Create(target);
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerMvcCompleted"),
                Consoles.Types.Success);
        }

        private static void CreateSolutionBackup()
        {
            Functions.CodeDefiner.BackupCreater.BackupSolutionFiles();
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerBackupCompleted"),
                Consoles.Types.Success);
        }

        private static void MigrateDatabase(ISqlObjectFactory factoryTo)
        {
            bool success = Functions.AspNetMvc.CSharp.Migrator.MigrateDatabaseAsync(
                logFolderName: LogFolderName,
                logNameText: LogNameText,
                factoryFrom: RdsFactory.Create(Parameters.Migration.Dbms),
                factoryTo: factoryTo);
            if (success)
            {
                Consoles.Write(
                    DisplayAccessor.Displays.Get("CodeDefinerMigrationCompleted"),
                    Consoles.Types.Success);
            }
            else
            {
                Consoles.Write(
                    string.Format(DisplayAccessor.Displays.Get($"CodeDefinerMigrationErrors"),
                        Path.GetFullPath(Path.Combine(LogFolderName, $"{LogNameText}_Migrate.log"))),
                    Consoles.Types.Error);
            }
        }

        private static void CanMigrate()
        {
            //移行可能な条件を追加する場合、以下処理を見直すこと。
            var checkMigrationDbms = false;
            switch (Parameters.Migration.Dbms)
            {
                case "SQLServer":
                    checkMigrationDbms = true;
                    break;
                default:
                    break;
            }
            if (!checkMigrationDbms)
            {
                throw new JsonException($"The value \"{Parameters.Migration.Dbms}\" cannot be set for \"Dbms\" in Migration.json.");
            }
            var checkMigrationProvider = false;
            switch (Parameters.Migration.Provider)
            {
                case "Local":
                    checkMigrationProvider = true;
                    break;
                default:
                    break;
            }
            if (!checkMigrationProvider)
            {
                throw new JsonException($"The value \"{Parameters.Migration.Provider}\" cannot be set for \"Provider\" in Migration.json.");
            }
            var checkRdsDbms = false;
            switch (Parameters.Rds.Dbms)
            {
                case "PostgreSQL":
                case "MySQL":
                    checkRdsDbms = true;
                    break;
                default:
                    break;
            }
            if (!checkRdsDbms)
            {
                throw new JsonException($"The value \"{Parameters.Rds.Dbms}\" cannot be set for \"Dbms\" in Rds.json.");
            }
            var checkRdsProvider = false;
            switch (Parameters.Rds.Provider)
            {
                case "Local":
                    checkRdsProvider = true;
                    break;
                default:
                    break;
            }
            if (!checkRdsProvider)
            {
                throw new JsonException($"The value \"{Parameters.Rds.Provider}\" cannot be set for \"Provider\" in Rds.json.");
            }
            if (Parameters.Rds.Dbms != Parameters.Migration.Dbms ||
                Parameters.Rds.Provider != Parameters.Migration.Provider ||
                Parameters.Migration.ServiceName != Parameters.Service.Name)
            {
                return;
            }
            DbConnectionStringBuilder o = new DbConnectionStringBuilder() { ConnectionString = Parameters.Migration.SourceConnectionString };
            DbConnectionStringBuilder n = new DbConnectionStringBuilder() { ConnectionString = Parameters.Rds.OwnerConnectionString };
            var oldInfo = (
                server: o.ContainsKey("server") ? o["server"].ToString() : "",
                dataSource: o.ContainsKey("data source") ? o["data source"].ToString() : "");
            var newInfo = (
                server: n.ContainsKey("server") ? n["server"].ToString() : "",
                dataSource: n.ContainsKey("data source") ? n["data source"].ToString() : "");
            if (oldInfo.server != "" && newInfo.server != "" && oldInfo.server == newInfo.server
                || oldInfo.dataSource != "" && newInfo.dataSource != "" && oldInfo.dataSource == newInfo.dataSource)
            {
                throw new JsonException($"The same schema cannot be specified for both Migration.json and Rds.json.");
            }
        }

        private static void ConvertTime(
            ISqlObjectFactory factory,
            int hourOffset)
        {
            TryOpenConnections(factory);
            Functions.Rds.TimeConverter.Convert(
                factory: factory,
                hourOffset: hourOffset);
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerRdsCompleted"),
                Consoles.Types.Success);
        }

        private static void TrialConfigureDatabase(
            ISqlObjectFactory factory,
            bool noInput = false,
            bool useExColumnsFile = false)
        {
            try
            {
                if (!TryOpenConnections(factory)) return;
                var completed = Functions.Rds.Configurator.TrialConfigure(
                    factory: factory,
                    noInput: noInput,
                    useExColumnsFile: useExColumnsFile);
                if (completed)
                {
                    Consoles.Write(
                        text: DisplayAccessor.Displays.Get("CodeDefinerRdsCompleted"),
                        type: Consoles.Types.Success);
                    CompleteConfigureDatabase = true;
                }
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                Consoles.Write(
                    text: $"[{e.Number}] {e.Message}",
                    type: Consoles.Types.Error,
                    abort: true);
            }
            catch (Exception e)
            {
                Consoles.Write(
                    text: e.ToString(),
                    type: Consoles.Types.Error,
                    abort: true);
            }
        }

        private static void WaitConsole(string[] args)
        {
            System.Threading.Thread.Sleep(10000);
            if (args.Contains("-r"))
            {
                Console.ReadKey();
            }
        }

        private static void WriteErrorToConsole(IEnumerable<string> args)
        {
            Consoles.Write(
                "Incorrect argument. {0}".Params(args.Join(" ")),
                Consoles.Types.Error,
                abort: true);
        }
    }
}
