using Application.Features.TodoItem.Command.Update;
using UnitTests.Support;
using Xunit;

namespace UnitTests.Features.TodoItem;

public sealed class UpdateTodoItemNotFoundTests
{
    [Fact]
    public async Task Returns_not_found_when_item_is_missing()
    {
        var repository = new FakeTodoItemRepository();

        var result = await new TodoItemUpdateCommandHandler(repository, TodoItemTestMapper.Instance)
            .Handle(new TodoItemUpdateCommand(5, "New", "After", false), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Response);
        TodoItemTestAssertions.AssertNotFound(result.Errors, 5);
        Assert.Null(repository.UpdatedItem);
    }
}
