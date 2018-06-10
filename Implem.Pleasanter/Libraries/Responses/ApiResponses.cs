using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ApiResponses
    {
        public static ApiResponse Success(long id, string message)
        {
            return new ApiResponse(id, 200, message);
        }

        public static ApiResponse Error(Error.Types type, params string[] data)
        {
            return new ApiResponse(500, data?.Any() == true
                ? Displays.Get(type.ToString()).Params(data)
                : Displays.Get(type.ToString()));
        }

        public static ApiResponse BadRequest()
        {
            return new ApiResponse(400, Displays.BadRequest());
        }

        public static ApiResponse Unauthorized()
        {
            return new ApiResponse(401, Displays.Unauthorized());
        }

        public static ApiResponse NotFound()
        {
            return new ApiResponse(404, Displays.NotFound());
        }
    }
}