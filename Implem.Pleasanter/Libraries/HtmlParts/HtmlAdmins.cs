using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlAdmins
    {
        public static string AdminsIndex(this HtmlBuilder hb)
        {
            if (!Permissions.CanManageTenant())
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var ss = new SiteSettings();
            return hb.Template(
                ss: ss,
                methodType: Pleasanter.Models.BaseModel.MethodTypes.NotSet,
                title: Displays.Admin(),
                verType: Versions.VerTypes.Latest,
                useNavigationMenu: false,
                action: () => hb
                    .Nav(css: "cf", action: () => hb
                        .Ul(css: "nav-sites", action: () => hb
                            .Li(css: "nav-site", action: () => hb
                                .A(
                                    attributes: new HtmlAttributes()
                                        .Href(Locations.Index("Depts")),
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Text(Displays.Depts()))
                                        .StackStyles()))
                            .Li(css: "nav-site", action: () => hb
                                .A(
                                    attributes: new HtmlAttributes()
                                        .Href(Locations.Index("Groups")),
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Text(Displays.Groups()))
                                        .StackStyles()))
                            .Li(css: "nav-site", action: () => hb
                                .A(
                                    attributes: new HtmlAttributes()
                                        .Href(Locations.Index("Users")),
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Text(Displays.Users()))
                                        .StackStyles()))))
                    .MainCommands(
                        ss: ss,
                        siteId: 0,
                        verType: Versions.VerTypes.Latest))
                            .ToString();
        }
    }
}