using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.Models
{
    public class ImportResultData
    {
        public int InsertCount { get; set; }

        public int UpdateCount { get; set; }

        public ErrorData ErrorData { get; set; }

        public Message CustomMessage { get; set; }

        public bool HasError()
        {
            return CustomMessage != null
                || (ErrorData != null
                    && ErrorData.Type != Error.Types.None);
        }
    }
}