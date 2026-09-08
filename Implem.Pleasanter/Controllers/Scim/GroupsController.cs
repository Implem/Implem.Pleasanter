using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Scim;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Implem.Pleasanter.Controllers.Scim
{
    [CheckScimContextAttributes]
    [AllowAnonymous]
    [ApiController]
    [Route("scim/v2/Groups")]
    public class GroupsController : ScimBaseController
    {
        private readonly ScimDirectoryService service = new();

        [HttpGet]
        public ContentResult List(
            [FromQuery] int startIndex = 1,
            [FromQuery] int count = ScimDirectoryService.DefaultCount,
            [FromQuery] string filter = null,
            [FromQuery] string excludedAttributes = null)
        {
            return WithSysLog(() => service
                .ListGroups(ScimContext(), startIndex, count, filter, excludedAttributes)
                .ToContent());
        }

        [HttpGet("{id}")]
        public ContentResult Get(
            string id,
            [FromQuery] string excludedAttributes = null)
        {
            return WithSysLog(() => service
                .GetGroup(ScimContext(), id, excludedAttributes)
                .ToContent());
        }

        [HttpPost]
        public ContentResult Create()
        {
            var (value, error) = ReadBody<ScimGroup>();
            return WithSysLog(() => error ?? CreatedContent(
                service.CreateGroup(ScimContext(), value)));
        }

        [HttpPut("{id}")]
        public ContentResult Replace(string id)
        {
            var (value, error) = ReadBody<ScimGroup>();
            return WithSysLog(() => error ?? service
                .ReplaceGroup(ScimContext(), id, value)
                .ToContent());
        }

        [HttpPatch("{id}")]
        public ContentResult Patch(string id)
        {
            var (value, error) = ReadBody<ScimPatchRequest>();
            return WithSysLog(() => error ?? service
                .PatchGroup(ScimContext(), id, value)
                .ToContent());
        }

        [HttpDelete("{id}")]
        public ContentResult Delete(string id)
        {
            return WithSysLog(() => service
                .DeleteGroup(ScimContext(), id)
                .ToContent());
        }
    }
}
