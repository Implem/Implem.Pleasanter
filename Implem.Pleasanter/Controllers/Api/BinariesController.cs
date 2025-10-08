using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Web;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
namespace Implem.Pleasanter.Controllers.Api
{
    [CheckApiContextAttributes]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class BinariesController : ControllerBase
    {
        [HttpPost("{guid}/Get")]
        public ContentResult Get(string guid)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.ContentType,
                api: true);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? BinaryUtilities.ApiDownload(
                    context: context,
                    guid: guid)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{guid}/getstream")]
        public FileStreamResult GetStream(string guid)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                 sessionStatus: User?.Identity?.IsAuthenticated == true,
                 sessionData: User?.Identity?.IsAuthenticated == true,
                 apiRequestBody: body,
                 contentType: Request.ContentType,
                 api: true);
            var log = new SysLogModel(context: context);
            if (!context.Authenticated)
            {
                return null;
            }
            var file = BinaryUtilities.Download(
                context: context,
                guid: guid.ToUpper());
            if (file == null)
            {
                return null;
            }
            var result = CreateFileSteramResult(file);
            log.Finish(
                context: context,
                responseSize: file?.Length ?? 0);
            return new FileStreamResult(result.FileStream, result.ContentType)
            {
                FileDownloadName = result.FileDownloadName
            };
        }

        private static FileStreamResult CreateFileSteramResult(ResponseFile file)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            var stream = file.IsFileInfo() == true
                ? System.IO.File.OpenRead(file?.FileInfo.FullName)
                : file.FileContentsStream;
            return new FileStreamResult(stream, file.ContentType) { FileDownloadName = file.FileDownloadName };
        }

        [HttpPost("{guid}/upload")]
        [HttpPost("upload")]
        public ContentResult Upload(string guid = null)
        {
            var context = new Context(
                files: Request.Form.Files.ToList(),
                apiRequestBody: new
                {
                    ApiKey = AuthorizationHeaderValue()
                }.ToJson(),
                api: true);
            var log = new SysLogModel(context: context);
            if (!context.Authenticated)
            {
                return ApiResults.Unauthorized(context: context);
            }
            if (context.PostedFiles == null || context.PostedFiles.Count == 0)
            {
                return ApiResults.BadRequest(context: context);
            }
            var postedFile = context.PostedFiles[0];
            var contentRangeHeader = Request.Form.Files[0].Headers.ContentRange.FirstOrDefault();
            var contentRange = BinaryUtilities.GetContentRange(contentRangeHeader: contentRangeHeader);
            postedFile.ContentRange = contentRange;
            var sessionGuid = $"BinariesApiTempGuid_{postedFile.FileName}_{context.QueryStrings.Long("id")}_{guid}".Sha512Cng();
            var tempGuid = contentRange == null || contentRange.From == 0
                ? Strings.NewGuid()
                : context.SessionData.Get(sessionGuid);
            var filePath = string.Empty;
            var uploaded = false;
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
                        return ApiResults.NotFound(context: context);
                    }
                    var targetGuid = context.QueryStrings.Bool("overwrite")
                        ? guid
                        : tempGuid;
                    filePath = BinaryUtilities.GetTempFileInfo(
                        fileUuid: targetGuid,
                        fileName: postedFile.FileName).FullName;
                    using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                    {
                        var saveError = Save(
                            fileStream: fileStream,
                            file: postedFile,
                            range: contentRange);
                        if (saveError != Error.Types.None)
                        {
                            uploaded = true;
                            return ApiResults.Error(
                                context: context,
                                errorData: new ErrorData(type: saveError));
                        }
                        uploaded = Uploaded(
                            context: context,
                            fileStream: fileStream,
                            range: contentRange,
                            sessionGuid: sessionGuid,
                            tempGuid: tempGuid);
                    }
                    if (uploaded)
                    {
                        var invalid = BinaryUtilities.ValidateFileHash(
                            fileInfo: new FileInfo(filePath),
                            contentRange: contentRange,
                            hash: context.Forms.Get("FileHash"));
                        if (invalid != Error.Types.None)
                        {
                            return ApiResults.Error(
                                context: context,
                                errorData: new ErrorData(type: invalid));
                        }
                        context.ApiRequestBody = CreateAttachmentsHashJson(
                            context: context,
                            guidParam: $"{guid},{targetGuid}",
                            referenceId: referenceId,
                            file: postedFile);
                        var response = new ItemModel(
                            context: context,
                            referenceId: referenceId)
                                .UpdateByApi(context: context);
                        log.Finish(
                            context: context,
                            responseSize: response?.Content?.Length ?? 0);
                        return response;
                    }
                    else
                    {
                        var response = ApiResults.Success(
                            id: referenceId,
                            message: guid);
                        log.Finish(
                            context: context,
                            responseSize: response?.Content?.Length ?? 0);
                        return response;
                    }
                }
                else
                {
                    if (context.QueryStrings.Long("id") == 0
                        || !Mime.ValidateOnApi(contentType: context.ContentType))
                    {
                        return ApiResults.BadRequest(context: context);
                    }
                    var targetGuid = tempGuid;
                    filePath = BinaryUtilities.GetTempFileInfo(
                        fileUuid: targetGuid,
                        fileName: postedFile.FileName).FullName;
                    using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                    {
                        var saveError = Save(
                            fileStream: fileStream,
                            file: postedFile,
                            range: contentRange);
                        if (saveError != Error.Types.None)
                        {
                            uploaded = true;
                            return ApiResults.Error(
                                context: context,
                                errorData: new ErrorData(type: saveError));
                        }
                        uploaded = Uploaded(
                            context: context,
                            fileStream: fileStream,
                            range: contentRange,
                            sessionGuid: sessionGuid,
                            tempGuid: tempGuid);
                    }
                    if (uploaded)
                    {
                        var invalid = BinaryUtilities.ValidateFileHash(
                            fileInfo: new FileInfo(filePath),
                            contentRange: contentRange,
                            hash: context.Forms.Get("FileHash"));
                        if (invalid != Error.Types.None)
                        {
                            return ApiResults.Error(
                                context: context,
                                errorData: new ErrorData(type: invalid));
                        }
                        var attachment = Attachment(
                            guidParam: targetGuid,
                            referenceId: context.QueryStrings.Long("id"),
                            file: postedFile);
                        var response = BinaryUtilities.CreateAttachment(
                            context: context,
                            attachment: attachment);
                        log.Finish(
                            context: context,
                            responseSize: response?.Content?.Length ?? 0);
                        return response;
                    }
                    else
                    {
                        var response = ApiResults.Success(
                            id: 0,
                            message: targetGuid);
                        log.Finish(
                            context: context,
                            responseSize: response?.Content?.Length ?? 0);
                        return response;
                    }
                }
            }
            catch
            {
                SessionUtilities.Remove(
                    context: context,
                    key: sessionGuid,
                    page: false);
                throw;
            }
            finally
            {
                if (uploaded)
                {
                    Libraries.DataSources.File.DeleteTemp(
                        context: context,
                        guid: postedFile.Guid);
                    Files.DeleteFile(path: filePath);
                }
            }
        }

        private string AuthorizationHeaderValue()
        {
            var authHeader = (string)Request.Headers["Authorization"];

            if (authHeader != null && authHeader.ToLower().StartsWith("bearer"))
            {
                return authHeader.Substring("bearer ".Length).Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        private string CreateAttachmentsHashJson(Context context, string guidParam, long referenceId, PostedFile file)
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
                            referenceId: referenceId,
                            file: file)
                    }
                }
            }.ToJson();
        }

        private static Attachment Attachment(string guidParam, long referenceId, PostedFile file)
        {
            return new Attachment
            {
                Guid = guidParam,
                Name = file.FileName,
                FileName = file.FileName,
                ReferenceId = referenceId,
                Size = file.ContentRange?.Length ?? file.Size,
                Extension = Path.GetExtension(file.FileName),
                ContentType = file.ContentType,
                Added = true
            };
        }

        private static Error.Types Save(FileStream fileStream, PostedFile file, System.Net.Http.Headers.ContentRangeHeaderValue range)
        {
            if (range != null && range.From != fileStream.Length)
            {
                return Error.Types.InvalidRequest;
            }
            file.InputStream.CopyTo(fileStream);
            return Error.Types.None;
        }

        private bool Uploaded(Context context, FileStream fileStream, System.Net.Http.Headers.ContentRangeHeaderValue range, string sessionGuid, string tempGuid)
        {
            if (range != null && range.Length != fileStream.Length)
            {
                SessionUtilities.Set(
                    context: context,
                    key: sessionGuid,
                    value: tempGuid);
                return false;
            }
            else
            {
                SessionUtilities.Remove(
                    context: context,
                    key: sessionGuid,
                    page: false);
                return true;
            }
        }
    }
}