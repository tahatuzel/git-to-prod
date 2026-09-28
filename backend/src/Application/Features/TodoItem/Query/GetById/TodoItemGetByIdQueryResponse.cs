using Application.Common.Results;

namespace Application.Features.TodoItem.Query.GetById;

public sealed record TodoItemGetByIdQueryResponse(
    int Id,
    string Title,
    string Description,
    bool IsCompleted) : IResponse;
