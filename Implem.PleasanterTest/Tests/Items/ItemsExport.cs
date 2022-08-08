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
    public class ItemsExport
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
                routeData: RouteData.ItemsExport(id: id));
            var file = Results(context: context);
            Assert.True(Compare.File(
                context: context,
                file: file,
                fileTests: fileTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var queryStrings = QueryStringsUtilities.Get(
                new KeyValue("id", "0"),
                new KeyValue("encoding", "Shift-JIS"),
                new KeyValue("GridCheckAll", "undefined"),
                new KeyValue("GridUnCheckedItems", "undefined"),
                new KeyValue("GridCheckedItems", "undefined"));
            var fileTests = FileData.Tests(FileData.Exists());
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "WBS",
                    queryStrings: queryStrings,
                    fileTests: fileTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "課題管理",
                    queryStrings: queryStrings,
                    fileTests: fileTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "レビュー記録",
                    queryStrings: queryStrings,
                    fileTests: fileTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "顧客マスタ",
                    queryStrings: queryStrings,
                    fileTests: fileTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "商談",
                    queryStrings: queryStrings,
                    fileTests: fileTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "仕入",
                    queryStrings: queryStrings,
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
            return itemModel.Export(context: context);
        }
    }
}