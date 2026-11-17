namespace Implem.ParameterAccessor.Parts
{
    public class Validation
    {
        public const int DefaultServerRegexValidationTimeout = 1000;
        public decimal? MaxLength;
        public string MaxLengthCountType;
        public string SingleByteCharactorRegexClient;
        public string SingleSyteCharactorRegexServer;
        public int ServerRegexValidationTimeout = DefaultServerRegexValidationTimeout;
    }
}