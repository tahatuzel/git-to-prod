namespace Application.Features.TodoItem.Query.GetAll;

public sealed record TodoItemGetAllQueryResponse(IReadOnlyList<TodoItemGetAllQueryResponseItem> Items);

public sealed record TodoItemGetAllQueryResponseItem(
    int Id,
    string Title,
    string Description,
    bool IsCompleted);
