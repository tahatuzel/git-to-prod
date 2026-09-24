namespace Application.Features.TodoItem.Command.Update;

public sealed record TodoItemUpdateCommandResponse(
    int Id,
    string Title,
    string Description,
    bool IsCompleted);
