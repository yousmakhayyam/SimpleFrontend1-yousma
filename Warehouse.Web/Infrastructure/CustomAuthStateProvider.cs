using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Warehouse.Web.Infrastructure;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor)
    {
            _httpContextAccessor = httpContextAccessor;
        }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                var authState = new AuthenticationState(new ClaimsPrincipal(user));
                return Task.FromResult(authState);
            }

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

    public async Task MarkUserAsAuthenticated(string username)
    {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "apiauth"));
            var authState = Task.FromResult(new AuthenticationState(user));

            NotifyAuthenticationStateChanged(authState);
        }

    public async Task MarkUserAsLoggedOut()
    {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));

            NotifyAuthenticationStateChanged(authState);
        }
}