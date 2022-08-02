using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Items
{
    public class ItemsSelectedIds
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<TextTest> textTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsSelectedIds(id: id),
                httpMethod: "POST",
                forms: forms);
            var text = GetText(context: context);
            Assert.True(Compare.Text(
                context: context,
                text: text,
                textTests: textTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var testParts = new List<TestPart>()
            {
                new TestPart(
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
                    forms: new Forms()
                    {
                        {
                            "GridCheckedItems",
                            testPart.Ids.Join()
                        }
                    },
                    userModel: UserData.Get(userType: testPart.UserType),
                    textTests: TextData.Equals(value: testPart.Ids.ToJson()).ToSingleList());
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel,
            List<TextTest> textTests)
        {
            return new object[]
            {
                title,
                forms,
                userModel,
                textTests
            };
        }

        private static string GetText(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.SelectedIds(context: context);
        }

        private class TestPart
        {
            public string Title { get; set; }
            public List<long> Ids { get; set; }
            public UserData.UserTypes UserType { get; set; }

            public TestPart(
                string title,
                List<string> recordTitles,
                UserData.UserTypes userType = UserData.UserTypes.General1)
            {
                Title = title;
                Ids = Initializer.Titles
                    .Where(o => recordTitles.Contains(o.Key))
                    .Select(o => o.Value)
                    .ToList();
                UserType = userType;
            }
        }
    }
}
