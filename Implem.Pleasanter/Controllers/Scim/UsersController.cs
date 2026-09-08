using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Scim;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Implem.Pleasanter.Controllers.Scim
{
    [CheckScimContextAttributes]
    [AllowAnonymous]
    [ApiController]
    [Route("scim/v2/Users")]
    public class UsersController : ScimBaseController
    {
        private readonly ScimDirectoryService service = new();

        [HttpGet]
        public ContentResult List(
            [FromQuery] int startIndex = 1,
            [FromQuery] int count = ScimDirectoryService.DefaultCount,
            [FromQuery] string filter = null)
        {
            return WithSysLog(() => service
                .ListUsers(ScimContext(), startIndex, count, filter)
                .ToContent());
        }

        [HttpGet("{id}")]
        public ContentResult Get(string id)
        {
            return WithSysLog(() => service
                .GetUser(ScimContext(), id)
                .ToContent());
        }

        [HttpPost]
        public ContentResult Create()
        {
            var (value, error) = ReadBody<ScimUser>();
            return WithSysLog(() => error ?? CreatedContent(
                service.CreateUser(ScimContext(), value)));
        }

        [HttpPut("{id}")]
        public ContentResult Replace(string id)
        {
            var (value, error) = ReadBody<ScimUser>();
            return WithSysLog(() => error ?? service
                .ReplaceUser(ScimContext(), id, value)
                .ToContent());
        }

        [HttpPatch("{id}")]
        public ContentResult Patch(string id)
        {
            var (value, error) = ReadBody<ScimPatchRequest>();
            return WithSysLog(() => error ?? service
                .PatchUser(ScimContext(), id, value)
                .ToContent());
        }

        [HttpDelete("{id}")]
        public ContentResult Delete(string id)
        {
            return WithSysLog(() => service
                .DeleteUser(ScimContext(), id)
                .ToContent());
        }
    }
}
