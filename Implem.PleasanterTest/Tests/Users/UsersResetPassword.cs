using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    [Collection(nameof(UsersResetPassword))]
    public class UsersResetPassword
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Users.Values.FirstOrDefault(o => o.Name == title).UserId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersResetPassword(id: id),
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
                    method: "Html",
                    target: "#RecordInfo"),
                JsonData.ExistsOne(method: "CloseDialog"),
                JsonData.ExistsOne(method: "Message"),
                JsonData.ExistsOne(method: "ClearFormData")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "小林 佳子",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "ResetPassword"),
                        new KeyValue("Users_AfterResetPassword", "ChangedPassword")),
                    baseTests: baseTests,
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                forms,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return UserUtilities.ResetPassword(
                context: context,
                userId: context.Id.ToInt());
        }
    }
}
