using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Net.Http;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.IO;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using Implem.Pleasanter.Libraries.DataTypes;
using System;
using Implem.Pleasanter.Libraries.Web;

namespace Implem.Pleasanter.Controllers.Api
{
    public class BinariesController
    {
        public ContentResult Get(Context context, string guid)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? BinaryUtilities.ApiDonwload(
                    context: context,
                    guid: guid)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        public ActionResult GetStream(Context context, string guid)
        {
            var log = new SysLogModel(context: context);
            if (!context.Authenticated)
            {
                return ApiResults.Unauthorized(context: context);
            }
            var file = BinaryUtilities.Donwload(
                context: context,
                guid: guid.ToUpper());
            if (file == null)
            {
                return ApiResults.NotFound(context: context);
            }
            var response = CreateFileSteramResult(file);
            log.Finish(
                context: context,
                responseSize: file?.Length ?? 0);
            return response;
        }

        private static ActionResult CreateFileSteramResult(ResponseFile file)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            var stream = file.IsFileInfo() == true
                ? File.OpenRead(file?.FileInfo.FullName)
                : file.FileContentsStream;
            return new FileStreamResult(stream, file.ContentType) { FileDownloadName = file.FileDownloadName };
        }

        public ContentResult Upload(Context context, string guid)
        {
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
