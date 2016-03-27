using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Views;
namespace Implem.Pleasanter.Interfaces
{
    public interface IConvertable
    {
        string ToControl(Column column);
        string ToResponse();
        HtmlBuilder Td(HtmlBuilder hb, Column column);
        string ToExport(Column column);
    }
}