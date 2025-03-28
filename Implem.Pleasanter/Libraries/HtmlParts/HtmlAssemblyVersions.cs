using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlAssemblyVersions
    {
        public static string AssemblyVersions(this HtmlBuilder hb, Context context)
        {
            var ss = new SiteSettings();
            var plan = context.ContractSettings.DisplayName;
            var databaseSize = DatabaseSize(context: context);
            var isCommercialLicense = (Parameters.GetLicenseType() & 0x04) != 0;
            var isTrialLicense = (Parameters.GetLicenseType() & 0x08) != 0;
            return hb
                .Template(
                    context: context,
                    ss: ss,
                    view: null,
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
                                    .Text(text: Environments.AssemblyVersion)))
                            .Div(action: () => hb
                                .Span(action: () => hb
                                    .Text(text: Displays.License(context: context)))
                                .Span(action: () =>
                                {
                                    if (isCommercialLicense)
                                    {
                                        hb.Text(text: Displays.CommercialLicense(context: context));
                                    }
                                    else
                                    {
                                        hb.A(
                                            href: Parameters.General.HtmlAGPLUrl,
                                            action: () => hb
                                                .Text(text: Displays.AGPL(context: context)));
                                    }
                                }))
                            .Div(
                                action: () => hb
                                    .Span(action: () => hb
                                        .A(
                                            href: Parameters.General.HtmlTrialLicenseUrl,
                                            action: () => hb
                                                .Text(text: Displays.TrialLicenseInUse(context: context)))),
                                _using: isTrialLicense)
                            .Div(
                                action: () => hb
                                    .Span(action: () => hb
                                        .Text(text: Displays.TrialLicenseDeadline(context: context)))
                                    .Span(action: () => hb
                                        .Text(text: Parameters.LicenseDeadline().ToString("yyyy/MM/dd"))),
                                _using: isTrialLicense)
                            .Div(
                                action: () => hb
                                    .Span(action: () => hb
                                        .A(
                                            href: Parameters.General.HtmlEnterPriseEditionUrl,
                                            action: () => hb
                                                .Text(text: Displays.SwitchToCommercialLicense(context: context)))),
                                _using: !isCommercialLicense)
                            .Div(
                                action: () => hb
                                    .Span(action: () => hb
                                        .Text(text: Displays.LicenseDeadline(context: context)))
                                    .Span(action: () => hb
                                        .Text(text: Parameters.LicenseDeadline().ToString("yyyy/MM/dd"))),
                                _using: isCommercialLicense
                                    && Parameters.Version.ShowDeadline)
                            .Div(
                                action: () => hb
                                    .Span(action: () => hb
                                        .Text(text: Displays.Licensee(context: context)))
                                    .Span(action: () => hb
                                        .Text(text: Parameters.Licensee())),
                                _using: isCommercialLicense
                                    && Parameters.Version.ShowLicensee)
                            .Div(
                                action: () => hb
                                    .Span(action: () => hb
                                        .Text(text: Displays.MaximumNumberOfUsers(context: context)))
                                    .Span(action: () => hb
                                        .Text(text: Parameters.LicensedUsers() == 0
                                            ? Displays.Unlimited(context: context)
                                            : Parameters.LicensedUsers().ToString())),
                                _using: isCommercialLicense
                                    && Parameters.Version.ShowMaximumNumberOfUsers)
                            .Div(
                                action: () => hb
                                    .Span(action: () => hb
                                        .Text(text: plan + Displays.Plan(context: context))),
                                _using: !plan.IsNullOrEmpty())
                            .Div(
                                action: () => hb
                                    .Span(action: () => hb
                                        .Text(text: Displays.DatabaseSize(
                                            context: context,
                                            data: databaseSize.ToString()))),
                                _using: context.HasPrivilege && databaseSize != null)
                            .Div(action: () => hb
                                .Span(action: () => hb
                                    .A(
                                        href: Parameters.CopyrightUrl(),
                                        action: () => hb
                                            .Raw(text: Parameters.Copyright())))))
                        .MainCommands(
                            context: context,
                            ss: ss,
                            verType: Versions.VerTypes.Latest))
                .ToString();
        }

        private static string DatabaseSize(Context context)
        {
            try
            {
                return Repository.ExecuteTable(
                    context: context,
                    connectionString: Parameters.Rds.OwnerConnectionString,
                    statements: new SqlStatement(
                        commandText: Def.Sql.Spaceused
                            .Replace("#InitialCatalog#", Environments.ServiceName)))
                                .AsEnumerable()
                                .FirstOrDefault()
                                .String("database_size");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}