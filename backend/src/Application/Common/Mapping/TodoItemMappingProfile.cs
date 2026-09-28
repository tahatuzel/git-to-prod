using Application.Features.TodoItem.Command.Create;
using Application.Features.TodoItem.Command.Update;
using Application.Features.TodoItem.Query.GetAll;
using Application.Features.TodoItem.Query.GetById;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mapping;

public sealed class TodoItemMappingProfile : Profile
{
    public TodoItemMappingProfile()
    {
        CreateMap<TodoItemCreateCommand, TodoItem>();
        CreateMap<TodoItem, TodoItemCreateCommandResponse>();
        CreateMap<TodoItemUpdateCommand, TodoItem>()
            .ForMember(todoItem => todoItem.Id, options => options.Ignore());
        CreateMap<TodoItem, TodoItemUpdateCommandResponse>();
        CreateMap<TodoItem, TodoItemGetAllQueryResponseItem>();
        CreateMap<TodoItem, TodoItemGetByIdQueryResponse>();
    }
}
