using Cesla.Portal.Domain.Common.Models;
using Cesla.Portal.Domain.Common.ValueObjects;
using Cesla.Portal.Domain.CompanyAggregate.ValueObjects;

namespace Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;

public sealed class EmployeeId : TypedId<Guid>
{
    private EmployeeId(Guid value) : base(value)
    {
    }

    public static EmployeeId Create(Guid id)
    {
        return new EmployeeId(id);
    }

    public static EmployeeId CreateUnique()
    {
        return new EmployeeId(Guid.NewGuid());
    }
}
