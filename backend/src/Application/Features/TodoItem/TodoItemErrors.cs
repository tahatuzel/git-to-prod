using Application.Common.ErrorHandling;

namespace Application.Features.TodoItem;

public static class TodoItemErrors
{
    public static Error NotFound(int id)
    {
        return new Error(
            "TodoItem.NotFound",
            $"Todo item with id {id} was not found.",
            Type: ErrorType.NotFound);
    }
}
