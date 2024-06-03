using Cesla.Portal.Domain.Common.Models;
using Throw;

namespace Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;

public sealed class JobInformation : ValueObject
{
    public string JobTitle { get; init; }
    public string Department { get; init; }

    public JobInformation(string jobTitle, string department)
    {
        JobTitle = jobTitle.ThrowIfNull().IfEmpty();
        Department = department.ThrowIfNull().IfEmpty();
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return JobTitle;
        yield return Department;
    }
}
