using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Warehouse.Domain;
using Warehouse.Domain.Data;
using Warehouse.Web.Components;
using Warehouse.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddLocalization();

builder.Services.AddWebServices(builder.Configuration);
builder.Services.AddDomainServices(builder.Configuration);

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

var app = builder.Build();

// Use resources to show labels
app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(new[] { "en-US", "pl-PL" })
    .AddSupportedUICultures(new []{ "en-US", "pl-PL" })
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitDatabaseAsync();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();

app.Run();
