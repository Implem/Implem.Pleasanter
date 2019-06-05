using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ApiResponses
    {
        public static ApiResponse Success(long id, string message)
        {
            return new ApiResponse(id, 200, message);
        }

        public static ApiResponse Error(Context context, ErrorData errorData, params string[] data)
        {
            return new ApiResponse(500, data?.Any() == true
                ? Displays.Get(
                    context: context,
                    id: errorData.Type.ToString()).Params(data)
                : Displays.Get(
                    context: context,
                    id: errorData.Type.ToString()));
        }

        public static ApiResponse BadRequest(Context context)
        {
            return new ApiResponse(400, Displays.BadRequest(context: context));
        }

        public static ApiResponse Unauthorized(Context context)
        {
            return new ApiResponse(401, Displays.Unauthorized(context: context));
        }

        public static ApiResponse NotFound(Context context)
        {
            return new ApiResponse(404, Displays.NotFound(context: context));
        }
    }
}