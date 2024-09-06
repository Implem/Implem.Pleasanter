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
    [Collection(nameof(ItemsSortSiteMenu))]
    public class ItemsSortSiteMenu
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
                routeData: RouteData.ItemsSortSiteMenu(id: id),
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
            var parentId = Initializer.Titles.Get("商談管理の例");
            var forms = FormsUtilities.Get(
                new KeyValue("ControlId", "SortSiteMenu"),
                new KeyValue("Data", Initializer.Sites
                    .Values
                    .Where(o => o.ParentId == parentId)
                    .OrderByDescending(o => o.SiteId)
                    .Select(o => o.SiteId.ToString())
                    .ToJson()));
            var baseTests = BaseData.Tests(JsonData.ExistsOne(method: "Log"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "商談管理の例",
                    forms: forms,
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
            var itemModel = Initializer.ItemIds.Get(context.Id) ?? new ItemModel();
            return SiteUtilities.SortSiteMenu(
                context: context,
                siteId: itemModel.ReferenceId);
        }
    }
}
