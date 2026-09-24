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

        var result = await new TodoItemDeleteCommandHandler(repository, TodoItemTestMapper.Instance)
            .Handle(new TodoItemDeleteCommand(5), CancellationToken.None);

        TodoItemTestAssertions.AssertNotFound(result.Error, 5);
    }
}
