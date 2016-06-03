using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Web;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Responses
{
    public class ResponseFile
    {
        public string FileContents;
        public string ContentType;
        public string FileDownloadName;
        public int Length;

        public ResponseFile(string fileContent, string fileDownloadName)
        {
            FileContents = fileContent;
            FileDownloadName = fileDownloadName;
            ContentType = Mime.Type(FileDownloadName);
            Length = fileContent.Length;
        }

        public FileContentResult ToFile()
        {
            var fileContentResult = new FileContentResult(FileContents.ToBytes(), ContentType)
            {
                FileDownloadName = FileDownloadName
            };
            return fileContentResult;
        }
    }
}