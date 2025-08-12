using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Web;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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
        public string Encoding;
        public FileInfo FileInfo;
        public long FileLength;
        public int ErrorStatusCode;
        public string ErrorMessage;

        public ResponseFile(
            string fileContent,
            string fileDownloadName,
            string contentType = null,
            string encoding = null)
        {
            FileContents = fileContent;
            FileDownloadName = fileDownloadName;
            ContentType = Strings.CoalesceEmpty(contentType, Mime.Type(FileDownloadName));
            Length = fileContent.Length;
            Encoding = encoding;
        }

        public ResponseFile(Stream fileContent, string fileDownloadName, string contentType = null)
        {
            FileContentsStream = fileContent;
            FileDownloadName = fileDownloadName;
            ContentType = Strings.CoalesceEmpty(contentType, Mime.Type(FileDownloadName));
            StreamLength = fileContent.Length;
        }

        public ResponseFile(FileInfo fileContent, string fileDownloadName, string contentType = null)
        {
            FileInfo = fileContent;
            FileDownloadName = fileDownloadName;
            ContentType = Strings.CoalesceEmpty(contentType, Mime.Type(FileDownloadName));
            StreamLength = fileContent.Length;
            FileLength = fileContent.Length;
        }

        public ResponseFile(int errorStatusCode, string errorMessage)
        {
            ErrorStatusCode = errorStatusCode;
            ErrorMessage = errorMessage;
        }

        public FileContentResult ToFile()
        {
            var fileContentResult = new FileContentResult(
                FileContents.ToBytes(Encoding == "Shift-JIS"
                    ? System.Text.Encoding.GetEncoding("Shift_JIS")
                    : null),
                ContentType)
            {
                FileDownloadName = FileDownloadName
            };
            return fileContentResult;
        }

        public ContentResultInheritance ToContentResult(
            long id,
            long referenceId,
            string binaryType,
            string guid,
            string extension,
            long size,
            long creator,
            long updator,
            DateTime createdTime,
            DateTime updatedTime)
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
            if (FileContentsStream == null)
            {
                using (var fs = new FileStream(FileInfo.FullName, FileMode.Open))
                using (var reader = new BinaryReader(fs))
                {
                    return new FileContentResult(reader.ReadBytes((int)fs.Length), ContentType)
                    {
                        FileDownloadName = FileDownloadName
                    };
                }
            }
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
            if (FileInfo != null)
            {
                using (FileContentsStream = new FileStream(FileInfo.FullName, FileMode.Open))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        FileContentsStream.CopyTo(ms);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    FileContentsStream.CopyTo(ms);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public bool IsFileInfo()
        {
            return FileInfo != null;
        }

        public bool IsError()
        {
            return ErrorMessage != null;
        }

        public static ResponseFile Get(ApiResponse apiResponse)
        {
            return new ResponseFile(apiResponse.StatusCode, apiResponse.Message);
        }
    }
}