using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
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
                case General.Error.Types.InvalidJsonData:
                    return InvalidJsonData(context: context);
                case General.Error.Types.Unauthorized:
                    return Unauthorized(context: context);
                case General.Error.Types.NotFound:
                    return NotFound(context: context);
                case General.Error.Types.InvalidRequest:
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
                case General.Error.Types.LockedTable:
                case General.Error.Types.LockedRecord:
                    return Locked(
                        context: context,
                        errorData: errorData);
                case General.Error.Types.TooLongText:
                    return TooLongText(
                        context: context,
                        errorData: errorData);
                case General.Error.Types.NotMatchRegex:
                    return NotMatchRegex(
                        context: context,
                        errorData: errorData);
                case General.Error.Types.InvalidUpsertKey:
                    return InvalidUpsertKey(
                        context: context,
                        errorData: errorData);
                default:
                    var message = Displays.Get(context: context, id: errorData.Type.ToString());
                    if (dataList?.Any() == true) message = message.Params(dataList.ToArray());
                    return new ApiResponse(id: context.Id, statusCode: 500, message: message);
            }
        }

        public static int StatusCode(Error.Types errorType)
        {
            switch (errorType)
            {
                case General.Error.Types.None:
                    return 200;
                case General.Error.Types.BadRequest:
                case General.Error.Types.InvalidJsonData:
                case General.Error.Types.Overlap:
                case General.Error.Types.InvalidUpsertKey:
                    return 400;
                case General.Error.Types.Unauthorized:
                    return 401;
                case General.Error.Types.NotFound:
                    return 404;
                case General.Error.Types.InvalidRequest:
                case General.Error.Types.HasNotPermission:
                    return 403;
                case General.Error.Types.OverLimitApi:
                    return 429;
                case General.Error.Types.OverLimitQuantity:
                    return 441;
                case General.Error.Types.OverLimitSize:
                    return 442;
                case General.Error.Types.OverTotalLimitSize:
                    return 443;
                case General.Error.Types.OverTenantStorageSize:
                    return 444;
                case General.Error.Types.LockedTable:
                case General.Error.Types.LockedRecord:
                    return 405;
                case General.Error.Types.TooLongText:
                case General.Error.Types.NotMatchRegex:
                    return 422;
            }
            return 500;
        }

        public static ApiResponse BadRequest(Context context)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 400,
                message: Displays.BadRequest(context: context));
        }

        public static ApiResponse InvalidJsonData(Context context)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 400,
                message: Displays.InvalidJsonData(context: context));
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

        public static ApiResponse Overlap(Context context)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 400,
                message: Displays.Overlap(context: context));
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

        public static ApiResponse Locked(Context context, ErrorData errorData)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 405,
                message: Displays.Get(
                    context: context,
                    id: errorData.Type.ToString()).Params(errorData.Data));
        }

        public static ApiResponse TooLongText(Context context, ErrorData errorData)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 422,
                message: Displays.TooLongText(
                    context: context,
                    data: errorData.Data));
        }

        public static ApiResponse NotMatchRegex(Context context, ErrorData errorData)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 422,
                message: Displays.NotMatchRegex(
                    context: context,
                    data: errorData.Data));
        }

        public static ApiResponse Duplicated(Context context, string message)
        {
            return new ApiResponse(
                id: context.Id,
                statusCode: 409,
                message: message);
        }
        private static ApiResponse InvalidUpsertKey(Context context, ErrorData errorData)
        {
            var statusCode = StatusCode(errorData.Type);
            var message = Displays.InvalidUpsertKey(
                context: context,
                data: errorData.Data);
            return new ApiResponse(id: context.Id, statusCode: statusCode, message: message);
        }
    }
}