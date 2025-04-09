using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Web.Mvc;
using Xunit;

namespace Implem.PleasanterTest.Tests.DashboardPart
{
    [Collection(nameof(UpdateDashboardPartsSiteSettings))]
    public class UpdateDashboardPartsSiteSettings
    {
        Dictionary<string, string> SessionData;
        int UserId;
        long SiteId;
        Forms Forms;
        public UpdateDashboardPartsSiteSettings(Dictionary<string, string> sessionData, int userId, Forms forms, long siteId)
        {
            this.SessionData = sessionData;
            this.UserId = userId;
            this.SiteId = siteId;
            this.Forms = forms;
        }
        public void Update()
        {
            var context = ContextData.Get(
                userId: UserId,
                routeData: RouteData.ItemsIndex(id: SiteId),
                forms: Forms,
                sessionData: SessionData);
            new ItemModel(
                context: context,
                referenceId: SiteId)
            .Update(context: context);
        }
    }
}
