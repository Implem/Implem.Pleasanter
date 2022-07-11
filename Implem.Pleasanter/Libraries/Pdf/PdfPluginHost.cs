using System.Collections.Generic;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Models;
using Implem.Plugins;
using Implem.Libraries.DataSources.SqlServer;

namespace Implem.Pleasanter.Libraries.Pdf
{
    public class PdfPluginHost : IPdfPluginHost
    {
        public string SiteTitle { get; }
        public int ReportId { get; }
        private Context Context { get; }
        private SiteSettings SiteSettings { get; }
        private View DefaultView { get; }
        private SqlWhereCollection SelectingWhere { get; }

        public PdfPluginHost(Context context, SiteSettings ss, View defaultView, SqlWhereCollection selectingWhere, int reportId)
        {
            Context = context;
            SiteSettings = ss;
            DefaultView = defaultView;
            ReportId = reportId;
            SelectingWhere = selectingWhere;
            SiteTitle = ss.Title;
        }

        public List<Dictionary<string, object>> GetGridData(string viewJson)
        {
            var view = viewJson.Deserialize<View>() ?? DefaultView;
            view.ApiDataType = View.ApiDataTypes.KeyValues;
            var gridData = new GridData(
                context: Context,
                ss: SiteSettings,
                where: SelectingWhere,
                view: view);
            return gridData.KeyValues(
                context: Context,
                ss: SiteSettings,
                view: view);
        }
    }
}
