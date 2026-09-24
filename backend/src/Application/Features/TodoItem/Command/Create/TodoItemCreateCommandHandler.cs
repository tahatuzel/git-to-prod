using Application.Common.Results;
using Application.Common.Repositories;
using AutoMapper;
using MediatR;
using TodoItemEntity = Domain.Entities.TodoItem;

namespace Application.Features.TodoItem.Command.Create;

public sealed class TodoItemCreateCommandHandler(
    ITodoItemRepository repository,
    IMapper mapper) : IRequestHandler<TodoItemCreateCommand, Result<TodoItemCreateCommandResponse>>
{
    public async Task<Result<TodoItemCreateCommandResponse>> Handle(
        TodoItemCreateCommand request,
        CancellationToken cancellationToken)
    {
        var todoItem = mapper.Map<TodoItemEntity>(request);

        await repository.AddAsync(todoItem, cancellationToken);

        var response = mapper.Map<TodoItemCreateCommandResponse>(todoItem);
        return Result<TodoItemCreateCommandResponse>.Success(response);
    }
}
