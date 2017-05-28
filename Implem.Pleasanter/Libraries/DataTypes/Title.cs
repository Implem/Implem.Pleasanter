using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    [Serializable]
    public class Title : IConvertable
    {
        public long Id;
        public string Value = string.Empty;
        public string DisplayValue;
        public List<string> PartCollection;

        public Title()
        {
        }

        public Title(DataRow dataRow, string name)
        {
            Id = dataRow.Long(name);
            Value = dataRow.String("Title");
        }

        public Title(long id, string value)
        {
            Id = id;
            Value = value;
        }

        public Title(string value)
        {
            Value = value;
        }

        public string ToControl(SiteSettings ss, Column column)
        {
            return Value;
        }

        public string ToResponse()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public virtual HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () => hb
                .P(action: () => TdTitle(hb, column)));
        }

        protected void TdTitle(HtmlBuilder hb, Column column)
        {
            switch (Url.RouteData("action").ToLower())
            {
                case "histories":
                    hb.Text(text: DisplayValue);
                    break;
                default:
                    hb.A(
                        href: Locations.ItemEdit(Id) + "?back=1",
                        text: DisplayValue);
                    break;
            }
        }

        public virtual string GridText(Column column)
        {
            return DisplayValue;
        }

        public virtual string ToExport(Column column)
        {
            return Value;
        }

        public string ToNotice(
            string saved,
            Column column,
            bool updated,
            bool update)
        {
            return Value.ToNoticeLine(
                saved,
                column,
                updated,
                update);
        }
    }
}
