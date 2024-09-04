using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsSetPermissionForCreating
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
                routeData: RouteData.ItemsSetPermissionForCreating(id: id),
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
            var forms = FormsUtilities.Get(
                new KeyValue("ControlId", "ChangePermissionForCreating"),
                new KeyValue("CurrentPermissionForCreating", $"[\"Dept,0,1\"]"),
                new KeyValue("CurrentPermissionForCreatingAll", $"[\"Dept,0,1\"]"),
                new KeyValue("Read", "true"),
                new KeyValue("Create", "false"),
                new KeyValue("Update", "false"),
                new KeyValue("Delete", "false"),
                new KeyValue("SendMail", "false"),
                new KeyValue("Export", "false"),
                new KeyValue("Import", "false"),
                new KeyValue("ManageSite", "false"),
                new KeyValue("ManagePermission", "false"));
            var baseTests = BaseData.Tests(
                JsonData.ExistsOne(method: "CloseDialog"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#CurrentPermissionForCreating"),
                JsonData.ExistsOne(
                    method: "SetData",
                    target: "#CurrentPermissionForCreating"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "ネットワークの構築",
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
            return PermissionUtilities.SetPermissionForCreating(
                context: context,
                referenceId: context.Id);
        }
    }
}
