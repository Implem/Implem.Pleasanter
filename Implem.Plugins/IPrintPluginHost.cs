namespace Implem.Plugins
{
    public interface IPdfPluginHost
    {
        int ReportId { get; }
        List<Dictionary<string, object>> GetGridData(string viewJson);
    }
}
