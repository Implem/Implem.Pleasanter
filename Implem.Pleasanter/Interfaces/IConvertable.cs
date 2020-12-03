using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using static Implem.Pleasanter.Models.ServerScriptModel;
namespace Implem.Pleasanter.Interfaces
{
    public interface IConvertable
    {
        string ToControl(Context context, SiteSettings ss, Column column);
        string ToResponse(Context context, SiteSettings ss, Column column);
        HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex,
            ServerScriptModelColumn serverScriptValues);
        string ToExport(Context context, Column column, ExportColumn exportColumn = null);
        bool InitialValue(Context context);
    }
}