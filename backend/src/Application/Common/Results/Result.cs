using Application.Common.ErrorHandling;

namespace Application.Common.Results;

public sealed class Result<TResponse> : IResult<Result<TResponse>> where TResponse : IResponse
{
    private Result(TResponse? response, IReadOnlyList<Error> errors)
    {
        Response = response;
        Errors = errors;
    }

    public bool IsSuccess => Errors.Count == 0;

    public TResponse? Response { get; }

    public IReadOnlyList<Error> Errors { get; }

    public static Result<TResponse> Success(TResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);
        return new Result<TResponse>(response, []);
    }

    public static Result<TResponse> Failure(Error error)
    {
        return Failure([error]);
    }

    public static Result<TResponse> Failure(IReadOnlyList<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        if (errors.Count == 0)
        {
            throw new ArgumentException("A failure must contain at least one error.", nameof(errors));
        }

        return new Result<TResponse>(default, errors);
    }
}
