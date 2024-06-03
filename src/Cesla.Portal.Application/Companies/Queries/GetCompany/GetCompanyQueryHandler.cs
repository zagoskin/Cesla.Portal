using Cesla.Portal.Application.Common;
using Cesla.Portal.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Companies.Queries.GetCompany;

internal sealed class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, ErrorOr<CompanyDto>>
{
    private readonly ICompanyRepository _companyRepository;
    public GetCompanyQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<ErrorOr<CompanyDto>> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetCompanyAsync(cancellationToken);
        return company is not null
            ? company.ToDto()
            : GeneralErrors.Unexpected("Company not found");
    }
}
