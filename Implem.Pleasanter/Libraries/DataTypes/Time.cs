using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Data;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Time : IConvertable
    {
        public DateTime Value = 0.ToDateTime();
        public DateTime DisplayValue = 0.ToDateTime();

        public Time()
        {
        }

        public Time(Context context, DataRow dataRow, string name)
        {
            Value = dataRow.DateTime(name);
            DisplayValue = Value.ToLocal(context: context);
        }

        public Time(Context context, DateTime value, bool byForm = false)
        {
            Value = value.InRange()
                ? byForm
                    ? value.ToUniversal(context: context)
                    : value
                : 0.ToDateTime();
            DisplayValue = value;
        }

        public virtual string ToControl(Context context, SiteSettings ss, Column column)
        {
            return Value.InRange()
                ? column.DisplayControl(
                    context: context,
                    value: DisplayValue)
                : string.Empty;
        }

        public virtual string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return Value.InRange()
                ? column.DisplayControl(
                    context: context,
                    value: DisplayValue)
                : string.Empty;
        }

        public override string ToString()
        {
            return Value.InRange()
                ? Value.ToString()
                : string.Empty;
        }

        public bool DifferentDate(Context context)
        {
            return DisplayValue.Date != DateTime.Now.ToLocal(context: context).Date;
        }

        public virtual HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex)
        {
            return hb.Td(
                css: column.CellCss(),
                action: () => hb
                .P(css: "time", action: () => hb
                    .Text(column.DisplayGrid(
                        context: context,
                        value: DisplayValue))));
        }

        public virtual string GridText(Context context, Column column)
        {
            return column.DisplayGrid(
                context: context,
                value: DisplayValue);
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return DisplayValue.Display(
                context: context,
                format: exportColumn?.Format
                    ?? column?.EditorFormat
                    ?? "Ymd");
        }

        public virtual string ToNotice(
            Context context,
            DateTime saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: column.DisplayControl(
                    context: context,
                    value: DisplayValue),
                saved: column.DisplayControl(
                    context: context,
                    value: saved.ToLocal(context: context)),
                column: column,
                updated: updated,
                update: update);
        }

        public bool InitialValue(Context context)
        {
            return Value == 0.ToDateTime();
        }
    }
}