using System.Net;
using System.Net.Http.Json;
using System.Text;
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
        var result = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken);
        Assert.Contains(result!.Errors, error => error.Code == "TodoItem.Title.Required");
    }

    [Fact]
    public async Task Returns_same_error_shape_for_invalid_json()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var content = new StringContent("{", Encoding.UTF8, "application/json");
        using var response = await fixture.Client.PostAsync("/api/todo-items", content, cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken);
        Assert.Contains(result!.Errors, error => error.Code == "Request.Validation");
    }
}
