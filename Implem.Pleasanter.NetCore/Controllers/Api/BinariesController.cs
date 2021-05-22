using Implem.Pleasanter.NetCore.Filters;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Libraries.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Implem.Libraries.Utilities;
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Implem.Pleasanter.NetCore.Controllers.Api
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
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.ContentType);
            var controller = new Pleasanter.Controllers.Api.BinariesController();
            var result = controller.Get(
                context: context,
                guid: guid);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{guid}/upload")]
        public ContentResult Upload(string guid)
        {
            var context = new ContextImplement(
                files: Request.Form.Files.ToList(),
                apiRequestBody: new
                {
                    ApiKey = AuthorizationHeaderValue()
                }.ToJson());
            var controller = new Pleasanter.Controllers.Api.BinariesController();
            var result = controller.Upload(context: context, guid);
            return result.ToHttpResponse(request: Request);
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

        [HttpPost("{guid}/getstream")]
        public FileStreamResult GetStream(string guid)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(
                 sessionStatus: User?.Identity?.IsAuthenticated == true,
                 sessionData: User?.Identity?.IsAuthenticated == true,
                 apiRequestBody: body,
                 contentType: Request.ContentType);
            var controller = new Pleasanter.Controllers.Api.BinariesController();
            var result = controller.GetStream(
                context: context,
                guid: guid) as System.Web.Mvc.FileStreamResult;
            return new FileStreamResult(result.FileStream, result.ContentType) { FileDownloadName = result.FileDownloadName };
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var factories = context.ValueProviderFactories;
            factories.RemoveType<Microsoft.AspNetCore.Mvc.ModelBinding.FormValueProviderFactory>();
            factories.RemoveType<FormFileValueProviderFactory>();
            factories.RemoveType<Microsoft.AspNetCore.Mvc.ModelBinding.JQueryFormValueProviderFactory>();
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }

}