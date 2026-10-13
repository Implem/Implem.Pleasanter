using Implem.Pleasanter.Libraries.General;
namespace Implem.Pleasanter.Libraries.Models
{
    public class BulkDeleteResultData
    {
        public int Count { get; set; }

        public ErrorData ErrorData { get; set; }

        public bool HasError()
        {
            return ErrorData != null
                && ErrorData.Type != Error.Types.None;
        }
    }
}