namespace Implem.Libraries.Utilities
{
    public static class Decimals
    {
        public static string TrimEndZero(this decimal? self)
        {
            return self.ToDecimal().TrimEndZero();
        }

        public static string TrimEndZero(this decimal self)
        {
            var data = self.ToString();
            return data.Contains(".")
                ? data.TrimEnd('0').EndsWith(".")
                    ? data.TrimEnd('0').TrimEnd('.')
                    : data.TrimEnd('0')
                : data;
        }
    }
}