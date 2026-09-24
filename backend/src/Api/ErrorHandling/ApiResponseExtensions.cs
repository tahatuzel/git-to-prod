using Application.Common.ErrorHandling;

namespace Api.ErrorHandling;

public static class ApiResponseExtensions
{
    public static int ToHttpStatusCode(this ErrorResult errorResult)
    {
        if (errorResult.Errors.Any(error => error.Type == ErrorType.Unexpected))
        {
            return StatusCodes.Status500InternalServerError;
        }

        if (errorResult.Errors.All(error => error.Type == ErrorType.NotFound))
        {
            return StatusCodes.Status404NotFound;
        }

        return StatusCodes.Status400BadRequest;
    }
}
