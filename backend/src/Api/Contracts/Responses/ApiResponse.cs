using Application.Common.ErrorHandling;

namespace Api.Contracts.Responses;

public sealed record ApiResponse<TResponse>(
    ApiResult Result,
    TResponse? Response,
    ErrorResponse? ErrorResponse)
{
    public static ApiResponse<TResponse> Success(TResponse response)
    {
        return new ApiResponse<TResponse>(new ApiResult(true), response, null);
    }

    public static ApiResponse<TResponse> Failure(ErrorResponse errorResponse)
    {
        return new ApiResponse<TResponse>(new ApiResult(false), default, errorResponse);
    }
}

public sealed record ApiResult(bool IsSuccess);
