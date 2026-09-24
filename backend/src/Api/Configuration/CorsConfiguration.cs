namespace Api.Configuration;

public static class CorsConfiguration
{
    public const string PolicyName = "Frontend";

    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        services.AddCors(options => options.AddPolicy(PolicyName, policy =>
        {
            policy.AllowAnyHeader().AllowAnyMethod();

            if (allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins);
            }
        }));

        return services;
    }
}
