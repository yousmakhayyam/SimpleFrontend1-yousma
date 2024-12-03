using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Warehouse.Domain.Data.Identity;

namespace Warehouse.Web.Infrastructure;

public class ApplicationUserClaimsPrincipalFactory : IClaimsTransformation
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }


    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null || !identity.IsAuthenticated)
        {
            return principal;
        }


        // Retrieve user from database
        var user = await _userManager.FindByNameAsync(identity.Name);
        if (user != null)
        {

            // Avoid adding the claims multiple times
            if (!identity.HasClaim(claim => claim.Type == "UserId"))
            {
                identity.AddClaim(new Claim("UserId", user.Id));
            }
                
            if (user.CompanyId != null && !identity.HasClaim(claim => claim.Type == "CompanyId"))
            {
                identity.AddClaim(new Claim("CompanyId", user.CompanyId.ToString()));
            }
        }

        return principal;
    }
}