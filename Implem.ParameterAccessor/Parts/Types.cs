namespace Implem.ParameterAccessor.Parts
{
    public static class Types
    {
        public enum OptionTypes : int
        {
            On = 0,
            Off = 1,
            Disabled = 2
        }

        public enum ContentEncodings : int
        {
            Default = 0,
            SevenBit = 1,
            EightBit = 2,
            Binary = 3,
            Base64 = 4,
            QuotedPrintable = 5,
            UUEncode = 6
        }
    }
}
