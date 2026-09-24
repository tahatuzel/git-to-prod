using Application.Common.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TodoItemRepository(AppDbContext dbContext) : ITodoItemRepository
{
    public async Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.TodoItems
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.TodoItems
            .AsNoTracking()
            .FirstOrDefaultAsync(todoItem => todoItem.Id == id, cancellationToken);
    }

    public async Task AddAsync(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        await dbContext.TodoItems.AddAsync(todoItem, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        dbContext.TodoItems.Update(todoItem);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var todoItem = await dbContext.TodoItems.FindAsync([id], cancellationToken);

        if (todoItem is null)
        {
            return false;
        }

        dbContext.TodoItems.Remove(todoItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
