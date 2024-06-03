using Cesla.Portal.Application.Common.Abstractions;
using Cesla.Portal.Application.Companies;
using Cesla.Portal.Domain.CompanyAggregate;
using Microsoft.EntityFrameworkCore;

namespace Cesla.Portal.Infrastructure.Persistence.Companies;
internal sealed class CompanyRepository : 
    BaseRepository,
    ICompanyRepository
{
    public CompanyRepository(CeslaDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<Company?> GetCompanyAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateCompanyAsync(Company company, CancellationToken cancellationToken = default)
    {
        _context.Companies.Update(company);
        await SaveChangesIfNotUnitOfWorkStartedAsync(cancellationToken);
    }
}
