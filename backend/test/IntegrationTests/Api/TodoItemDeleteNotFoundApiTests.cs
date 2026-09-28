using System.Net;
using System.Net.Http.Json;
using Api.Contracts.Responses;
using IntegrationTests.Support;
using Xunit;

namespace IntegrationTests.Api;

[Collection(TodoItemsApiCollection.Name)]
public sealed class TodoItemDeleteNotFoundApiTests(TodoItemsApiFixture fixture)
{
    [Fact]
    public async Task Returns_not_found_when_deleting_unknown_id()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var response = await fixture.Client.DeleteAsync("/api/todo-items/2147483647", cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken);
        Assert.Contains(result!.Errors, error => error.Code == "TodoItem.NotFound");
    }
}
