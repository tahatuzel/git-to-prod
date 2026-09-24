using Infrastructure.Persistence;
using IntegrationTests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests.Database;

[Collection(TodoItemsApiCollection.Name)]
public sealed class MigrationsApplyTests(TodoItemsApiFixture fixture)
{
    [Fact]
    public async Task Applies_all_migrations_to_fresh_postgresql_database()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = fixture.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var migrations = dbContext.Database.GetMigrations().ToArray();
        var appliedMigrations = (await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).ToArray();
        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToArray();

        Assert.NotEmpty(migrations);
        Assert.Equal(migrations, appliedMigrations);
        Assert.Empty(pendingMigrations);
        Assert.False(dbContext.Database.HasPendingModelChanges());
    }
}
