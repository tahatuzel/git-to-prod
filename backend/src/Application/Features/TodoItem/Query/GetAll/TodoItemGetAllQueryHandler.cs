using Application.Common.Results;
using Application.Common.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.TodoItem.Query.GetAll;

public sealed class TodoItemGetAllQueryHandler(
    ITodoItemRepository repository,
    IMapper mapper) : IRequestHandler<TodoItemGetAllQuery, Result<TodoItemGetAllQueryResponse>>
{
    public async Task<Result<TodoItemGetAllQueryResponse>> Handle(
        TodoItemGetAllQuery request,
        CancellationToken cancellationToken)
    {
        var todoItems = await repository.GetAllAsync(cancellationToken);
        var items = mapper.Map<List<TodoItemGetAllQueryResponseItem>>(todoItems);
        return Result<TodoItemGetAllQueryResponse>.Success(new TodoItemGetAllQueryResponse(items));
    }
}
