namespace Api.Contracts.Responses;

public sealed record TodoItemsResponse(IReadOnlyList<TodoItemResponse> Items);
