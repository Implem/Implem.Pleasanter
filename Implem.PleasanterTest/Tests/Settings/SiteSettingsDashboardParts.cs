using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;
using static Implem.PleasanterTest.Utilities.UserData;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsDashboardParts))]
    public class SiteSettingsDashboardParts
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
            var siteId = Initializer.Sites.Get("課題管理").SiteId;
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveUpDashboardParts"),
                        new KeyValue("EditDashboardPart","[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditDashboardPart"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveDownDashboardParts"),
                        new KeyValue("EditDashboardPart","[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditDashboardPart"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "NewDashboardPart")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DashboardPartDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditDashboardPart"),
                        new KeyValue("EditDashboardPart","[\"1\"]"),
                        new KeyValue("DashboardPartId","1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DashboardPartDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateDashboardPart"),
                        new KeyValue("DashboardPartId","1"),
                        new KeyValue("EditDashboardPart","[\"1\"]"),
                        new KeyValue("DashboardPartTitle","DashboardParts-chage")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditDashboardPart"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.HtmlTextContains(
                            method: "Html",
                            target: "#EditDashboardPart",
                            selector: "#EditDashboardPart",
                            value: "DashboardParts-chage")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "AddDashboardPart"),
                        new KeyValue("DashboardPartTitle","DashboardParts-test")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditDashboardPart"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.HtmlTextContains(
                            method: "ReplaceAll",
                            target: "#EditDashboardPart",
                            selector: "#EditDashboardPart",
                            value: "DashboardParts-test")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "CopyDashboardParts"),
                        new KeyValue("DashboardPartId","1"),
                        new KeyValue("EditDashboardPart","[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditDashboardPart"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.HtmlTextContains(
                            method: "ReplaceAll",
                            target: "#EditDashboardPart",
                            selector: "#EditDashboardPart",
                            value: "DashboardParts")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DeleteDashboardParts"),
                        new KeyValue("EditDashboardPart","[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditDashboardPart"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "AddDashboardPartViewFilter"),
                        new KeyValue("DashboardPartType","6"),
                        new KeyValue("DashboardPartViewFilterSelector", "Ver"),
                        new KeyValue("DashboardPartId","1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Append",
                            target: "#DashboardPartViewFiltersTab .items"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditTimeLineSites"),
                        new KeyValue("DashboardPartId","1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DashboardPartTimeLineSitesDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateDashboardPartTimeLineSites"),
                        new KeyValue("DashboardPartId","1"),
                        new KeyValue("DashboardPartTimeLineSitesEdit",$"{siteId}")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Set",
                            target: "#DashboardPartTimeLineSites"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditCalendarSites"),
                        new KeyValue("DashboardPartId","1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DashboardPartCalendarSitesDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateDashboardPartCalendarSites"),
                        new KeyValue("DashboardPartId","1"),
                        new KeyValue("DashboardPartCalendarSitesEdit",$"{siteId}")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Set",
                            target: "#DashboardPartCalendarSites"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditKambanSites"),
                        new KeyValue("DashboardPartId","1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DashboardPartKambanSitesDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateDashboardPartKambanSites"),
                        new KeyValue("DashboardPartId","1"),
                        new KeyValue("DashboardPartKambanSitesEdit",$"{siteId}")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Set",
                            target: "#DashboardPartKambanSites"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditIndexSites"),
                        new KeyValue("DashboardPartId","1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DashboardPartIndexSitesDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateDashboardPartIndexSites"),
                        new KeyValue("DashboardPartId","1"),
                        new KeyValue("DashboardPartIndexSitesEdit",$"{siteId}")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Set",
                            target: "#DashboardPartIndexSites"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "ClearDashboardView"),
                        new KeyValue("DashboardPartId","2"),
                        new KeyValue("DashboardPartTimeLineSitesEdit",$"{siteId}")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DashboardPartViewFiltersTabContainer"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "ClearDashboardCalendarView"),
                        new KeyValue("DashboardPartId","3"),
                        new KeyValue("DashboardPartCalendarSitesEdit",$"{siteId}")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DashboardPartViewFiltersTabContainer"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "ClearDashboardKambanView"),
                        new KeyValue("DashboardPartId","4"),
                        new KeyValue("DashboardPartKambanSitesEdit",$"{siteId}")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DashboardPartViewFiltersTabContainer"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - DashboardsParts",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "ClearDashboardIndexView"),
                        new KeyValue("DashboardPartId","5"),
                        new KeyValue("DashboardPartIndexSitesEdit",$"{siteId}")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DashboardPartViewFiltersTabContainer"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
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
