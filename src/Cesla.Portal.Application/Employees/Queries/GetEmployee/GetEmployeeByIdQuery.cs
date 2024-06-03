using Cesla.Portal.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Employees.Queries.GetEmployee;
public record GetEmployeeByIdQuery(string EmployeeId) : IRequest<ErrorOr<EmployeeDto>>;
