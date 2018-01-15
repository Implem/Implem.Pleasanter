using System;
using System.Globalization;
using System.Linq;
namespace Implem.Libraries.Utilities
{
    public static class Strings
    {
        public static string MaxLength(this string self, int maxLength)
        {
            return self?.Length > maxLength
                ? self.Substring(0, maxLength)
                : self ?? string.Empty;
        }

        public static bool IsNullOrEmpty(this string self) 
        {
            return string.IsNullOrEmpty(self); 
        }

        public static string IsNotEmpty(this string self, string trueStr = null)
        {
            return !self.IsNullOrEmpty()
                ? trueStr != null
                    ? trueStr
                    : self
                : string.Empty;
        }

        public static string NewGuid()
        {
            return Guid.NewGuid()
                .ToString()
                .Replace("-", string.Empty)
                .ToUpper();
        }

        public static string Tab(int number)
        {
            return ("{0, " + (number * 4).ToString() + "}").Params("");
        }

        public static string ToLowerFirstChar(this string self)
        {
            var strTemp = self;
            return !self.IsNullOrEmpty()
                ? strTemp.Length == 1
                    ? strTemp = strTemp.ToLower()
                    : strTemp = char.ToLower(strTemp[0]) + strTemp.Substring(1)
                : strTemp;
        }

        public static string ToUpperFirstChar(this string self) 
        {
            return !self.IsNullOrEmpty()
                ? self.Length == 1
                    ? self.ToUpper()
                    : char.ToUpper(self[0]) + self.Substring(1)
                : self;
        }

        public static string CutBracket(this string self, string bracket)
        {
            return self.StartsWith(bracket) && self.EndsWith(bracket)
                ? self.Substring(1, self.Length - 2)
                : self;
        }

        public static string CoalesceEmpty(params string[] args)
        {
            return args
                .Where(o => !o.IsNullOrEmpty())
                .FirstOrDefault() ?? string.Empty;
        }

        public static string NoSpace(this string self, bool noSpace = true)
        {
            return noSpace
                ? self.SplitReturn().Select(o => o.Trim()).Join(string.Empty)
                : self;
        }

        public static string Params(this string format, params object[] args)
        {
            return args.Any()
                ? string.Format(format, args)
                : string.Empty;
        }

        public static int IndexOf(this string self, string value, CompareOptions compareOptions)
        {
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(self, value, compareOptions);
        }

        public static string EnclosedString(string self)
        {
            var started = false;
            var openCount = 0;
            for (var count = 0; count < self.Length; count++)
            {
                switch (self[count])
                {
                    case '(': 
                        openCount++; 
                        started = true; 
                        break;
                    case ')': 
                        openCount--;
                        if (started && openCount == 0)
                        {
                            return self.Substring(0, count + 1);
                        }
                        break;
                }

            }
            return self;
        }

        public static string ChangePrefixNumber(this string self, int number)
        {
            var before = self.RegexFirst("[0-9]+$");
            return before != string.Empty
                ? self.Substring(0, self.Length - before.Length) + (before.ToInt() + number)
                : self + number;
        }
    }
}
