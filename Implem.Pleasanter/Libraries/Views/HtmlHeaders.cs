using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
namespace Implem.Pleasanter.Libraries.Views
{
    public static class HtmlHeaders
    {
        public static HtmlBuilder PageHeader(this HtmlBuilder hb)
        {
            return hb.Header(css: "header", action: () => hb
                .H(number: 2, css: "logo", action: () => hb
                    .A(
                        attributes: Html.Attributes().Href(Navigations.Top()),
                        action: () => hb
                            .Img(
                                src: Navigations.Images("logo-corp.png"),
                                css: "logo-corp")
                            .Span(css: "logo-product", action: () => hb
                                .Text(text: Parameters.General.HtmlLogoText))))
                .LoginUser());
        }

        private static HtmlBuilder LoginUser(this HtmlBuilder hb)
        {
            return Sessions.LoggedIn()
                ? hb.Div(css: "login-user", action: () => hb
                    .P(action: () => hb
                        .Displays_Login())
                    .HtmlUser(Sessions.UserId())
                    .Admin()
                    .P(action: () => hb
                        .Icon(iconCss: "ui-icon-wrench")
                        .A(
                            href: Navigations.Edit("Users", Sessions.UserId()),
                            text: Displays.EditProfile()))
                    .P(action: () => hb
                        .Icon(iconCss: "ui-icon-locked")
                        .A(
                            href: Navigations.Logout(),
                            text: Displays.Logout())))
                    .Hidden(controlId: "Language", value: "_" + Sessions.Language())
                : hb;
        }

        private static HtmlBuilder Admin(this HtmlBuilder hb)
        {
            return Sessions.User().TenantAdmin
                ? hb.P(action: () => hb
                    .Icon(iconCss: "ui-icon-wrench")
                    .A(
                        href: Navigations.Index("Admins"),
                        text: Displays.Admin()))
                : hb;
        }
    }
}