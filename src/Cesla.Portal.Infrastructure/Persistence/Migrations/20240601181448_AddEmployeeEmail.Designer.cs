﻿// <auto-generated />
using System;
using Cesla.Portal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cesla.Portal.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(CeslaDbContext))]
    [Migration("20240601181448_AddEmployeeEmail")]
    partial class AddEmployeeEmail
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("cesla")
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Cesla.Portal.Domain.CompanyAggregate.Company", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(36)");

                    b.HasKey("Id");

                    b.ToTable("Companies", "cesla");
                });

            modelBuilder.Entity("Cesla.Portal.Domain.EmployeeAggregate.Employee", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(36)");

                    b.Property<bool>("Deleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.HasKey("Id");

                    b.ToTable("Employees", "cesla");
                });

            modelBuilder.Entity("Cesla.Portal.Infrastructure.Persistence.OutboxDomainEvents.OutboxDomainEvent", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Error")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("ProcessedOnUtc")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("OutboxDomainEvents", "cesla");
                });

            modelBuilder.Entity("Cesla.Portal.Domain.CompanyAggregate.Company", b =>
                {
                    b.OwnsOne("Cesla.Portal.Domain.Common.ValueObjects.ContactInformation", "ContactInformation", b1 =>
                        {
                            b1.Property<string>("CompanyId")
                                .HasColumnType("varchar(36)");

                            b1.HasKey("CompanyId");

                            b1.ToTable("Companies", "cesla");

                            b1.WithOwner()
                                .HasForeignKey("CompanyId");

                            b1.OwnsOne("Cesla.Portal.Domain.Common.ValueObjects.Address", "Address", b2 =>
                                {
                                    b2.Property<string>("ContactInformationCompanyId")
                                        .HasColumnType("varchar(36)");

                                    b2.Property<string>("City")
                                        .IsRequired()
                                        .HasMaxLength(255)
                                        .HasColumnType("varchar(255)")
                                        .HasColumnName("City");

                                    b2.Property<string>("Country")
                                        .IsRequired()
                                        .HasMaxLength(255)
                                        .HasColumnType("varchar(255)")
                                        .HasColumnName("Country");

                                    b2.Property<string>("Line")
                                        .IsRequired()
                                        .HasMaxLength(255)
                                        .HasColumnType("varchar(255)")
                                        .HasColumnName("AddressLine");

                                    b2.Property<string>("State")
                                        .IsRequired()
                                        .HasMaxLength(255)
                                        .HasColumnType("varchar(255)")
                                        .HasColumnName("State");

                                    b2.HasKey("ContactInformationCompanyId");

                                    b2.ToTable("Companies", "cesla");

                                    b2.HasAnnotation("Relational:JsonPropertyName", "address");

                                    b2.WithOwner()
                                        .HasForeignKey("ContactInformationCompanyId");
                                });

                            b1.OwnsOne("Cesla.Portal.Domain.Common.ValueObjects.EmailAddress", "EmailAddress", b2 =>
                                {
                                    b2.Property<string>("ContactInformationCompanyId")
                                        .HasColumnType("varchar(36)");

                                    b2.Property<string>("Value")
                                        .IsRequired()
                                        .HasMaxLength(255)
                                        .HasColumnType("varchar(255)")
                                        .HasColumnName("Email");

                                    b2.HasKey("ContactInformationCompanyId");

                                    b2.ToTable("Companies", "cesla");

                                    b2.HasAnnotation("Relational:JsonPropertyName", "emailAddress");

                                    b2.WithOwner()
                                        .HasForeignKey("ContactInformationCompanyId");
                                });

                            b1.OwnsOne("Cesla.Portal.Domain.Common.ValueObjects.PhoneNumber", "PhoneNumber", b2 =>
                                {
                                    b2.Property<string>("ContactInformationCompanyId")
                                        .HasColumnType("varchar(36)");

                                    b2.Property<string>("Value")
                                        .IsRequired()
                                        .HasMaxLength(25)
                                        .HasColumnType("varchar(25)")
                                        .HasColumnName("PhoneNumber");

                                    b2.HasKey("ContactInformationCompanyId");

                                    b2.ToTable("Companies", "cesla");

                                    b2.HasAnnotation("Relational:JsonPropertyName", "phoneNumber");

                                    b2.WithOwner()
                                        .HasForeignKey("ContactInformationCompanyId");
                                });

                            b1.Navigation("Address")
                                .IsRequired();

                            b1.Navigation("EmailAddress")
                                .IsRequired();

                            b1.Navigation("PhoneNumber")
                                .IsRequired();
                        });

                    b.OwnsOne("Cesla.Portal.Domain.CompanyAggregate.ValueObjects.CompanyInformation", "CompanyInformation", b1 =>
                        {
                            b1.Property<string>("CompanyId")
                                .HasColumnType("varchar(36)");

                            b1.Property<string>("FantasyName")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("varchar(255)")
                                .HasColumnName("FantasyName")
                                .HasAnnotation("Relational:JsonPropertyName", "fantasyName");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("varchar(255)")
                                .HasColumnName("CompanyName")
                                .HasAnnotation("Relational:JsonPropertyName", "name");

                            b1.HasKey("CompanyId");

                            b1.ToTable("Companies", "cesla");

                            b1.WithOwner()
                                .HasForeignKey("CompanyId");

                            b1.OwnsOne("Cesla.Portal.Domain.CompanyAggregate.ValueObjects.Cnpj", "Cnpj", b2 =>
                                {
                                    b2.Property<string>("CompanyInformationCompanyId")
                                        .HasColumnType("varchar(36)");

                                    b2.Property<string>("Value")
                                        .IsRequired()
                                        .HasMaxLength(18)
                                        .HasColumnType("varchar(18)")
                                        .HasColumnName("Cnpj");

                                    b2.HasKey("CompanyInformationCompanyId");

                                    b2.ToTable("Companies", "cesla");

                                    b2.HasAnnotation("Relational:JsonPropertyName", "cnpj");

                                    b2.WithOwner()
                                        .HasForeignKey("CompanyInformationCompanyId");
                                });

                            b1.Navigation("Cnpj")
                                .IsRequired();
                        });

                    b.Navigation("CompanyInformation")
                        .IsRequired();

                    b.Navigation("ContactInformation")
                        .IsRequired();
                });

            modelBuilder.Entity("Cesla.Portal.Domain.EmployeeAggregate.Employee", b =>
                {
                    b.OwnsOne("Cesla.Portal.Domain.EmployeeAggregate.ValueObjects.JobInformation", "JobInformation", b1 =>
                        {
                            b1.Property<string>("EmployeeId")
                                .HasColumnType("varchar(36)");

                            b1.Property<string>("Department")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("varchar(255)")
                                .HasColumnName("Department");

                            b1.Property<string>("JobTitle")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("varchar(255)")
                                .HasColumnName("JobTitle");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employees", "cesla");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.OwnsOne("Cesla.Portal.Domain.EmployeeAggregate.ValueObjects.PersonalInformation", "PersonalInformation", b1 =>
                        {
                            b1.Property<string>("EmployeeId")
                                .HasColumnType("varchar(36)");

                            b1.Property<DateTime>("DateOfBirth")
                                .HasColumnType("date")
                                .HasColumnName("DateOfBirth");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("varchar(255)")
                                .HasColumnName("FirstName");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("varchar(255)")
                                .HasColumnName("LastName");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employees", "cesla");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");

                            b1.OwnsOne("Cesla.Portal.Domain.Common.ValueObjects.EmailAddress", "EmailAddress", b2 =>
                                {
                                    b2.Property<string>("PersonalInformationEmployeeId")
                                        .HasColumnType("varchar(36)");

                                    b2.Property<string>("Value")
                                        .IsRequired()
                                        .HasMaxLength(255)
                                        .HasColumnType("varchar(255)")
                                        .HasColumnName("Email");

                                    b2.HasKey("PersonalInformationEmployeeId");

                                    b2.ToTable("Employees", "cesla");

                                    b2.WithOwner()
                                        .HasForeignKey("PersonalInformationEmployeeId");
                                });

                            b1.Navigation("EmailAddress")
                                .IsRequired();
                        });

                    b.Navigation("JobInformation")
                        .IsRequired();

                    b.Navigation("PersonalInformation")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
