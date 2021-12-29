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
using System.Linq;
using Implem.Pleasanter.Libraries.Web;

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
        public async Task<HttpResponseMessage> Upload(string guid = null)
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
                return ToHttpResponseMessage(
                    context: context,
                    log: log,
                    result: ApiResults.BadRequest(context: context));
            }
            if (!context.Authenticated)
            {
                return ToHttpResponseMessage(
                    context: context,
                    log: log,
                    result: ApiResults.Unauthorized(context: context));
            }
            string filePath = string.Empty;
            try
            {
                if (!guid.IsNullOrEmpty())
                {
                    guid = guid.ToUpper();
                    var referenceId = FileContentResults.GetReferenceId(
                        context: context,
                        guid: guid);
                    if (referenceId == 0)
                    {
                        return ToHttpResponseMessage(
                            context: context,
                            log: log,
                            result: ApiResults.NotFound(context: context));
                    }
                    var targetGuid = context.QueryStrings.Bool("overwrite")
                        ? guid
                        : Strings.NewGuid();
                    filePath = await SaveFileToTemp(guid: targetGuid);
                    context.ApiRequestBody = CreateAttachmentsHashJson(
                        context: context,
                        guidParam: $"{guid},{targetGuid}",
                        referenceId: referenceId,
                        filePath: filePath);
                    var response = new ItemModel(
                        context: context,
                        referenceId: referenceId)
                            .UpdateByApi(context: context);
                    return ToHttpResponseMessage(
                        context: context,
                        log: log,
                        result: response);
                }
                else
                {
                    if (context.QueryStrings.Long("id") == 0
                        || !Mime.ValidateOnApi(contentType: context.ContentType))
                    {
                        return ToHttpResponseMessage(
                            context: context,
                            log: log,
                            result: ApiResults.BadRequest(context: context));
                    }
                    var targetGuid = Strings.NewGuid();
                    filePath = await SaveFileToTemp(guid: targetGuid);
                    var attachment = Attachment(
                        guidParam: targetGuid,
                        referenceId: context.QueryStrings.Long("id"),
                        filePath: filePath);
                    var response = attachment.Create(context: context);
                    return ToHttpResponseMessage(
                        context: context,
                        log: log,
                        result: response);
                }
            }
            finally
            {
                Files.DeleteFile(filePath);
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
                return ToHttpResponseMessage(
                    context: context,
                    log: log,
                    result: ApiResults.Unauthorized(context: context));
            }
            var file = BinaryUtilities.Donwload(
                context: context,
                guid: guid.ToUpper());
            if (file == null)
            {
                return ToHttpResponseMessage(
                    context: context,
                    log: log,
                    result: ApiResults.NotFound(context: context));
            }
            HttpResponseMessage response = CreateStreamContentResponse(file: file);
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

        private string CreateAttachmentsHashJson(Context context, string guidParam, string filePath, long referenceId)
        {
            return new
            {
                VerUp = context.QueryStrings.ContainsKey("verup")
                    ? context.QueryStrings.Bool("verup")
                    : (bool?)null,
                AttachmentsHash = new Dictionary<string, Attachment[]>
                {
                    ["Attachments#Uploading"] = new Attachment[]
                    {
                        Attachment(
                            guidParam: guidParam,
                            filePath: filePath,
                            referenceId: referenceId)
                    }
                }}.ToJson();
        }

        private static Attachment Attachment(string guidParam, string filePath, long referenceId)
        {
            var fileName = Path.GetFileName(filePath);
            return new Attachment
            {
                Guid = guidParam,
                Name = fileName,
                FileName = fileName,
                ReferenceId = referenceId,
                Size = new FileInfo(filePath).Length,
                Extention = Path.GetExtension(fileName),
                ContentType = MimeMapping.GetMimeMapping(fileName),
                Added = true
            };
        }

        private async Task<string> SaveFileToTemp(string guid)
        {
            var directory = Path.Combine(DefinitionAccessor.Directories.Temp(), guid);
            Directory.CreateDirectory(directory);
            var provider = new MultipartFormDataStreamProvider(directory);
            await Request.Content.ReadAsMultipartAsync(provider);
            var fileData = provider.FileData[0];
            var fileName = fileData.Headers.ContentDisposition.FileName.Replace("\"", "");
            var tempFilePath = Path.Combine(
                Path.GetDirectoryName(fileData.LocalFileName), fileName);
            Files.DeleteFile(tempFilePath);
            File.Move(fileData.LocalFileName, tempFilePath);
            return tempFilePath;
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