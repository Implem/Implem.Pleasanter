using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsServerScripts))]
    public class SiteSettingsServerScripts
    {

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            List<BaseTest> baseTests, 
            UserModel userModel)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsSetSiteSettings(id: siteId),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context, siteId: siteId);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "サイト設定 - ServerScripts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveUpServerScripts"),
                        new KeyValue("EditServerScript", "[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - ServerScripts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveDownServerScripts"),
                        new KeyValue("EditServerScript", "[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - ServerScripts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "NewServerScript"),
                        new KeyValue("EditServerScript", "[]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#ServerScriptDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - ServerScripts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditServerScript"),
                        new KeyValue("ServerScriptId", "1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#ServerScriptDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - ServerScripts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "AddServerScript"),
                        new KeyValue("ServerScriptTitle", "TESTServerScript3"),
                        new KeyValue("ServerScriptName", ""),
                        new KeyValue("ServerScriptBody", "context.Log('このサーバスクリプトIDは3です。')"),
                        new KeyValue("ServerScriptBeforeOpeningPage", "true"),
                        new KeyValue("ServerScriptDisabled", "false"),
                        new KeyValue("ServerScriptFunctionalize", "false"),
                        new KeyValue("ServerScriptTryCatch", "false"),
                        new KeyValue("ServerScriptWhenloadingSiteSettings", "false"),
                        new KeyValue("ServerScriptWhenViewProcessing", "false"),
                        new KeyValue("ServerScriptWhenloadingRecord", "false"),
                        new KeyValue("ServerScriptBeforeFormula", "false"),
                        new KeyValue("ServerScriptAfterFormula", "false"),
                        new KeyValue("ServerScriptBeforeCreate", "false"),
                        new KeyValue("ServerScriptAfterCreate", "false"),
                        new KeyValue("ServerScriptBeforeUpdate", "false"),
                        new KeyValue("ServerScriptAfterUpdate", "false"),
                        new KeyValue("ServerScriptBeforeDelete", "false"),
                        new KeyValue("ServerScriptAfterDelete", "false"),
                        new KeyValue("ServerScriptBeforeOpeningRow", "false"),
                        new KeyValue("ServerScriptShared", "false")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - ServerScripts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateServerScript"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("ServerScriptName", ""),
                        new KeyValue("ServerScriptBody", "context.Log('このサーバスクリプトIDは1です。')"),
                        new KeyValue("ServerScriptBeforeOpeningPage", "true"),
                        new KeyValue("ServerScriptDisabled", "false"),
                        new KeyValue("ServerScriptFunctionalize", "false"),
                        new KeyValue("ServerScriptTryCatch", "false"),
                        new KeyValue("ServerScriptWhenloadingSiteSettings", "false"),
                        new KeyValue("ServerScriptWhenViewProcessing", "false"),
                        new KeyValue("ServerScriptWhenloadingRecord", "false"),
                        new KeyValue("ServerScriptBeforeFormula", "false"),
                        new KeyValue("ServerScriptAfterFormula", "false"),
                        new KeyValue("ServerScriptBeforeCreate", "false"),
                        new KeyValue("ServerScriptAfterCreate", "false"),
                        new KeyValue("ServerScriptBeforeUpdate", "false"),
                        new KeyValue("ServerScriptAfterUpdate", "false"),
                        new KeyValue("ServerScriptBeforeDelete", "false"),
                        new KeyValue("ServerScriptAfterDelete", "false"),
                        new KeyValue("ServerScriptBeforeOpeningRow", "false"),
                        new KeyValue("ServerScriptShared", "false")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - ServerScripts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "CopyServerScripts"),
                        new KeyValue("EditServerScript", "[\"1\"]"),
                        new KeyValue("ServerScriptId", "1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - ServerScripts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DeleteServerScripts"),
                        new KeyValue("EditServerScript", "[\"1\"]"),
                        new KeyValue("ServerScriptId", "1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    baseTests: testPart.BaseTests,
                    userModel: testPart.UserModel);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel)
        {
            return new object[]
            {
                title,
                forms,
                baseTests,
                userModel
            };
        }

        private static string Results(Context context, long siteId)
        {
            return SiteUtilities.SetSiteSettings(
                context: context,
                siteId: siteId);
        }
    }
}
