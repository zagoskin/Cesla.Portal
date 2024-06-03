using FluentValidation;

namespace Cesla.Portal.Application.Employees.Queries.GetEmployee;

internal sealed class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
{
    public GetEmployeeByIdQueryValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("EmployeeId is required")
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage("EmployeeId is not a valid Guid");
    }
}
