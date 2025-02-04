using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Binaries
{
    [Collection(nameof(BinariesDeleteTenantImage))]
    public class BinariesDeleteTenantImage
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
                routeData: RouteData.BinariesDeleteTenantImage(id: Initializer.TenantId),
                httpMethod: "POST",
                forms: forms,
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
            // 事前にテスト用の画像をアップロード。
            var context = ContextData.Get(
                userId: UserData.Get(userType: UserData.UserTypes.TenantManager1).UserId,
                routeData: RouteData.BinariesUpdateTenantImage(id: Initializer.TenantId),
                httpMethod: "POST",
                fileName: "Image1.png",
                contentType: "image/png");
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context: context);
            var tenantModel = new TenantModel(
                context: context,
                ss: ss)
                    .Get(
                        context: context,
                        ss: ss);
            BinaryUtilities.UpdateTenantImage(
                context: context,
                tenantModel: tenantModel);
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#Logo"),
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#TenantImageSettingsEditor"),
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Id\":\"FileDeleteCompleted\",\"Text\":\"ファイルの削除が完了しました。\",\"Css\":\"alert-success\"}")),
                    userType: UserData.UserTypes.TenantManager1)
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
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context: context);
            var tenantModel = new TenantModel(
                context: context,
                ss: ss)
                    .Get(
                        context: context,
                        ss: ss);
            return BinaryUtilities.DeleteTenantImage(
                context: context,
                tenantModel: tenantModel);
        }
    }
}
