using Cesla.Portal.Application.Common.Abstractions;
using Cesla.Portal.Application.Companies;
using Cesla.Portal.Application.Employees;
using Cesla.Portal.Infrastructure.BackgroundServices;
using Cesla.Portal.Infrastructure.Persistence;
using Cesla.Portal.Infrastructure.Persistence.Companies;
using Cesla.Portal.Infrastructure.Persistence.Employees;
using Cesla.Portal.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Cesla.Portal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddRepositories(configuration)
            .AddInterceptors(configuration)
            .AddPersistence(configuration)
            .AddBackgroundServices(configuration);
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddHostedService<OutboxDomainEventPublisher>();
    }

    private static IServiceCollection AddInterceptors(this IServiceCollection services, IConfiguration configuration)
    {
        //the order here is important, whe want first to mark entities as deleted and then write outbox events
        return services
            .AddScoped<ISaveChangesInterceptor, LogicDeleteInterceptor>()
            .AddScoped<ISaveChangesInterceptor, OutboxDomainEventWriterInterceptor>();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<ICompanyRepository, CompanyRepository>()
            .AddScoped<IEmployeeRepository, EmployeeRepository>();
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(SchemaConstants.Name)!;
        return services.AddDbContext<CeslaDbContext>((serviceProvider, options) =>
        {
            options.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
            options.UseMySQL(connectionString, mySqlOptionsAction =>
            {
                mySqlOptionsAction.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);                
            });
        });
    }    
}
