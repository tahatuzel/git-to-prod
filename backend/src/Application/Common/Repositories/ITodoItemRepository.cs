using Domain.Entities;

namespace Application.Common.Repositories;

public interface ITodoItemRepository
{
    Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(TodoItem todoItem, CancellationToken cancellationToken = default);

    Task UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
