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
using Implem.Pleasanter.Libraries.General;

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
            var files = ToArray();
            var postedFile = files[0];
            var sessionKey = $"BinariesApiTempGuid_{postedFile.FileName}_{context.QueryStrings.Long("id")}_{guid}".Sha512Cng();
            var fileHash = HttpContext.Current.Request.Form["FileHash"];
            var filePath = string.Empty;
            var uploaded = false;
            var targetGuid = string.Empty;
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
                    var tempGuid = context.QueryStrings.Bool("overwrite")
                        ? guid
                        : Strings.NewGuid();
                    var fileData = await GetFileData(guid: tempGuid);
                    var contentRange = fileData.Headers.ContentRange;
                    targetGuid = contentRange == null || contentRange.From == 0
                        ? tempGuid
                        : SessionUtilities.Get(
                            context: context,
                            sessionGuid: fileHash).Get(sessionKey);
                    filePath = GetTempFilePath(
                        fileData: fileData,
                        targetGuid: targetGuid);
                    using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                    {
                        var saveError = await SaveAsync(
                            fileStream: fileStream,
                            file: postedFile,
                            range: contentRange);
                        if (saveError != Error.Types.None)
                        {
                            uploaded = true;
                            return ToHttpResponseMessage(
                                context: context,
                                log: log,
                                result: ApiResults.Error(
                                context: context,
                                errorData: new ErrorData(type: saveError)));
                        }
                        uploaded = Uploaded(
                            context: context,
                            fileStream: fileStream,
                            range: contentRange,
                            sessionKey: sessionKey,
                            tempGuid: tempGuid,
                            sessionGuid: fileHash);
                    }
                    if (uploaded)
                    {
                        var invalid = BinaryUtilities.ValidateFileHash(
                            fileInfo: new FileInfo(filePath),
                            contentRange: contentRange,
                            hash: fileHash);
                        if (invalid != Error.Types.None)
                        {
                            return ToHttpResponseMessage(
                                context: context,
                                log: log,
                                result: ApiResults.Error(
                                    context: context,
                                    errorData: new ErrorData(type: invalid)));
                        }
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
                        var response = ApiResults.Success(
                            id: referenceId,
                            message: guid);
                        return ToHttpResponseMessage(
                            context: context,
                            log: log,
                            result: response);
                    }
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
                    targetGuid = SessionUtilities.Get(
                            context: context,
                            sessionGuid: fileHash).Get(sessionKey) ?? Strings.NewGuid();
                    var fileData = await GetFileData(guid: targetGuid);
                    var contentRange = fileData.Headers.ContentRange;
                    filePath = GetTempFilePath(
                        fileData: fileData,
                        targetGuid: targetGuid);
                    using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                    {
                        var saveError = await SaveAsync(
                            fileStream: fileStream,
                            file: postedFile,
                            range: contentRange);
                        if (saveError != Error.Types.None)
                        {
                            uploaded = true;
                            return ToHttpResponseMessage(
                                context: context,
                                log: log,
                                result: ApiResults.Error(
                                    context: context,
                                    errorData: new ErrorData(type: saveError)));
                        }
                        uploaded = Uploaded(
                            context: context,
                            fileStream: fileStream,
                            range: contentRange,
                            sessionKey: sessionKey,
                            tempGuid: targetGuid,
                            sessionGuid: fileHash);
                    }
                    if (uploaded)
                    {
                        var invalid = BinaryUtilities.ValidateFileHash(
                            fileInfo: new FileInfo(filePath),
                            contentRange: contentRange,
                            hash: fileHash);
                        if (invalid != Error.Types.None)
                        {
                            return ToHttpResponseMessage(
                                context: context,
                                log: log,
                                result: ApiResults.Error(
                                    context: context,
                                    errorData: new ErrorData(type: invalid)));
                        }
                        var attachment = Attachment(
                            guidParam: targetGuid,
                            referenceId: context.QueryStrings.Long("id"),
                            filePath: filePath);
                        var response = BinaryUtilities.CreateAttachment(
                            context: context,
                            attachment: attachment);
                        return ToHttpResponseMessage(
                            context: context,
                            log: log,
                            result: response);
                    }
                    else
                    {
                        var response = ApiResults.Success(
                            id: 0,
                            message: targetGuid);
                        return ToHttpResponseMessage(
                            context: context,
                            log: log,
                            result: response);
                    }
                }
            }
            catch
            {
                SessionUtilities.Remove(
                    context: context,
                    key: sessionKey,
                    page: false,
                    sessionGuid: fileHash);
                throw;
            }
            finally
            {
                if (uploaded)
                {
                    Libraries.DataSources.File.DeleteTemp(guid: targetGuid);
                    Files.DeleteFile(path: filePath);
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

        private async Task<MultipartFileData> GetFileData(string guid)
        {
            var directory = Path.Combine(DefinitionAccessor.Directories.Temp(), guid);
            Directory.CreateDirectory(directory);
            var provider = new MultipartFormDataStreamProvider(directory);
            await Request.Content.ReadAsMultipartAsync(provider);
            return provider.FileData[0];
        }

        private string GetTempFilePath(MultipartFileData fileData, string targetGuid)
        {
            var directory = Path.Combine(
                Path.GetDirectoryName(Path.GetDirectoryName(fileData.LocalFileName)), targetGuid);
            var fileName = fileData.Headers.ContentDisposition.FileName.Replace("\"", "");
            var tempFilePath = Path.Combine(directory, fileName);
            Directory.CreateDirectory(directory);
            return tempFilePath;
        }

        private static async Task<Error.Types> SaveAsync(FileStream fileStream, HttpPostedFileBase file, System.Net.Http.Headers.ContentRangeHeaderValue range)
        {
            if (range != null && range.From != fileStream.Length)
            {
                return Error.Types.InvalidRequest;
            }
            await file.InputStream.CopyToAsync(fileStream);
            return Error.Types.None;
        }

        private bool Uploaded(Context context, FileStream fileStream, System.Net.Http.Headers.ContentRangeHeaderValue range, string sessionKey, string tempGuid, string sessionGuid)
        {
            if (range != null && range.Length != fileStream.Length)
            {
                SessionUtilities.Set(
                    context: context,
                    key: sessionKey,
                    value: tempGuid,
                    sessionGuid: sessionGuid);
                return false;
            }
            else
            {
                SessionUtilities.Remove(
                    context: context,
                    key: sessionKey,
                    page: false,
                    sessionGuid: HttpContext.Current.Request.Form["FileHash"]);
                return true;
            }
        }

        private HttpPostedFileBase[] ToArray()
        {
            var files = new List<HttpPostedFileBase>();
            foreach (string file in HttpContext.Current.Request.Files)
            {
                files.Add(new HttpPostedFileWrapper(HttpContext.Current.Request.Files[file]));
            }
            return files.ToArray();
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