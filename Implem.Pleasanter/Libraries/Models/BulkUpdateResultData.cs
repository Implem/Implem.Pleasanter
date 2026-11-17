using Implem.Pleasanter.Libraries.General;
namespace Implem.Pleasanter.Libraries.Models
{
    public class BulkUpdateResultData
    {
        public int SuccessCount { get; set; }

        public int FailureCount { get; set; }

        public ErrorData ErrorData { get; set; }

        public bool HasError()
        {
            return ErrorData != null
                && ErrorData.Type != Error.Types.None;
        }
    }
}
