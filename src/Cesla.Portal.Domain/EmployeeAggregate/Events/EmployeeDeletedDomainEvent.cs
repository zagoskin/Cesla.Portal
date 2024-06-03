using Cesla.Portal.Domain.Common.Abstractions;

namespace Cesla.Portal.Domain.EmployeeAggregate.Events;

public record EmployeeDeletedDomainEvent(string EmployeeId) : IDomainEvent;
