using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Libraries.ViewParts;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
namespace Implem.Pleasanter.Libraries.DataTypes
{
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

        public string ToControl(Column column)
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
                case "gethistories":
                    hb.Text(text: DisplayValue);
                    break;
                default:
                    hb.A(
                        href: Navigations.ItemEdit(Id),
                        text: DisplayValue);
                    break;
            }
        }

        public virtual string ToExport(Column column)
        {
            return Value;
        }
    }

    public class TitleBody : Title, IConvertable
    {
        public string Body = string.Empty;

        public TitleBody()
        {
        }

        public TitleBody(long id, string title, string displayValue, string body)
        {
            Id = id;
            Value = title;
            DisplayValue = displayValue;
            Body = body;
        }

        public override string ToString()
        {
            return Value + "\r\n" + Body;
        }

        public override HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () => hb
                .Div(css: "grid-title-body", action: () => hb
                    .P(css: "title", action: () => TdTitle(hb, column))
                    .P(css: "body markup", action: () => hb
                         .Text(text: Body))));
        }

        public override string ToExport(Column column)
        {
            return ToString();
        }
    }

    public static class TitleUtility
    {
        public static string DisplayValue(SiteSettings siteSettings, DataRow dataRow)
        {
            switch (siteSettings.ReferenceType)
            {
                case "Issues": return IssuesUtility.TitleDisplayValue(siteSettings, dataRow);
                case "Results": return ResultsUtility.TitleDisplayValue(siteSettings, dataRow);
                case "Wikis": return WikisUtility.TitleDisplayValue(siteSettings, dataRow);
                default: return string.Empty;
            }
        }
    }
}
