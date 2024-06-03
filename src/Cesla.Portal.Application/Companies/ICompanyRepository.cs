using Cesla.Portal.Domain.CompanyAggregate;

namespace Cesla.Portal.Application.Companies;
public interface ICompanyRepository
{    
    Task<Company?> GetCompanyAsync(CancellationToken cancellationToken = default);
    Task UpdateCompanyAsync(Company company, CancellationToken cancellationToken = default);
}
