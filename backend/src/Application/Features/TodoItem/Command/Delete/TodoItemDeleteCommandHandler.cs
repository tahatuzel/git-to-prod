using Application.Common.Results;
using Application.Common.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.TodoItem.Command.Delete;

public sealed class TodoItemDeleteCommandHandler(
    ITodoItemRepository repository,
    IMapper mapper) : IRequestHandler<TodoItemDeleteCommand, Result<TodoItemDeleteCommandResponse>>
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

        var response = mapper.Map<TodoItemDeleteCommandResponse>(request);
        return Result<TodoItemDeleteCommandResponse>.Success(response);
    }
}
