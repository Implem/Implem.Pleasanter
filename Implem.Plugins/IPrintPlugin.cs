namespace Implem.Plugins
{
    public interface IPrintPlugin
    {
        PdfData CreatePdf(IPrintPluginHost host);
    }
}
