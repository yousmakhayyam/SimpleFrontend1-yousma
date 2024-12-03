using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Warehouse.Domain.Data.Identity;
using Warehouse.Domain.Models.Constants;
using Warehouse.Web.ViewModels.User;

namespace Warehouse.Web.Components.Account;

public partial class Register
{
    private IEnumerable<IdentityError>? _identityErrors;

    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = null!;
    [Inject] private IUserStore<ApplicationUser> UserStore { get; set; } = null!;
    [Inject] private SignInManager<ApplicationUser> SignInManager { get; set; } = null!;
    [Inject] private ILogger<Register> Logger { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [SupplyParameterFromForm]
    private UserRegisterModel Model { get; set; } = new();

    private string? Message => _identityErrors is null ? null : $"Error: {string.Join(", ", _identityErrors.Select(error => error.Description))}";

    public async Task RegisterUser(EditContext editContext)
    {
        var user = CreateUser();

        await UserStore.SetUserNameAsync(user, Model.Email, CancellationToken.None);
        user.FirstName = Model.FirstName;
        user.LastName = Model.LastName;

        user.Email = Model.Email;
        user.NormalizedEmail = UserManager.NormalizeEmail(Model.Email);
        user.EmailConfirmed = true;
        user.Role = Roles.User;
        user.CreatedOn = DateTimeOffset.UtcNow;

        var result = await UserManager.CreateAsync(user, Model.Password);

        if (!result.Succeeded)
        {
            _identityErrors = result.Errors;
            return;
        }

        Logger.LogInformation("User created a new account with password.");

        await UserManager.AddToRoleAsync(user, Roles.User);

        //TODO: Send confirmation email
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