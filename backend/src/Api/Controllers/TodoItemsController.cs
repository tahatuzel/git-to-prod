using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.ErrorHandling;
using Application.Common.ErrorHandling;
using Application.Common.Results;
using Application.Features.TodoItem.Command.Create;
using Application.Features.TodoItem.Command.Delete;
using Application.Features.TodoItem.Command.Update;
using Application.Features.TodoItem.Query.GetAll;
using Application.Features.TodoItem.Query.GetById;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/todo-items")]
public sealed class TodoItemsController(
    ISender sender,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TodoItemsResponse>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new TodoItemGetAllQuery(), cancellationToken);
        return ToActionResult<TodoItemGetAllQueryResponse, TodoItemsResponse>(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItemResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new TodoItemGetByIdQuery(id), cancellationToken);
        return ToActionResult<TodoItemGetByIdQueryResponse, TodoItemResponse>(result);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemResponse>> Create(
        [FromBody] CreateTodoItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<TodoItemCreateCommand>(request);
        var result = await sender.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return ToFailureResult<TodoItemResponse>(result.Errors);
        }

        var response = mapper.Map<TodoItemResponse>(result.Response!);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoItemResponse>> Update(
        int id,
        [FromBody] UpdateTodoItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<TodoItemUpdateCommand>(request) with { Id = id };
        var result = await sender.Send(command, cancellationToken);
        return ToActionResult<TodoItemUpdateCommandResponse, TodoItemResponse>(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<TodoItemDeletedResponse>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new TodoItemDeleteCommand(id), cancellationToken);
        return ToActionResult<TodoItemDeleteCommandResponse, TodoItemDeletedResponse>(result);
    }

    private ActionResult<TApiResponse> ToActionResult<TApplicationResponse, TApiResponse>(
        Result<TApplicationResponse> result)
        where TApplicationResponse : IResponse
    {
        if (!result.IsSuccess)
        {
            return ToFailureResult<TApiResponse>(result.Errors);
        }

        return Ok(mapper.Map<TApiResponse>(result.Response!));
    }

    private ActionResult<TResponse> ToFailureResult<TResponse>(IReadOnlyList<Error> errors)
    {
        return StatusCode(errors.ToHttpStatusCode(), ErrorResponse.FromErrors(errors));
    }
}
