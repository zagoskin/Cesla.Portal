using Cesla.Portal.Domain.Common.Abstractions;
using Cesla.Portal.Domain.Common.ValueObjects;
using Cesla.Portal.Domain.CompanyAggregate.ValueObjects;

namespace Cesla.Portal.Domain.CompanyAggregate.Events;
public record CompanyUpdatedDomainEvent(
    string CompanyId,
    CompanyInformation CompanyInformation,
    ContactInformation ContactInformation) : IDomainEvent;
