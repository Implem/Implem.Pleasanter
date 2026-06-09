using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using MimeKit;
using System;
using System.Linq;
using System.Text;
namespace Implem.Pleasanter.Libraries.Mails
{
    public static class MailEncodings
    {
        public static Encoding GetEncodingOrDefault(Context context, string encoding)
        {
            if (encoding == null)
            {
                return Encoding.UTF8;
            }
            var encodingInfo = Encoding.GetEncodings()
                .FirstOrDefault(o => o.Name == encoding
                    || o.DisplayName == encoding
                    || o.CodePage.ToString() == encoding);
            if (encodingInfo == null)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(GetEncodingOrDefault),
                    message: $"{encoding} is not supported Encoding. Falling back to UTF-8.",
                    errStackTrace: $"Supported Encodings are {Encoding.GetEncodings().Select(o => o.Name).Join(",")}.",
                    sysLogType: SysLogModel.SysLogTypes.Exception);
                return Encoding.UTF8;
            }
            return Encoding.GetEncoding(encodingInfo.Name);
        }

        public static ContentEncoding GetContentEncodingForTransfer(
            Encoding encoding, Implem.ParameterAccessor.Parts.Types.ContentEncodings? contentEncoding)
        {
            if (encoding == Encoding.UTF8 || contentEncoding == null)
            {
                return ContentEncoding.Default;
            }
            if (Enum.TryParse(contentEncoding.ToString(), out ContentEncoding type)
                && Enum.IsDefined(typeof(ContentEncoding), type))
            {
                return type;
            }
            return ContentEncoding.Default;
        }
    }
}