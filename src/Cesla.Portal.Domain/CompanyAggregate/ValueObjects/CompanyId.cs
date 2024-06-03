using Cesla.Portal.Domain.Common.ValueObjects;

namespace Cesla.Portal.Domain.CompanyAggregate.ValueObjects;
public sealed class CompanyId : TypedId<Guid>
{
    private CompanyId(string value) : base(Guid.Parse(value))
    {
    }

    public static CompanyId Create(Guid id)
    {
        return new CompanyId(id.ToString());
    }

    public static CompanyId CreateUnique()
    {
        return new CompanyId(Guid.NewGuid().ToString());
    }
}
