using System.Net;
using System.Net.Http.Json;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using IntegrationTests.Support;
using Xunit;

namespace IntegrationTests.Api;

[Collection(TodoItemsApiCollection.Name)]
public sealed class TodoItemsCrudApiTests(TodoItemsApiFixture fixture)
{
    [Fact]
    public async Task Persists_and_returns_expected_responses_for_crud_operations()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var client = fixture.Client;
        using var createResponse = await client.PostAsJsonAsync(
            "/api/todo-items",
            new CreateTodoItemRequest("First task", "Created by integration test", false),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<TodoItemResponse>>(cancellationToken);
        Assert.NotNull(created);
        Assert.True(created.Result.IsSuccess);
        Assert.NotNull(created.Response);
        var id = created.Response.Id;
        Assert.True(id > 0);
        Assert.Equal($"/api/todo-items/{id}", createResponse.Headers.Location!.AbsolutePath);

        var fetched = await client.GetFromJsonAsync<ApiResponse<TodoItemResponse>>($"/api/todo-items/{id}", cancellationToken);
        Assert.NotNull(fetched);
        Assert.Equal("First task", fetched.Response?.Title);

        var list = await client.GetFromJsonAsync<ApiResponse<TodoItemsResponse>>("/api/todo-items", cancellationToken);
        Assert.NotNull(list);
        Assert.Contains(list.Response!.Items, item => item.Id == id);

        using var updateResponse = await client.PutAsJsonAsync(
            $"/api/todo-items/{id}",
            new UpdateTodoItemRequest("Updated task", "Done", true),
            cancellationToken);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<TodoItemResponse>>(cancellationToken);
        Assert.NotNull(updated);
        Assert.Equal("Updated task", updated.Response?.Title);
        Assert.True(updated.Response?.IsCompleted);

        using var deleteResponse = await client.DeleteAsync($"/api/todo-items/{id}", cancellationToken);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        var deleted = await deleteResponse.Content.ReadFromJsonAsync<ApiResponse<TodoItemDeletedResponse>>(cancellationToken);
        Assert.Equal(id, deleted?.Response?.Id);

        using var missingResponse = await client.GetAsync($"/api/todo-items/{id}", cancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
    }
}
