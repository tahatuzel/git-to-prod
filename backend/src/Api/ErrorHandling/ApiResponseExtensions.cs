using Application.Common.ErrorHandling;

namespace Api.ErrorHandling;

public static class ApiResponseExtensions
{
    public static int ToHttpStatusCode(this IReadOnlyList<Error> errors)
    {
        if (errors.Any(error => error.Type == ErrorType.Unexpected))
        {
            return StatusCodes.Status500InternalServerError;
        }

        if (errors.All(error => error.Type == ErrorType.NotFound))
        {
            return StatusCodes.Status404NotFound;
        }

        return StatusCodes.Status400BadRequest;
    }
}
