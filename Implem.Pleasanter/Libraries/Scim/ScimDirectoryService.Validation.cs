using Implem.Libraries.Utilities;
using Microsoft.AspNetCore.Http;

namespace Implem.Pleasanter.Libraries.Scim
{
    public partial class ScimDirectoryService
    {
        private static ScimServiceResult ValidateUser(ScimUser user)
        {
            if (user == null)
            {
                return Error(StatusCodes.Status400BadRequest, "User is required", "invalidValue");
            }
            return UserLoginId(user).IsNullOrEmpty()
                ? Error(StatusCodes.Status400BadRequest, "userName is required", "invalidValue")
                : null;
        }

        private static ScimServiceResult ValidateGroup(ScimGroup group)
        {
            if (group == null)
            {
                return Error(StatusCodes.Status400BadRequest, "Group is required", "invalidValue");
            }
            return group.DisplayName.IsNullOrEmpty()
                ? Error(StatusCodes.Status400BadRequest, "displayName is required", "invalidValue")
                : null;
        }
    }
}
