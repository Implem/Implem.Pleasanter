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
    public class ItemsExportCrosstab
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            QueryStrings queryStrings,
            UserModel userModel,
            List<FileTest> fileTests)
        {
            var id = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                queryStrings: queryStrings,
                userId: userModel.UserId,
                routeData: RouteData.ItemsExportCrosstab(id: id));
            var results = Results(context: context);
            Assert.True(Compare.File(
                context: context,
                results: results,
                fileTests: fileTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var fileTests = FileData.Tests(FileData.Exists());
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "WBS",
                    fileTests: fileTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "課題管理",
                    fileTests: fileTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "レビュー記録",
                    fileTests: fileTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "顧客マスタ",
                    fileTests: fileTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "商談",
                    fileTests: fileTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "仕入",
                    fileTests: fileTests,
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    queryStrings: testPart.QueryStrings,
                    userModel: testPart.UserModel,
                    fileTests: testPart.FileTests);
            }
        }

        private static object[] TestData(
            string title,
            QueryStrings queryStrings,
            UserModel userModel,
            List<FileTest> fileTests)
        {
            return new object[]
            {
                title,
                queryStrings,
                userModel,
                fileTests
            };
        }

        private static ResponseFile Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.ExportCrosstab(context: context);
        }
    }
}