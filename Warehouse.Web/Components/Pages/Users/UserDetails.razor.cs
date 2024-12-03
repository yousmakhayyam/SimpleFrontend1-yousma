using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Warehouse.Domain.Data.Identity;
using Warehouse.Web.MappingProfiles;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels.User;

namespace Warehouse.Web.Components.Pages.Users;

[Authorize(Roles = "Administrator,CompanyManager")]
public partial class UserDetails
{
    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private CompanyService CompanyService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private UserMappings UserMappings { get; set; } = null!;

    [Parameter] public string Id { get; set; }
    private UserModel? Model { get; set; }
    private string? CompanyName { get; set; }
    private bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var user = await UserService.GetUserById(Id);
            if (user is not null)
            {
                Model = await UserMappings.ToUserModel(user);
                CompanyName = await CompanyService.GetCurrentCompanyName();
            }
        }
        catch (Exception ex)
        {
            // Handle error (e.g., log and redirect)
            Console.WriteLine($"Error loading user details: {ex.Message}");
            NavigationManager.NavigateTo("/error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void NavigateBack()
    {
        NavigationManager.NavigateTo("/");
    }

    private void NavigateToEditPage()
    {
        NavigationManager.NavigateTo($"/users/edit/{Id}");
    }
}