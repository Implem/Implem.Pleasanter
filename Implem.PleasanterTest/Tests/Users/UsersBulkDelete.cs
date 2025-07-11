using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    [Collection(nameof(UsersBulkDelete))]
    public class UsersBulkDelete
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var bulkDeleteTarget1 = Initializer.Users.Values.FirstOrDefault(o => o.Name == "ユーザ24（一括削除用）").UserId;
            var bulkDeleteTarget2 = Initializer.Users.Values.FirstOrDefault(o => o.Name == "ユーザ25（一括削除用）").UserId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersIndex(),
                queryStrings: new QueryStrings()
                    {
                        { "ControlId", "BulkDeleteCommand" },
                        { "GridCheckedItems", $"{bulkDeleteTarget1},{bulkDeleteTarget2}" },
                        { "EditOnGrid", "NewRowId" }
                    });
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
                JsonData.Value(
                    method: "Message",
                    value: "{\"Id\":\"BulkDeleted\",\"Text\":\": 2 件 一括削除しました。\",\"Css\":\"alert-success\"}")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    userType: UserData.UserTypes.TenantManager1,
                    baseTests: baseTests),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return UserUtilities.BulkDelete(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
        }
    }
}
