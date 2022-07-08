namespace Implem.Plugins
{
    public class PdfData
    {
        public Stream? Stream { get; }
        public string? FileName { get; }
        public PdfData(Stream? stream, string? fileName)
        {
            Stream = stream;
            FileName = fileName;
        }
    }
}
