using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Tests.Groups;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Groups
{
    [Collection(nameof(GroupsImport))]
    public class GroupsImport
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
                routeData: RouteData.GroupsImport(),
                httpMethod: "POST",
                forms: forms,
                fileName: fileName,
                contentType: "text/csv");
            var results = Results(context: context);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validJsonTests = new List<BaseTest>()
            {
                JsonData.ExistsOne(method: "Log"),
                JsonData.Value(
                    method: "Message",
                    value: "{\"Id\":\"GroupImported\",\"Text\":\": グループ 1 件追加、0 件更新しました。グループメンバー 0 件追加、0 件更新しました。\",\"Css\":\"alert-success\"}")
            };
            var testParts = new List<MyTestPart>()
            {
                new MyTestPart(
                    fileName: "Groups_Import_tmp.csv",
                    encoding: "UTF-8",
                    updatableImport: false,
                    key: null,
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    forms: FormsUtilities.Get(
                        new KeyValue($"Encoding", testPart.Encoding),
                        new KeyValue($"UpdatableImport", null),
                        new KeyValue("Key", null),
                        new KeyValue("ReplaceAllGroupMembers", null),
                        new KeyValue("MigrationMode", null)),
                    fileName: testPart.FileName,
                    userModel: testPart.UserModel,
                    baseTests: validJsonTests);
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
            return GroupUtilities.Import(context: context);
        }

        private class MyTestPart : TestPart
        {
            public string Encoding { get; }
            public bool UpdatableImport { get; }
            public string Key { get; }

            public MyTestPart(
                string fileName,
                string encoding,
                bool updatableImport,
                string key,
                UserData.UserTypes userType = UserData.UserTypes.General1)
            {
                FileName = fileName;
                UpdatableImport = updatableImport;
                Encoding = encoding;
                Key = key;
                UserModel = UserData.Get(userType: userType);
            }
        }
    }
}
