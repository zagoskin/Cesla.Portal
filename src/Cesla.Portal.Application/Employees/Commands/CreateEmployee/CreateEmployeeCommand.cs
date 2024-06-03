using Cesla.Portal.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Employees.Commands.CreateEmployee;
public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string EmailAddress,
    string JobTitle,
    string Department
) : IRequest<ErrorOr<EmployeeDto>>;
