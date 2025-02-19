using Implem.Libraries.Utilities;
using System;

namespace Implem.Libraries.Exceptions
{
    // Implem.Pleasanter.Libraries.Responses.MessageのText(言語変換後)とIdを格納し、throwで処理を抜けるためのクラス
    // 本来は Messages.BadRequest 等の単位で例外クラスを生成すべきだが、catch側でクラス毎に処理を分けないので共通クラスとする。
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
