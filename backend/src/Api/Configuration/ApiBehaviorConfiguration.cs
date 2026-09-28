using Api.Contracts.Responses;
using Application.Common.ErrorHandling;
using Microsoft.AspNetCore.Mvc;

namespace Api.Configuration;

public static class ApiBehaviorConfiguration
{
    public static IServiceCollection AddApiBehaviorConfiguration(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(entry => entry.Value is not null)
                    .SelectMany(entry => entry.Value!.Errors.Select(modelError => new Error(
                        "Request.Validation",
                        string.IsNullOrWhiteSpace(modelError.ErrorMessage)
                            ? "The request is invalid."
                            : modelError.ErrorMessage,
                        entry.Key,
                        ErrorType.Validation)))
                    .ToArray();

                var response = ErrorResponse.FromErrors(errors);
                return new BadRequestObjectResult(response);
            };
        });

        return services;
    }
}
