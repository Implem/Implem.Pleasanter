using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class TestPart
    {
        public string TestPartId;
        public string ElementId;
        public string ElementXpath;
        public string ElementCss;
        public string ElementLinkText;
        public string Url;
        public ActionTypes Action;
        public string Value;
        public List<ResultCheck> Results;
        public List<TestInput> Inputs;
        public string CreatedTabeId;
        public string TargetTestPartId;
        public string PartDescription;
        public int? WaitTime;
    }

    public enum ActionTypes
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
        GoToUrl,
        UploadFile
    }
}
