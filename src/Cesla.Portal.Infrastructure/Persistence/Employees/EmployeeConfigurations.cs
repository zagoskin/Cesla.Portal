using Cesla.Portal.Domain.EmployeeAggregate;
using Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cesla.Portal.Infrastructure.Persistence.Employees;
internal sealed class EmployeeConfigurations : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnType("varchar(36)")
            .HasConversion(
                id => id.Value.ToString(),
                str => EmployeeId.Create(Guid.Parse(str)));

        builder.ComplexProperty(x => x.PersonalInformation, pib =>
        {
            pib.Property(pi => pi.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(255)
                .IsRequired();

            pib.Property(pi => pi.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(255)
                .IsRequired();

            pib.Property(pi => pi.DateOfBirth)
                .HasColumnName("DateOfBirth")
                .HasColumnType("date")
                .IsRequired();

            pib.ComplexProperty(cib => cib.EmailAddress, em =>
            {
                em.Property(x => x.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(255)
                    .IsRequired();
            });

        });

        builder.ComplexProperty(x => x.JobInformation, jib =>
        {
            jib.Property(ji => ji.JobTitle)
                .HasColumnName("JobTitle")
                .HasMaxLength(255)
                .IsRequired();

            jib.Property(ji => ji.Department)
                .HasColumnName("Department")
                .HasMaxLength(255)
                .IsRequired();
        });

        builder
            .Property<bool>("Deleted")
            .HasDefaultValue(false);

        builder
            .HasQueryFilter(g => EF.Property<bool>(g, "Deleted") == false);
    }

    
}
