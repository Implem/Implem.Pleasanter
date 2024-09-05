using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Binaries
{
    public class BinariesUpdateTenantImage
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.BinariesUpdateTenantImage(id: Initializer.TenantId),
                httpMethod: "POST",
                forms: forms,
                fileName: fileName,
                contentType: "image/png");
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
                    fileName: "Image1.png",
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#Logo"),
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#TenantImageSettingsEditor"),
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Id\":\"FileUpdateCompleted\",\"Text\":\"ファイルのアップロードが完了しました。\",\"Css\":\"alert-success\"}")),
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    forms: testPart.Forms,
                    fileName: testPart.FileName,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                forms,
                fileName,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context: context);
            var tenantModel = new TenantModel(
                context: context,
                ss: ss)
                    .Get(
                        context: context,
                        ss: ss);
            return BinaryUtilities.UpdateTenantImage(
                context: context,
                tenantModel: tenantModel);
        }
    }
}
