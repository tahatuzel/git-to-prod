namespace Application.Features.TodoItem.Command.Create;

public sealed record TodoItemCreateCommandResponse(
    int Id,
    string Title,
    string Description,
    bool IsCompleted);
