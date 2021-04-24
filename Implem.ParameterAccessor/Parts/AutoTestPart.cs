using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class TestPart
    {
        public string TestPartId;
        public string TargetElementId;
        public string TargetElementXpath;
        public string TargetElementLinkText;
        public string TargetUrl;
        public ActionType Action;
        public string Value;
        public List<ResultCheck> Results;
        public List<TestInput> Inputs;
        public string CreatedTabeId;
        public string TargetTestPartId;
        public string PartDescription;
    }

    public enum ActionType
    {
        Input,
        Inputs,
        Click,
        ClickSettingsMenu,
        Hover,
        Clear,
        Create,
        Update,
        Delete,
        Copy,
        Select,
        AlertAccept,
        AlertDismiss,
        ResultCheck,
        GoToUrl
    }
}
