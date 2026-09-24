using Application.Common.Results;
using MediatR;

namespace Application.Features.TodoItem.Query.GetAll;

public sealed record TodoItemGetAllQuery : IRequest<Result<TodoItemGetAllQueryResponse>>;
