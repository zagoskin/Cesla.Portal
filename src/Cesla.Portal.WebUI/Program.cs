using Cesla.Portal.Application;
using Cesla.Portal.Infrastructure;
using Cesla.Portal.WebUI.Components;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration.AddEnvironmentVariables("CeslaPortal_");
    builder.Services
        .AddApplication(builder.Configuration)
        .AddInfrastructure(builder.Configuration);

    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();
}



var app = builder.Build();
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    using (var scope = app.Services.CreateScope())
    {
        app.UseDatabaseSeed(scope.ServiceProvider);
    }

    app.UseHttpsRedirection();
    app.UseStatusCodePagesWithRedirects("/404");
    app.UseStaticFiles();
    app.UseAntiforgery();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();
}
app.Run();
