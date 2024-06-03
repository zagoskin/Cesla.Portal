using Cesla.Portal.Domain.Common.ValueObjects;
using Cesla.Portal.Domain.EmployeeAggregate;
using Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;

namespace Cesla.Portal.Application.Employees;
public interface IEmployeeRepository
{
    Task<Employee?> GetEmployeeByIdAsync(EmployeeId employeeId, CancellationToken cancellationToken = default);
    Task<List<Employee>> ListEmployeesAsync(CancellationToken cancellationToken = default);
    Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);
    Task UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);
    Task DeleteEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsyncAsync(EmailAddress emailAddress, EmployeeId? employeeId = null, CancellationToken cancellationToken = default);
}
