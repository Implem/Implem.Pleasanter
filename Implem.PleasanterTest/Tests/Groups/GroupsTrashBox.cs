using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Operations;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Groups
{
    [Collection(nameof(GroupsTrashBox))]
    public class GroupsTrashBox
    {
        static string DeletedGroup = "グループ14（ごみ箱）";
        public GroupsTrashBox()
        {
            GroupsOperations.Delete(groupName: DeletedGroup);
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
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
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    baseTests: BaseData.Tests(
                        HtmlData.ExistsOne("#Grid"),
                        HtmlData.TextContains(
                            selector: "#Grid",
                            value: DeletedGroup)),
                    userType: UserData.UserTypes.TenantManager1),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                userModel,
                baseTests
            };
        }

        private static string Results(
            Context context)
        {
            return GroupUtilities.TrashBox(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(
                        context: context,
                        tableTypes: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Deleted));
        }
    }
}
