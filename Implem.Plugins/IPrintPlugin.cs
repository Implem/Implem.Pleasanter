namespace Implem.Plugins
{
    public interface IPrintPlugin
    {
        Stream CreatePdf(IPrintPluginHost host);
    }
}
