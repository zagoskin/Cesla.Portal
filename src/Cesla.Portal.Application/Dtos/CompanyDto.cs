using Cesla.Portal.Domain.CompanyAggregate;

namespace Cesla.Portal.Application.Dtos;

public record CompanyDto(
    string Id,
    string CompanyName,
    string FantasyName,
    string Cnpj,
    string PhoneNumber,
    string EmailAddress,
    string AddressLine,
    string City,
    string State,
    string Country
);

internal static class CompanyMappings
{
    internal static CompanyDto ToDto(this Company company)
    {
        return new CompanyDto(
            company.Id.Value.ToString(),
            company.CompanyInformation.CompanyName,
            company.CompanyInformation.FantasyName,
            company.CompanyInformation.Cnpj.Value,
            company.ContactInformation.PhoneNumber.Value,
            company.ContactInformation.EmailAddress.Value,
            company.ContactInformation.Address.Line,
            company.ContactInformation.Address.City,
            company.ContactInformation.Address.State,
            company.ContactInformation.Address.Country);
    }
}
