using Bogus;
using Cesla.Portal.Domain.EmployeeAggregate.Events;
using Cesla.Portal.Infrastructure.Persistence;
using Cesla.Portal.Infrastructure.Persistence.Interceptors;
using Cesla.Portal.WebUI.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Playwright;

namespace Cesla.Portal.Tests.Integration.Pages.EmployeePages;

[Collection("Test collection")]
public class EmployeeNewTests
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
    public EmployeeNewTests(SharedTestContext testContext)
    {
        _testContext = testContext;
        var options = new DbContextOptionsBuilder<CeslaDbContext>()
            .UseMySQL("server=localhost;port=33306;database=cesla;user=ceslatester;password=t35t3r!1234;")
            .AddInterceptors(new LogicDeleteInterceptor())
            .Options;
        _ceslaDbContext = new CeslaDbContext(options);
    }

    [Fact]
    public async Task EmployeeCreate_ShouldCreateEmployee_WhenValid()
    {
        // Arrange
        var employeeFormModel = _employeeGenerator.Generate();
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });

        // Act
        await CreateEmployee(page, employeeFormModel);
        var employeeFromDb = await _ceslaDbContext.Employees.AsNoTracking()
            .SingleOrDefaultAsync(x => x.PersonalInformation.EmailAddress.Value == employeeFormModel.EmailAddress);

        // Assert
        employeeFromDb.Should().NotBeNull();
        employeeFromDb!.PersonalInformation.FirstName.Should().Be(employeeFormModel.FirstName);
        employeeFromDb.PersonalInformation.LastName.Should().Be(employeeFormModel.LastName);
        DateOnly.FromDateTime(employeeFromDb.PersonalInformation.DateOfBirth)
            .Should().Be(employeeFormModel.DateOfBirth);
        employeeFromDb.JobInformation.JobTitle.Should().Be(employeeFormModel.JobTitle);
        employeeFromDb.JobInformation.Department.Should().Be(employeeFormModel.Department);

        await page.CloseAsync();
    }

    [Fact]
    public async Task EmployeeCreate_ShouldNotCreateEmployee_WhenExistsWithSameEmail()
    {
        // Arrange
        var employeeFormModel = _employeeGenerator.Generate();
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });
        await CreateEmployee(page, employeeFormModel);
        var employeeFormModel2 = _employeeGenerator.Generate();
        employeeFormModel2.EmailAddress = employeeFormModel.EmailAddress;

        // Act
        await CreateEmployee(page, employeeFormModel2);
        var employeesWithSameEmailCount = await _ceslaDbContext.Employees.AsNoTracking()
            .Where(x => x.PersonalInformation.EmailAddress.Value == employeeFormModel.EmailAddress)
            .CountAsync();

        // Assert
        employeesWithSameEmailCount.Should().Be(1);
        var error = await page.GetByTestId("error-alert").InnerTextAsync();
        error.Should().Contain($"Employee '{employeeFormModel.EmailAddress.ToLower()}' already exists");

        await page.CloseAsync();
    }

    [Fact]
    public async Task EmployeeCreate_ShouldNotCreateEmployee_WhenDataIsInvalid()
    {
        // Arrange
        var employeeFormModel = new EmployeeFormModel
        {
            FirstName = "Some random data",
            LastName = "Some random data",
            EmailAddress = "notanemail",
            DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(5),
            Department = "Some random data",
            JobTitle = "Some random data"
        };

        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });

        // Act
        await CreateEmployee(page, employeeFormModel);
        var employeeFromDb = await _ceslaDbContext.Employees.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PersonalInformation.EmailAddress.Value == employeeFormModel.EmailAddress);
        var errors = await page.Locator("li.validation-message").AllInnerTextsAsync();

        // Assert
        employeeFromDb.Should().BeNull();
        errors[0].Should().Be($"'DateOfBirth' must be before {DateTime.UtcNow:yyyy/MM/dd}");        
        errors[1].Should().Be("The email address is not valid");

        await page.CloseAsync();
    }

    [Fact]
    public async Task EmployeeCreate_ShouldNotCreateEmployee_WhenEmployeeIsNotAnAdult()
    {
        // Arrange
        var employeeFormModel = _employeeGenerator.Generate();
        employeeFormModel.DateOfBirth = new DateOnly(2020, 1, 1);
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });

        // Act
        await CreateEmployee(page, employeeFormModel);        
        var employeeFromDb = await _ceslaDbContext.Employees.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PersonalInformation.EmailAddress.Value == employeeFormModel.EmailAddress);

        // Assert
        employeeFromDb.Should().BeNull();
        var error = await page.GetByTestId("error-alert").InnerTextAsync();
        error.Should().Contain("Only people at least 18 years old can work in our company");

        await page.CloseAsync();
    }

    [Fact]
    public async Task EmployeeCreate_ShouldLogCreatedEvent_WhenCreated()
    {
        // Arrange
        var employeeFormModel = _employeeGenerator.Generate();
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });

        // Act
        var now = await CreateEmployee(page, employeeFormModel);
        var createdEvent = await _ceslaDbContext.OutboxDomainEvents.AsNoTracking()
            .OrderByDescending(x => x.CreatedOnUtc)
            .FirstOrDefaultAsync(x => x.Type == nameof(EmployeeCreatedDomainEvent));

        // Assert
        createdEvent.Should().NotBeNull();
        createdEvent!.CreatedOnUtc.Should().BeCloseTo(now, precision: TimeSpan.FromSeconds(1));

        await page.CloseAsync();
    }

    private async Task<DateTime> CreateEmployee(IPage page, EmployeeFormModel employeeFormModel)
    {
        await page.GotoAsync("/employees/create");

        await page.FillAsync("input[id=FirstName]", employeeFormModel.FirstName);
        await page.FillAsync("input[id=LastName]", employeeFormModel.LastName);
        await page.FillAsync("input[id=DateOfBirth]", employeeFormModel.DateOfBirth.ToString("yyyy-MM-dd"));
        await page.FillAsync("input[id=EmailAddress]", employeeFormModel.EmailAddress);
        await page.FillAsync("input[id=Department]", employeeFormModel.Department);
        await page.FillAsync("input[id=JobTitle]", employeeFormModel.JobTitle);
        var now = DateTime.UtcNow;
        await page.ClickAsync("button[type=submit]");
        return now;

    }
}
