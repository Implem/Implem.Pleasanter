using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToLookupExtensions
    {
        public static string ToLookup(
            this bool self,
            Context context,
            SiteSettings ss,
            Column column,
            Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return self.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return self.ToString();
            }
        }

        public static string ToLookup(
            this DateTime self,
            Context context,
            SiteSettings ss,
            Column column,
            Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return self.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return self.ToControl(
                        context: context,
                        ss: ss,
                        column: column);
            }
        }

        public static string ToLookup(
            this string self,
            Context context,
            SiteSettings ss,
            Column column,
            Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return self.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return self;
            }
        }

        public static string ToLookup(
            this int self,
            Context context,
            SiteSettings ss,
            Column column,
            Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return self.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return self.ToString();
            }
        }

        public static string ToLookup(
            this long self,
            Context context,
            SiteSettings ss,
            Column column,
            Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return self.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return self.ToString();
            }
        }
    }
}