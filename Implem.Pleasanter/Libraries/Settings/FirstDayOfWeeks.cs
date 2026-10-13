using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class FirstDayOfWeeks
    {
        public const int Inherit = -2;

        public const int Auto = -1;

        private const string InheritText = " ";

        public const string DataAttributeName = "data-firstdayofweek";

        public static int Resolve(
            Context context,
            SiteSettings ss = null)
        {
            var value = Parameters.General.FirstDayOfWeek;
            value = Inherited(
                value: value,
                setting: context?.TenantFirstDayOfWeek);
            value = Inherited(
                value: value,
                setting: ss?.FirstDayOfWeek);
            if (value == Auto || value == Inherit)
            {
                value = context != null
                    ? context.CultureInfo().DateTimeFormat.FirstDayOfWeek.ToInt()
                    : DayOfWeek.Sunday.ToInt();
            }
            return ToDayOfWeek(dayOfWeek: value).ToInt();
        }

        public static DayOfWeek ToDayOfWeek(int dayOfWeek)
        {
            return (DayOfWeek)(((dayOfWeek % 7) + 7) % 7);
        }

        public static string DisplayName(
            Context context,
            int dayOfWeek)
        {
            return Displays.Get(
                context: context,
                id: ToDayOfWeek(dayOfWeek: dayOfWeek).ToString());
        }

        public static Dictionary<string, string> OptionCollection(
            Context context,
            bool inherit)
        {
            var hash = new Dictionary<string, string>();
            if (inherit)
            {
                hash.Add(
                    Inherit.ToString(),
                    InheritText);
            }
            hash.Add(
                Auto.ToString(),
                Displays.Auto(context: context));
            for (var dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
            {
                hash.Add(
                    dayOfWeek.ToString(),
                    DisplayName(
                        context: context,
                        dayOfWeek: dayOfWeek));
            }
            return hash;
        }

        public static Dictionary<string, string> DataAttributes(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> attributes = null)
        {
            attributes = attributes ?? new Dictionary<string, string>();
            attributes[DataAttributeName] = Resolve(
                context: context,
                ss: ss).ToString();
            return attributes;
        }

        private static int Inherited(
            int value,
            int? setting)
        {
            return setting == null || setting == Inherit
                ? value
                : setting.ToInt();
        }
    }
}
