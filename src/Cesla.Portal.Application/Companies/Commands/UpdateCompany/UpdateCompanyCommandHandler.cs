using Cesla.Portal.Application.Common;
using Cesla.Portal.Application.Dtos;
using Cesla.Portal.Domain.Common.ValueObjects;
using Cesla.Portal.Domain.CompanyAggregate.ValueObjects;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Companies.Commands.UpdateCompany;

internal sealed class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, ErrorOr<CompanyDto>>
{
    private readonly ICompanyRepository _companyRepository;

    public UpdateCompanyCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<ErrorOr<CompanyDto>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetCompanyAsync(cancellationToken);
        if (company is null)
        {
            return GeneralErrors.Unexpected("Company not found");
        }

        var companyInformationResult = CreateCompanyInformation(request);
        if (companyInformationResult.IsError)
        {
            return companyInformationResult.Errors;
        }

        var contactInformationResult = CreateContactInformation(request);
        if (contactInformationResult.IsError)
        {
            return contactInformationResult.Errors;
        }

        ErrorOr<Success> updateResult = company.Update(
            companyInformationResult.Value,
            contactInformationResult.Value);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _companyRepository.UpdateCompanyAsync(company, cancellationToken);
        return company.ToDto();
    }

    private ErrorOr<CompanyInformation> CreateCompanyInformation(UpdateCompanyCommand request)
    {       
        var cnpjResult = Cnpj.Create(request.Cnpj);
        if (cnpjResult.IsError)
        {
            return cnpjResult.Errors;
        }

        var companyInformationResult = CompanyInformation.Create(
            request.CompanyName,
            request.FantasyName,
            cnpjResult.Value);
        
        if (companyInformationResult.IsError)
        {
            return companyInformationResult.Errors;
        }

        return companyInformationResult.Value;
    }

    private ErrorOr<ContactInformation> CreateContactInformation(UpdateCompanyCommand request)
    {
        List<Error> errors = [];
        var emailAddressResult = EmailAddress.Create(request.EmailAddress);
        var phoneResult = PhoneNumber.Create(request.PhoneNumber);
        var addressResult = Address.Create(request.AddressLine, request.City, request.State, request.Country);

        errors.AddRange(emailAddressResult.ErrorsOrEmptyList);
        errors.AddRange(phoneResult.ErrorsOrEmptyList);
        errors.AddRange(addressResult.ErrorsOrEmptyList);

        if (errors.Count is not 0)
        {
            return errors;
        }

        return new ContactInformation(
            emailAddressResult.Value,
            phoneResult.Value,
            addressResult.Value);
    }
}
