using Api.ErrorHandling;
using Application;

namespace Api.Configuration;

public static class ApiServiceConfiguration
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddApiBehaviorConfiguration();
        services.AddApplication(
            configuration["LuckyPenny:LicenseKey"],
            typeof(ApiServiceConfiguration).Assembly);
        services.AddDatabase(configuration);
        services.AddCorsConfiguration(configuration);
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
