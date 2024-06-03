using Cesla.Portal.Application.Common;
using Cesla.Portal.Application.Dtos;
using Cesla.Portal.Domain.Common.ValueObjects;
using Cesla.Portal.Domain.EmployeeAggregate;
using Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Employees.Commands.CreateEmployee;

internal sealed class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, ErrorOr<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<ErrorOr<EmployeeDto>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var emailAddressResult = EmailAddress.Create(request.EmailAddress);
        if (emailAddressResult.IsError)
        {
            return emailAddressResult.Errors;
        }

        if (await _employeeRepository.ExistsByEmailAsyncAsync(emailAddressResult.Value, cancellationToken: cancellationToken))
        {
            return GeneralErrors.Conflict($"Employee '{request.EmailAddress.ToLower()}' already exists");
        }

        var personalInformation = new PersonalInformation(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            emailAddressResult.Value);

        var jobInformation = new JobInformation(
            request.JobTitle,
            request.Department);

        var createEmployeeResult = Employee.Create(personalInformation, jobInformation);
        if (createEmployeeResult.IsError)
        {
            return createEmployeeResult.Errors;
        }

        await _employeeRepository.AddEmployeeAsync(createEmployeeResult.Value, cancellationToken);
        return createEmployeeResult.Value.ToDto();
    }
}
