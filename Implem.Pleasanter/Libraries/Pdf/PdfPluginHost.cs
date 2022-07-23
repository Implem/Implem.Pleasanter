using System.Collections.Generic;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Models;
using Implem.Plugins;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.Responses;

namespace Implem.Pleasanter.Libraries.Pdf
{
    public class PdfPluginHost : IPdfPluginHost
    {
        public string SiteTitle { get; }
        public string Url { get; }
        public int ReportId { get; }
        private Context Context { get; }
        private SiteSettings SiteSettings { get; }
        private View DefaultView { get; }
        private SqlWhereCollection Where { get; }

        public PdfPluginHost(Context context, SiteSettings ss, View view, SqlWhereCollection where, int reportId)
        {
            Context = context;
            Url = Locations.ItemPdfAbsoluteUri(context, context.Id);
            if (!context.Query.IsNullOrEmpty())
            {
                Url = Url.Replace(context.Query, "");
            }
            SiteSettings = ss;
            DefaultView = view;
            ReportId = reportId;
            Where = where;
            SiteTitle = ss.Title;
        }

        public List<Dictionary<string, object>> GetGridData(string viewJson)
        {
            var view = viewJson.Deserialize<View>() ?? DefaultView;
            var gridData = new GridData(
                context: Context,
                ss: SiteSettings,
                where: Where,
                view: view);
            return gridData.KeyValues(
                context: Context,
                ss: SiteSettings,
                view: view);
        }
    }
}
