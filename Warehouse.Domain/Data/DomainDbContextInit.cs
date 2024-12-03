using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Warehouse.Domain.Data.Identity;
using Warehouse.Domain.Models.Constants;

namespace Warehouse.Domain.Data;

public static class InitExtensions
{
    public static async Task InitDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var init = scope.ServiceProvider.GetRequiredService<DomainDbContextInit>();

        await init.InitAsync();
        await init.SeedAsync();
    }
}

public class DomainDbContextInit
{
    private readonly ILogger<DomainDbContextInit> _logger;
    private readonly DomainDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DomainDbContextInit(ILogger<DomainDbContextInit> logger, DomainDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to init the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while seeding the database");
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles

        var adminRole = new IdentityRole(Roles.Administrator);
        if (_roleManager.Roles.All(r => r.Name != adminRole.Name))
        {
            await _roleManager.CreateAsync(adminRole);
        }

        var userRole = new IdentityRole(Roles.User);
        if (_roleManager.Roles.All(r => r.Name != userRole.Name))
        {
            await _roleManager.CreateAsync(userRole);
        }

        var companyUserRole = new IdentityRole(Roles.CompanyUser);
        if (_roleManager.Roles.All(r => r.Name != companyUserRole.Name))
        {
            await _roleManager.CreateAsync(companyUserRole);
        }

        var companyManagerRole = new IdentityRole(Roles.CompanyManager);
        if (_roleManager.Roles.All(r => r.Name != companyManagerRole.Name))
        {
            await _roleManager.CreateAsync(companyManagerRole);
        }

        // Default user

        var admin = new ApplicationUser() { UserName = "admin@local", Email = "admin@local", FirstName = "John", LastName = "Doe", Role = Roles.Administrator, CreatedOn = DateTimeOffset.UtcNow};
        if (_userManager.Users.All(u => u.UserName != admin.UserName))
        {
            await _userManager.CreateAsync(admin, "zaq1@WSX");
            if (!string.IsNullOrEmpty(adminRole.Name))
            {
                await _userManager.AddToRolesAsync(admin, new[] { adminRole.Name });
            }
        }

        // Default data if needed
    }
}