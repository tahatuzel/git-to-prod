namespace Application.Common.ErrorHandling;

public sealed record ErrorResponse(IReadOnlyList<Error> Errors);
