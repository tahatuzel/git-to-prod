using Application.Features.TodoItem.Query.GetAll;
using UnitTests.Support;
using Xunit;
using TodoItemEntity = Domain.Entities.TodoItem;

namespace UnitTests.Features.TodoItem;

public sealed class GetAllTodoItemsHandlerTests
{
    [Fact]
    public async Task Returns_items()
    {
        var repository = new FakeTodoItemRepository();
        repository.Items.Add(new TodoItemEntity { Id = 1, Title = "Read", Description = "Book" });
        repository.Items.Add(new TodoItemEntity { Id = 2, Title = "Write", IsCompleted = true });

        var result = await new TodoItemGetAllQueryHandler(repository, TodoItemTestMapper.Instance)
            .Handle(new TodoItemGetAllQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var response = Assert.IsType<TodoItemGetAllQueryResponse>(result.Response);
        Assert.Equal(
            [
                new TodoItemGetAllQueryResponseItem(1, "Read", "Book", false),
                new TodoItemGetAllQueryResponseItem(2, "Write", "", true)
            ],
            response.Items);
        Assert.Empty(result.Errors);
    }
}
