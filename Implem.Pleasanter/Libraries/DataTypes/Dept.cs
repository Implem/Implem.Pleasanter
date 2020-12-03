using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;
using static Implem.Pleasanter.Models.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Dept : IConvertable
    {
        public int TenantId;
        public int Id;
        public string Code;
        public string Name;

        public Dept()
        {
        }

        public Dept(DataRow dataRow)
        {
            TenantId = dataRow.Int("TenantId");
            Id = dataRow.Int("DeptId");
            Code = dataRow.String("DeptCode");
            Name = dataRow.String("DeptName");
        }

        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return Id.ToString();
        }

        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return Id.ToString();
        }

        public HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex,
            ServerScriptModelColumn serverScriptValues)
        {
            return hb.Td(
                css: column.CellCss(serverScriptValues?.ExtendedCellCss),
                action: () => hb
                    .Text(text: Name));
        }

        public string GridText(Context context, Column column)
        {
            return Name;
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return Name;
        }

        public bool InitialValue(Context context)
        {
            return Id == 0;
        }
    }
}