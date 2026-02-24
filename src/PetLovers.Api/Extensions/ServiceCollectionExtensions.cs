using PetLovers.Api.Filters;

namespace PetLovers.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        });

        services.AddEndpointsApiExplorer();

        services.AddJwtAuthentication(configuration);
        services.AddSwaggerWithAuth();

        services.AddHealthChecks();

        return services;
    }
}
