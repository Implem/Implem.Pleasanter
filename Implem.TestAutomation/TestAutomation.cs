using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Implem.Libraries.Utilities;
using Implem.DefinitionAccessor;
using Implem.ParameterAccessor.Parts;
using Implem.Libraries.Classes;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Requests;
using Implem.TestAutomation.Parts;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Edge;
namespace Implem.TestAutomation
{
    internal class TestAutomation
    {
        static void Main(string[] args)
        {
            var (path, executeType) = GetArgParams(args);
            Initializer.Initialize(
                path,
                assemblyVersion: Assembly.GetExecutingAssembly().GetName().Version.ToString());
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            var (testList, checkResult) = CheckPreExecution(executeType);
            if (!checkResult) { return; }
            try
            {
                using (IWebDriver driver = SelectBrowser(Parameters.ExtendedAutoTestSettings.BrowserType))
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(Parameters.ExtendedAutoTestSettings.TimeoutSeconds);
                    TestAutomationOperate.LaunchBrowser(driver, Parameters.ExtendedAutoTestSettings);
                    Thread.Sleep(500);
                    Parameters.ExtendedAutoTestScenarios
                        .Where((autoTestScenario, index) => testList.Contains(index))
                        .ForEach(autoTestScenario =>
                        {
                            TestAutomationOperate.WriteResult(
                                resultString: new AutoTestResult()
                                {
                                    Description = autoTestScenario.CasesDescription
                                },
                                ResultFileName: autoTestScenario.ResultFileName,
                                resultInitial: true);
                            TestAutomationOperate.WriteLog(
                                logFileName: Parameters.ExtendedAutoTestSettings.LogFileName,
                                logMessage: $"Start case:{autoTestScenario.CaseName}",
                                logInitial: true
                                );
                            autoTestScenario.TestCases
                                .SelectMany(testCase => Parameters.ExtendedAutoTestOperations
                                    .Where(testOperateion => testOperateion.TestPartsPath
                                        .EndsWith($"{testCase.TestPartsPath}{Path.DirectorySeparatorChar}{testCase.TestPartsName}")))
                                .ForEach(testOperation =>
                                    TestAutomationExecute.ExecuteAutoTest(
                                        testOperation: testOperation,
                                        driver: driver,
                                        testSettings: Parameters.ExtendedAutoTestSettings,
                                        autoTestScenario: autoTestScenario));
                        });
                    TestAutomationOperate.WriteLog(
                        logFileName: Parameters.ExtendedAutoTestSettings.LogFileName,
                        logMessage: Displays.AutoTestFinished(context: context)
                        );
                }
            }
            catch (NoSuchElementException ex)
            {
                TestAutomationOperate.WriteLog(
                    logFileName: Parameters.ExtendedAutoTestSettings.LogFileName,
                    logMessage: Displays.AutoTestHtmlError(context: context)
                    );
                TestAutomationOperate.WriteLog(
                    logFileName: Parameters.ExtendedAutoTestSettings.LogFileName,
                    logMessage: ex.Message
                    );
                if (!Console.IsInputRedirected && Console.KeyAvailable)
                {
                    Console.ReadKey(intercept: true);
                }
            }
            catch (Exception ex)
            {
                TestAutomationOperate.WriteLog(
                    logFileName: Parameters.ExtendedAutoTestSettings.LogFileName,
                    logMessage: Displays.AutoTestOtherError(context: context)
                    );
                TestAutomationOperate.WriteLog(
                    logFileName: Parameters.ExtendedAutoTestSettings.LogFileName,
                    logMessage: ex.Message
                    );
                if (!Console.IsInputRedirected && Console.KeyAvailable)
                {
                    Console.ReadKey(intercept: true);
                }
            }
        }

        private static (string,string) GetArgParams(string[] args)
        {
            var argList = args.Select(o => o.Trim());
            var argHash = new TextData(argList.Join(string.Empty), '/', 1);
            var path = argHash.Get("p");
            var executeType = argHash.Get("s");
            if (path.IsNullOrEmpty())
            {
                var parts = new DirectoryInfo(Assembly.GetEntryAssembly().Location).FullName.Split(Path.DirectorySeparatorChar);
                path = new DirectoryInfo(
                    Path.Combine(
                        Path.Join(parts.Take(Array.IndexOf(parts, "Implem.TestAutomation")).ToArray()),
                        "Implem.Pleasanter")
                    ).FullName;
            }
            return (path, executeType);
        }

        private static (List<int>, bool) CheckPreExecution(string executeType)
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            var testList = new List<int>();
            if (Parameters.ExtendedAutoTestSettings.Url.IsNullOrEmpty())
            {
                Parameters.ExtendedAutoTestSettings.Url = InputUrl();
            }
            if (CheckTestPartsJson() == false)
            {
                Console.ReadKey(intercept: true);
                return (null, false);
            }
            Console.WriteLine(Displays.AutoTestNumberSelect(context: context));
            Console.WriteLine(Displays.AutoTestTargetPartsMessage(context: context));
            Console.Write(Displays.AutoTestArrow(context: context));
            if (!executeType.Contains("select"))
            {
                Parameters.ExtendedAutoTestScenarios
                    .Select((autoTestScenario, i) => new {Index = i})
                    .ForEach(autoTestScenario =>
                    {
                        testList.Add(autoTestScenario.Index);
                    });
            }
            else
            {
                try
                {
                    testList = Console.ReadLine()
                        .Split(',')
                        .Select(selectNumber => int.Parse(selectNumber) - 1)
                        .ToList();
                    Console.Write(Displays.AutoTestNumber(context: context));
                }
                catch
                {
                    Console.WriteLine(Displays.AutoTestInputErrorHalfNumber(context: context));
                    Console.ReadKey(intercept: true);
                    return (null, false);
                }
            }
            foreach (var list in testList)
            {
                Console.Write($"{list + 1},");
            }
            Console.WriteLine(Displays.AutoTestSelected(context: context));
            Console.WriteLine(Displays.AutoTestConfirmRun(context: context));
            Console.WriteLine(Displays.AutoTestRunning(context: context));
            return (testList, true);
        }

        private static IWebDriver SelectBrowser(BrowserTypes browserType)
        {
            switch (browserType)
            {
                case BrowserTypes.Chrome:
                    var opt = new ChromeOptions();
                    opt.AddArgument("--headless");
                    opt.AddArgument("--disable-gpu");
                    opt.AddArgument("--no-sandbox");
                    opt.AddArgument("--disable-dev-shm-usage");
                    return new ChromeDriver(options: opt);
                case BrowserTypes.IE:
                    return new InternetExplorerDriver();
                case BrowserTypes.Edge:
                    return new EdgeDriver();
                default:
                    return new ChromeDriver();
            }
        }

        private static bool CheckTestPartsJson()
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            var result = true;
            Parameters.ExtendedAutoTestScenarios
                .Select((autoTestScenario, i) => new { Index = i + 1 , Data = autoTestScenario })
                .ForEach(autoTestScenario =>
                {
                    Console.WriteLine(Displays.AutoTestCasesList(
                            context: context,
                            data: new string[]
                            {
                                autoTestScenario.Index.ToString(),
                                autoTestScenario.Data.CaseName,
                                autoTestScenario.Data.CasesDescription
                            }));
                    autoTestScenario.Data.TestCases
                        .ForEach(testCase =>
                        {
                            var testPartsList = new List<string>();
                            testCase.TestPartsPath = testCase.TestPartsPath.Replace('\\', Path.DirectorySeparatorChar);
                            Parameters.ExtendedAutoTestOperations
                                .ForEach(testParts =>
                                {
                                    var s = testParts.TestPartsPath.Replace('\\', Path.DirectorySeparatorChar);
                                    s = s.Replace("/TestParts", "");
                                    testPartsList.Add(s);
                                });
                            string partCheck = null;
                            if ((!testCase.TestPartsName.IsNullOrEmpty()) &&
                                (!testPartsList.Contains($"{testCase.TestPartsPath}{Path.DirectorySeparatorChar}{testCase.TestPartsName}")))
                            {
                                if (result)
                                {
                                    result = false;
                                }
                                partCheck = Displays.AutoTestResultNg(context: context);
                            }
                            string partMessage = !testCase.TestPartsName.IsNullOrEmpty()
                                ? $"{Path.DirectorySeparatorChar}{testCase.TestPartsName}"
                                : $"{Path.DirectorySeparatorChar}*";


                            Console.WriteLine(Displays.AutoTestPartsList(
                                    context: context,
                                    data: new string[]
                                    {
                                        testCase.TestPartsPath,
                                        partMessage,
                                        partCheck
                                    }));
                        });
                });
            if(!result)
            {
                Console.WriteLine(Displays.AutoTestFileDescribed(context: context));
            }
            return result;
        }

        private static string InputUrl()
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            Console.WriteLine(Displays.AutoTestNoUrl(context: context));
            Console.Write(Displays.AutoTestArrow(context: context));
            var strUrl = Console.ReadLine();
            if (strUrl.IsNullOrEmpty())
            {
                InputUrl();
            }
            return strUrl;
        }
    }
}
