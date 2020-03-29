using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ApiResponses
    {
        public static ApiResponse Success(long id, string message, int? limitPerDate = null, int? limitRemaining = null)
        {
            return new ApiResponse(
                id: id,
                statusCode: 200,
                message: message,
                limitPerDate: limitPerDate,
                limitRemaining: limitRemaining);
        }

        public static ApiResponse Error(Context context, ErrorData errorData, params string[] data)
        {
            var dataList = errorData.Data.ToList();
            dataList.AddRange(data);
            switch (errorData.Type)
            {
                case General.Error.Types.BadRequest:
                    return BadRequest(context: context);
                case General.Error.Types.Unauthorized:
                    return Unauthorized(context: context);
                case General.Error.Types.NotFound:
                    return NotFound(context: context);
                case General.Error.Types.HasNotPermission:
                    return Forbidden(context: context);
                case General.Error.Types.OverLimitQuantity:
                    return OverLimitQuantity(
                        context: context,
                        dataList[0].ToDecimal());
                case General.Error.Types.OverLimitSize:
                    return OverLimitSize(
                        context: context,
                        dataList[0].ToDecimal());
                case General.Error.Types.OverTotalLimitSize:
                    return OverTotalLimitSize(
                        context: context,
                        dataList[0].ToDecimal());
                case General.Error.Types.OverTenantStorageSize:
                    return OverTenantStorageSize(
                        context: context,
                        dataList[0].ToDecimal());
                default:
                    return new ApiResponse(
                        id: context.Id,
                        statusCode: 500,
                        message: dataList?.Any() == true
                            ? Displays.Get(
                                context: context,
                                id: errorData.Type.ToString()).Params(dataList)
                            : Displays.Get(
                                context: context,
                                id: errorData.Type.ToString()));
            }
        }

        public static ApiResponse BadRequest(Context context)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 400,
                message: Displays.BadRequest(context: context));
        }

        public static ApiResponse Unauthorized(Context context)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 401,
                message: Displays.Unauthorized(context: context));
        }

        public static ApiResponse NotFound(Context context)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 404,
                message: Displays.NotFound(context: context));
        }

        public static ApiResponse Forbidden(Context context)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 403,
                message: Displays.HasNotPermission(context: context));
        }

        public static ApiResponse OverLimitApi(Context context, long siteId, int limitPerSite)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 429,
                message: Displays.OverLimitApi(
                    context: context, siteId.ToString(),
                    limitPerSite.ToString()));
        }

        public static ApiResponse OverLimitQuantity(Context context, decimal? maxSize)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 441,
                message: Displays.OverLimitQuantity(
                    context: context,
                    data: maxSize.ToString()));
        }

        public static ApiResponse OverLimitSize(Context context, decimal? maxSize)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 442,
                message: Displays.OverLimitSize(
                    context: context,
                    data: maxSize.ToString()));
        }

        public static ApiResponse OverTotalLimitSize(Context context, decimal? maxSize)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 443,
                message: Displays.OverTotalLimitSize(
                    context: context,
                    data: maxSize.ToString()));
        }

        public static ApiResponse OverTenantStorageSize(Context context, decimal? maxSize)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 444,
                message: Displays.OverTenantStorageSize(
                    context: context,
                    data: maxSize.ToString()));
        }
    }
}