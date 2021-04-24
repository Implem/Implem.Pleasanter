namespace Implem.ParameterAccessor.Parts
{
    public class ResultCheck
    {
        public string ResultElementId;
        public string ResultElementXpath;
        public string ResultElementLinkText;
        public string ResultItemId;       
        public string ResultExpectedValue;
        public string ResultDescription;
        public CheckType ResultCheckType;
    }

    public enum CheckType
    {
        Default,
        Regex
    }
}
