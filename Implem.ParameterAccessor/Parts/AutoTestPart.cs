using Implem.DefinitionAccessor;
using System.Collections.Generic;
using System.Linq;
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
        public string Description;
        public bool AutoPostBack;
        public int? WaitTime;

        public string RecordId()
        {
            return Parameters.ExtendedAutoTestOperations
                .SelectMany(autoTestOperation => autoTestOperation.TestParts
                    .Where(autoTestPart => autoTestPart.TestPartId == TargetTestPartId)
                    .Select(autoTestPart => autoTestPart.CreatedTabeId)).ToList()
                        .FirstOrDefault(p => p != null);
        }
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
        UploadFile,
        Execute,
    }
}
