using Cesla.Portal.Application.Common;
using Cesla.Portal.Application.Dtos;
using Cesla.Portal.Domain.EmployeeAggregate;
using Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Employees.Queries.GetEmployee;

internal sealed class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, ErrorOr<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<ErrorOr<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employeeId = EmployeeId.Create(Guid.Parse(request.EmployeeId));
        var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId, cancellationToken);
        return employee is null
            ? GeneralErrors.NotFound(request.EmployeeId, typeof(Employee))
            : employee.ToDto();
    }
}
