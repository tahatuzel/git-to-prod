using Application.Features.TodoItem.Query.GetById;
using UnitTests.Support;
using Xunit;

namespace UnitTests.Features.TodoItem;

public sealed class GetTodoItemByIdNotFoundTests
{
    [Fact]
    public async Task Returns_not_found_when_item_is_missing()
    {
        var result = await new TodoItemGetByIdQueryHandler(
                new FakeTodoItemRepository(),
                TodoItemTestMapper.Instance)
            .Handle(new TodoItemGetByIdQuery(5), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Response);
        TodoItemTestAssertions.AssertNotFound(result.Errors, 5);
    }
}
