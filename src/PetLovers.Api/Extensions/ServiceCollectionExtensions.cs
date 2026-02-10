using Microsoft.OpenApi.Models;
using PetLovers.Api.Filters;

namespace PetLovers.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        });

        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PetLovers API",
                Version = "v1",
                Description = "API for Pet Adoption Platform - PetLovers",
                Contact = new OpenApiContact
                {
                    Name = "PetLovers Team",
                    Email = "contact@petlovers.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            options.UseInlineDefinitionsForEnums();
        });

        services.AddHealthChecks();

        return services;
    }
}
