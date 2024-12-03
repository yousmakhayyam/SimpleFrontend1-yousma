using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Warehouse.Domain.Data.Identity;
using Warehouse.Domain.Models.Constants;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels.User;

namespace Warehouse.Web.Components.Account;

public partial class RegisterCompany
{
    private IEnumerable<IdentityError>? _identityErrors;

    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = null!;
    [Inject] private IUserStore<ApplicationUser> UserStore { get; set; } = null!;
    [Inject] private SignInManager<ApplicationUser> SignInManager { get; set; } = null!;
    [Inject] private ILogger<Register> Logger { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private HttpClient HttpClient { get; set; } = null!;
    [Inject] private CompanyService CompanyService { get; set; } = null!;


    [SupplyParameterFromForm]
    private ViewModels.Company.RegisterCompanyModel Model { get; set; } = new();

    private string? Message => _identityErrors is null ? null : $"Error: {string.Join(", ", _identityErrors.Select(error => error.Description))}";

    public async Task RegisterCompanyAndUser(EditContext editContext)
    {
        // Verify if company exists already by Tax Number
        var taxNumberVerificationResult = await CompanyService.VerifyTaxIdentificationNumber(Model.TaxIdentificationNumber);

        if (!taxNumberVerificationResult.Succeeded)
        {
            _identityErrors = new[]
            {
                new IdentityError()
                {
                    Description = "Tax number already exists"
                }
            };
            return;
        }

        // Create new company user
        var user = CreateUser();

        await UserStore.SetUserNameAsync(user, Model.Email, CancellationToken.None);
        user.FirstName = Model.ManagerFirstName;
        user.LastName = Model.ManagerLastName;

        user.Email = Model.Email;
        user.NormalizedEmail = UserManager.NormalizeEmail(Model.Email);
        user.EmailConfirmed = true;
        user.Role = Roles.CompanyManager;
        user.CreatedOn = DateTimeOffset.UtcNow;

        var result = await UserManager.CreateAsync(user, Model.Password);

        if (!result.Succeeded)
        {
            _identityErrors = result.Errors;
            return;
        }

        await UserManager.AddToRolesAsync(user, new []{ Roles.CompanyManager});

        Model.UserId = user.Id;

        var response = await CompanyService.RegisterCompany(Model);
        
        Logger.LogInformation("User created a new account with password.");

        await SignInManager.SignInAsync(user, isPersistent: false);

        //During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
        NavigationManager.NavigateTo("/");
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                                                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
        }
    }
}