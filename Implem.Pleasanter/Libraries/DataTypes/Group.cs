using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;
using System.Text.RegularExpressions;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Group : IConvertable
    {
        public int TenantId;
        public int Id;
        public string Name;
        public string Body;
        public bool Disabled;

        public Group()
        {
        }

        public Group(DataRow dataRow)
        {
            TenantId = dataRow.Int("TenantId");
            Id = dataRow.Int("GroupId");
            Name = dataRow.String("GroupName");
            Body = dataRow.String("Body");
            Disabled = dataRow.Bool("Disabled");
        }

        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return Id.ToString();
        }

        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return Id.ToString();
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column)
        {
            return Name;
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
                    return Id.ToString();
            }
        }

        public HtmlBuilder Td(
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
                    .Text(text: Name));
        }

        public string GridText(Context context, Column column)
        {
            return Name;
        }

        public string SelectableText(Context context, string format)
        {
            var value = format;
            foreach (Match match in format.RegexMatches("\\[[A-Za-z]+?\\]"))
            {
                switch (match.Value)
                {
                    case "[Group]":
                        value = value.Replace(match.Value, Displays.Groups(context: context));
                        break;
                    case "[GroupId]":
                        value = value.Replace(match.Value, Id > 0
                            ? Id.ToString()
                            : string.Empty);
                        break;
                    case "[GroupName]":
                        value = value.Replace(match.Value, Name);
                        break;
                    case "[Body]":
                        value = value.Replace(match.Value, Body);
                        break;
                }
            }
            return value;
        }

        public string Tooltip()
        {
            return Body;
        }

        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column)
        {
            var name = SiteInfo.Name(
                context: context,
                id: Id,
                type: Column.Types.Group);
            return !name.IsNullOrEmpty()
                ? name
                : null;
        }

        public object ToApiValue(Context context, SiteSettings ss, Column column)
        {
            return Id;
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