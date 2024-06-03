using Cesla.Portal.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Employees.Commands.UpdateEmployee;
public record UpdateEmployeeCommand(
    string EmployeeId,
    string FirstName,
    string LastName,
    string EmailAddress,
    DateTime DateOfBirth,
    string JobTitle,
    string Department
) : IRequest<ErrorOr<EmployeeDto>>;
