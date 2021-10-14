using System;
using System.Threading;
using OpenQA.Selenium;
using Implem.ParameterAccessor.Parts;
using Implem.DefinitionAccessor;
namespace Implem.TestAutomation
{
    public static class TestAutomationExecute
    {
        public static void ExecuteAutoTest(
            AutoTestOperation testOperation,
            IWebDriver driver,
            AutoTestSettings testSettings,
            AutoTestScenario autoTestScenario = null)
        {
            TestAutomationOperate.WriteLog(
                logFileName: Parameters.ExtendedAutoTestSettings.LogFileName,
                logMessage: $"Parts:{testOperation.TestPartsPath}"
                );
            testOperation.TestParts
                .ForEach(testPart =>
                {
                    TestAutomationOperate.WriteLog(
                        logFileName: Parameters.ExtendedAutoTestSettings.LogFileName,
                        logMessage: $"Action:{testPart.Action}"
                        );

                    switch (testPart.Action)
                    {
                        case ActionTypes.Input:
                            TestAutomationOperate.InputOpe(driver, testPart);
                            break;
                        case ActionTypes.Inputs:
                            TestAutomationOperate.InputsOpe(driver, testPart);
                            break;
                        case ActionTypes.Clear:
                            TestAutomationOperate.ClearOpe(driver, testPart);
                            break;
                        case ActionTypes.Click:
                            TestAutomationOperate.ClickOpe(driver, testPart);
                            break;
                        case ActionTypes.Hover:
                            TestAutomationOperate.HoverOpe(driver, testPart);
                            break;
                        case ActionTypes.Create:
                            TestAutomationOperate.CreateOpe(driver, testPart);
                            break;
                        case ActionTypes.Update:
                            TestAutomationOperate.UpdateOpe(driver);
                            break;
                        case ActionTypes.Delete:
                            TestAutomationOperate.DeleteOpe(driver);
                            break;
                        case ActionTypes.Copy:
                            TestAutomationOperate.CopyOpe(driver);
                            break;
                        case ActionTypes.Select:
                            TestAutomationOperate.SelectOpe(driver, testPart);
                            break;
                        case ActionTypes.AlertAccept:
                            TestAutomationOperate.AlertAccept(driver);
                            break;
                        case (ActionTypes.AlertDismiss):
                            TestAutomationOperate.AlertDismiss(driver);
                            break;
                        case (ActionTypes.GoToUrl):
                            TestAutomationOperate.GoToUrl(driver, testPart);
                            break;
                        case ActionTypes.UploadFile:
                            TestAutomationOperate.UploadFileOpe(driver, testPart);
                            break;
                        case ActionTypes.WaitingAlertSuccess:
                            TestAutomationOperate.WaitingAlertSuccess(driver);
                            break;
                        case ActionTypes.WaitingAlertError:
                            TestAutomationOperate.WaitingAlertError(driver);
                            break;
                    }
                    Thread.Sleep(testPart.WaitTime ?? 500);

                    //スクリーンショットとるメソッドが動かないので調査
                    if (testSettings.ScreenShot)
                    {
                        TestAutomationOperate.GetScreenShot(
                            driver: driver,
                            ActionName: testPart.Action.ToString());
                    }
                    if (testPart.Results?.Count > 0)
                    {
                        testPart.Results.ForEach(resultCheck =>
                        {
                            TestAutomationOperate.CheckResult(
                                driver,
                                resultCheck,
                                autoTestScenario.ResultFileName);
                        });
                    }
                });
        }
    }
}
