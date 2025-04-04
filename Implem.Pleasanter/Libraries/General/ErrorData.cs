using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.General
{
    public class ErrorData
    {
        public Error.Types Type;
        public long Id;
        public string ColumnName;
        public string[] Data;

        public ErrorData(
            Context context,
            Error.Types type,
            bool api = false,
            int sysLogsStatus = 200,
            string sysLogsDescription = null,
            long id = 0,
            string columnName = null,
            params string[] data)
        {
            Type = type;
            Id = id;
            ColumnName = columnName;
            Data = data;
            context.Api = api;
            context.SysLogsStatus = sysLogsStatus;
            if (sysLogsStatus != 200) {
                context.SysLogsDescription = $"{sysLogsDescription}:{Message(context: context)?.Text}";
            }
        }

        public ErrorData(
            Error.Types type,
            long id = 0,
            string columnName = null,
            params string[] data)
        {
            Type = type;
            Id = id;
            ColumnName = columnName;
            Data = data;
        }

        public Message Message(Context context)
        {
            return Type.Message(
                context: context,
                data: Data);
        }

        public string MessageJson(Context context)
        {
            return Type.MessageJson(
                context: context,
                data: Data);
        }

        public string SdMessageJson(Context context)
        {
            return Type.SdMessageJson(
                context: context,
                data: Data);
        }
    }
}