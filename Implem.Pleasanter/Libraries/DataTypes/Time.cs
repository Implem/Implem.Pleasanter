using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Data;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Time : IConvertable
    {
        public DateTime Value;
        public DateTime DisplayValue;

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
            Value = byForm
                ? value.ToUniversal()
                : value;
            DisplayValue = value;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            DisplayValue = Value.ToLocal();
        }

        public virtual string ToControl(Column column, Permissions.Types pt)
        {
            return Value.InRange()
                ? DisplayValue.ToControl(column, pt)
                : string.Empty;
        }

        public virtual string ToResponse()
        {
            return Value.InRange()
                ? DisplayValue.ToString()
                : string.Empty;
        }

        public override string ToString()
        {
            return Value.InRange() 
                ? Value.ToString() 
                : string.Empty;
        }

        public virtual string ToViewText(string format = "")
        {
            return Value.InRange() 
                ? DisplayValue.ToString(format, Sessions.CultureInfo())
                : string.Empty;
        }

        public bool DifferentDate()
        {
            return DisplayValue.Date != DateTime.Now.ToLocal().Date;
        }

        public virtual HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () => hb
                .P(css: "time", action: () => hb
                    .Text(column.DisplayGrid(DisplayValue))));
        }

        public string ToExport(Column column)
        {
            return column.DisplayExport(DisplayValue);
        }

        public string ToNotice(
            DateTime saved,
            Column column,
            bool updated,
            bool update)
        {
            return column.DisplayExport(DisplayValue).ToNoticeLine(
                column.DisplayExport(saved.ToLocal()),
                column,
                updated,
                update);
        }
    }
}