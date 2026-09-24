using Application.Common.ErrorHandling;
using Application.Common.Results;
using MediatR;

namespace Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IRequestValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var errors = validators
            .SelectMany(validator => validator.Validate(request))
            .ToArray();

        if (errors.Length > 0)
        {
            return TResponse.Failure(new ErrorResult(errors));
        }

        return await next();
    }
}
