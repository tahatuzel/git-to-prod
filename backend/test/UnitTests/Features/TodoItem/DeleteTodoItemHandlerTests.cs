using Application.Features.TodoItem.Command.Delete;
using UnitTests.Support;
using Xunit;
using TodoItemEntity = Domain.Entities.TodoItem;

namespace UnitTests.Features.TodoItem;

public sealed class DeleteTodoItemHandlerTests
{
    [Fact]
    public async Task Removes_item_and_returns_its_id()
    {
        var repository = new FakeTodoItemRepository();
        repository.Items.Add(new TodoItemEntity { Id = 5 });

        var result = await new TodoItemDeleteCommandHandler(repository, TodoItemTestMapper.Instance)
            .Handle(new TodoItemDeleteCommand(5), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(new TodoItemDeleteCommandResponse(5), result.Value);
        Assert.Empty(repository.Items);
    }
}
