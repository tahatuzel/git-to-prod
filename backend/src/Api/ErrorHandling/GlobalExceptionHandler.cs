using Api.Contracts.Responses;
using Application.Common.ErrorHandling;
using Microsoft.AspNetCore.Diagnostics;

namespace Api.ErrorHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            return false;
        }

        logger.LogError(exception, "Unhandled API exception for {Path}.", httpContext.Request.Path);

        if (httpContext.Response.HasStarted)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var error = new Error(
            "General.Unexpected",
            "An unexpected error occurred.",
            Type: ErrorType.Unexpected);
        var response = ErrorResponse.FromErrors([error]);

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
