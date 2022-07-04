namespace Implem.Plugins
{
    public interface IPrintPluginHost
    {
        int ReportId { get; }
        List<Dictionary<string, object>> GetGridData(string viewJson);
    }
}
