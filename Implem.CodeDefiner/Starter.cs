using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
namespace Implem.CodeDefiner
{
    internal class Starter
    {
        static void Main(string[] args)
        {
            var argList = args.Select(o => o.Trim());
            ValidateArgs(argList);
            var argHash = new TextData(argList.Skip(1).Join(string.Empty), '/', 1);
            var action = args[0];
            var path = Arg(argHash, 'p', Assembly.GetEntryAssembly().Location);
            var target = Arg(argHash, 't', string.Empty);
            Initializer.Initialize(
                path,
                codeDefiner: true,
                setSaPassword: argHash.ContainsKey("s"),
                setRandomPassword: argHash.ContainsKey("r"));
            Performances.Record(MethodBase.GetCurrentMethod().Name);
            DeleteTemporaryFiles();
            switch (action)
            {
                case "_rds":
                    ConfigureDatabase();
                    break;
                case "rds":
                    ConfigureDatabase();
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
                case "css":
                    CreateCssCode();
                    break;
                case "backup":
                    CreateSolutionBackup();
                    break;
                case "insert_testdata":
                    InsertTestData();
                    break;
                default:
                    WriteErrorToConsole(args);
                    break;
            }
            Performances.Record(MethodBase.GetCurrentMethod().Name);
            Performances.PerformanceCollection.Save(Directories.Logs());
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerCompleted"),
                Consoles.Types.Success);
            WaitConsole(args);
        }

        private static string Arg(TextData argHash, char key, string _default)
        {
            return argHash.ContainsKey(key.ToString())
                ? argHash[key.ToString()]
                : _default;
        }

        private static void DeleteTemporaryFiles()
        {
            Files.DeleteTemporaryFiles(
                Directories.Temp(), Parameters.General.DeleteTempOldThan);
            Files.DeleteTemporaryFiles(
                Directories.Histories(), Parameters.General.DeleteHistoriesOldThan);
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

        private static void ConfigureDatabase()
        {
            TryOpenConnections();
            Performances.Record(MethodBase.GetCurrentMethod().Name);
            Functions.SqlServer.Configurator.Configure();
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerRdsCompleted"),
                Consoles.Types.Success);
            Performances.Record(MethodBase.GetCurrentMethod().Name);
        }

        private static void TryOpenConnections()
        {
            int number;
            string message;
            if (!Sqls.TryOpenConnections(
                out number, out message, Parameters.Rds.SaConnectionString))
            {
                Console.Write("[{0}] {1}", number, message);
                Environment.Exit(0);
            }
        }

        private static void CreateDefinitionAccessorCode()
        {
            Performances.Record(MethodBase.GetCurrentMethod().Name);
            Functions.AspNetMvc.CSharp.DefinitionAccessorCreator.Create();
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerDefCompleted"),
                Consoles.Types.Success);
            Performances.Record(MethodBase.GetCurrentMethod().Name);
        }

        private static void CreateMvcCode(string target)
        {
            Performances.Record(MethodBase.GetCurrentMethod().Name);
            Functions.AspNetMvc.CSharp.MvcCreator.Create(target);
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerMvcCompleted"),
                Consoles.Types.Success);
            Performances.Record(MethodBase.GetCurrentMethod().Name);
        }

        private static void CreateCssCode()
        {
            Performances.Record(MethodBase.GetCurrentMethod().Name);
            Functions.Web.Styles.CssCreator.Create();
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerCssCompleted"),
                Consoles.Types.Success);
            Performances.Record(MethodBase.GetCurrentMethod().Name);
        }

        private static void CreateSolutionBackup()
        {
            Performances.Record(MethodBase.GetCurrentMethod().Name);
            Functions.CodeDefiner.BackupCreater.BackupSolutionFiles();
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerBackupCompleted"),
                Consoles.Types.Success);
            Performances.Record(MethodBase.GetCurrentMethod().Name);
        }

        private static void InsertTestData()
        {
            Performances.Record(MethodBase.GetCurrentMethod().Name);
            Functions.SqlServer.TestData.Insert();
            Consoles.Write(
                DisplayAccessor.Displays.Get("CodeDefinerInsertTestDataCompleted"),
                Consoles.Types.Success);
            Performances.Record(MethodBase.GetCurrentMethod().Name);
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

        [Conditional("DEBUG")]
        private static void TestPerformance(int loopCount, params Action[] actionCollection)
        {
            actionCollection
                .Select((o, i) => new { Count = i + 1, Action = o })
                .ForEach(data =>
                {
                    Performances.Record("Action:" + data.Count);
                    for (int count = 1; count <= loopCount; count++)
                    {
                        data.Action();
                    }
                    Performances.Record("Action:" + data.Count);
                });
        }
    }
}
