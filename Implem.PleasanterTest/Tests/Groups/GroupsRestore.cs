using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Operations;
using Implem.PleasanterTest.Utilities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Groups
{
    [Collection(nameof(GroupsRestore))]
    public class GroupsRestore
    {
        static string DeletedGroup = "グループ15（復元）";
        public GroupsRestore()
        {
            GroupsOperations.Delete(groupName: DeletedGroup);
        }

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
                routeData: RouteData.GroupsTrashBox());
            var results = Results(
                context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var id = Initializer.Groups.Values.FirstOrDefault(o => o.GroupName == DeletedGroup).GroupId;
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    queryStrings: QueryStringsUtilities.Get(
                        new KeyValue("GridCheckedItems", $"{id}"),
                        new KeyValue("EditOnGrid", ""),
                        new KeyValue("NewRowId", "")),
                    baseTests: BaseData.Tests(
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Id\":\"BulkRestored\",\"Text\":\"1 件 復元しました。\",\"Css\":\"alert-success\"}")),
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

        private static string Results(
            Context context)
        {
            return GroupUtilities.Restore(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(
                    context: context,
                    tableTypes: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Deleted));
        }
    }
}
