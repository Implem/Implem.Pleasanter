using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsSelectedIds
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsSelectedIds(id: id),
                httpMethod: "POST",
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
            var testParts = new List<MyTestPart>()
            {
                new MyTestPart(
                    title: "WBS",
                    recordTitles: new List<string>()
                    {
                        "ネットワークの構築",
                        "サーバの構築"
                    })
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: FormsUtilities.Get(
                        new KeyValue("GridCheckedItems", testPart.Ids.Join())),
                    userModel: testPart.UserModel,
                    baseTests: BaseData.Tests(TextData.ListEquals(value: testPart.Ids.ToJson())));
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
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.SelectedIds(context: context);
        }

        private class MyTestPart : TestPart
        {
            public List<long> Ids { get; set; }

            public MyTestPart(
                string title,
                List<string> recordTitles,
                UserData.UserTypes userType = UserData.UserTypes.General1)
            {
                Title = title;
                Ids = Initializer.Titles
                    .Where(o => recordTitles.Contains(o.Key))
                    .Select(o => o.Value)
                    .ToList();
                UserModel = UserData.Get(userType: userType);
            }
        }
    }
}