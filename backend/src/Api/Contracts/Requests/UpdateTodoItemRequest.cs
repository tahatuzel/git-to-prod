namespace Api.Contracts.Requests;

public sealed record UpdateTodoItemRequest(
    string? Title,
    string? Description,
    bool IsCompleted);
