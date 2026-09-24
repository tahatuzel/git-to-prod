namespace Application.Common.ErrorHandling;

public enum ErrorType
{
    Validation,
    NotFound,
    Unexpected
}

public sealed record Error(
    string Code,
    string Message,
    string? PropertyName = null,
    ErrorType Type = ErrorType.Validation);
