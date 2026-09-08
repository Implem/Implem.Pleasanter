using System;
using System.IO;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Scim;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Implem.Pleasanter.Controllers.Scim
{
    public abstract class ScimBaseController : ControllerBase
    {
        [NonAction]
        protected Context ScimContext()
        {
            return (HttpContext.Items[ScimRequestContext.ItemKey] as ScimRequestContext)?.Context;
        }

        [NonAction]
        protected ContentResult WithSysLog(Func<ContentResult> action)
        {
            var scim = HttpContext.Items[ScimRequestContext.ItemKey] as ScimRequestContext;
            var context = scim?.Context;
            if (context == null)
            {
                return action();
            }
            if (!scim.TokenPrefix.IsNullOrEmpty())
            {
                context.SysLogsDescription = $"SCIM TokenPrefix: {scim.TokenPrefix}";
            }
            var log = new SysLogModel(context: context);
            var result = action();
            log.Finish(
                context: context,
                responseSize: result?.Content?.Length ?? 0);
            return result;
        }

        [NonAction]
        protected ContentResult CreatedContent(ScimServiceResult result)
        {
            var location = result.Body switch
            {
                ScimUser user => user.Meta?.Location,
                ScimGroup group => group.Meta?.Location,
                _ => null
            };
            if (result.StatusCode == StatusCodes.Status201Created
                && !string.IsNullOrWhiteSpace(location))
            {
                Response.Headers.Location = location;
            }
            return result.ToContent();
        }

        [NonAction]
        protected (T value, ContentResult error) ReadBody<T>() where T : class
        {
            if (!SupportedContentType(Request.ContentType))
            {
                return (null, ScimResponse.Error(
                    StatusCodes.Status415UnsupportedMediaType,
                    "Unsupported media type"));
            }
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = reader.ReadToEnd();
                return (JsonConvert.DeserializeObject<T>(body), null);
            }
            catch (JsonException)
            {
                return (null, ScimResponse.Error(StatusCodes.Status400BadRequest, "Invalid JSON", "invalidSyntax"));
            }
        }

        [NonAction]
        private static bool SupportedContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }
            var mediaType = contentType.Split(';')[0].Trim();
            return string.Equals(mediaType, "application/scim+json", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase);
        }
    }
}
