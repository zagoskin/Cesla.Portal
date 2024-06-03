using System.Text.Json.Serialization;
using Cesla.Portal.Domain.Common.Models;
using ErrorOr;

namespace Cesla.Portal.Domain.CompanyAggregate.ValueObjects;

public sealed class CompanyInformation : ValueObject
{
    [JsonPropertyName("companyName")]
    public string CompanyName { get; init; } = null!;

    [JsonPropertyName("fantasyName")]
    public string FantasyName { get; init; } = null!;

    [JsonPropertyName("cnpj")]
    public Cnpj Cnpj { get; init; } = null!;

    [JsonConstructor]
    private CompanyInformation(string companyName, string fantasyName, Cnpj cnpj)
    {
        CompanyName = companyName;
        FantasyName = fantasyName;
        Cnpj = cnpj;
    }

    public static ErrorOr<CompanyInformation> Create(
        string name,
        string fantasyName,
        Cnpj cnpj)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation(code: "CompanyInformation.NameRequired", description: "Name is required");
        }

        if (string.IsNullOrWhiteSpace(fantasyName))
        {
            return Error.Validation(code: "CompanyInformation.FantasyNameRequired", description: "FantasyName is required");
        }

        return new CompanyInformation(name, fantasyName, cnpj);
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CompanyName;
        yield return FantasyName;
        yield return Cnpj.GetEqualityComponents();
    }

    private CompanyInformation() { }
}
