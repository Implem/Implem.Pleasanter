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
    [Collection(nameof(SiteSettingsStyles))]
    public class SiteSettingsStyles
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
                    title: "サイト設定 - Styles",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveUpStyles"),
                        new KeyValue("EditStyle", "[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditStyleWrap"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - Styles",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveDownStyles"),
                        new KeyValue("EditStyle", "[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditStyleWrap"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - Styles",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "NewStyle"),
                        new KeyValue("EditStyle", "[]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#StyleDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - Styles",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditStyle"),
                        new KeyValue("StyleId", "1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#StyleDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - Styles",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "AddStyle"),
                        new KeyValue("EditStyle", "[]"),
                        new KeyValue("StyleTitle", "TESTSTYLE3"),
                        new KeyValue("StyleBody", ".field-test {\n      background-color: blue;\n}")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditStyleWrap"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - Styles",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateStyle"),
                        new KeyValue("EditStyle", "[]"),
                        new KeyValue("StyleId", "1"),
                        new KeyValue("StyleBody", ".field-test {\n      background-color: red;\n}")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditStyleWrap"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - Styles",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "CopyStyles"),
                        new KeyValue("EditStyle", "[\"1\"]"),
                        new KeyValue("StyleId", "1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditStyleWrap"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - Styles",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DeleteStyles"),
                        new KeyValue("EditStyle", "[\"1\"]"),
                        new KeyValue("StyleId", "1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditStyleWrap"),
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
