using Cesla.Portal.Domain.Common.Models;
using Cesla.Portal.Domain.Common.ValueObjects;
using Cesla.Portal.Domain.CompanyAggregate.Events;
using Cesla.Portal.Domain.CompanyAggregate.ValueObjects;
using ErrorOr;

namespace Cesla.Portal.Domain.CompanyAggregate;
public sealed class Company : AggregateRoot<CompanyId>
{
    public CompanyInformation CompanyInformation { get; private set; } = null!;
    public ContactInformation ContactInformation { get; private set; } = null!;

    public Company(
        CompanyInformation companyInformation,
        ContactInformation contactInformation,
        CompanyId? companyId = null) 
        : base(companyId ?? CompanyId.CreateUnique())
    {        
        CompanyInformation = companyInformation;
        ContactInformation = contactInformation;
    }    

    private Company(): base() { } // EF Core

    public ErrorOr<Success> Update(CompanyInformation companyInformation, ContactInformation contactInformation)
    {
        CompanyInformation = companyInformation;
        ContactInformation = contactInformation;
        RaiseDomainEvent(new CompanyUpdatedDomainEvent(
            Id.Value.ToString(),
            CompanyInformation,
            ContactInformation));
        return Result.Success;
    }
}
