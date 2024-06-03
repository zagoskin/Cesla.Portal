namespace Cesla.Portal.Domain.Common.Models;
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : TypedId
{
    protected AggregateRoot(TId id) : base(id)
    {
    }

    protected AggregateRoot()
    {
    }
}
