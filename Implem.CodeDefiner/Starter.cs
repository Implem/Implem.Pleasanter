using Implem.DefinitionAccessor;
using Implem.Factory;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Exceptions;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
namespace Implem.CodeDefiner
{
    public class Starter
    {
        private static ISqlObjectFactory factory;

        public static void Main(string[] args)
        {
            Directory.CreateDirectory("logs");
            var logName = Path.Combine("logs", $"Implem.CodeDefiner_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.log");
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
            var argList = args.Select(o => o.Trim());
            ValidateArgs(argList);
            var argHash = new TextData(argList.Skip(1).Join(string.Empty), '/', 1);
            var action = args[0];
            var path = argHash.Get("p")?.Replace('\\', System.IO.Path.DirectorySeparatorChar);
            var target = argHash.Get("t");
            try
            {
                Initializer.Initialize(
                    path,
                    assemblyVersion: Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    codeDefiner: true,
                    setSaPassword: argHash.ContainsKey("s"),
                    setRandomPassword: argHash.ContainsKey("r"));
                factory = RdsFactory.Create(Parameters.Rds.Dbms);
                switch (action)
                {
                    case "_rds":
                        ConfigureDatabase(factory: factory);
                        break;
                    case "rds":
                        ConfigureDatabase(factory: factory);
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
                        ConfigureDatabase(factory: factory);
                        MigrateDatabase();
                        break;
                    case "ConvertTime":
                        ConvertTime(factory: factory);
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
            catch (Exception e)
            {
                Consoles.Write(
                    "UnhandledException : " + e.Message + "\n" + e.StackTrace,
                    Consoles.Types.Error);
            }
            WaitConsole(args);
        }

        private static void ValidateArgs(IEnumerable<string> argList)
        {
            if (argList.Count() == 0)
            {
                WriteErrorToConsole(argList);
            }
            var argNames = argList.Skip(1)
                .Where(o => o.Length >= 2)
                .Select(o => o.Substring(0, 2));
            if (argNames.Count() != argNames.Distinct().Count())
            {
                WriteErrorToConsole(argList);
            }
        }

        private static void ConfigureDatabase(ISqlObjectFactory factory)
        {
            TryOpenConnections(factory);
            Functions.Rds.Configurator.Configure(factory);
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerRdsCompleted"),
                Consoles.Types.Success);
        }

        private static void TryOpenConnections(ISqlObjectFactory factory)
        {
            int number;
            string message;
            if (!Sqls.TryOpenConnections(
                factory,
                out number, out message, Parameters.Rds.SaConnectionString))
            {
                Consoles.Write($"[{number}] {message}",Consoles.Types.Error, true);
            }
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

        private static void MigrateDatabase()
        {
            Functions.AspNetMvc.CSharp.Migrator.MigrateDatabaseAsync();
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerMigrationCompleted"),
                Consoles.Types.Success);
        }

        private static void ConvertTime(ISqlObjectFactory factory)
        {
            TryOpenConnections(factory);
            Functions.Rds.TimeConverter.Convert(factory: factory);
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerRdsCompleted"),
                Consoles.Types.Success);
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
