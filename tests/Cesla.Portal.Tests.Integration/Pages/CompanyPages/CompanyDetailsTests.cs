using Bogus;
using Bogus.DataSets;
using Cesla.Portal.Infrastructure.Persistence;
using Cesla.Portal.Infrastructure.Persistence.Interceptors;
using Cesla.Portal.WebUI.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Mysqlx.Expr;

namespace Cesla.Portal.Tests.Integration.Pages.CompanyPages;
[Collection("Test collection")]
public class CompanyDetailsTests
{
    private readonly SharedTestContext _testContext;    
    private readonly CeslaDbContext _ceslaDbContext;
    public CompanyDetailsTests(SharedTestContext testContext)
    {
        _testContext = testContext;
        var options = new DbContextOptionsBuilder<CeslaDbContext>()
            .UseMySQL("server=localhost;port=33306;database=cesla;user=ceslatester;password=t35t3r!1234;")
            .AddInterceptors(new LogicDeleteInterceptor())
            .Options;
        _ceslaDbContext = new CeslaDbContext(options);
    }

    [Fact]
    public async Task CompanyDetails_ShouldBeAlwaysOnlyOne_WhenListingCompanies()
    {
        // Act 
        var companies = await _ceslaDbContext.Companies.AsNoTracking().ToListAsync();

        // Assert
        companies.Should().HaveCount(1);
    }

    [Fact]
    public async Task CompanyDetails_ShouldDisplayCompanyDetails_WhenCompanyExists()
    {
        // Arrange        
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });

        // Act & Assert
        await page.GotoAsync("/company");
        var name = (await page.GetByTestId($"company-name")
            .InnerTextAsync())
            .Should()
            .NotBeNullOrEmpty();

        var fantasy = (await page.GetByTestId($"company-fantasy")
            .InnerTextAsync())
            .Should()
            .NotBeNullOrEmpty();
            
        var identifier = (await page.GetByTestId($"company-identifier")
            .InnerTextAsync())
            .Should()
            .NotBeNullOrEmpty();

        var addressline = (await page.GetByTestId($"company-addressline")
            .InnerTextAsync())
            .Should()
            .NotBeNullOrEmpty();

        var email = (await page.GetByTestId($"company-email")
            .InnerTextAsync())
            .Should()
            .NotBeNullOrEmpty();

        var phone = (await page.GetByTestId($"company-phone")
            .InnerTextAsync())
            .Should()
            .NotBeNullOrEmpty();

        await page.CloseAsync();
    }
}
