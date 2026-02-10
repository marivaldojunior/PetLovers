using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PetLovers.Application.Behaviors;
using System.Reflection;

namespace PetLovers.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        
        services.AddAutoMapper(assembly);
        
        services.AddValidatorsFromAssembly(assembly);
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
