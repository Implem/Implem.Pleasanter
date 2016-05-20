using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Utilities;
namespace Implem.Pleasanter.Libraries.ViewParts
{
    public static class HtmlAdmins
    {
        public static string Index()
        {
            var hb = Html.Builder();
            var permissionType = Permissions.Admins();
            return hb.Template(
                siteId: 0,
                modelName: string.Empty,
                title: Displays.Admin(),
                permissionType: permissionType,
                methodType: Models.BaseModel.MethodTypes.NotSet,
                allowAccess: Sessions.User().TenantAdmin,
                verType: Versions.VerTypes.Latest,
                useNavigationButtons: false,
                action: () => hb
                    .Nav(css: "cf", action: () => hb
                        .Ul(css: "nav-sites", action: () => hb
                            .Li(css: "nav-site", action: () => hb
                                .A(
                                    attributes: Html.Attributes()
                                        .Href(Navigations.Index("Depts")),
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Text(Displays.Depts()))
                                        .Div(css: "stacking1")
                                        .Div(css: "stacking2")))
                            .Li(css: "nav-site", action: () => hb
                                .A(
                                    attributes: Html.Attributes()
                                        .Href(Navigations.Index("Users")),
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Text(Displays.Users()))
                                        .Div(css: "stacking1")
                                        .Div(css: "stacking2")))))
                    .MainCommands(
                        siteId: 0,
                        permissionType: permissionType,
                        verType: Versions.VerTypes.Latest,
                        backUrl: Navigations.Top())).ToString();
        }
    }
}