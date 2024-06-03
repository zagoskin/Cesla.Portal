using Cesla.Portal.Application.Common.Abstractions;
using Cesla.Portal.Application.Employees;
using Cesla.Portal.Domain.Common.ValueObjects;
using Cesla.Portal.Domain.EmployeeAggregate;
using Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Cesla.Portal.Infrastructure.Persistence.Employees;
internal sealed class EmployeeRepository :
    BaseRepository,
    IEmployeeRepository
{
    public EmployeeRepository(CeslaDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(employee, cancellationToken);
        await SaveChangesIfNotUnitOfWorkStartedAsync(cancellationToken);
    }

    public async Task DeleteEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Remove(employee);
        await SaveChangesIfNotUnitOfWorkStartedAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsyncAsync(EmailAddress emailAddress, EmployeeId? employeeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Employees
            .Where(e => e.PersonalInformation.EmailAddress == emailAddress);
        if (employeeId is not null)
        {
            query = query.Where(e => e.Id != employeeId);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByIdAsync(EmployeeId employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);
    }

    public async Task<List<Employee>> ListEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Update(employee);
        await SaveChangesIfNotUnitOfWorkStartedAsync(cancellationToken);
    }
}
