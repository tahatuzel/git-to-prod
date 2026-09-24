namespace Api.Configuration;

public static class ApiApplicationConfiguration
{
    public static WebApplication UseApiConfiguration(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseCors(CorsConfiguration.PolicyName);
        app.MapControllers();

        return app;
    }
}
