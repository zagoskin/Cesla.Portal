using FluentValidation;

namespace Cesla.Portal.Application.Employees.Commands.DeleteEmployee;

internal sealed class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("EmployeeId is required.")
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage("EmployeeId must be a valid GUID.");
    }
}
