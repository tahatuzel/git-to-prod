using Application.Common.Results;
using MediatR;

namespace Application.Features.TodoItem.Command.Create;

public sealed record TodoItemCreateCommand(
    string Title,
    string Description,
    bool IsCompleted) : IRequest<Result<TodoItemCreateCommandResponse>>;
