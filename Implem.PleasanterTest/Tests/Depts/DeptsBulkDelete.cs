using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Depts
{
    [Collection(nameof(DeptsBulkDelete))]
    public class DeptsBulkDelete
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            QueryStrings queryStrings,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Depts.GetDeptIdByName(title);
            var context = ContextData.Get(
                queryStrings: queryStrings,
                userId: userModel.UserId,
                routeData: RouteData.DeptsBulkDelete(id: id));
            var results = Results(context: context);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var title = "組織12（一括削除用）";
            var deptId = Initializer.Depts.GetDeptIdByName(title);
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: title,
                    queryStrings: QueryStringsUtilities.Get(
                        new KeyValue("EditOnGrid", "Shift-JIS"),
                        new KeyValue("NewRowId", "undefined"),
                        new KeyValue("ControlId", "BulkDeleteCommand")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(method:"Log"),
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Id\":\"SelectTargets\",\"Text\":\"対象を選択してください。\",\"Css\":\"alert-error\"}")),
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: title,
                    queryStrings: QueryStringsUtilities.Get(
                        new KeyValue("EditOnGrid", "Shift-JIS"),
                        new KeyValue("NewRowId", "undefined"),
                        new KeyValue("CcontrolId", "BulkDeleteCommand"),
                        new KeyValue("GridCheckedItems", $"{deptId}")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(method:"Log"),
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Id\":\"BulkDeleted\",\"Text\":\": 1 件 一括削除しました。\",\"Css\":\"alert-success\"}")),
                    userType: UserData.UserTypes.TenantManager1),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    queryStrings: testPart.QueryStrings,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            QueryStrings queryStrings,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                queryStrings,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return DeptUtilities.BulkDelete(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context));
        }
    }
}