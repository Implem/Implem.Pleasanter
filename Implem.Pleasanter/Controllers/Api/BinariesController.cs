using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
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
                contentType: Request.ContentType);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? BinaryUtilities.ApiDonwload(
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
                 contentType: Request.ContentType);
            var log = new SysLogModel(context: context);
            if (!context.Authenticated)
            {
                return null;
            }
            var file = BinaryUtilities.Donwload(
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
                }.ToJson());
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
                        return ApiResults.NotFound(context: context);
                    }
                    var targetGuid = context.QueryStrings.Bool("overwrite")
                        ? guid
                        : Strings.NewGuid();
                    filePath = SaveFileToTemp(
                        guid: targetGuid,
                        file: postedFile);
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
                    if (context.QueryStrings.Long("id") == 0
                        || !Mime.ValidateOnApi(contentType: context.ContentType))
                    {
                        return ApiResults.BadRequest(context: context);
                    }
                    var targetGuid = Strings.NewGuid();
                    filePath = SaveFileToTemp(
                        guid: targetGuid,
                        file: postedFile);
                    var attachment = Attachment(
                        guidParam: targetGuid,
                        referenceId: context.QueryStrings.Long("id"),
                        file: postedFile);
                    var response = attachment.Create(context: context);
                    log.Finish(
                        context: context,
                        responseSize: response?.Content?.Length ?? 0);
                    return response;
                }
            }
            finally
            {
                Files.DeleteFile(filePath);
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
                Size = file.Size,
                Extention = Path.GetExtension(file.FileName),
                ContentType = file.ContentType,
                Added = true
            };
        }

        private string SaveFileToTemp(string guid, PostedFile file)
        {
            var directory = Path.Combine(DefinitionAccessor.Directories.Temp(), guid);
            Directory.CreateDirectory(directory);
            var tempFilePath = Path.Combine(directory, file.FileName);
            using (var fileStream = new FileStream(
                tempFilePath,
                FileMode.Create,
                FileAccess.Write))
            {
                file.InputStream.CopyTo(fileStream);
            }
            return tempFilePath;
        }
    }
}