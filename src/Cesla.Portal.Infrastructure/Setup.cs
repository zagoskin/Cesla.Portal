using Cesla.Portal.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cesla.Portal.Infrastructure;
public static class Setup
{
    public static void UseDatabaseSeed(this IApplicationBuilder app, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<CeslaDbContext>();
        context.Database.Migrate();
        context.SeedCompanyData();
    }
}
