using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
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

        public string ToControl(Column column, Permissions.Types pt)
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

        public string ToExport(Column column)
        {
            return Name;
        }
    }
}