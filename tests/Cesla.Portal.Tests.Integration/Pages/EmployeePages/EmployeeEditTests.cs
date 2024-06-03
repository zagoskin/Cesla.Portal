using Bogus;
using Cesla.Portal.Infrastructure.Persistence;
using Cesla.Portal.Infrastructure.Persistence.Interceptors;
using Cesla.Portal.Application.Dtos;
using Cesla.Portal.WebUI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using FluentAssertions;
using Cesla.Portal.Domain.EmployeeAggregate.Events;
using Cesla.Portal.Domain.EmployeeAggregate.ValueObjects;

namespace Cesla.Portal.Tests.Integration.Pages.EmployeePages;

[Collection("Test collection")]
public class EmployeeEditTests
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
    public EmployeeEditTests(SharedTestContext testContext)
    {
        _testContext = testContext;
        var options = new DbContextOptionsBuilder<CeslaDbContext>()
            .UseMySQL("server=localhost;port=33306;database=cesla;user=ceslatester;password=t35t3r!1234;")
            .AddInterceptors(new LogicDeleteInterceptor())
            .Options;
        _ceslaDbContext = new CeslaDbContext(options);
    }    
    [Fact]
    public async Task EditEmployee_ShouldEditEmployee_WhenValid()
    {
        // Arrange
        var employeeFormModel = _employeeGenerator.Generate();
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });
        await CreateEmployee(page, employeeFormModel);
        var employeeId = await _ceslaDbContext.Employees.AsNoTracking()
            .Where(x => x.PersonalInformation.EmailAddress.Value == employeeFormModel.EmailAddress)
            .Select(x => x.Id)
            .SingleAsync();
        var editedFormModel = new EmployeeFormModel
        {
            FirstName = $"{employeeFormModel.FirstName} edited",
            LastName = $"{employeeFormModel.LastName} edited",
            DateOfBirth = employeeFormModel.DateOfBirth.AddDays(1),
            EmailAddress = "anotheremail@test.com",
            Department = $"{employeeFormModel.Department} edited",
            JobTitle = $"{employeeFormModel.JobTitle} edited"
        };

        // Act
        await page.GotoAsync($"/employees/{employeeId.Value}/edit");
        await FillEmployeeForm(page, editedFormModel);
        await page.ClickAsync("button[type=submit]");
        var employeeFromDb = await _ceslaDbContext.Employees.AsNoTracking()
            .SingleOrDefaultAsync(x => x.PersonalInformation.EmailAddress.Value == editedFormModel.EmailAddress);

        // Assert
        employeeFromDb.Should().NotBeNull();
        var dto = employeeFromDb!.ToDto();
        dto.FirstName.Should().BeEquivalentTo(editedFormModel.FirstName);
        dto.LastName.Should().BeEquivalentTo(editedFormModel.LastName);
        dto.DateOfBirth.ToString("yyyy/MM/dd").Should().Be(editedFormModel.DateOfBirth.ToString("yyyy/MM/dd"));
        dto.EmailAddress.Should().BeEquivalentTo(editedFormModel.EmailAddress);
        dto.Department.Should().BeEquivalentTo(editedFormModel.Department);
        dto.JobTitle.Should().BeEquivalentTo(editedFormModel.JobTitle);

        await page.CloseAsync();
    }
    
    [Fact]
    public async Task EditEmployeePage_ShouldShowError_WhenTryingToEditUnexistingEmployee()
    {
        // Arrange
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });        
        var employeeId = EmployeeId.CreateUnique();

        // Act
        await page.GotoAsync($"/employees/{employeeId.Value}/edit");
        var employeeFromDb = await _ceslaDbContext.Employees.AsNoTracking()            
            .FirstOrDefaultAsync(x => x.Id == employeeId);

        // Assert
        employeeFromDb.Should().BeNull();
        var error = await page.GetByTestId("error-alert").InnerTextAsync();
        error.Should().Contain($"Employee with id {employeeId.Value} not found");
    }

    [Fact]
    public async Task EmployeeEdit_ShouldLogUpdatedEvent_WhenUpdated()
    {
        // Arrange
        var employeeFormModel = _employeeGenerator.Generate();
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });
        await CreateEmployee(page, employeeFormModel);
        var employeeId = await _ceslaDbContext.Employees.AsNoTracking()
            .Where(x => x.PersonalInformation.EmailAddress.Value == employeeFormModel.EmailAddress)
            .Select(x => x.Id)
            .SingleAsync();
        var editedFormModel = new EmployeeFormModel
        {
            FirstName = $"{employeeFormModel.FirstName} edited",
            LastName = $"{employeeFormModel.LastName} edited",
            DateOfBirth = employeeFormModel.DateOfBirth.AddDays(1),
            EmailAddress = employeeFormModel.EmailAddress,
            Department = $"{employeeFormModel.Department} edited",
            JobTitle = $"{employeeFormModel.JobTitle} edited"
        };
        
        // Act
        var now = await EditEmployee(page, editedFormModel, employeeId.Value);
        var updatedEvent = await _ceslaDbContext.OutboxDomainEvents
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedOnUtc)
            .FirstOrDefaultAsync(x => x.Type == nameof(EmployeeUpdatedDomainEvent));

        // Assert
        updatedEvent.Should().NotBeNull();
        updatedEvent!.CreatedOnUtc.Should().BeCloseTo(now, precision: TimeSpan.FromMilliseconds(3500));

        await page.CloseAsync();
    }

    private async Task CreateEmployee(IPage page, EmployeeFormModel employeeFormModel)
    {
        await page.GotoAsync("/employees/create");
        await FillEmployeeForm(page, employeeFormModel);
        await page.ClickAsync("button[type=submit]");
    }

    private async Task<DateTime> EditEmployee(IPage page, EmployeeFormModel employeeFormModel, Guid employeeId)
    {
        await page.GotoAsync($"/employees/{employeeId}/edit");
        await FillEmployeeForm(page, employeeFormModel);
        await page.ClickAsync("button[type=submit]");        
        return DateTime.UtcNow;
    }

    private async Task FillEmployeeForm(IPage page, EmployeeFormModel employeeFormModel)
    {
        await page.FillAsync("input[id=FirstName]", employeeFormModel.FirstName);
        await page.FillAsync("input[id=LastName]", employeeFormModel.LastName);
        await page.FillAsync("input[id=DateOfBirth]", employeeFormModel.DateOfBirth.ToString("yyyy-MM-dd"));
        await page.FillAsync("input[id=EmailAddress]", employeeFormModel.EmailAddress);
        await page.FillAsync("input[id=Department]", employeeFormModel.Department);
        await page.FillAsync("input[id=JobTitle]", employeeFormModel.JobTitle);
    }
}
