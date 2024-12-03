using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Warehouse.Domain.Data.Identity;
using Warehouse.Web.ViewModels.User;

namespace Warehouse.Web.Components.Account;

public partial class Login
{
    private string? errorMessage;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private UserLoginModel Model { get; set; } = new();

    [Inject] private SignInManager<ApplicationUser> SignInManager { get; set; } = null!;
    [Inject] private ILogger<Login> Logger { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;


    protected override async Task OnInitializedAsync()
    {
        if (HttpMethods.IsGet(HttpContext.Request.Method))
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }
    }

    public async Task LoginUser()
    {
        var result = await SignInManager.PasswordSignInAsync(Model.Email, Model.Password, Model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            Logger.LogInformation("User logged in.");

            //During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
            NavigationManager.NavigateTo("/");
        }
        else
        {
            errorMessage = "Error: Invalid login attempt.";
        }
    }
}