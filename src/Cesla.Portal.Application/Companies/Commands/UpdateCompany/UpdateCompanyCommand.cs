using Cesla.Portal.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Companies.Commands.UpdateCompany;
public record UpdateCompanyCommand(    
    string CompanyName,
    string FantasyName,
    string Cnpj,
    string PhoneNumber,
    string EmailAddress,
    string AddressLine,
    string City,
    string State,
    string Country
) : IRequest<ErrorOr<CompanyDto>>;
