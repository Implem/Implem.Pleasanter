﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsCreateDashboard
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
                routeData: RouteData.ItemsCreateByTemplate(id: id),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context, parentId: id);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var forms = FormsUtilities.Get(
                new KeyValue("ControlId", "CreateByTemplate"),
                new KeyValue("TemplateId", "Dashboard_ja"));
            var baseTests = BaseData.Tests(
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#SiteMenu"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#MainCommandsContainer"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setSiteMenu"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: string.Empty,
                    forms: forms,
                    baseTests: baseTests),
                new TestPart(
                    title: "ダッシュボードの例",
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

        private static string Results(Context context, long parentId)
        {
            return SiteUtilities.CreateByTemplate(
                context: context,
                parentId: parentId,
                inheritPermission: parentId);
        }
    }
}
