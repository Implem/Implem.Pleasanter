using System.Collections.Generic;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Models;
using Implem.Plugins;

namespace Implem.Pleasanter.Libraries.Prints
{
    public class PrintPluginHost : IPrintPluginHost
    {
        public Context Context { get; }
        public SiteSettings SiteSettings { get; }
        public View DefaultView { get; }
        public PrintPluginHost(Context context, SiteSettings ss, View defaultView)
        {
            Context = context;
            SiteSettings = ss;
            DefaultView = defaultView;
        }

        public List<Dictionary<string, object>> GetGridData(string viewJson , int offset = 0, int pagesize = 0)
        {
            var view = viewJson.Deserialize<View>() ?? DefaultView;
            view.ApiDataType = View.ApiDataTypes.KeyValues;
            var gridData = new GridData(
                context: Context,
                ss: SiteSettings,
                view: view,
                offset: offset,
                pageSize: pagesize);
            return gridData.KeyValues(
                context: Context,
                ss: SiteSettings,
                view: view);
        }
    }
}
