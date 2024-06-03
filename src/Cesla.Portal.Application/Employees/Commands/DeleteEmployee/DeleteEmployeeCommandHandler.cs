using Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Employees.Commands.DeleteEmployee;

internal sealed class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, ErrorOr<Success>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }
    public async Task<ErrorOr<Success>> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetEmployeeByIdAsync(
            EmployeeId.Create(Guid.Parse(request.EmployeeId)),
            cancellationToken);

        if (employee is null)
        {
            return Error.NotFound(description: $"Employee {request.EmployeeId} does not exist.");
        }

        await _employeeRepository.DeleteEmployeeAsync(employee, cancellationToken);
        return Result.Success;
    }
}
