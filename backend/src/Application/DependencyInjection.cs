using Application.Common.Behaviors;
using Application.Features.TodoItem.Command.Create;
using Application.Features.TodoItem.Command.Delete;
using Application.Features.TodoItem.Command.Update;
using Application.Features.TodoItem.Query.GetById;
using Application.Common.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        string? luckyPennyLicenseKey = null,
        params Assembly[] additionalMappingAssemblies)
    {
        var applicationAssembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
        {
            configuration.LicenseKey = luckyPennyLicenseKey;
            configuration.RegisterServicesFromAssembly(applicationAssembly);
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        var mappingAssemblies = new[] { applicationAssembly }
            .Concat(additionalMappingAssemblies)
            .Distinct()
            .ToArray();
        services.AddAutoMapper(
            configuration => configuration.LicenseKey = luckyPennyLicenseKey,
            mappingAssemblies);

        services.AddScoped<IRequestValidator<TodoItemCreateCommand>, TodoItemCreateCommandValidator>();
        services.AddScoped<IRequestValidator<TodoItemUpdateCommand>, TodoItemUpdateCommandValidator>();
        services.AddScoped<IRequestValidator<TodoItemDeleteCommand>, TodoItemDeleteCommandValidator>();
        services.AddScoped<IRequestValidator<TodoItemGetByIdQuery>, TodoItemGetByIdQueryValidator>();

        return services;
    }
}
