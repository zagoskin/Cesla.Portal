using System.Text.Json.Serialization;
using Cesla.Portal.Domain.Common.Models;
using Cesla.Portal.Domain.Common.ValueObjects;
using Throw;

namespace Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;

public sealed class PersonalInformation : ValueObject
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public DateTime DateOfBirth { get; init; }
    public EmailAddress EmailAddress { get; init; } = null!;
    
    public PersonalInformation(string firstName, string lastName, DateTime dateOfBirth, EmailAddress emailAddress)
    {
        FirstName = firstName.ThrowIfNull().IfEmpty();
        LastName = lastName.ThrowIfNull().IfEmpty();
        DateOfBirth = dateOfBirth.Throw().IfGreaterThanOrEqualTo(DateTime.UtcNow.Date);
        EmailAddress = emailAddress.ThrowIfNull();
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        yield return DateOfBirth;
        yield return EmailAddress.GetEqualityComponents();
    }

    [JsonConstructor]
    private PersonalInformation() { } // EF
}
