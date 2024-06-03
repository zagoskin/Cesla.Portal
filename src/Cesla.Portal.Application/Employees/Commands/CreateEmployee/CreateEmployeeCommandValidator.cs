using FluentValidation;

namespace Cesla.Portal.Application.Employees.Commands.CreateEmployee;

internal sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(255)
            .WithMessage("First name must not exceed 255 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(255)
            .WithMessage("Last name must not exceed 255 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required")
            .Must(x => x < DateTime.UtcNow.Date)
            .WithMessage("Date of birth must be in the past");

        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("Email address is required")
            .EmailAddress()
            .WithMessage("Email address is not valid");

        RuleFor(x => x.JobTitle)
            .NotEmpty()
            .WithMessage("Job title is required");

        RuleFor(x => x.Department)
            .NotEmpty()
            .WithMessage("Department is required");
    }
}
