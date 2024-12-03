using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Warehouse.Domain.Data.Identity;
using Warehouse.Web.MappingProfiles;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels.User;

namespace Warehouse.Web.Components.Pages.Users;

[Authorize(Roles = "Administrator,CompanyManager")]
public partial class Users
{
    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private UserMappings UserMappings { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private string ViewMode { get; set; } = "company";

    private List<UserModel> UserList { get; set; } = new();

    private string SearchQuery { get; set; }
    private int CurrentPage { get; set; } = 1;
    private int PageSize { get; set; } = 9;
    private int TotalListResults { get; set; }
    private int TotalPages => (int)Math.Ceiling((double)TotalListResults / PageSize);

    protected override async Task OnInitializedAsync()
    {
        ViewMode = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query.Contains("viewMode=admin") ? "admin" : "company";

        await LoadUsers();
    }

    private async Task LoadUsers()
    {
        var currentUser = await UserService.GetCurrentUser();
        if (currentUser is null)
        {
            NavigationManager.NavigateTo("/error");
            return;
        }

        UserList.Clear();

        switch (ViewMode)
        {
            case "admin":
                await RetrieveAllUsers();
                break;
            case "company":
                await RetrieveCompanyUsers(currentUser);
                break;
            default:
                NavigationManager.NavigateTo("/error");
                break;
        }
    }

    private async Task RetrieveCompanyUsers(ApplicationUser currentUser)
    {
        var (userList, totalCount) = await UserService.GetCompanyUsers(SearchQuery, CurrentPage, PageSize, currentUser.CompanyId.GetValueOrDefault());
        foreach (var user in userList) { 
            UserList.Add(await UserMappings.ToUserModel(user));
        }
        TotalListResults = totalCount;
    }

    private async Task RetrieveAllUsers()
    {
        if (!await UserService.IsCurrentUserAdmin())
        {
            NavigationManager.NavigateTo("/error");
            return;
        }

        var (userList, totalCount) = await UserService.GetUsers(SearchQuery, CurrentPage, PageSize);
        foreach (var user in userList)
        {
            UserList.Add(await UserMappings.ToUserModel(user));
        }
        TotalListResults = totalCount;
    }

    private async Task ChangePage(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > TotalPages)
            return;

        CurrentPage = pageNumber;
        await LoadUsers();
    }

    private async Task SearchUsers()
    {
        CurrentPage = 1;
        await LoadUsers();
    }
}