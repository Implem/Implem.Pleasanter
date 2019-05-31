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
                                    .Text(text: Environments.AssemblyVersion)))
                            .Div(action: () => hb
                                .Span(action: () => hb
                                    .Text(text: Displays.License(context: context)))
                                .Span(action: () => hb
                                    .Text(text: Parameters.CommercialLicense()
                                        ? Displays.CommercialLicense(context: context)
                                        : Displays.AGPL(context: context))))
                            .Div(
                                action: () => hb
                                    .Span(action: () => hb
                                        .Text(text: Displays.LicenseDeadline(context: context)))
                                    .Span(action: () => hb
                                        .Text(text: Parameters.LicenseDeadline().ToString("yyyy/MM/dd"))),
                                _using: Parameters.CommercialLicense()
                                    && Parameters.Version.ShowDeadline)
                            .Div(
                                action: () => hb
                                    .Span(action: () => hb
                                        .Text(text: Displays.Licensee(context: context)))
                                    .Span(action: () => hb
                                        .Text(text: Parameters.Licensee())),
                                _using: Parameters.CommercialLicense()
                                    && Parameters.Version.ShowLicensee)
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
                                        href: "https://implem.co.jp",
                                        action: () => hb
                                            .Raw(text: "Copyright &copy; Implem Inc. 2014 - "
                                                + DateTime.Now.Year)))))
                        .MainCommands(
                            context: context,
                            ss: ss,
                            siteId: 0,
                            verType: Versions.VerTypes.Latest))
                .ToString();
        }

        private static string DatabaseSize(Context context)
        {
            try
            {
                return Rds.ExecuteTable(
                    context: context,
                    connectionString: Parameters.Rds.OwnerConnectionString,
                    statements: new SqlStatement(
                        commandText: Def.Sql.Spaceused))
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