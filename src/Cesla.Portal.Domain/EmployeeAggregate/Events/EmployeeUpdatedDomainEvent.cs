using Cesla.Portal.Domain.Common.Abstractions;
using Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;

namespace Cesla.Portal.Domain.EmployeeAggregate.Events;

public record EmployeeUpdatedDomainEvent(
    string EmployeeId,
    PersonalInformation PersonalInformation,
    JobInformation JobInformation) : IDomainEvent;
