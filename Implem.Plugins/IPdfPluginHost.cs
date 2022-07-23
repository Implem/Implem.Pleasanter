using System.Collections.Generic;

namespace Implem.Plugins
{
    public interface IPdfPluginHost
    {
        string SiteTitle { get; }
        string Url { get; }
        int ReportId { get; }

        List<Dictionary<string, object>> GetGridData(string viewJson);
    }
}
