using Application.Common.Repositories;
using TodoItemEntity = Domain.Entities.TodoItem;

namespace UnitTests.Support;

internal sealed class FakeTodoItemRepository : ITodoItemRepository
{
    public List<TodoItemEntity> Items { get; } = [];

    public TodoItemEntity? UpdatedItem { get; private set; }

    public Task<IReadOnlyList<TodoItemEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<TodoItemEntity>>(Items);

    public Task<TodoItemEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.SingleOrDefault(item => item.Id == id));

    public Task AddAsync(TodoItemEntity todoItem, CancellationToken cancellationToken = default)
    {
        todoItem.Id = Items.Count + 1;
        Items.Add(todoItem);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TodoItemEntity todoItem, CancellationToken cancellationToken = default)
    {
        UpdatedItem = todoItem;
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.RemoveAll(item => item.Id == id) > 0);
}
