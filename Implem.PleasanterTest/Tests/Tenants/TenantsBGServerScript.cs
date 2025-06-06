using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Operations;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Implem.PleasanterTest.Utilities.UserData;

namespace Implem.PleasanterTest.Tests.Tenants
{
    [Collection(nameof(TenantsBGServerScript))]
    public class TenantsBGServerScript
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var tenantsettings = TenantData.Get();
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.TenantsBGServerScript(),
                httpMethod: "POST",
                forms: forms);
            context.SessionData.Add("TenantSettings", tenantsettings);
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var userModel = UserData.Get(UserData.UserTypes.Privileged);
            var validHtmlTests = BaseData.Tests(HtmlData.ExistsOne(selector: "#Editor"));
            var notFoundMessage = BaseData.Tests(HtmlData.NotFoundMessage());
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "NewServerScript")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#ServerScriptDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "AddServerScript"),
                        new KeyValue("ServerScriptBody", "context.Log(\"AddServerScript\");"),
                        new KeyValue("ServerScriptUser", $"{userModel.UserId}"),
                        new KeyValue("ServerScriptTitle", "AddServerScript-xUnit"),
                        new KeyValue("ServerScriptId", "0")),
                    baseTests: BaseData.Tests(
                        JsonData.HtmlTextContains(
                            method: "ReplaceAll",
                            target: "#EditServerScript",
                            selector: "#EditServerScriptWrap",
                            value: "<td>3</td><td>AddServerScript-xUnit</td>"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateServerScript"),
                        new KeyValue("ServerScriptUser", $"{userModel.UserId}"),
                        new KeyValue("ServerScriptTitle", "BGServerScript:Test2-update"),
                        new KeyValue("ServerScriptId", "2"),
                        new KeyValue("EditServerScript", "[\"2\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.HtmlTextContains(
                            method: "Html",
                            target: "#EditServerScript",
                            selector: "#EditServerScriptWrap",
                            value: "<td>2</td><td>BGServerScript:Test2-update</td>"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveUpServerScripts"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("EditServerScript", "[\"2\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.TextCheckOrder(
                            method: "ReplaceAll",
                            target: "#EditServerScript",
                            wordArray: new string[]
                            {
                                "<td>BGServerScript:Test2</td>",
                                "<td>BGServerScript:Test1</td>"
                            }),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveDownServerScripts"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("EditServerScript", "[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.TextCheckOrder(
                            method: "ReplaceAll",
                            target: "#EditServerScript",
                            wordArray: new string[]
                            {
                                "<td>BGServerScript:Test2</td>",
                                "<td>BGServerScript:Test1</td>"
                            }),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "CopyServerScripts"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("EditServerScript", "[\"2\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.TextCountOf(
                            method: "ReplaceAll",
                            target: "#EditServerScript",
                            value: "<td>BGServerScript:Test2</td>",
                            estimate: 2),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DeleteServerScripts"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("EditServerScript", "[\"2\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.TextCountOf(
                            method: "ReplaceAll",
                            target: "#EditServerScript",
                            value: "<td>BGServerScript:Test2</td>",
                            estimate: 0),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditServerScript"),
                        new KeyValue("ServerScriptId", "1")),
                    baseTests: BaseData.Tests(
                        JsonData.HtmlTests(
                            method: "Html",
                            target: "#ServerScriptDialog",
                            selector: "#ServerScriptForm"),
                        JsonData.HtmlTextContains(
                            method: "Html",
                            target: "#ServerScriptDialog",
                            selector: "#ServerScriptForm",
                            value: "<input id=\"ServerScriptTitle\" name=\"ServerScriptTitle\" class=\"control-textbox always-send\" type=\"text\" value=\"BGServerScript:Test1\" data-validate-required=\"1\" autofocus=\"autofocus\">"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                 new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "ExecServerScript"),
                        new KeyValue("ServerScriptUser", $"{userModel.UserId}"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("ServerScriptName", "xUnitTest")),
                    baseTests: BaseData.Tests(
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Text\":\"\\\" xUnitTest \\\" を実行しました。\",\"Css\":\"alert-success\"}"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    forms : testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                forms,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return TenantUtilities.SetBGServerScript(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: context.TenantId);
        }
    }
}
