using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Domain.Data;
using Warehouse.Domain.Data.Identity;
using Warehouse.Domain.Data.Repositories;

namespace Warehouse.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Missing connection string");
        }

        services.AddDbContext<DomainDbContext>((sb, options) =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<DomainDbContext>();
        services.AddScoped<DomainDbContextInit>();

        services.AddScoped<CompanyRepository>();
        services.AddScoped<ProductRepository>();

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DomainDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }
}
