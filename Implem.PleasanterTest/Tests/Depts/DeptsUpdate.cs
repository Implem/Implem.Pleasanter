using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Depts
{
    public class DeptsUpdate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Depts.Values.FirstOrDefault(o => o.DeptName == title).DeptId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.DeptsUpdate(id: id));
            var results = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                results: results,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var jsonTests = new List<JsonTest>()
            {
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#HeaderTitle"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#RecordInfo"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(method: "Message"),
                JsonData.ExistsOne(method: "ClearFormData")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "開発1部",
                    jsonTests: jsonTests,
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    jsonTests: testPart.JsonTests);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                userModel,
                jsonTests
            };
        }

        private static string Results(Context context)
        {
            return DeptUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                deptId: context.Id.ToInt());
        }
    }
}
