using Application.Common.Results;
using Application.Common.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.TodoItem.Command.Update;

public sealed class TodoItemUpdateCommandHandler(
    ITodoItemRepository repository,
    IMapper mapper) : IRequestHandler<TodoItemUpdateCommand, Result<TodoItemUpdateCommandResponse>>
{
    public async Task<Result<TodoItemUpdateCommandResponse>> Handle(
        TodoItemUpdateCommand request,
        CancellationToken cancellationToken)
    {
        var todoItem = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (todoItem is null)
        {
            return Result<TodoItemUpdateCommandResponse>.Failure(TodoItemErrors.NotFound(request.Id));
        }

        mapper.Map(request, todoItem);
        await repository.UpdateAsync(todoItem, cancellationToken);

        var response = mapper.Map<TodoItemUpdateCommandResponse>(todoItem);
        return Result<TodoItemUpdateCommandResponse>.Success(response);
    }
}
