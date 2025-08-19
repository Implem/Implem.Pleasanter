using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
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

        public string Title(Context context)
        {
            return SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .SiteMenu
                .Get(SiteId)?
                .Title ?? string.Empty;
        }

        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return Title(context: context);
        }

        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return Title(context: context);
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column)
        {
            return Title(context: context);
        }

        public string ToLookup(Context context, SiteSettings ss, Column column, Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return SiteId.ToString();
            }
        }

        public virtual HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn)
        {
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                attributes: new HtmlAttributes()
                    .DataCellSticky(column.CellSticky)
                    .DataCellWidth(column.CellWidth),
                action: () => hb
                    .P(action: () => hb
                        .Text(Title(context: context))));
        }

        public virtual string GridText(Context context, Column column)
        {
            return Title(context: context);
        }

        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column)
        {
            return Title(context: context);
        }

        public object ToApiValue(Context context, SiteSettings ss, Column column)
        {
            return Title(context: context);
        }

        public virtual string ToExport(
            Context context, Column column, ExportColumn exportColumn = null)
        {
            return Title(context: context);
        }

        public bool InitialValue(Context context)
        {
            return SiteId == 0;
        }
    }
}
