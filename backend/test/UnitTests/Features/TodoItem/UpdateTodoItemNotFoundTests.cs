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

        TodoItemTestAssertions.AssertNotFound(result.Error, 5);
        Assert.Null(repository.UpdatedItem);
    }
}
