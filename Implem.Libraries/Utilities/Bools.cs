namespace Implem.Libraries.Utilities
{
    public static class Bools
    {
        public static string ToOneOrZeroString(this bool self)
        {
            return self ? "1" : "0";
        }
    }
}
