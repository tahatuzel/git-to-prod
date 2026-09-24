using Api.Contracts.Responses;
using Application.Features.TodoItem.Command.Create;
using Application.Features.TodoItem.Command.Delete;
using Application.Features.TodoItem.Command.Update;
using Application.Features.TodoItem.Query.GetAll;
using Application.Features.TodoItem.Query.GetById;
using AutoMapper;

namespace Api.Mapping;

public sealed class TodoItemResponseMappingProfile : Profile
{
    public TodoItemResponseMappingProfile()
    {
        CreateMap<TodoItemCreateCommandResponse, TodoItemResponse>();
        CreateMap<TodoItemUpdateCommandResponse, TodoItemResponse>();
        CreateMap<TodoItemGetByIdQueryResponse, TodoItemResponse>();
        CreateMap<TodoItemGetAllQueryResponseItem, TodoItemResponse>();
        CreateMap<TodoItemGetAllQueryResponse, TodoItemsResponse>();
        CreateMap<TodoItemDeleteCommandResponse, TodoItemDeletedResponse>();
    }
}
