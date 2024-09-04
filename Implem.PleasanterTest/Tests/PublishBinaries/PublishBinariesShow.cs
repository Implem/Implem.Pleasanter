using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.PublishBinaries
{
    public class PublishBinariesShow
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            string guid)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userType: UserData.UserTypes.Anonymous,
                routeData: RouteData.PublishBinariesShow(id: id));
            var results = Results(
                context: context,
                guid: guid);
			Assert.True(results);
        }

        public static IEnumerable<object[]> GetData()
        {
            // 事前にテスト用の画像をアップロードしてGUIDを取得。
            var id = Initializer.Titles.Get("ネットワークの構築");
            BinaryUtilities.UploadImage(
                context: ContextData.Get(
                    userId: UserData.Get(userType: UserData.UserTypes.General1).UserId,
                    routeData: RouteData.BinariesShow(id: id),
                    httpMethod: "POST",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "Issues_Body")),
                    fileName: "Image2.png",
                    contentType: "image/png"),
                id: id);
            var guid = Rds.ExecuteScalar_string(
                context: Initializer.Context,
                statements: Rds.SelectBinaries(
                    column: Rds.BinariesColumn().Guid(),
                    where: Rds.BinariesWhere()
                        .TenantId(Initializer.TenantId)
                        .ReferenceId(id)
                        .FileName("Image2.png")));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "ネットワークの構築",
                    guid: guid,
                    fileName: "Image2.png")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    guid: testPart.Guid);
            }
        }

        private static object[] TestData(
            string title,
            string guid)
        {
            return new object[]
            {
                title,
                guid
            };
        }

        private static bool Results(Context context, string guid)
        {
            var file = BinaryUtilities.Donwload(
                context: context,
                guid: guid)
                    ?.FileStream();
            var result = file != null
                ? new FileContentResult(file.FileContents, file.ContentType)
                : null;
            return result != null;
        }
    }
}
