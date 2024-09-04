using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsExportSitePackage
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            QueryStrings queryStrings,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                queryStrings: queryStrings,
                userId: userModel.UserId,
                routeData: RouteData.ItemsExportSitePackage(id: id));
            var results = Results(context: context);
			Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var queryStrings = QueryStringsUtilities.Get(
                new KeyValue("SitePackagesSelectableAll", Initializer.Sites
                    .Values
                    .Where(o => o.ParentId == Initializer.Titles.Get("商談管理の例"))
                    .Select(o => $"\"{o.SiteId}-true\"")
                    .ToJson()),
                new KeyValue("UseIndentOption", "true"),
                new KeyValue("IncludeSitePermission", "true"),
                new KeyValue("IncludeRecordPermission", "true"),
                new KeyValue("IncludeColumnPermission", "true"),
                new KeyValue("IncludeNotifications", "true"),
                new KeyValue("IncludeReminders", "true"));
            var baseTests = BaseData.Tests(FileData.Exists());
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "商談管理の例",
                    queryStrings: queryStrings,
                    baseTests: baseTests,
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    queryStrings: testPart.QueryStrings,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            QueryStrings queryStrings,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                queryStrings,
                userModel,
                baseTests
            };
        }

        private static ResponseFile Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id) ?? new ItemModel();
            return itemModel.ExportSitePackage(context: context);
        }
    }
}