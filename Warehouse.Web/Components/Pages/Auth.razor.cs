using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Warehouse.Web.Components.Pages;

public partial class Auth
{
    [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    private AuthenticationState? authState;

    protected override async Task OnInitializedAsync()
    {
        authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
    }
}