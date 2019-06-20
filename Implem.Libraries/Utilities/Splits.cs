namespace Implem.Libraries.Utilities
{
    public static class Splits
    {
        public static string[] SplitReturn(this string self)
        {
            return self != null
                ? self.Replace("\r\n", "\n").Split('\n')
                : new string[] { };
        }

        public static string SplitNo(string str, char delimiter, int index)
        {
            var data = str.Split(delimiter);
            return data.Length > index
                ? data[index]
                : string.Empty;
        }

        public static string Split_1st(this string str, char delimiter = ',')
        {
            return SplitNo(str, delimiter, 0);
        }

        public static string Split_2nd(this string str, char delimiter = ',')
        {
            return SplitNo(str, delimiter, 1);
        }

        public static string Split_3rd(this string str, char delimiter = ',')
        {
            return SplitNo(str, delimiter, 2);
        }

        public static string Split_4th(this string str, char delimiter = ',')
        {
            return SplitNo(str, delimiter, 3);
        }

        public static string Split_5th(this string str, char delimiter = ',')
        {
            return SplitNo(str, delimiter, 4);
        }
    }
}
