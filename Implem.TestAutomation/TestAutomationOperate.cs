using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CsvHelper;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.TestAutomation.Parts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WDSE;
using WDSE.Decorators;
using WDSE.ScreenshotMaker;
using OpenQA.Selenium.Remote;

namespace Implem.TestAutomation
{
    public static class TestAutomationOperate
    {

        public static void LaunchBrowser(IWebDriver driver,
            AutoTestSettings testSettings)
        {
            driver.Manage().Window.Size
                = new Size(testSettings.Width, testSettings.Height);
            driver.Url = testSettings.Url;
        }

        public static void InputOpe(IWebDriver driver,
            TestPart testPart)
        {
            IJavaScriptExecutor jsDriver = driver as IJavaScriptExecutor;
            jsDriver.ExecuteScript($"$p.disableAutPostback = true;");
            ClearOpe(driver, testPart);
            driver.FindElement(SelectItem(testPart: testPart))
                .SendKeys(testPart.Value);
        }

        public static void InputsOpe(IWebDriver driver,
            TestPart testPart)
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            IJavaScriptExecutor jsDriver = driver as IJavaScriptExecutor;
            jsDriver.ExecuteScript($"$p.disableAutPostback = true;");
            testPart.Inputs.ForEach(testInput =>
            {
                if (!testInput.InputWaitTime.IsNullOrEmpty())
                {
                    Thread.Sleep(testInput.InputWaitTime.ToInt());
                }
                if (testInput.InputTarget.StartsWith("Check"))
                {
                    jsDriver.ExecuteScript($"$p.set($p.getControl('{testInput.InputTarget}').prop('checked',{testInput.InputValue}));");
                }
                else
                {
                    var script = $@"
                        var $element;
                        var target = '{testInput.InputTarget}';
                        if (target.indexOf('#') === -1) {{
                            $element = $p.getControl('{testInput.InputTarget}');
                        }}
                        if ($element === undefined) {{
                            $element = $(target);
                        }}
                        var value = '{testInput.InputValue}';
                        if ($element.prop('tagName') === 'SELECT') {{
                            $element.find('option').each(function(index) {{
	                            if ($(this).text() === value) {{
                                    value = $(this).val();
                                }}
                            }});
                        }}
                        $p.set($element, value);";
                    jsDriver.ExecuteScript(script);
                }
                Console.WriteLine(Displays.AutoTestEntered(
                    context: context,
                    data: new string[]
                    {
                            testInput.InputTarget,
                            testInput.InputValue
                    }));
            });
        }

        public static void ClearOpe(IWebDriver driver,
            TestPart testPart)
        {
            driver.FindElement(SelectItem(testPart)).Clear();
        }

        public static void ClickOpe(IWebDriver driver,
            TestPart testPart)
        {
            driver.FindElement(SelectItem(testPart: testPart)).Click();
        }

        public static void HoverOpe(IWebDriver driver,
            TestPart testPart)
        {
            IWebElement mouseTarget = driver.FindElement(SelectItem(testPart: testPart));
            var builder = new OpenQA.Selenium.Interactions.Actions(driver);
            builder.MoveToElement(mouseTarget).Perform();
        }

        public static void GoToUrl(IWebDriver driver,
            TestPart testPart)
        {
            var url = string.Empty;
            if (!testPart.Url.IsNullOrEmpty())
            {
                url = testPart.Url.StartsWith("http")
                    ? testPart.Url
                    : $"{Parameters.ExtendedAutoTestSettings.Url}{testPart.Url}";
            }
            else if (!testPart.TestPartId.IsNullOrEmpty())
            {
                var recordId = testPart.RecordId();
                url = $"{Parameters.ExtendedAutoTestSettings.Url}items/{recordId}";
            }
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(500);
        }

        public static void CreateOpe(IWebDriver driver,
            TestPart testPart)
        {
            driver.FindElement(By.XPath("//button[@data-action='Create']")).Click();
            WaitingAlertSuccess(driver);
            IJavaScriptExecutor jsDriver = driver as IJavaScriptExecutor;
            testPart.CreatedTabeId = jsDriver.ExecuteScript("return $p.id();").ToString();
        }

        public static void UpdateOpe(IWebDriver driver)
        {
            driver.FindElement(By.XPath("//button[@data-action='Update']")).Click();
            WaitingAlertSuccess(driver);
        }

        public static void DeleteOpe(IWebDriver driver)
        {
            driver.FindElement(By.XPath("//button[@data-action='Delete']")).Click();
        }

        public static void CopyOpe(IWebDriver driver)
        {
            driver.FindElement(By.XPath("//button[@data-selector='#CopyDialog']")).Click();
            driver.FindElement(By.XPath("//button[@data-action='Copy']")).Click();
        }

        public static void SelectOpe(IWebDriver driver,
            TestPart testPart)
        {
            var selectElement = new SelectElement(driver.FindElement(SelectItem(testPart)));
            selectElement.SelectByText(testPart.Value);
        }

        public static void AlertAccept(IWebDriver driver)
        {
            driver.SwitchTo().Alert().Accept();
        }

        public static void AlertDismiss(IWebDriver driver)
        {
            driver.SwitchTo().Alert().Dismiss();
        }

        private static By SelectItem(TestPart testPart)
        {
            if (!testPart.ElementId.IsNullOrEmpty())
            {
                return By.Id(testPart.ElementId);
            }
            else if (!testPart.ElementXpath.IsNullOrEmpty())
            {
                return By.XPath(testPart.ElementXpath);
            }
            else if (!testPart.ElementCss.IsNullOrEmpty())
            {
                return By.CssSelector(testPart.ElementCss);
            }
            else if (!testPart.TestPartId.IsNullOrEmpty())
            {
                var recordId = testPart.RecordId();
                return By.XPath($"//tr[@data-id='{recordId}']");
            }
            else
            {
                return By.PartialLinkText(testPart.ElementLinkText);
            }
        }

        private static By SelectItem(ResultCheck resultCheck)
        {
            if (!resultCheck.ElementId.IsNullOrEmpty())
            {
                return By.Id(resultCheck.ElementId);
            }
            else if (!resultCheck.ElementXpath.IsNullOrEmpty())
            {
                return By.XPath(resultCheck.ElementXpath);
            }
            else if (!resultCheck.ElementCss.IsNullOrEmpty())
            {
                return By.CssSelector(resultCheck.ElementCss);
            }
            else
            {
                return By.PartialLinkText(resultCheck.ElementLinkText);
            }
        }

        public static void CheckResult(
            IWebDriver driver,
            ResultCheck resultCheck,
            string resultFileName)
        {
            var executionValue = GetExecutionValue(driver, resultCheck);
            WriteResult(
                resultCheck,
                resultFileName,
                ComparisonValue(driver, resultCheck, executionValue),
                executionValue);
        }

        private static string ComparisonValue(
            IWebDriver driver,
            ResultCheck resultCheck,
            string executionValue)
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            if (resultCheck.CheckType.Equals(CheckTypes.Regex))
            {
                if (Regex.IsMatch(executionValue, resultCheck.ExpectedValue))

                {
                    return Displays.AutoTestResultOk(context: context);
                }
                else
                {
                    return Displays.AutoTestResultNg(context: context);
                }
            }
            else if (resultCheck.CheckType.Equals(CheckTypes.Existance))
            {
                executionValue = executionValue == null ? "ExistanceFalse" : "ExistanceTrue";
                if (executionValue.Equals(resultCheck.ExpectedValue))
                {
                    return Displays.AutoTestResultOk(context: context);
                }
                else
                {
                    return Displays.AutoTestResultNg(context: context);
                }
            }
            else if (resultCheck.CheckType.Equals(CheckTypes.HasClass))
            {
                if (HasClass(driver, resultCheck))
                {
                    return Displays.AutoTestResultOk(context: context);
                }
                else
                {
                    return Displays.AutoTestResultNg(context: context);
                }
            }
            else if (resultCheck.CheckType.Equals(CheckTypes.HasNotClass))
            {
                if (!HasClass(driver, resultCheck))
                {
                    return Displays.AutoTestResultOk(context: context);
                }
                else
                {
                    return Displays.AutoTestResultNg(context: context);
                }
            }
            else if (resultCheck.CheckType.Equals(CheckTypes.ReadOnly))
            {
                executionValue = ReadOnly(driver, resultCheck);
                if (executionValue.Equals(resultCheck.ExpectedValue))
                {
                    return Displays.AutoTestResultOk(context: context);
                }
                else
                {
                    return Displays.AutoTestResultNg(context: context);
                }
            }
            else if (resultCheck.CheckType.Equals(CheckTypes.SelectOptions))
            {
                if (SelectOptions(driver, resultCheck, executionValue))
                {
                    return Displays.AutoTestResultOk(context: context);
                }
                else
                {
                    return Displays.AutoTestResultNg(context: context);
                }
            }
            else
            {
                if (executionValue == resultCheck.ExpectedValue)

                {
                    return Displays.AutoTestResultOk(context: context);
                }
                else
                {
                    return Displays.AutoTestResultNg(context: context);
                }
            }
        }

        private static string GetExecutionValue(IWebDriver driver
            ,ResultCheck resultCheck)
        {
            var executionValue = (string)null;
            if (!resultCheck.ItemId.IsNullOrEmpty())
            {
                IJavaScriptExecutor jsDriver = driver as IJavaScriptExecutor;
                if (!Exists(driver, resultCheck.ItemId))
                {
                    return executionValue;
                }
                if (resultCheck.ItemId.StartsWith("Check"))
                {
                    executionValue = jsDriver.ExecuteScript($"return $p.getControl('{resultCheck.ItemId}').prop('checked');")
                        .ToString();
                }
                else
                {
                    executionValue = jsDriver.ExecuteScript($"return $p.getControl('{resultCheck.ItemId}').val();")
                        .ToString();
                    if (executionValue.IsNullOrEmpty() &&
                        !jsDriver.ExecuteScript($"return $p.getControl('{resultCheck.ItemId}').prop('tagName');")
                            .ToString()
                            .Equals("SELECT"))
                    {
                        executionValue = jsDriver.ExecuteScript($"return $p.getControl('{resultCheck.ItemId}').text();")
                            .ToString();
                    }
                }
            }
            else
            {
                if (driver.FindElement(SelectItem(resultCheck)).TagName.Equals("select"))
                {
                    var selectElement = new SelectElement(driver.FindElement(SelectItem(resultCheck)));

                    if (resultCheck.CheckType.Equals(CheckTypes.SelectOptions))
                    {
                        executionValue = string.Join(",",
                            selectElement.Options
                            .Where(o => !o.Text.Equals(" "))
                            .Select(o => o.Text).ToList());
                    }
                    else
                    {
                        executionValue = selectElement.SelectedOption.Text;
                    }
                }
                else
                {
                    executionValue = driver.FindElement(SelectItem(resultCheck)).Text;
                    if (executionValue.IsNullOrEmpty())
                    {
                        executionValue = driver.FindElement(SelectItem(resultCheck)).GetAttribute("value");
                    }
                }
            }
            return executionValue;
        }

        private static bool Exists(IWebDriver driver, string resultItemId)
        {
            IJavaScriptExecutor jsDriver = driver as IJavaScriptExecutor;
            return jsDriver.ExecuteScript($@"
                if ($p.getControl('{resultItemId}')) {{
                    return $p.getControl('{resultItemId}').length === 1;
                }} else {{
                    return false;
                }}").ToBool();
        }

        private static string ReadOnly(IWebDriver driver, ResultCheck resultCheck)
        {
            IJavaScriptExecutor jsDriver = driver as IJavaScriptExecutor;
            return jsDriver.ExecuteScript($@"
                if ($p.getControl('{resultCheck.ItemId}').attr('data-readonly')) {{
                    return 'ReadOnlyTrue';
                }} else {{
                    return 'ReadOnlyFalse';
                }}").ToString();
        }

        private static bool HasClass(IWebDriver driver, ResultCheck resultCheck)
        {
            IJavaScriptExecutor jsDriver = driver as IJavaScriptExecutor;
            if (!resultCheck.ItemId.IsNullOrEmpty())
            {
                return jsDriver.ExecuteScript($@"
                if ($p.getControl('{resultCheck.ItemId}')) {{
                    return $p.getControl('{resultCheck.ItemId}').hasClass('{resultCheck.ExpectedValue}');
                }} else {{
                    return false;
                }}").ToBool();
            }
            else
            {
                return jsDriver.ExecuteScript($@"
                if ($('{resultCheck.ElementCss}')) {{
                    return $('{resultCheck.ElementCss}').hasClass('{resultCheck.ExpectedValue}');
                }} else {{
                    return false;
                }}").ToBool();
            }
        }

        private static bool SelectOptions(IWebDriver driver, ResultCheck resultCheck, string executionValue)
        {
            var executionValueList = executionValue
                .Split(',')
                .ToList();
            var expectedList = resultCheck.ExpectedValue
                .Split(',')
                .ToList();
            executionValueList.Sort();
            expectedList.Sort();
            return expectedList.SequenceEqual(executionValueList);
        }

        private static void WriteResult(ResultCheck resultCheck
            ,string resultFileName
            ,string comparisonResult
            ,string executionValue)
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            var resultStr = new AutoTestResult();
            resultStr.ExpectedContent = resultCheck.Description;
            resultStr.ConfirmationTarget = SetConfirmationTaget(resultCheck);
            resultStr.ExpectedValue = resultCheck.ExpectedValue;
            resultStr.ExecutionValue = executionValue;
            resultStr.ComparisonResult = comparisonResult;
            WriteResult(resultString: resultStr, ResultFileName: resultFileName);
            if (resultStr.ComparisonResult.Equals("OK")){
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine(Displays.AutoTestResultMessage(
                context: context,
                data: new string[]
                {
                    resultStr.ComparisonResult,
                    resultStr.ExpectedContent,
                    resultStr.ConfirmationTarget,
                    resultStr.ExpectedValue,
                    resultStr.ExecutionValue
                }));
            Console.ResetColor();
        }

        private static string SetConfirmationTaget(ResultCheck resultCheck)
        {

            if (!resultCheck.ItemId.IsNullOrEmpty())
            {
                return resultCheck.ItemId;
            }
            else if (!resultCheck.ElementId.IsNullOrEmpty())
            {
                return resultCheck.ElementId;
            }
            else if (!resultCheck.ElementLinkText.IsNullOrEmpty())
            {
                return resultCheck.ElementLinkText;
            }
            else
            {
                return resultCheck.ElementXpath;
            }
        }

        public static void WriteResult(string ResultFileName
            , AutoTestResult resultString = null
            , Boolean resultInitial = false)
        {
            var parts = new DirectoryInfo(Assembly.GetEntryAssembly().Location).FullName.Split('\\');
            var path = new DirectoryInfo(Path.Combine(
                parts.Take(Array.IndexOf(parts, "implem.TestAutomation") + 1).Join("\\"),
                "Log",
                Parameters.ExtendedAutoTestSettings.ResultsPath)).FullName;
            if (!new DirectoryInfo(path).Exists) Directory.CreateDirectory(path);
            var file = Path.Combine(path, ResultFileName);
            if (resultInitial == true)
            {
                WriteResultCsvHeader(file);
            }
            WriteResultCsvData(resultString, file);
        }

        private static void WriteResultCsvHeader(string CsvFilePath)
        {
            using (var streamWriter = new StreamWriter(CsvFilePath, false, Encoding.GetEncoding("shift_jis")))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.Configuration.HasHeaderRecord = true;
                csvWriter.Configuration.ShouldQuote = (s, context) => true;
                csvWriter.Configuration.RegisterClassMap<AutoTestResultTable>();
                csvWriter.WriteHeader<AutoTestResult>();
                csvWriter.NextRecord();
            }
        }

        private static void WriteResultCsvData(AutoTestResult resultString, string CsvFilePath)
        {
            using (var streamWriter = new StreamWriter(CsvFilePath, true, Encoding.GetEncoding("shift_jis")))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.Configuration.HasHeaderRecord = true;
                csvWriter.Configuration.ShouldQuote = (s, context) => true;
                csvWriter.Configuration.RegisterClassMap<AutoTestResultTable>();
                csvWriter.WriteRecord(resultString);
                csvWriter.NextRecord();
            }
        }

        public static void GetScreenShot(IWebDriver driver, string ActionName = null)
        {
            if (!AlertPresent(driver))
            {
                var parts = new DirectoryInfo(Assembly.GetEntryAssembly().Location).FullName.Split('\\');
                var path = new DirectoryInfo(Path.Combine(
                    parts.Take(Array.IndexOf(parts, "implem.TestAutomation") + 1).Join("\\"),
                    "Log",
                    Parameters.ExtendedAutoTestSettings.ScreenShotPath)).FullName;
                if (!new DirectoryInfo(path).Exists) Directory.CreateDirectory(path);
                var result = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string str = null;
                if (!ActionName.IsNullOrEmpty()) { str = $"_{ActionName}"; }
                var file = Path.Combine(path, result + str + ".jpg");
                VisibleItem(driver, "MainCommandsContainer");
                VisibleItem(driver, "Footer");
                VisibleItem(driver, "Message");
                VerticalCombineDecorator vcd = new VerticalCombineDecorator(new ScreenshotMaker());
                driver.TakeScreenshot(vcd).ToMagickImage().ToBitmap().Save(file);
                VisibleItem(driver, "MainCommandsContainer");
                VisibleItem(driver, "Footer");
                VisibleItem(driver, "Message");
            }
        }

        public static void VisibleItem(IWebDriver driver, string itemID)
        {
            try
            {
                IJavaScriptExecutor jsDriver = driver as IJavaScriptExecutor;
                var paramVisible = jsDriver.ExecuteScript($"return document.getElementById(\"{itemID}\").style.visibility;").ToString();
                string action;
                if (paramVisible == "hidden")
                {
                    action = "visible";
                }
                else
                {
                    action = "hidden";
                }
                jsDriver.ExecuteScript($"document.getElementById(\"{itemID}\").style.visibility=\"{action}\";");
            }
            catch { }
        }

        public static void WaitingAlertSuccess(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            IWebElement firstResult = wait.Until(e => e.FindElement(By.ClassName("alert-success")));
        }

        private static bool AlertPresent(IWebDriver driver)
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        public static void UploadFileOpe(IWebDriver driver, TestPart testPart)
        {
            var allowsDetection = driver as IAllowsFileDetection;
            if(allowsDetection != null)
            {
                allowsDetection.FileDetector = new LocalFileDetector();
                var filePath = Path.GetFullPath(testPart.Value) == testPart.Value
                    ? testPart.Value
                    : Path.Combine(
                        Environments.CurrentDirectoryPath,
                        "App_Data\\Parameters\\ExtendedAutoTest\\TestFiles",
                        testPart.Value);
                driver.FindElement(SelectItem(testPart)).SendKeys(filePath);
            }
            Thread.Sleep(testPart.WaitTime ?? 1000);
        }

        public static void Execute(IWebDriver driver, TestPart testPart)
        {
            IJavaScriptExecutor jsDriver = driver as IJavaScriptExecutor;
            jsDriver.ExecuteScript(testPart.Value);
        }
    }
}
