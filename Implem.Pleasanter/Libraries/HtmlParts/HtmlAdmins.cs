using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlAdmins
    {
        public static string AdminsIndex(this HtmlBuilder hb, Context context)
        {
            if (!Permissions.CanManageTenant(context: context))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.HasNotPermission));
            }
            var ss = new SiteSettings();
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                methodType: Pleasanter.Models.BaseModel.MethodTypes.NotSet,
                title: Displays.Admin(context: context),
                verType: Versions.VerTypes.Latest,
                useNavigationMenu: false,
                action: () => hb
                    .Nav(css: "cf", action: () => hb
                        .Ul(css: "nav-sites", action: () => hb
                            .Li(css: "nav-site", action: () => hb
                                .A(
                                    attributes: new HtmlAttributes()
                                        .Href(Locations.Edit(
                                            context: context,
                                            controller: "Tenants")),
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Text(Displays.Tenants(context: context)))),
                                _using: Permissions.CanManageTenant(context))
                            .Li(css: "nav-site", action: () => hb
                                .A(
                                    attributes: new HtmlAttributes()
                                        .Href(Locations.Index(
                                            context: context,
                                            controller: "Depts")),
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Text(Displays.Depts(context: context)))
                                        .StackStyles()))
                            .Li(css: "nav-site", action: () => hb
                                .A(
                                    attributes: new HtmlAttributes()
                                        .Href(Locations.Index(
                                            context: context,
                                            controller: "Groups")),
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Text(Displays.Groups(context: context)))
                                        .StackStyles()))
                            .Li(css: "nav-site", action: () => hb
                                .A(
                                    attributes: new HtmlAttributes()
                                        .Href(Locations.Index(
                                            context: context,
                                            controller: "Users")),
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Text(Displays.Users(context: context)))
                                        .StackStyles()))
                            .Li(css: "nav-site", action: () => hb
                                .A(
                                    attributes: new HtmlAttributes()
                                        .Href(Locations.Index(
                                            context: context,
                                            controller: "Registrations")),
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Text(Displays.Registrations(context: context)))
                                        .StackStyles()),
                                _using: Parameters.Registration.Enabled)
                            ))
                    .MainCommands(
                        context: context,
                        ss: ss,
                        verType: Versions.VerTypes.Latest))
                            .ToString();
        }
    }
}