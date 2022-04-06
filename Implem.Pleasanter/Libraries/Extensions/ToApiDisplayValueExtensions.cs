using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToApiDisplayValueExtensions
    {
        public static object ToApiDisplayValue(
            this bool self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self;
        }

        public static object ToApiDisplayValue(
            this bool? self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self;
        }

        public static object ToApiDisplayValue(
            this DateTime self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return column.DisplayControl(
                context: context,
                value: self.ToLocal(context: context));
        }

        public static object ToApiDisplayValue(
            this string value,
            Context context,
            SiteSettings ss,
            Column column,
            string delimiter = ", ",
            ExportColumn.Types type = ExportColumn.Types.Text)
        {
            if (column.HasChoices())
            {
                if (column.MultipleSelections == true)
                {
                    var choiceParts = column.ChoiceParts(
                        context: context,
                        selectedValues: value,
                        type: type);
                    return column.MultipleSelections == true
                        ? choiceParts.Join(delimiter)
                        : choiceParts.FirstOrDefault();
                }
                else
                {
                    column.AddToChoiceHash(
                        context: context,
                        value: value);
                    var choice = column.Choice(
                        value,
                        nullCase: value.IsNullOrEmpty()
                            ? null
                            : "? " + value);
                    return choice.Text;
                }
            }
            else
            {
                return value;
            }
        }

        public static object ToApiDisplayValue(
            this int self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self;
        }

        public static object ToApiDisplayValue(
            this long self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self;
        }

        public static object ToApiDisplayValue(
            this TimeZoneInfo self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self.DisplayName;
        }
    }
}