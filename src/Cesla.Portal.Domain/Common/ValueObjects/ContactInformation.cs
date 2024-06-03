using System.Text.Json.Serialization;
using Cesla.Portal.Domain.Common.Models;

namespace Cesla.Portal.Domain.Common.ValueObjects;

public sealed class ContactInformation : ValueObject
{
    [JsonPropertyName("phoneNumber")]
    public PhoneNumber PhoneNumber { get; } = null!;

    [JsonPropertyName("emailAddress")]
    public EmailAddress EmailAddress { get; } = null!;

    [JsonPropertyName("address")]
    public Address Address { get; } = null!;

    [JsonConstructor]
    public ContactInformation(EmailAddress emailAddress, PhoneNumber phoneNumber, Address address)
    {        
        EmailAddress = emailAddress;
        PhoneNumber = phoneNumber;
        Address = address;
    }
    
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return PhoneNumber.GetEqualityComponents();
        yield return EmailAddress.GetEqualityComponents();
        yield return Address.GetEqualityComponents();
    }

    private ContactInformation() { }
}
