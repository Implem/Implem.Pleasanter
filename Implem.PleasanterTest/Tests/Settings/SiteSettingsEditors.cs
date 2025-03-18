using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
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
    [Collection(nameof(SiteSettingsEditors))]
    public class SiteSettingsEditors
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
                    title: "課題管理",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","OpenEditorColumnDialog"),
                        new KeyValue("EditorSourceColumnsType","Columns"),
                        new KeyValue("EditorColumns","[\"Status\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditorColumnDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#EditorColumnDialog","#EditorColumnForm")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "課題管理",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","OpenEditorOtherColumnDialog"),
                        new KeyValue("EditorSourceColumnsType","Columns"),
                        new KeyValue("EditorOtherColumn","Creator")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditorColumnDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#EditorColumnDialog","#EditorColumnForm")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "課題管理",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","SetEditorColumn"),
                        new KeyValue("LabelText","状況-A"),
                        new KeyValue("EditorColumnName","Status")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditorColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "課題管理",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","ResetEditorColumn"),
                        new KeyValue("EditorColumnName","Status")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditorColumnDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#EditorColumnDialog","#EditorColumnForm")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "課題管理",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","ToDisableEditorColumns"),
                        new KeyValue("EditorColumns","[\"Title\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "SetValue",
                            target: "#EditorColumnsTabsTarget"),
                        JsonData.ExistsOne(
                            method: "SetData",
                            target: "#EditorSourceColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "課題管理",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","EditorSourceColumnsType"),
                        new KeyValue("EditorSourceColumnsType","Others")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditorSourceColumns"),
                        JsonData.ExistsOne(
                            method: "SetData",
                            target: "#EditorSourceColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "課題管理",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","ToEnableEditorColumns"),
                        new KeyValue("EditorSourceColumnsType","Others"),
                        new KeyValue("EditorSourceColumns","[\"_Section-0\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditorColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "課題管理",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","UpdateLink")),
                    baseTests: BaseData.Tests(
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
