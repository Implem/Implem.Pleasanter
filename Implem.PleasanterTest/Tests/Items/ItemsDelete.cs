using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsDelete
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            Forms forms,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsDelete(id: id),
                forms: forms);
            var results = Results(context: context);
            Initializer.SaveResults(results);
			Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var baseTests = new List<BaseTest>()
            {
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "back")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "RecordToDeleteSite2",
                    baseTests: baseTests),
                new TestPart(
                    title: "RecordToDeleteSite3",
                    baseTests: baseTests),
                new TestPart(
                    title: "RecordToDeleteSite4",
                    baseTests: baseTests),
                new TestPart(
                    title: "RecordToDeleteSite6",
                    baseTests: baseTests),
                new TestPart(
                    title: "RecordToDeleteSite7",
                    baseTests: baseTests),
                new TestPart(
                    title: "RecordToDeleteSite8",
                    baseTests: baseTests),
                new TestPart(
                    title: "ダッシュボード削除用",
                    userType: UserData.UserTypes.TenantManager1,
                    baseTests: new List<BaseTest>()
                    {
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.ExistsOne(
                            method: "Href")
                    })
            };
            foreach (var testPart in testParts)
            {
                var forms = testPart.Title == "ダッシュボード削除用"
                    ? FormsUtilities.Get(
                        new KeyValue("DeleteSiteTitle", testPart.Title),
                        new KeyValue("Users_LoginId", testPart.UserModel.LoginId),
                        new KeyValue("Users_Password", "ABCDEF"))
                    : null;
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    forms: forms,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            Forms forms,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                userModel,
                forms,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Delete(context: context);
        }
    }
}
