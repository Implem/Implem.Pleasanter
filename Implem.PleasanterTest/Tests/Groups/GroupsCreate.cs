using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Groups
{
    public class GroupsCreate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.GroupsCreate());
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
                    method: "Response",
                    target: "id"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(method: "Href")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    userType: UserData.UserTypes.General5,
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
            return GroupUtilities.Create(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
        }
    }
}
