using Application.Common.Results;
using MediatR;

namespace Application.Features.TodoItem.Query.GetById;

public sealed record TodoItemGetByIdQuery(int Id) : IRequest<Result<TodoItemGetByIdQueryResponse>>;
