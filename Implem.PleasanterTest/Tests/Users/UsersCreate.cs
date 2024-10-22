using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    [Collection(nameof(UsersCreate))]
    public class UsersCreate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersCreate(),
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
                    userType: UserData.UserTypes.TenantManager2,
                    baseTests: baseTests),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    forms: FormsUtilities.Get(
                        new KeyValue("Users_LoginId", Strings.NewGuid()),
                        new KeyValue("Users_Password", "password")),
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
            return UserUtilities.Create(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
        }
    }
}
