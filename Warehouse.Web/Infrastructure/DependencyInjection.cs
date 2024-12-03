using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Warehouse.Domain.Data.Identity;
using Warehouse.Web.Helpers;
using Warehouse.Web.MappingProfiles;
using Warehouse.Web.Services;

namespace Warehouse.Web.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Missing connection string");
        }

        services.AddCascadingAuthenticationState();

        services.AddHttpContextAccessor();  // Required for IHttpContextAccessor
        services.AddScoped<CustomAuthStateProvider>();  // Register CustomAuthStateProvider
        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(); // Bind the state provider
        services.AddTransient<IClaimsTransformation, ApplicationUserClaimsPrincipalFactory>();

        services.AddScoped<CompanyService>();
        services.AddScoped<ProductService>();
        services.AddScoped<UserService>();
        services.AddScoped<ObjectStoreHelper>();

        services.AddScoped<MapProductsToModel>();
        services.AddScoped<UserMappings>();

        services.AddAuthorization();
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();


        services.AddScoped(sp =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            return new HttpClient()
            {
                BaseAddress = new Uri(navigationManager.BaseUri)
            };
        });

        return services;
    }
}
