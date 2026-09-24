using Api.Contracts.Requests;
using Application.Features.TodoItem.Command.Create;
using Application.Features.TodoItem.Command.Update;
using AutoMapper;

namespace Api.Mapping;

public sealed class TodoItemRequestMappingProfile : Profile
{
    public TodoItemRequestMappingProfile()
    {
        CreateMap<CreateTodoItemRequest, TodoItemCreateCommand>()
            .ConstructUsing(request => new TodoItemCreateCommand(
                request.Title ?? string.Empty,
                request.Description ?? string.Empty,
                request.IsCompleted));

        CreateMap<UpdateTodoItemRequest, TodoItemUpdateCommand>()
            .ConstructUsing(request => new TodoItemUpdateCommand(
                0,
                request.Title ?? string.Empty,
                request.Description ?? string.Empty,
                request.IsCompleted));
    }
}
