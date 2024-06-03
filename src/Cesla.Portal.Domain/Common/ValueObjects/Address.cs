using System.Text.Json.Serialization;
using Cesla.Portal.Domain.Common.Models;
using ErrorOr;
using Throw;

namespace Cesla.Portal.Domain.Common.ValueObjects;

public sealed class Address : ValueObject
{
    public string Line { get; } = null!;
    public string City { get; } = null!;
    public string State { get; } = null!;
    public string Country { get; } = null!;

    [JsonConstructor]
    private Address(string line, string city, string state, string country)
    {
        line.Throw().IfNullOrWhiteSpace(l => l);
        city.Throw().IfNullOrWhiteSpace(c => c);
        state.Throw().IfNullOrWhiteSpace(s => s);
        country.Throw().IfNullOrWhiteSpace(c => c);
        Line = line;
        City = city;
        State = state;
        Country = country;
    }

    public static ErrorOr<Address> Create(
        string line,
        string city,
        string state,
        string country)
    {
        List<Error> errors = [];
        if (string.IsNullOrWhiteSpace(line))
        {
            errors.Add(AddressErrors.LineRequired);
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            errors.Add(AddressErrors.CityRequired);
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            errors.Add(AddressErrors.StateRequired);
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            errors.Add(AddressErrors.CountryRequired);
        }

        return errors.Count is 0
            ? new Address(line, city, state, country)
            : errors;
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Line;
        yield return City;
        yield return State;
        yield return Country;
    }
}

public static class AddressErrors
{
    public static readonly Error LineRequired = Error.Validation(
        code: "Address.LineRequired", 
        description: "Line is required and cannot be empty");
    public static readonly Error CityRequired = Error.Validation(
        code: "Address.CityRequired", 
        description: "City is required and cannot be empty");
    public static readonly Error StateRequired = Error.Validation(
        code: "Address.StateRequired", 
        description: "State is required and cannot be empty");
    public static readonly Error CountryRequired = Error.Validation(
        code: "Address.CountryRequired", 
        description: "Country is required and cannot be empty");
}
