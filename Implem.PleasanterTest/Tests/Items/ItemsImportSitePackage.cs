using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsImportSitePackage
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsImportSitePackage(id: id),
                httpMethod: "POST",
                forms: forms,
                fileName: fileName,
                contentType: "text/json");
            var results = Results(context: context);
            Initializer.SaveResults(results);
			Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var baseTests = BaseData.Tests(
                JsonData.Value(
                    method: "Href",
                    value: Locations.ItemIndex(
                        context: Initializer.Context,
                        id: 0)));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: string.Empty,
                    fileName: "ProjectManagement.json",
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: FormsUtilities.Get(
                        new KeyValue("IncludeData", "true"),
                        new KeyValue("IncludeSitePermission", "true"),
                        new KeyValue("IncludeRecordPermission", "true"),
                        new KeyValue("IncludeColumnPermission", "true"),
                        new KeyValue("IncludeNotifications", "true"),
                        new KeyValue("IncludeReminders", "true")),
                    fileName: testPart.FileName,
                    userModel: testPart.UserModel,
                    baseTests: baseTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                forms,
                fileName,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id) ?? new ItemModel();
            return itemModel.ImportSitePackage(context: context);
        }
    }
}
