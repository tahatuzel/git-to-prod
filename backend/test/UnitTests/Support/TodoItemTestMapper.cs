using Application.Common.Mapping;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace UnitTests.Support;

internal static class TodoItemTestMapper
{
    public static IMapper Instance { get; } = new MapperConfiguration(
        configuration => configuration.AddProfile<TodoItemMappingProfile>(),
        NullLoggerFactory.Instance).CreateMapper();
}
