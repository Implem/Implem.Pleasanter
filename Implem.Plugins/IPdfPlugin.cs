namespace Implem.Plugins
{
    public interface IPdfPlugin
    {
        PdfData CreatePdf(IPdfPluginHost host);
    }
}
