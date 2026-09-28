using Application.Common.Results;

namespace Application.Features.TodoItem.Command.Delete;

public sealed record TodoItemDeleteCommandResponse(int Id) : IResponse;
