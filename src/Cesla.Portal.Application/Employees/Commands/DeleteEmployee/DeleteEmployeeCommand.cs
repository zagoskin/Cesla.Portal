using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Employees.Commands.DeleteEmployee;
public record DeleteEmployeeCommand(string EmployeeId) : IRequest<ErrorOr<Success>>;
