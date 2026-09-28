using Application.Common.ErrorHandling;

namespace Api.Contracts.Responses;

public sealed record ErrorResponse(IReadOnlyList<ApiErrorResponse> Errors)
{
    public static ErrorResponse FromErrors(IEnumerable<Error> errors)
    {
        return new ErrorResponse(errors
            .Select(error => new ApiErrorResponse(error.Code, error.Message, error.PropertyName))
            .ToArray());
    }
}

public sealed record ApiErrorResponse(string Code, string Message, string? PropertyName);
