using Application.Common.ErrorHandling;
using Xunit;

namespace UnitTests.Support;

internal static class TodoItemTestAssertions
{
    public static void AssertNotFound(IReadOnlyList<Error> errors, int id)
    {
        var itemError = Assert.Single(errors);
        Assert.Equal("TodoItem.NotFound", itemError.Code);
        Assert.Equal($"Todo item with id {id} was not found.", itemError.Message);
        Assert.Equal(ErrorType.NotFound, itemError.Type);
    }
}
