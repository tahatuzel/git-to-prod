namespace Application.Common.ErrorHandling;

public sealed record ErrorResult(IReadOnlyList<Error> Errors);
