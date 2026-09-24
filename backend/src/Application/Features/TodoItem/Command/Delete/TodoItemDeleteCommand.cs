using Application.Common.Results;
using MediatR;

namespace Application.Features.TodoItem.Command.Delete;

public sealed record TodoItemDeleteCommand(int Id) : IRequest<Result<TodoItemDeleteCommandResponse>>;
