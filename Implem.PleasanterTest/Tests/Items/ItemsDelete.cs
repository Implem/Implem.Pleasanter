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
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsDelete(id: id));
            var json = GetJson(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var jsonTests = new List<JsonTest>()
            {
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "back")
            };
            var titles = new Dictionary<string, List<JsonTest>>()
            {
                { "RecordToDeleteSite2", jsonTests },
                { "RecordToDeleteSite3", jsonTests },
                { "RecordToDeleteSite4", jsonTests },
                { "RecordToDeleteSite6", jsonTests },
                { "RecordToDeleteSite7", jsonTests },
                { "RecordToDeleteSite8", jsonTests }
            };
            foreach (var data in titles)
            {
                yield return TestData(
                    title: data.Key,
                    userModel: UserData.Get(userType: UserData.UserTypes.General1),
                    jsonTests: data.Value);
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

        private static string GetJson(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Delete(context: context);
        }
    }
}
