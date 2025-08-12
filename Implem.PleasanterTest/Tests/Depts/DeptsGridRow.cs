using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Depts
{
    [Collection(nameof(DeptsGridRow))]
    public class DeptsGridRow
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.DeptsIndex(),
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

            var testParts = new List<TestPart>()
            {
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("TableId", "Grid"),
                        new KeyValue("EditOnGrid", "0"),
                        new KeyValue("NewRowId", ""),
                        new KeyValue("ViewSorters__DeptCode", "asc"),
                        new KeyValue("ControlId", "Grid")),
                    baseTests: BaseData.Tests(
                        JsonData.TextCheckOrder(
                            method: "Append",
                            target: "#Grid",
                            wordArray: new string[]
                            {
                                "<td>000000</td><td>会社</td>",
                                "<td>000010</td><td>取締役会</td>",
                                "<td>000110</td><td>人事部</td>",
                                "<td>000210</td><td>経理部</td>",
                                "<td>000310</td><td>総務部</td>",
                                "<td>000410</td><td>営業部</td>",
                                "<td>000510</td><td>開発1部</td>",
                                "<td>000610</td><td>開発2部（無効）</td>",
                                "<td>000710</td><td>設備管理部（無効）</td>"
                            })),
                    userType: UserData.UserTypes.TenantManager1),
                
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                forms,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return DeptUtilities.GridRows(context: context);
        }
    }
}
