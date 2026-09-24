using Application.Common.Results;
using Application.Common.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.TodoItem.Query.GetById;

public sealed class TodoItemGetByIdQueryHandler(
    ITodoItemRepository repository,
    IMapper mapper) : IRequestHandler<TodoItemGetByIdQuery, Result<TodoItemGetByIdQueryResponse>>
{
    public async Task<Result<TodoItemGetByIdQueryResponse>> Handle(
        TodoItemGetByIdQuery request,
        CancellationToken cancellationToken)
    {
        var todoItem = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (todoItem is null)
        {
            return Result<TodoItemGetByIdQueryResponse>.Failure(TodoItemErrors.NotFound(request.Id));
        }

        var response = mapper.Map<TodoItemGetByIdQueryResponse>(todoItem);
        return Result<TodoItemGetByIdQueryResponse>.Success(response);
    }
}
