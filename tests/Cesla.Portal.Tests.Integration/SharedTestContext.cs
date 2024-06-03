using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.Playwright;

namespace Cesla.Portal.Tests.Integration;
public class SharedTestContext : IAsyncLifetime
{    
    public const string AppUrl = "https://localhost:12345";

    private static readonly string _dockerComposeFile =
        Path.Combine(Directory.GetCurrentDirectory(), (TemplateString)"../../../docker-compose.integration.yml");

    private IPlaywright _playwright = null!;

    public IBrowser Browser { get; private set; } = null!;

    private readonly ICompositeService _dockerService = new Builder()
        .UseContainer()
        .UseCompose()
        .FromFile(_dockerComposeFile)
        .RemoveOrphans()
        .WaitForHttp("test-app", AppUrl, timeout: 10000)
        .RemoveNonTaggedImages()
        .Build();

    public async Task InitializeAsync()
    {        
        _dockerService.Start();

        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 300
        });
    }

    public async Task DisposeAsync()
    {
        await Browser.DisposeAsync();
        _playwright.Dispose();
        _dockerService.Dispose();
    }
}
