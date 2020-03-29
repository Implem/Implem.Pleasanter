using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Web;
using System;
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

        public ContentResult ToContentResult(
            long id,
            long referenceId,
            string binaryType,
            string guid,
            string extension,
            long size,
            long creator,
            long updator,
            string createdTime,
            string updatedTime
            )
        {
            return ApiResults.Get(new
            {
                Id = id,
                StatusCode = 200,
                Message = FileDownloadName + "を取得しました。",
                Response = new
                {
                    ReferenceId = referenceId,
                    BinaryType = binaryType,
                    Base64 = GetBase64Content(),
                    Guid = guid,
                    FileName = FileDownloadName,
                    Extension = extension,
                    Size = size,
                    ContentType,
                    Creator = creator,
                    Updator = updator,
                    CreatedTime = createdTime,
                    UpdatedTime = updatedTime
                }
            }.ToJson());
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

        private string GetBase64Content()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                FileContentsStream.CopyTo(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}