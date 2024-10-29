using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Binaries
{
    [Collection(nameof(BinariesDeleteImage))]
    public class BinariesDeleteImage
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string guid,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.BinariesDeleteImage(guid: guid),
                httpMethod: "DELETE",
                forms: forms);
            var results = Results(
                context: context,
                guid: guid);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var id = Initializer.Titles.Get("利用者向けマニュアルを作成する");
            BinaryUtilities.UploadImage(
                context: ContextData.Get(
                    userId: UserData.Get(userType: UserData.UserTypes.General1).UserId,
                    routeData: RouteData.BinariesUploadImage(id: id),
                    httpMethod: "POST",
                    forms: FormsUtilities.Get(new KeyValue("ControlId", "Issues_Body")),
                    fileName: "Image1.png",
                    contentType: "image/png"),
                id: id);
            var guid = Rds.ExecuteScalar_string(
                context: Initializer.Context,
                statements: Rds.SelectBinaries(
                    column: Rds.BinariesColumn().Guid(),
                    where: Rds.BinariesWhere()
                        .TenantId(Initializer.TenantId)
                        .ReferenceId(id)
                        .FileName("Image1.png")));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    guid: guid,
                    forms: FormsUtilities.Get(new KeyValue("Guid", guid)),
                    baseTests: BaseData.Tests(
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Id\":\"DeletedImage\",\"Text\":\"画像を削除しました。\",\"Css\":\"alert-success\"}"),
                        JsonData.ExistsOne(
                            method: "Remove",
                            target: $"#ImageLib .item[data-id=\"{guid}\"]")))
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    guid: testPart.Guid,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string guid,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                guid,
                forms,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context, string guid)
        {
            return BinaryUtilities.DeleteImage(
                context: context,
                guid: guid);
        }
    }
}
