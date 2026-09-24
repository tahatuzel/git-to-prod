using System.Net;
using System.Net.Http.Json;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using IntegrationTests.Support;
using Xunit;

namespace IntegrationTests.Api;

[Collection(TodoItemsApiCollection.Name)]
public sealed class TodoItemCreateValidationApiTests(TodoItemsApiFixture fixture)
{
    [Fact]
    public async Task Returns_validation_error_when_title_is_blank()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var response = await fixture.Client.PostAsJsonAsync(
            "/api/todo-items",
            new CreateTodoItemRequest(" ", "", false),
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<TodoItemResponse>>(cancellationToken);
        Assert.Contains(result!.ErrorResponse!.Errors, error => error.Code == "TodoItem.Title.Required");
    }
}
