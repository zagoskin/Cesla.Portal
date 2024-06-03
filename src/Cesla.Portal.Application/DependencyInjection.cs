using Cesla.Portal.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Cesla.Portal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddMediatRPipeline(configuration);
    }

    private static IServiceCollection AddMediatRPipeline(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>(includeInternalTypes: true)
            .AddMediatR(options =>
            {
                options.RegisterServicesFromAssemblyContaining<IApplicationAssemblyMarker>();
                options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
    }
}
