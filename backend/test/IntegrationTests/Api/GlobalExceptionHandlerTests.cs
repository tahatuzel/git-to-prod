using System.Net;
using System.Text.Json;
using Api.Contracts.Responses;
using Api.ErrorHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace IntegrationTests.Api;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task Returns_error_response_for_unhandled_exception()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var context = new DefaultHttpContext();
        await using var body = new MemoryStream();
        context.Response.Body = body;
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

        var handled = await handler.TryHandleAsync(
            context,
            new InvalidOperationException("Internal details"),
            cancellationToken);

        Assert.True(handled);
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        body.Position = 0;
        var response = await JsonSerializer.DeserializeAsync<ErrorResponse>(
            body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web),
            cancellationToken);
        var error = Assert.Single(response!.Errors);
        Assert.Equal("General.Unexpected", error.Code);
        Assert.DoesNotContain("Internal details", error.Message);
    }
}
