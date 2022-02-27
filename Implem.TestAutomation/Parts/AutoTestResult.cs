using CsvHelper.Configuration;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.TestAutomation.Parts
{
    public class AutoTestResult
    {
        public string Description { get; set; }
        public string ComparisonResult { get; set; }
        public string ConfirmationTarget { get; set; }
        public string ExpectedContent { get; set; }
        public string ExpectedValue { get; set; }
        public string ExecutionValue { get; set; }
    }

    public class AutoTestResultTable : ClassMap<AutoTestResult>
    {
        private AutoTestResultTable()
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            Map(c => c.Description).Index(0).Name(Displays.AutoTestExecutionTestCase(context: context));
            Map(c => c.ComparisonResult).Index(1).Name(Displays.AutoTestComparisonResult(context: context));
            Map(c => c.ConfirmationTarget).Index(2).Name(Displays.AutoTestConfirmationTarget(context: context));
            Map(c => c.ExpectedContent).Index(3).Name(Displays.AutoTestExpectedContent(context: context));
            Map(c => c.ExpectedValue).Index(4).Name(Displays.AutoTestExpectedValue(context: context));
            Map(c => c.ExecutionValue).Index(5).Name(Displays.AutoTestExecutionValue(context: context));
        }
    }
}
