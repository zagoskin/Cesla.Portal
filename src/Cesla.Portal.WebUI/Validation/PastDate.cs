using System.ComponentModel.DataAnnotations;

namespace Cesla.Portal.WebUI.Validation;


[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PastDate : ValidationAttribute
{
    private readonly DateOnly _upperLimit = DateOnly.FromDateTime(DateTime.UtcNow.Date);
    private string GetErrorMessage(string propertyName) =>
        $"'{propertyName}' must be before {_upperLimit:yyyy/MM/dd}";

    protected override ValidationResult? IsValid(
        object? value, ValidationContext validationContext)
    {
        var propertyName = validationContext.MemberName ?? nameof(DateOnly);
        if (value is not DateOnly dateTime)
        {
            return new ValidationResult("Invalid value type", [propertyName]);
        }

        if (dateTime >= _upperLimit)
        {
            return new ValidationResult(GetErrorMessage(propertyName), [propertyName]);
        }

        return ValidationResult.Success;
    }
}
