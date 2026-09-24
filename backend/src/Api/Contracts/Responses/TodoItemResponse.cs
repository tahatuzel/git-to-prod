namespace Api.Contracts.Responses;

public sealed record TodoItemResponse(
    int Id,
    string Title,
    string Description,
    bool IsCompleted);
