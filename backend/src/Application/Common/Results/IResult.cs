using Application.Common.ErrorHandling;

namespace Application.Common.Results;

public interface IResult<TSelf> where TSelf : IResult<TSelf>
{
    static abstract TSelf Failure(IReadOnlyList<Error> errors);
}
