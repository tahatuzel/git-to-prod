using Application.Common.Behaviors;
using Application.Common.ErrorHandling;

namespace Application.Features.TodoItem.Query.GetById;

public sealed class TodoItemGetByIdQueryValidator : IRequestValidator<TodoItemGetByIdQuery>
{
    public IEnumerable<Error> Validate(TodoItemGetByIdQuery request)
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
