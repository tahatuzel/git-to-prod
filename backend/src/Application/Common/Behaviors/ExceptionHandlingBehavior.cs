using Application.Common.ErrorHandling;
using Application.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public sealed class ExceptionHandlingBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "An error occurred while handling request {RequestType}.",
                typeof(TRequest).Name);

            return TResponse.Failure(new ErrorResult(
            [
                new Error(
                    "General.Unexpected",
                    "An unexpected error occurred.",
                    Type: ErrorType.Unexpected)
            ]));
        }
    }
}
