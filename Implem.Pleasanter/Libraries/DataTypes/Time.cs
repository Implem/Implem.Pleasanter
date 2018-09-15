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
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Time : IConvertable
    {
        public DateTime Value = 0.ToDateTime();
        public DateTime DisplayValue = 0.ToDateTime();

        public Time()
        {
        }

        public Time(DataRow dataRow, string name)
        {
            Value = dataRow.DateTime(name);
            DisplayValue = Value.ToLocal();
        }

        public Time(DateTime value, bool byForm = false)
        {
            Value = value.InRange()
                ? byForm
                    ? value.ToUniversal()
                    : value
                : 0.ToDateTime();
            DisplayValue = value;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            DisplayValue = Value.ToLocal();
        }

        public virtual string ToControl(Context context, SiteSettings ss, Column column)
        {
            return Value.InRange()
                ? column.DisplayControl(DisplayValue)
                : string.Empty;
        }

        public virtual string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return Value.InRange()
                ? column.DisplayControl(DisplayValue)
                : string.Empty;
        }

        public override string ToString()
        {
            return Value.InRange()
                ? Value.ToString()
                : string.Empty;
        }

        public bool DifferentDate()
        {
            return DisplayValue.Date != DateTime.Now.ToLocal().Date;
        }

        public virtual HtmlBuilder Td(HtmlBuilder hb, Context context, Column column)
        {
            return hb.Td(action: () => hb
                .P(css: "time", action: () => hb
                    .Text(column.DisplayGrid(DisplayValue))));
        }

        public virtual string GridText(Context context, Column column)
        {
            return column.DisplayGrid(DisplayValue);
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return DisplayValue.Display(
                exportColumn?.Format ??
                column?.EditorFormat ??
                "Ymd");
        }

        public virtual string ToNotice(
            Context context,
            DateTime saved,
            Column column,
            bool updated,
            bool update)
        {
            return column.DisplayControl(DisplayValue).ToNoticeLine(
                context: context,
                saved: column.DisplayControl(saved.ToLocal()),
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