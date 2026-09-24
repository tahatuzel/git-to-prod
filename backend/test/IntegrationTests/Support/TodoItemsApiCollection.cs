using Xunit;

namespace IntegrationTests.Support;

public static class TodoItemsApiCollection
{
    public const string Name = "TodoItems API";
}

[CollectionDefinition(TodoItemsApiCollection.Name)]
public sealed class TodoItemsApiCollectionFixture : ICollectionFixture<TodoItemsApiFixture>
{
}
