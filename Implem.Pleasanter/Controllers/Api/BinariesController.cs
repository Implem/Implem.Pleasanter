using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Http;
using System.Net.Http;
using Implem.Pleasanter.Libraries.Responses;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.IO;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using System.Collections.Generic;
using System.Web;

namespace Implem.Pleasanter.Controllers.Api
{
    [AllowAnonymous]
    public class BinariesController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Get(string guid)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(apiRequestBody: body);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? BinaryUtilities.ApiDonwload(
                    context: context,
                    guid: guid)
                : ApiResults.Unauthorized(context: context);
            log.Finish(
                context: context,
                responseSize: result?.Content.Length ?? 0);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Upload(string guid)
        {
            var context = new Context(
                apiRequestBody: new Libraries.Requests.Api()
                {
                    ApiKey = Request.Headers.Authorization?.Parameter
                }.ToJson());
            var log = new SysLogModel(context: context);
            if (!Request.Content.IsMimeMultipartContent()
                || Request.Headers.Authorization?.Scheme?.ToLower() != "bearer")
            {
                return ToHttpResponseMessage(context, log, ApiResults.BadRequest(context: context));
            }
            if (!context.Authenticated)
            {
                return ToHttpResponseMessage(context, log, ApiResults.Unauthorized(context: context));
            }
            guid = guid.ToUpper();
            var referenceId = FileContentResults.GetReferenceId(context: context, guid: guid);
            if (referenceId == 0)
            {
                return ToHttpResponseMessage(context, log, ApiResults.NotFound(context: context));
            }
            string filePath = string.Empty;
            try
            {
                var fileData = await SaveFileToTemp(guid);
                var fileName = fileData.Headers.ContentDisposition.FileName.Replace("\"", "");
                filePath = Path.Combine(Path.GetDirectoryName(fileData.LocalFileName), fileName);
                var size = new FileInfo(fileData.LocalFileName).Length;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Move(fileData.LocalFileName, filePath);
                context.ApiRequestBody = CreateAttachmentsHashJson(guid, context, fileName, size);
                var response = new ItemModel(context: context, referenceId: referenceId)
                    .UpdateByApi(context: context);
                return ToHttpResponseMessage(context, log, response);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetStream(string guid)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(apiRequestBody: body);
            var log = new SysLogModel(context: context);
            if (!context.Authenticated)
            {
                return ToHttpResponseMessage(context, log, ApiResults.Unauthorized(context: context));
            }
            var file = BinaryUtilities.Donwload(
                context: context,
                guid: guid.ToUpper());
            if (file == null)
            {
                return ToHttpResponseMessage(context, log, ApiResults.NotFound(context: context));
            }
            HttpResponseMessage response = CreateStreamContentResponse(file);
            log.Finish(
                context: context,
                responseSize: file?.Length ?? 0);
            return response;
        }

        private static HttpResponseMessage CreateStreamContentResponse(ResponseFile file)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            var stream = file.IsFileInfo() == true
                ? File.OpenRead(file?.FileInfo.FullName)
                : file.FileContentsStream;
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = file.FileDownloadName
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            response.Content.Headers.ContentLength = stream.Length;
            return response;
        }

        private string CreateAttachmentsHashJson(string guid, Context context, string fileName, long size)
        {
            return new
            {
                AttachmentsHash = new Dictionary<string, Attachment[]>
                {
                    ["Attachments#Uploading"] = new Attachment[]
                    {
                        new Attachment
                        {
                            Guid=guid,
                            Name = fileName,
                            FileName =fileName,
                            Size = size,
                            Extention = Path.GetExtension(fileName),
                            ContentType = MimeMapping.GetMimeMapping(fileName),
                            Added = true
                        }
                    }
                }
            }.ToJson();
        }

        private async Task<MultipartFileData> SaveFileToTemp(string guid)
        {
            var directory = Path.Combine(DefinitionAccessor.Directories.Temp(), guid);
            Directory.CreateDirectory(directory);
            var provider = new MultipartFormDataStreamProvider(directory);
            await Request.Content.ReadAsMultipartAsync(provider);
            var fileData = provider.FileData[0];
            return fileData;
        }

        private HttpResponseMessage ToHttpResponseMessage(Context context, SysLogModel log, System.Web.Mvc.ContentResult result)
        {
            log.Finish(
                context: context,
                responseSize: result?.Content.Length ?? 0);
            return result.ToHttpResponse(Request);
        }
    }
}