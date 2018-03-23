using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Dept : IConvertable
    {
        public int TenantId;
        public int Id;
        public string Name;

        public Dept(int tenantId, int id, string name)
        {
            TenantId = tenantId;
            Id = id;
            Name = name;
        }

        public Dept(DataRow dataRow)
        {
            TenantId = dataRow["TenantId"].ToInt();
            Id = dataRow["DeptId"].ToInt();
            Name = dataRow["DeptName"].ToString();
        }

        public string ToControl(SiteSettings ss, Column column)
        {
            return Id.ToString();
        }

        public string ToResponse()
        {
            return Id.ToString();
        }

        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () => hb
                .Text(text: Name));
        }

        public string GridText(Column column)
        {
            return Name;
        }

        public string ToExport(Column column, ExportColumn exportColumn = null)
        {
            return Name;
        }

        public bool InitialValue()
        {
            return Id == 0;
        }
    }
}