using Implem.Libraries.Utilities;
using System;

namespace Implem.Libraries.Exceptions
{
    public class ProcessingFailureException : Exception
    {
        public class ResponsesInfo
        {
            public string Text;
            public string Id;
        }

        public readonly ResponsesInfo ResponsesMessage;

        public ProcessingFailureException(string message, string id = "") : base(id + ":" + message)
        {
            ResponsesMessage = new ResponsesInfo { Text = message, Id = id };
        }

        public ProcessingFailureException(string message, string id = "", Exception innerException = null) : base(id + ":" + message, innerException)
        {
            ResponsesMessage = new ResponsesInfo { Text = message, Id = id };
        }

        public string ResponsesMessageJson(string isNullVal = null)
        {
            return ResponsesMessage?.ToJson() ?? isNullVal;
        }
    }
}
