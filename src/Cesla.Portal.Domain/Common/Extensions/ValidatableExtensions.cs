using Cesla.Portal.Domain.Common.ValueObjects;
using Cesla.Portal.Domain.CompanyAggregate.ValueObjects;
using Throw;

namespace Cesla.Portal.Domain.Common.Extensions;
internal static class ValidatableExtensions
{
    internal static ref readonly Validatable<string> IfNotEmail(this in Validatable<string> validatable)
    {
        if (!EmailAddress.IsValidEmail(validatable))
        {
            throw new ArgumentException("The provided string is not a valid email address", validatable.ParamName);
        }

        return ref validatable;
    }
}
