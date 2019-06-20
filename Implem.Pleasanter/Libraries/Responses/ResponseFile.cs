using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Web;
using System.IO;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Responses
{
    public class ResponseFile
    {
        public string FileContents;
        public Stream FileContentsStream;
        public string ContentType;
        public string FileDownloadName;
        public int Length;
        public long StreamLength;

        public ResponseFile(string fileContent, string fileDownloadName, string contentType = null)
        {
            FileContents = fileContent;
            FileDownloadName = fileDownloadName;
            ContentType = Strings.CoalesceEmpty(contentType, Mime.Type(FileDownloadName));
            Length = fileContent.Length;
        }

        public ResponseFile(Stream fileContent, string fileDownloadName, string contentType = null)
        {
            FileContentsStream = fileContent;
            FileDownloadName = fileDownloadName;
            ContentType = Strings.CoalesceEmpty(contentType, Mime.Type(FileDownloadName));
            StreamLength = fileContent.Length;
        }

        public FileContentResult ToFile()
        {
            var fileContentResult = new FileContentResult(FileContents.ToBytes(), ContentType)
            {
                FileDownloadName = FileDownloadName
            };
            return fileContentResult;
        }

        public FileContentResult FileStream()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                FileContentsStream.CopyTo(ms);
                return new FileContentResult(ms.ToArray(), ContentType)
                {
                    FileDownloadName = FileDownloadName
                };
            }
        }
    }
}