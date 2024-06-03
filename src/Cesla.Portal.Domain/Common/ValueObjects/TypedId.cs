using Cesla.Portal.Domain.Common.Models;

namespace Cesla.Portal.Domain.Common.ValueObjects;
public abstract class TypedId<TIdType> : TypedId
    where TIdType : notnull
{
    public TIdType Value { get; }    

    protected TypedId(TIdType value)
    {
        Value = value;
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
