using Cesla.Portal.Domain.Common.Abstractions;
using Cesla.Portal.Domain.Common.Models;
using Cesla.Portal.Domain.EmployeeAggregate.Events;
using Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;
using ErrorOr;

namespace Cesla.Portal.Domain.EmployeeAggregate;
public sealed class Employee : AggregateRoot<EmployeeId>,
    ILogicDeletable
{
    public PersonalInformation PersonalInformation { get; private set; } = null!;
    public JobInformation JobInformation { get; private set; } = null!;

    internal Employee(
        PersonalInformation personalInformation,
        JobInformation jobInformation,
        EmployeeId? employeeId)
        : base(employeeId ?? EmployeeId.CreateUnique())
    {        
        PersonalInformation = personalInformation;
        JobInformation = jobInformation;
        RaiseDomainEvent(new EmployeeCreatedDomainEvent(
            Id.Value.ToString(),
            PersonalInformation,
            JobInformation));
    }

    public static ErrorOr<Employee> Create(
        PersonalInformation personalInformation,
        JobInformation jobInformation,
        EmployeeId? employeeId = null)
    {
        // We imagine only adults can work in our company and of course this is a naive way to calculate age
        var age = DateTime.UtcNow.Year - personalInformation.DateOfBirth.Year;
        return age < 18
            ? Error.Validation(
                code: "Employee.InvalidAge",
                description: "Only people at least 18 years old can work in our company")
            : new Employee(personalInformation, jobInformation, employeeId);
    }

    public ErrorOr<Success> Update(
        PersonalInformation personalInformation,
        JobInformation jobInformation)
    {
        PersonalInformation = personalInformation;
        JobInformation = jobInformation;
        RaiseDomainEvent(new EmployeeUpdatedDomainEvent(
            Id.Value.ToString(),
            PersonalInformation,
            JobInformation));
        return Result.Success;
    }

    public void MarkAsDeleted()
    {
        RaiseDomainEvent(new EmployeeDeletedDomainEvent(Id.Value.ToString()));
    }

    private Employee() { }
}
