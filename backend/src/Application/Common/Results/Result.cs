using Application.Common.ErrorHandling;

namespace Application.Common.Results;

public sealed class Result<TValue> : IResult<Result<TValue>>
{
    private Result(TValue? value, ErrorResult? error)
    {
        Value = value;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public TValue? Value { get; }

    public ErrorResult? Error { get; }

    public static Result<TValue> Success(TValue value)
    {
        return new Result<TValue>(value, null);
    }

    public static Result<TValue> Failure(Error error)
    {
        return Failure(new ErrorResult([error]));
    }

    public static Result<TValue> Failure(ErrorResult error)
    {
        return new Result<TValue>(default, error);
    }
}
