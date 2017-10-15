using Implem.Pleasanter.Interfaces;
using System.Collections.Generic;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Attachments : List<Attachment>, IConvertable
    {
        public Attachments()
        {
        }

        public Attachments(List<string> data)
        {

        }

        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb;
        }

        public string ToControl(SiteSettings ss, Column column)
        {
            return string.Empty;
        }

        public string ToExport(Column column, ExportColumn exportColumn = null)
        {
            return string.Empty;
        }

        public string ToResponse()
        {
            return string.Empty;
        }
    }
}