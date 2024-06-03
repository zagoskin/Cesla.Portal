using System.Text.Json.Serialization;
using Cesla.Portal.Domain.Common.Models;
using ErrorOr;

namespace Cesla.Portal.Domain.Common.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private const string ValidCharacters = "0123456789-+()";
    public string Value { get; } = null!;

    [JsonConstructor]
    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static ErrorOr<PhoneNumber> Create(string phoneNumber)
    {
        phoneNumber = phoneNumber.Trim().Replace(" ","");
        if (!IsValidPhoneNumber(phoneNumber))
        {
            return Error.Validation(code: "PhoneNumber.InvalidInput", description: "Invalid phone number");
        }

        return new PhoneNumber(phoneNumber);
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return false;
        }

        return phoneNumber.All(ValidCharacters.Contains)
            && phoneNumber.Any(char.IsDigit);
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
