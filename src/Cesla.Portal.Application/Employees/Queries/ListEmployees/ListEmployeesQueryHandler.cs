using Cesla.Portal.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Employees.Queries.ListEmployees;

internal sealed class ListEmployeesQueryHandler 
    : IRequestHandler<ListEmployeesQuery, ErrorOr<List<EmployeeDto>>>
{
    private readonly IEmployeeRepository _employeeRepository;
    public ListEmployeesQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<ErrorOr<List<EmployeeDto>>> Handle(ListEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.ListEmployeesAsync(cancellationToken);
        return employees.ConvertAll(e => e.ToDto());
    }
}
