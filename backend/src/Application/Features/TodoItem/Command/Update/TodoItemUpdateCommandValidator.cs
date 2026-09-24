using Application.Common.Behaviors;
using Application.Common.ErrorHandling;

namespace Application.Features.TodoItem.Command.Update;

public sealed class TodoItemUpdateCommandValidator : IRequestValidator<TodoItemUpdateCommand>
{
    public IEnumerable<Error> Validate(TodoItemUpdateCommand request)
    {
        if (request.Id <= 0)
        {
            yield return new Error(
                "TodoItem.Id.Invalid",
                "Id must be greater than zero.",
                nameof(request.Id));
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            yield return new Error(
                "TodoItem.Title.Required",
                "Title is required.",
                nameof(request.Title));
        }
        else if (request.Title.Length > 200)
        {
            yield return new Error(
                "TodoItem.Title.MaxLength",
                "Title cannot exceed 200 characters.",
                nameof(request.Title));
        }
    }
}
