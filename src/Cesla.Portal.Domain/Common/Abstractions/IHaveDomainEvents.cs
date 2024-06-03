namespace Cesla.Portal.Domain.Common.Abstractions;

public interface IHaveDomainEvents
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void RaiseDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
}
