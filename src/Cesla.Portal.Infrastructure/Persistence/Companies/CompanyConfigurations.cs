using Cesla.Portal.Domain.Common.ValueObjects;
using Cesla.Portal.Domain.CompanyAggregate;
using Cesla.Portal.Domain.CompanyAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cesla.Portal.Infrastructure.Persistence.Companies;
internal sealed class CompanyConfigurations : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnType("varchar(36)")
            .HasConversion(
                id => id.Value.ToString(),
                str => CompanyId.Create(Guid.Parse(str)));

        builder.ComplexProperty(x => x.CompanyInformation, cib =>
        {
            cib.Property(ci => ci.CompanyName)
                .HasColumnName("CompanyName")
                .HasMaxLength(255)
                .IsRequired();

            cib.Property(ci => ci.FantasyName)
                .HasColumnName("FantasyName")
                .HasMaxLength(255)
                .IsRequired();

            cib.ComplexProperty(ci => ci.Cnpj, cnpj =>
            {
                cnpj.Property(x => x.Value)
                    .HasColumnName("Cnpj")
                    .HasMaxLength(18)
                    .IsRequired();
            });
        });

        builder.ComplexProperty(x => x.ContactInformation, cib =>
        {
            cib.ComplexProperty(cib => cib.PhoneNumber, pn =>
            {
                pn.Property(x => x.Value)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(25)
                    .IsRequired();
            });

            cib.ComplexProperty(cib => cib.EmailAddress, em =>
            {
                em.Property(x => x.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(255)
                    .IsRequired();
            });

            cib.ComplexProperty(cib => cib.Address, ad =>
            {
                ad.Property(x => x.Line)
                    .HasColumnName("AddressLine")
                    .HasMaxLength(255)
                    .IsRequired();

                ad.Property(x => x.City)
                    .HasColumnName("City")
                    .HasMaxLength(255)
                    .IsRequired();

                ad.Property(x => x.State)
                    .HasColumnName("State")
                    .HasMaxLength(255)
                    .IsRequired();

                ad.Property(x => x.Country)
                    .HasColumnName("Country")
                    .HasMaxLength(255)
                    .IsRequired();
            });
        });        
    }
}

internal static class CompanySeed
{
    internal static Company FirstCompany = new(
        CompanyInformation.Create(
            "Cesla",
            "Cesla Fantasy",
            Cnpj.Create("29.220.197/0001-56").Value)
        .Value,
        new ContactInformation(
            EmailAddress.Create("suporte@ws-solution.com").Value,
            PhoneNumber.Create("+5508000023752").Value,
            Address.Create(
                "Av. Barão de Itapura, 2323 - Ed. Saint Etienne, conjunto 4 - Jd. Guanabara",
                "Campinas",
                "SP",
                "Brasil").Value),
        CompanyId.Create(Guid.Parse("f3b3b3b3-3b3b-3b3b-3b3b-3b3b3b3b3b3b")));

    internal static object ToAnonymous(this Company company)
    {
        return new
        {
            company.Id,
        };
    }

    internal static object GetContactInformationAnonymous(this Company company)
    {
        return new
        {
            CompanyId = company.Id,
            Email = company.ContactInformation.EmailAddress.Value,
            company.ContactInformation.PhoneNumber.Value,
            company.ContactInformation.Address.Line,
            company.ContactInformation.Address.City,
            company.ContactInformation.Address.State,
            company.ContactInformation.Address.Country
        };
    }

    internal static object GetCompanyInformationAnonymous(this Company company)
    {
        return new
        {
            CompanyId = company.Id,
            company.CompanyInformation.CompanyName,
            company.CompanyInformation.FantasyName,
            Cnpj = new
            {
                Value = company.CompanyInformation.Cnpj.Value
            }
        };
    }
}
