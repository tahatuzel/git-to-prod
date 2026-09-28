using Application.Features.TodoItem.Command.Delete;
using UnitTests.Support;
using Xunit;

namespace UnitTests.Features.TodoItem;

public sealed class DeleteTodoItemNotFoundTests
{
    [Fact]
    public async Task Returns_not_found_when_item_is_missing()
    {
        var repository = new FakeTodoItemRepository();

        var result = await new TodoItemDeleteCommandHandler(repository)
            .Handle(new TodoItemDeleteCommand(5), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Response);
        TodoItemTestAssertions.AssertNotFound(result.Errors, 5);
    }
}
