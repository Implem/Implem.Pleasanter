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
            guid = guid.ToUpper();
            var referenceId = FileContentResults.GetReferenceId(context: context, guid: guid);
            if (referenceId == 0)
            {
                return ApiResults.NotFound(context: context);
            }
            var newGuid = context.QueryStrings.Bool("overwrite")
                ? guid
                : Strings.NewGuid();

            if (context.PostedFiles == null || context.PostedFiles.Count == 0)
            {
                return ApiResults.BadRequest(context: context);
            }
            var postedFile = context.PostedFiles[0];
            SaveFileToTemp(newGuid, postedFile);

            var verup = context.QueryStrings.ContainsKey("verup")
                ? context.QueryStrings.Bool("verup")
                : (bool?)null;
            context.ApiRequestBody = CreateAttachmentsHashJson($"{guid},{newGuid}", context, postedFile, verup);
            var response = new ItemModel(context: context, referenceId: referenceId)
                .UpdateByApi(context: context);
            log.Finish(
                context: context,
                responseSize: response?.Content?.Length ?? 0);
            return response;
        }

        private string CreateAttachmentsHashJson(string guid, Context context, PostedFile file, bool? verup)
        {
            return new
            {
                VerUp = verup,

                AttachmentsHash = new Dictionary<string, Attachment[]>
                {
                    ["Attachments#Uploading"] = new Attachment[]
                    {
                        new Attachment
                        {
                            Guid=guid,
                            Name = file.FileName,
                            FileName =file.FileName,
                            Size = file.Size,
                            Extention = Path.GetExtension(file.FileName),
                            ContentType = file.ContentType,
                            Added = true
                        }
                    }
                }
            }.ToJson();
        }

        private void SaveFileToTemp(string guid, PostedFile file)
        {
            var directory = Path.Combine(DefinitionAccessor.Directories.Temp(), guid);
            Directory.CreateDirectory(directory);
            using (var fileStream = new FileStream(
                Path.Combine(directory, file.FileName),
                FileMode.Create,
                FileAccess.Write))
            {
                file.InputStream.CopyTo(fileStream);
            }
        }
    }
}
