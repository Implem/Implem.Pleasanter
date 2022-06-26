namespace Implem.Plugins
{
    public interface IPrintPluginHost
    {
        List<Dictionary<string, object>> GetGridData(string viewJson, int offset = 0, int pagesize = 0);
    }
}
