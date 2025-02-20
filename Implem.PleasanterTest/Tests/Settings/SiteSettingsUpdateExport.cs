using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using Newtonsoft.Json.Linq;
using Quartz.Util;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsUpdateExport))]
    public class SiteSettingsUpdateExport
    {

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsSetSiteSettings(id: siteId),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context, siteId: siteId);
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
                    title: "サイト設定 - Exports",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateExport"),
                        new KeyValue("ExportId", "1"),
                        new KeyValue("ExportName", "エクスポート更新テスト"),
                        new KeyValue("ExportColumnsAll","["
                            + @"""{\""Id\"":1,\""ColumnName\"":\""ResultId\""}"""
                            + @",""{\""Id\"":2,\""ColumnName\"":\""Title\""}"""
                            + @",""{\""Id\"":3,\""ColumnName\"":\""Body\""}"""
                            + @",""{\""Id\"":4,\""ColumnName\"":\""TitleBody\""}"""
                            + @",""{\""Id\"":5,\""ColumnName\"":\""Comments\""}"""
                            + @",""{\""Id\"":6,\""ColumnName\"":\""Status\""}"""
                            + @",""{\""Id\"":7,\""ColumnName\"":\""Manager\""}"""
                            + @",""{\""Id\"":8,\""ColumnName\"":\""Owner\""}"""
                            + @",""{\""Id\"":9,\""ColumnName\"":\""Creator\""}"""
                            + @",""{\""Id\"":10,\""ColumnName\"":\""Updator\""}"""
                            + @",""{\""Id\"":11,\""ColumnName\"":\""UpdatedTime\""}"""
                            + "]"),
                        new KeyValue("ExportType", "0"),
                        new KeyValue("ExportHeader", "true"),
                        new KeyValue("DelimiterType", "0"),
                        new KeyValue("EncloseDoubleQuotes", "true"),
                        new KeyValue("ExecutionType", "0")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditExport"),
                        JsonData.ExistsOne(
                            method: "CloseDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.TextContains(
                            method: "ReplaceAll",
                            target: "#EditExport",
                            value: "<td>エクスポート更新テスト</td>")),
                    userType: UserData.UserTypes.Privileged)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    baseTests: testPart.BaseTests,
                    userModel: testPart.UserModel);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel)
        {
            return new object[]
            {
                title,
                forms,
                baseTests,
                userModel
            };
        }

        private static string Results(Context context, long siteId)
        {
            return SiteUtilities.SetSiteSettings(
                context: context,
                siteId: siteId);
        }
    }
}
