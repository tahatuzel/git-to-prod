namespace Api.Contracts.Requests;

public sealed record CreateTodoItemRequest(
    string? Title,
    string? Description,
    bool IsCompleted);
