using Cesla.Portal.Domain.CompanyAggregate;
using Cesla.Portal.Domain.EmployeeAggregate;
using Cesla.Portal.Infrastructure.Persistence.Companies;
using Cesla.Portal.Infrastructure.Persistence.OutboxDomainEvents;
using Microsoft.EntityFrameworkCore;

namespace Cesla.Portal.Infrastructure.Persistence;

internal sealed class CeslaDbContext : DbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<OutboxDomainEvent> OutboxDomainEvents => Set<OutboxDomainEvent>();

    public CeslaDbContext(DbContextOptions<CeslaDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CeslaDbContext).Assembly);
    }

    public void SeedCompanyData()
    {        
        if (Companies.Any(c => c.Id == CompanySeed.FirstCompany.Id))
        {
            return;
        }
        Companies.Add(CompanySeed.FirstCompany);
        SaveChanges();
    }
}
