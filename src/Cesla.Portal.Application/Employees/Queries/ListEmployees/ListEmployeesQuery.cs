using Cesla.Portal.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Employees.Queries.ListEmployees;
public record ListEmployeesQuery : IRequest<ErrorOr<List<EmployeeDto>>>;
