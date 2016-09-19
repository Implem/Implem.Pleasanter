using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlHeaders
    {
        public static HtmlBuilder PageHeader(this HtmlBuilder hb, bool useSearch)
        {
            return hb.Header(css: "header", action: () => hb
                .H(number: 2, css: "logo", action: () => hb
                    .A(
                        attributes: new HtmlAttributes().Href(Navigations.Top()),
                        action: () => hb
                            .Img(
                                src: Navigations.Images("logo-corp.png"),
                                css: "logo-corp")
                            .Span(css: "logo-product", action: () => hb
                                .Displays_ProductName())))
                .Search(_using: useSearch)
                .LoginUser());
        }

        private static HtmlBuilder Search(this HtmlBuilder hb, bool _using)
        {
            return _using
                ? hb
                    .Div(css: "search", action: () => hb
                        .Div(css: "ui-icon ui-icon-search")
                        .TextBox(
                            controlId: "Search",
                            controlCss: " w200 redirect",
                            placeholder: Displays.Search()))
                : hb;
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