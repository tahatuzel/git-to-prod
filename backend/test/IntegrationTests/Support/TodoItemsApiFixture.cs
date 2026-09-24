using Api.Controllers;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace IntegrationTests.Support;

public sealed class TodoItemsApiFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer database = new PostgreSqlBuilder("postgres:17.11-alpine").Build();
    private WebApplicationFactory<TodoItemsController>? factory;

    public HttpClient Client { get; private set; } = null!;

    public IServiceProvider Services => factory!.Services;

    public async ValueTask InitializeAsync()
    {
        await database.StartAsync();

        factory = new WebApplicationFactory<TodoItemsController>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configuration) =>
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = database.GetConnectionString()
                    }));
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(database.GetConnectionString()));
                });
            });

        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Database.GetConnectionString() != database.GetConnectionString())
        {
            throw new InvalidOperationException("The test API is not using its disposable database.");
        }

        await dbContext.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        Client?.Dispose();
        factory?.Dispose();
        await database.DisposeAsync();
    }
}
