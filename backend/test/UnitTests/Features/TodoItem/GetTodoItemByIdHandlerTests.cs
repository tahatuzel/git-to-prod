using Application.Features.TodoItem.Query.GetById;
using UnitTests.Support;
using Xunit;
using TodoItemEntity = Domain.Entities.TodoItem;

namespace UnitTests.Features.TodoItem;

public sealed class GetTodoItemByIdHandlerTests
{
    [Fact]
    public async Task Returns_matching_item()
    {
        var repository = new FakeTodoItemRepository();
        repository.Items.Add(new TodoItemEntity
        {
            Id = 5,
            Title = "Read",
            Description = "Book",
            IsCompleted = true
        });

        var result = await new TodoItemGetByIdQueryHandler(repository, TodoItemTestMapper.Instance)
            .Handle(new TodoItemGetByIdQuery(5), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(new TodoItemGetByIdQueryResponse(5, "Read", "Book", true), result.Value);
    }
}
