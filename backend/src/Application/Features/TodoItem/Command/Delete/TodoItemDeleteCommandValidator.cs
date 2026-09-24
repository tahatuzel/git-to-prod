using Application.Common.Behaviors;
using Application.Common.ErrorHandling;

namespace Application.Features.TodoItem.Command.Delete;

public sealed class TodoItemDeleteCommandValidator : IRequestValidator<TodoItemDeleteCommand>
{
    public IEnumerable<Error> Validate(TodoItemDeleteCommand request)
    {
        if (request.Id <= 0)
        {
            yield return new Error(
                "TodoItem.Id.Invalid",
                "Id must be greater than zero.",
                nameof(request.Id));
        }
    }
}
