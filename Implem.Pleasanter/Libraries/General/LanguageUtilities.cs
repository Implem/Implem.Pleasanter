using System;

namespace Implem.Pleasanter.Libraries.General
{
    public static class LanguageUtilities
    {
        public static string AcceptLanguageToLanguage(string acceptLanguage)
        {
            var tag = acceptLanguage?
                .Split(',')[0]
                .Split(';')[0]
                .Trim();
            if (string.IsNullOrEmpty(tag)) return null;
            var subtags = tag.Split(
                '-',
                StringSplitOptions.RemoveEmptyEntries);
            if (subtags.Length == 0) return null;
            var language = subtags[0].ToLowerInvariant();
            if (language == "zh")
            {
                for (var i = 1; i < subtags.Length && subtags[i].Length != 1; i++)
                {
                    switch (subtags[i].ToLowerInvariant())
                    {
                        case "hant":
                        case "tw":
                        case "hk":
                        case "mo":
                            return "zh_tw";
                        case "hans":
                            return "zh";
                    }
                }
                return "zh";
            }
            return language switch
            {
                "en" or "ja" or "de" or "ko" or "es" => language,
                "vi" => "vn",
                _ => null,
            };
        }

        public static string LanguageTag(string language)
        {
            return language switch
            {
                "zh_tw" => "zh-TW",
                "vn" => "vi",
                _ => language,
            };
        }
    }
}
