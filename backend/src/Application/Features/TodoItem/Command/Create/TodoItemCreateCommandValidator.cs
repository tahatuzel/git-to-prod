using Application.Common.Behaviors;
using Application.Common.ErrorHandling;

namespace Application.Features.TodoItem.Command.Create;

public sealed class TodoItemCreateCommandValidator : IRequestValidator<TodoItemCreateCommand>
{
    public IEnumerable<Error> Validate(TodoItemCreateCommand request)
    {
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
