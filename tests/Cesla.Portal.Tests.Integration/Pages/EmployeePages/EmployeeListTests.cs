using Bogus;
using Cesla.Portal.Domain.EmployeeAggregate;
using Cesla.Portal.Infrastructure.Persistence.Interceptors;
using Cesla.Portal.Infrastructure.Persistence;
using Cesla.Portal.WebUI.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Cesla.Portal.Application.Dtos;
using Cesla.Portal.Domain.EmployeeAggregate.Events;

namespace Cesla.Portal.Tests.Integration.Pages.EmployeePages;

[Collection("Test collection")]
public class EmployeeListTests
{
    private readonly SharedTestContext _testContext;
    private readonly Faker<EmployeeFormModel> _employeeGenerator = new Faker<EmployeeFormModel>()
        .RuleFor(x => x.FirstName, faker => faker.Person.FirstName)
        .RuleFor(x => x.LastName, faker => faker.Person.LastName)
        .RuleFor(x => x.EmailAddress, faker => faker.Person.Email)
        .RuleFor(x => x.DateOfBirth, faker => DateOnly.FromDateTime(faker.Person.DateOfBirth.Date))
        .RuleFor(x => x.Department, faker => faker.Commerce.Department())
        .RuleFor(x => x.JobTitle, faker => faker.Name.JobTitle());
    private readonly CeslaDbContext _ceslaDbContext;

    public EmployeeListTests(SharedTestContext testContext)
    {
        _testContext = testContext;
        var options = new DbContextOptionsBuilder<CeslaDbContext>()
            .UseMySQL("server=localhost;port=33306;database=cesla;user=ceslatester;password=t35t3r!1234;")
            .AddInterceptors(new LogicDeleteInterceptor())
            .Options;
        _ceslaDbContext = new CeslaDbContext(options);
    }

    [Fact]
    public async Task EmployeeList_ShouldContainEmployee_WhenEmployeeExists()
    {
        // Arrange
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });
        var employee = await CreateEmployee(page);
        var key = employee.EmailAddress.ToLower();

        // Act
        await page.GotoAsync("/employees");

        var fullname = await page.GetByTestId($"{key}-fullname")
            .InnerTextAsync();
        var emailAddress = await page.GetByTestId($"{key}-email")
            .InnerTextAsync();
        var department = await page.GetByTestId($"{key}-department")
            .InnerTextAsync();
        var job = await page.GetByTestId($"{key}-job")
            .InnerTextAsync();
        var dateOfBirth = await page.GetByTestId($"{key}-dob")
            .InnerTextAsync();
        
        // Assert
        fullname.Should()
            .Be($"{employee.FirstName} {employee.LastName}");
        emailAddress.Should()
            .BeEquivalentTo(employee.EmailAddress);
        department.Should()
            .Be(employee.Department);
        job.Should()
            .Be(employee.JobTitle);
        dateOfBirth.Should()
            .Be(employee.DateOfBirth.ToString("yyyy/MM/dd"));

        await page.CloseAsync();
    }

    [Fact]
    public async Task EmployeeList_ShouldBeOrderedByName_WhenEmployeesExists()
    {
        // Arrange
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });
        var employee1 = await CreateEmployee(page);
        var employee2 = await CreateEmployee(page);

        // Act
        await page.GotoAsync("/employees");

        var firstEmployeeName = await page.Locator("tbody tr:first-child td:first-child")
           .InnerTextAsync();
        var secondEmployeeName = await page.Locator("tbody tr:nth-child(2) td:first-child")
            .InnerTextAsync();

        var areOrdered = firstEmployeeName.CompareTo(secondEmployeeName) <= 0;

        // Assert
        firstEmployeeName.Should()
            .NotBeNullOrEmpty();
        secondEmployeeName.Should()
            .NotBeNullOrEmpty();
        areOrdered.Should().BeTrue();

        await page.CloseAsync();
    }

    [Fact]
    public async Task EmployeeList_ShouldNotContainDeletedEmployee_WhenEmployeeIsSoftDeleted()
    {
        // Arrange
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });
        var employee = await CreateEmployee(page);
        var key = employee.EmailAddress.ToLower();

        // Act
        await DeleteEmployee(page, key);
        await page.GotoAsync("/employees");
        var findEmailAction = async() => await page.GetByTestId($"{key}-email")
            .InnerTextAsync(new LocatorInnerTextOptions
            {
                Timeout = 2000
            });

        // Assert
        await findEmailAction
            .Should().ThrowAsync<TimeoutException>();

        await page.CloseAsync();
    }

    [Fact]
    public async Task Employee_ShouldStillExistButBeFiltered_WhenDeletedFromList()
    {
        // Arrange
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });
        var employeeFormModel = await CreateEmployee(page);
        var key = employeeFormModel.EmailAddress.ToLower();
        await DeleteEmployee(page, key);

        // Act
        var filteredEmployee = await _ceslaDbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PersonalInformation.EmailAddress.Value == employeeFormModel.EmailAddress);
        var unfilteredEmployee = await _ceslaDbContext.Employees
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.PersonalInformation.EmailAddress.Value == employeeFormModel.EmailAddress);

        // Assert
        filteredEmployee.Should().BeNull();
        unfilteredEmployee.Should().NotBeNull();
        var dto = unfilteredEmployee!.ToDto();
        dto.FirstName.Should().BeEquivalentTo(employeeFormModel.FirstName);
        dto.LastName.Should().BeEquivalentTo(employeeFormModel.LastName);
        dto.DateOfBirth.ToString("yyyy/MM/dd").Should().Be(employeeFormModel.DateOfBirth.ToString("yyyy/MM/dd"));
        dto.EmailAddress.Should().BeEquivalentTo(employeeFormModel.EmailAddress);
        dto.Department.Should().BeEquivalentTo(employeeFormModel.Department);
        dto.JobTitle.Should().BeEquivalentTo(employeeFormModel.JobTitle);

        await page.CloseAsync();
    }

    [Fact]
    public async Task EmployeeDelete_ShouldLogDeletedEvent_WhenDeleted()
    {
        // Arrange
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });
        var employeeFormModel = await CreateEmployee(page);
        var key = employeeFormModel.EmailAddress.ToLower();
        var now = DateTime.UtcNow;
        await DeleteEmployee(page, key);

        // Act
        var deletedEvent = await _ceslaDbContext.OutboxDomainEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Type == nameof(EmployeeDeletedDomainEvent));

        // Assert
        deletedEvent.Should().NotBeNull();
        deletedEvent!.CreatedOnUtc.Should().BeCloseTo(now, precision: TimeSpan.FromSeconds(1));

        await page.CloseAsync();
    }

    private async Task<EmployeeFormModel> CreateEmployee(IPage page)
    {
        await page.GotoAsync("/employees/create");
        var employee = _employeeGenerator.Generate();

        await page.FillAsync("input[id=FirstName]", employee.FirstName);
        await page.FillAsync("input[id=LastName]", employee.LastName);
        await page.FillAsync("input[id=DateOfBirth]", employee.DateOfBirth.ToString("yyyy-MM-dd"));
        await page.FillAsync("input[id=EmailAddress]", employee.EmailAddress);
        await page.FillAsync("input[id=Department]", employee.Department);
        await page.FillAsync("input[id=JobTitle]", employee.JobTitle);

        await page.ClickAsync("button[type=submit]");
        return employee;
    }

    private async Task DeleteEmployee(IPage page, string key)
    {
        await page.GotoAsync("/employees");
        await page.GetByTestId($"{key}-delete").ClickAsync();        
    }
}
