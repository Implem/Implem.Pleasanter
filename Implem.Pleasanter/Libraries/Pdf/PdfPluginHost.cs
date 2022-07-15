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
        public string DefaultView { get; }
        private Context Context { get; }
        private SiteSettings SiteSettings { get; }
        private View DefaultViewObj { get; }
        private SqlWhereCollection SelectingWhere { get; }

        public PdfPluginHost(Context context, SiteSettings ss, View defaultView, SqlWhereCollection selectingWhere, int reportId)
        {
            Context = context;
            SiteSettings = ss;
            DefaultViewObj = defaultView;
            ReportId = reportId;
            SelectingWhere = selectingWhere;
            SiteTitle = ss.Title;
            DefaultView = defaultView.ToJson();
        }

        public List<Dictionary<string, object>> GetGridData(string viewJson)
        {
            var view = viewJson.Deserialize<View>() ?? DefaultViewObj;
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
