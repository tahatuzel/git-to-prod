using Application.Features.TodoItem.Command.Create;
using UnitTests.Support;
using Xunit;

namespace UnitTests.Features.TodoItem;

public sealed class CreateTodoItemHandlerTests
{
    [Fact]
    public async Task Adds_item_and_returns_saved_fields()
    {
        var repository = new FakeTodoItemRepository();

        var result = await new TodoItemCreateCommandHandler(repository, TodoItemTestMapper.Instance)
            .Handle(new TodoItemCreateCommand("Read", "Book", true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(new TodoItemCreateCommandResponse(1, "Read", "Book", true), result.Response);
        Assert.Empty(result.Errors);
        Assert.Single(repository.Items);
        Assert.Equal(1, repository.Items[0].Id);
        Assert.Equal("Read", repository.Items[0].Title);
    }
}
