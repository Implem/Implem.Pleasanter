namespace Implem.Plugins
{
    public interface IPdfPluginHost
    {
        string SiteTitle { get; }
        int ReportId { get; }
        
        List<Dictionary<string, object>> GetGridData(string viewJson);
    }
}
