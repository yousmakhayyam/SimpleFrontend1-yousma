using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels.User;

namespace Warehouse.Web.Components.Pages.Users;

[Authorize(Roles = "Administrator,CompanyManager")]
public partial class EditCreateUser
{
    [Parameter] public string? UserId { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected UserService UserService { get; set; }
    [Inject] protected CompanyService CompanyService { get; set; }

    protected List<string> ErrorMessages { get; set; } = new();
    protected UserEditModel Model { get; set; } = new();
    protected string Message { get; set; }
    protected bool IsAdmin { get; set; }
    protected bool IsEditMode => !string.IsNullOrEmpty(UserId);
    protected string? CompanyName { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Check if the current user is an Admin or a Company Manager
        IsAdmin = await UserService.IsCurrentUserAdmin();
        if (!IsAdmin)
        {
            CompanyName = await CompanyService.GetCurrentCompanyName();
        }

        // If editing, load the user details
        if (IsEditMode)
        {
            var user = await UserService.GetUserById(UserId);
            if (user == null)
            {
                Message = "User not found.";
                return;
            }

            // Populate the model
            Model = new UserEditModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
            };
        }
    }

    protected async Task SaveUser()
    {
        var results = await UserService.UpdateCreateUser(Model);

        if (results.Succeeded)
        {
            NavigationManager.NavigateTo(IsAdmin ? "/users?viewMode=admin" : "/users?viewMode=company");
        }
        else
        {
            ErrorMessages.AddRange(results.Errors);
        }
    }

    protected void Cancel()
    {
        NavigationManager.NavigateTo(IsAdmin ? "/users?viewMode=admin" : "/users?viewMode=company");
    }
}