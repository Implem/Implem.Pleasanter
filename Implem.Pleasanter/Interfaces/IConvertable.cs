using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Interfaces
{
    public interface IConvertable
    {
        string ToControl(SiteSettings ss, Column column);
        string ToResponse();
        HtmlBuilder Td(HtmlBuilder hb, Column column);
        string ToExport(Column column, ExportColumn exportColumn = null);
        bool InitialValue();
    }
}