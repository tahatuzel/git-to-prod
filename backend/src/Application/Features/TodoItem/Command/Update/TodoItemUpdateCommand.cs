using Application.Common.Results;
using MediatR;

namespace Application.Features.TodoItem.Command.Update;

public sealed record TodoItemUpdateCommand(
    int Id,
    string Title,
    string Description,
    bool IsCompleted) : IRequest<Result<TodoItemUpdateCommandResponse>>;
