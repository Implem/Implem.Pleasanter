using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    [Serializable]
    public class SiteTitle : IConvertable
    {
        public long SiteId;

        public SiteTitle(long siteId)
        {
            SiteId = siteId;
        }

        public string ToControl(SiteSettings ss, Column column)
        {
            return ToString();
        }

        public string ToResponse()
        {
            return ToString();
        }

        public override string ToString()
        {
            return SiteInfo.TenantCaches.Get(Sessions.TenantId())?
                .SiteMenu.Get(SiteId)?.Title ?? string.Empty;
        }

        public virtual HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () => hb
                .P(action: () => TdTitle(hb, column)));
        }

        protected void TdTitle(HtmlBuilder hb, Column column)
        {
            hb.Text(text: ToString());
        }

        public virtual string GridText(Column column)
        {
            return ToString();
        }

        public virtual string ToExport(Column column, ExportColumn exportColumn = null)
        {
            return ToString();
        }

        public bool InitialValue()
        {
            return SiteId == 0;
        }
    }
}
