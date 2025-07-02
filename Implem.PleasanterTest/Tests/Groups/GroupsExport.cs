using AngleSharp.Common;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Groups
{
    [Collection(nameof(GroupsExport))]
    public class GroupsExport
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            QueryStrings queryStrings,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                queryStrings: queryStrings,
                userId: userModel.UserId,
                routeData: RouteData.GroupsExport());
            var results = Results(context: context);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var queryStrings = QueryStringsUtilities.Get(
                new KeyValue("ExportId", "0"),
                new KeyValue("ExportEncoding", "Shift-JIS"),
                new KeyValue("GridCheckAll", "undefined"),
                new KeyValue("GridUnCheckedItems", "undefined"),
                new KeyValue("GridCheckedItems", "undefined"),
                new KeyValue("ExportCommentsJsonFormat", "false"));
            var baseTests = BaseData.Tests(FileData.Exists());
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    queryStrings: queryStrings,
                    baseTests: baseTests,
                    userType: UserData.UserTypes.TenantManager1),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    queryStrings: testPart.QueryStrings,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            QueryStrings queryStrings,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                queryStrings,
                userModel,
                baseTests
            };
        }

        private static ResponseFile Results(Context context)
        {
            return GroupUtilities.Export(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
        }
    }
}