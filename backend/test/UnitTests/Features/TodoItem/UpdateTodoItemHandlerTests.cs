using Application.Features.TodoItem.Command.Update;
using UnitTests.Support;
using Xunit;
using TodoItemEntity = Domain.Entities.TodoItem;

namespace UnitTests.Features.TodoItem;

public sealed class UpdateTodoItemHandlerTests
{
    [Fact]
    public async Task Updates_existing_item_without_changing_id()
    {
        var repository = new FakeTodoItemRepository();
        var item = new TodoItemEntity { Id = 5, Title = "Old", Description = "Before" };
        repository.Items.Add(item);

        var result = await new TodoItemUpdateCommandHandler(repository, TodoItemTestMapper.Instance)
            .Handle(new TodoItemUpdateCommand(5, "New", "After", true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(new TodoItemUpdateCommandResponse(5, "New", "After", true), result.Value);
        Assert.Same(item, repository.UpdatedItem);
        Assert.Equal(5, item.Id);
        Assert.Equal("New", item.Title);
        Assert.Equal("After", item.Description);
        Assert.True(item.IsCompleted);
    }
}
