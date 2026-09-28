using Application.Common.Results;
using Application.Common.Repositories;
using MediatR;

namespace Application.Features.TodoItem.Command.Delete;

public sealed class TodoItemDeleteCommandHandler(
    ITodoItemRepository repository) : IRequestHandler<TodoItemDeleteCommand, Result<TodoItemDeleteCommandResponse>>
{
    public async Task<Result<TodoItemDeleteCommandResponse>> Handle(
        TodoItemDeleteCommand request,
        CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(request.Id, cancellationToken);

        if (!deleted)
        {
            return Result<TodoItemDeleteCommandResponse>.Failure(TodoItemErrors.NotFound(request.Id));
        }

        return Result<TodoItemDeleteCommandResponse>.Success(new TodoItemDeleteCommandResponse(request.Id));
    }
}
