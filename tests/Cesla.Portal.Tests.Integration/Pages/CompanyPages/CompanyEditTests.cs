using Bogus;
using Cesla.Portal.Application.Dtos;
using Cesla.Portal.Domain.CompanyAggregate.Events;
using Cesla.Portal.Infrastructure.Persistence;
using Cesla.Portal.Infrastructure.Persistence.Interceptors;
using Cesla.Portal.WebUI.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;

namespace Cesla.Portal.Tests.Integration.Pages.CompanyPages;

[Collection("Test collection")]
public class CompanyEditTests
{
    private readonly SharedTestContext _testContext;
    private readonly Faker<CompanyFormModel> _companyGenerator = new Faker<CompanyFormModel>()
        .RuleFor(x => x.CompanyName, faker => faker.Company.CompanyName())
        .RuleFor(x => x.FantasyName, faker => faker.Company.CompanyName())
        .RuleFor(x => x.Cnpj, "55.326.453/0001-02")
        .RuleFor(x => x.EmailAddress, faker => faker.Person.Email)
        .RuleFor(x => x.PhoneNumber, "+5521977053535")
        .RuleFor(x => x.AddressLine, faker => faker.Address.StreetAddress())
        .RuleFor(x => x.City, faker => faker.Address.City())
        .RuleFor(x => x.State, faker => faker.Address.State())
        .RuleFor(x => x.Country, faker => faker.Address.Country());
    private readonly CeslaDbContext _ceslaDbContext;
    public CompanyEditTests(SharedTestContext testContext)
    {
        _testContext = testContext;
        var options = new DbContextOptionsBuilder<CeslaDbContext>()
            .UseMySQL("server=localhost;port=33306;database=cesla;user=ceslatester;password=t35t3r!1234;")
            .AddInterceptors(new LogicDeleteInterceptor())
            .Options;
        _ceslaDbContext = new CeslaDbContext(options);
    }

    [Fact]
    public async Task CompanyEdit_ShouldEditCompany_WhenValid()
    {
        // Arrange
        var companyFormModel = _companyGenerator.Generate();
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });

        // Act
        await EditCompany(page, companyFormModel);
        var company = await _ceslaDbContext.Companies
            .AsNoTracking()
            .SingleOrDefaultAsync();

        // Assert
        company.Should().NotBeNull();
        var dto = company!.ToDto();
        dto.CompanyName.Should().BeEquivalentTo(companyFormModel.CompanyName);
        dto.FantasyName.Should().BeEquivalentTo(companyFormModel.FantasyName);
        dto.Cnpj.Should().BeEquivalentTo(companyFormModel.Cnpj);
        dto.EmailAddress.Should().BeEquivalentTo(companyFormModel.EmailAddress);
        dto.PhoneNumber.Should().BeEquivalentTo(companyFormModel.PhoneNumber);
        dto.AddressLine.Should().BeEquivalentTo(companyFormModel.AddressLine);
        dto.City.Should().BeEquivalentTo(companyFormModel.City);
        dto.State.Should().BeEquivalentTo(companyFormModel.State);
        dto.Country.Should().BeEquivalentTo(companyFormModel.Country);

        await page.CloseAsync();
    }

    [Fact]
    public async Task CompanyEdit_ShouldNotEditCompany_WhenInValid()
    {
        // Arrange
        var companyFormModel = new CompanyFormModel
        {
            CompanyName = $"",
            FantasyName = $"",
            Cnpj = "nonvalidvalue",
            EmailAddress = "notanemail",
            PhoneNumber = "not valid phone",
            AddressLine = $"",
            City = $"",
            State = $"",
            Country = $""
        };

        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });

        // Act
        await EditCompany(page, companyFormModel);
        var company = await _ceslaDbContext.Companies
            .AsNoTracking()
            .SingleOrDefaultAsync();
        var validationErrors = await page.Locator("li.validation-message").AllInnerTextsAsync();

        // Assert
        validationErrors.Should().NotBeEmpty();

        company.Should().NotBeNull();
        var dto = company!.ToDto();
        dto.CompanyName.Should().NotBeEquivalentTo(companyFormModel.CompanyName);
        dto.FantasyName.Should().NotBeEquivalentTo(companyFormModel.FantasyName);
        dto.Cnpj.Should().NotBeEquivalentTo(companyFormModel.Cnpj);
        dto.EmailAddress.Should().NotBeEquivalentTo(companyFormModel.EmailAddress);
        dto.PhoneNumber.Should().NotBeEquivalentTo(companyFormModel.PhoneNumber);
        dto.AddressLine.Should().NotBeEquivalentTo(companyFormModel.AddressLine);
        dto.City.Should().NotBeEquivalentTo(companyFormModel.City);
        dto.State.Should().NotBeEquivalentTo(companyFormModel.State);
        dto.Country.Should().NotBeEquivalentTo(companyFormModel.Country);

        await page.CloseAsync();
    }

    [Fact]
    public async Task CompanyEdit_ShouldShowSpecificError_WhenCNPJIsInvalid()
    {
        // Arrange
        var companyFormModel = _companyGenerator.Generate();
        companyFormModel.Cnpj = "55.326.453/0001-01"; 
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });

        // Act
        await EditCompany(page, companyFormModel);
        var company = await _ceslaDbContext.Companies
            .AsNoTracking()
            .SingleOrDefaultAsync();
        var error = await page.GetByTestId("error-alert").InnerTextAsync();

        // Assert        
        company.Should().NotBeNull();
        var dto = company!.ToDto();
        dto.Cnpj.Should().NotBeEquivalentTo(companyFormModel.Cnpj);
        error.Should().NotBeNull();
        error.Should().Contain("Cnpj validation failed");

        await page.CloseAsync();
    }

    [Fact]
    public async Task CompanyEdit_ShouldLogUpdatedEvent_WhenEdited()
    {
        // Arrange
        var company = await _ceslaDbContext.Companies.AsNoTracking().SingleOrDefaultAsync();
        var companyFormModel = new CompanyFormModel
        {
            CompanyName = $"{company!.CompanyInformation.CompanyName} edit",
            FantasyName = $"{company.CompanyInformation.FantasyName} edit",
            Cnpj = company.CompanyInformation.Cnpj.Value,
            EmailAddress = "companyedit@test.com",
            PhoneNumber = company.ContactInformation.PhoneNumber.Value,
            AddressLine = $"{company.ContactInformation.Address.Line} edit",
            City = $"{company.ContactInformation.Address.City} edit",
            State = $"{company.ContactInformation.Address.State} edit",
            Country = $"{company.ContactInformation.Address.Country} edit"
        };
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });


        // Act
        var now = await EditCompany(page, companyFormModel);
        var updatedEvent = await _ceslaDbContext.OutboxDomainEvents
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedOnUtc)
            .FirstOrDefaultAsync(x => x.Type == nameof(CompanyUpdatedDomainEvent));

        // Assert
        updatedEvent!.Should().NotBeNull();
        updatedEvent!.CreatedOnUtc.Should().BeCloseTo(now, precision: TimeSpan.FromSeconds(1));

        await page.CloseAsync();
    }

    private async Task<DateTime> EditCompany(IPage page, CompanyFormModel companyFormModel)
    {
        await page.GotoAsync($"/company/edit");
        await FillCompanyForm(page, companyFormModel);
        await page.ClickAsync("button[type=submit]");        
        return DateTime.UtcNow;
    }

    private async Task FillCompanyForm(IPage page, CompanyFormModel companyFormModel)
    {
        await page.FillAsync("input[id=CompanyName]", companyFormModel.CompanyName);
        await page.FillAsync("input[id=FantasyName]", companyFormModel.FantasyName);
        await page.FillAsync("input[id=Cnpj]", companyFormModel.Cnpj);
        await page.FillAsync("input[id=EmailAddress]", companyFormModel.EmailAddress);
        await page.FillAsync("input[id=PhoneNumber]", companyFormModel.PhoneNumber);
        await page.FillAsync("input[id=AddressLine]", companyFormModel.AddressLine);
        await page.FillAsync("input[id=City]", companyFormModel.City);
        await page.FillAsync("input[id=State]", companyFormModel.State);
        await page.FillAsync("input[id=Country]", companyFormModel.Country);
    }
}
