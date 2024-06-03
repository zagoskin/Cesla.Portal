using Cesla.Portal.Domain.EmployeeAggregate;

namespace Cesla.Portal.Application.Dtos;

public record EmployeeDto(
    string Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string EmailAddress,
    string JobTitle,
    string Department
);

internal static class EmployeeMappings 
{
    internal static EmployeeDto ToDto(this Employee employee)
    {
        return new EmployeeDto(
            employee.Id.Value.ToString(),
            employee.PersonalInformation.FirstName,
            employee.PersonalInformation.LastName,
            employee.PersonalInformation.DateOfBirth,
            employee.PersonalInformation.EmailAddress.Value,
            employee.JobInformation.JobTitle,
            employee.JobInformation.Department);
    }
}
