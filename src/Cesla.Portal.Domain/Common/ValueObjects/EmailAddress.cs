using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Cesla.Portal.Domain.Common.Extensions;
using Cesla.Portal.Domain.Common.Models;
using ErrorOr;
using Throw;

namespace Cesla.Portal.Domain.Common.ValueObjects;

public sealed class EmailAddress : ValueObject
{
    public string Value { get; } = null!;

    [JsonConstructor]
    private EmailAddress(string value)
    {
        value = value.Trim().ToLower();
        value.Throw().IfNotEmail();
        Value = value;
    }

    public static ErrorOr<EmailAddress> Create(string value)
    {
        if (!IsValidEmail(value))
        {
            return Error.Validation(code: "EmailAddress.InvalidInput", description: "Invalid email address.");
        }
        return new EmailAddress(value);
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            email = Regex.Replace(
                email, @"(@)(.+)$",
                DomainMapper,
                RegexOptions.Compiled,
                TimeSpan.FromMilliseconds(200));

            static string DomainMapper(Match match)
            {
                var idn = new IdnMapping();
                string domainName = idn.GetAscii(match.Groups[2].Value);
                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase,
                TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
