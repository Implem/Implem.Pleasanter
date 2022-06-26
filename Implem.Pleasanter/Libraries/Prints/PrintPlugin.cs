
using System.Linq;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using Implem.Plugins;

namespace Implem.Pleasanter.Libraries.Prints
{
    public class PrintPlugin : IPrintPlugin
    {
        public System.IO.Stream CreatePdf(IPrintPluginHost host)
        {
            var gridData = host.GetGridData(string.Empty);
            var stream = new System.IO.MemoryStream();
            var doc = new PdfDocument();
            var page = doc.AddPage();
            page.Size = PageSize.A4;
            var g = XGraphics.FromPdfPage(page);

            g.DrawRectangle(new XPen(XColors.Black, 1), new XRect(50, 50, g.PageSize.Width - 100, g.PageSize.Height - 100));

            var i = 0;
            foreach (var kvp in gridData.First())
            {
                var font = new XFont("游明朝", 14);
                var text = $"{kvp.Key} : {kvp.Value}";
                var height = g.MeasureString(text, font).Height;
                g.DrawString(text, font, XBrushes.Black, new XPoint(60, 62 + (height + 2) * i++));
            }
            doc.Save(stream);
            stream.Position = 0;
            return stream;
        }
    }
}
