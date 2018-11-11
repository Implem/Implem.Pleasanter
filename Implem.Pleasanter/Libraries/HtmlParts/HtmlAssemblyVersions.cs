using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlAssemblyVersions
    {
        public static string AssemblyVersions(this HtmlBuilder hb, Context context)
        {
            var ss = new SiteSettings();
            var plan = context.ContractSettings.DisplayName;
            return hb
                .Template(
                    context: context,
                    ss: ss,
                    view: null,
                    verType: Versions.VerTypes.Latest,
                    methodType: BaseModel.MethodTypes.NotSet,
                    useBreadcrumb: false,
                    useTitle: false,
                    useNavigationMenu: true,
                    action: () => hb
                        .Div(id: "Versions", action: () => hb
                            .Div(action: () => hb
                                .Span(action: () => hb
                                    .Img(id: "logoVersion",
                                        src: Locations.Images(
                                            context: context,
                                            parts: "logo-version.png"))))
                            .Div(action: () => hb
                                .Span(action: () => hb
                                    .Text(text: Displays.Version(context: context)))
                                .Span(action: () => hb
                                    .Text(text: Environments.AssemblyVersion +
                                        (Parameters.Enterprise
                                            ? " EE"
                                            : string.Empty))))
                            .Div(
                                action: () => hb
                                    .Span(action: () => hb
                                        .Text(text: plan + Displays.Plan(context: context))),
                                _using: !plan.IsNullOrEmpty())
                            .Div(action: () => hb
                                .Span(action: () => hb
                                    .A(
                                        href: Parameters.General.HtmlCopyrightUrl,
                                        action: () => hb
                                            .Raw(text: Parameters.General.HtmlCopyright.Params(
                                                DateTime.Now.Year))))))
                        .MainCommands(
                            context: context,
                            ss: ss,
                            siteId: 0,
                            verType: Versions.VerTypes.Latest))
                .ToString();
        }
    }
}