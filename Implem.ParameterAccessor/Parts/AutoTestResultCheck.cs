namespace Implem.ParameterAccessor.Parts
{
    public class ResultCheck
    {
        public string ElementId;
        public string ElementXpath;
        public string ElementCss;
        public string ElementLinkText;
        public string ItemId;       
        public string ExpectedValue;
        public string Description;
        public CheckTypes CheckType;
    }

    public enum CheckTypes
    {
        Default,
        Regex,
        ExistanceTrue,
        ExistanceFalse,
        HasClass,
        SelectOptions
    }
}
