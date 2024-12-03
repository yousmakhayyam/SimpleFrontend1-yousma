using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Warehouse.Domain.Data.Identity;
using Warehouse.Domain.Models.Common;
using Warehouse.Domain.Models.Constants;
using Warehouse.Web.Components.Pages.Users;
using Warehouse.Web.Localization;
using Warehouse.Web.ViewModels.User;

namespace Warehouse.Web.Services;

public class UserService : BaseService<UserService>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserService(ILogger<UserService> logger, IStringLocalizer<LabelResources> stringLocalizer, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor) : base(logger, stringLocalizer)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> AddExistingUserToCompany(string userId, Guid companyId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return false;
            }

            user.CompanyId = companyId;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while adding user {userId} to the company {companyId}", userId, companyId);
            return false;
        }
    }

    /// <summary>
    /// Checks if the currently logged-in user is an admin.
    /// </summary>
    public async Task<bool> IsCurrentUserAdmin()
    {
        var user = await GetCurrentUser();
        if (user == null) return false;

        return await _userManager.IsInRoleAsync(user, Roles.Administrator);
    }

    

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    public async Task<ApplicationUser?> GetUserById(string userId)
    {
        return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    /// <summary>
    /// Creates or updates a user based on the provided UserEditModel.
    /// </summary>
    public async Task<Result> UpdateCreateUser(UserEditModel model)
    {
        ApplicationUser? existingUser = null;
        var isNew = false;
        if (!string.IsNullOrEmpty(model.Id))
        {
            existingUser = await GetUserById(model.Id);
        }

        if (existingUser is null)
        {
            existingUser = new ApplicationUser()
            {
                CompanyId = model.CompanyId,
                CreatedOn = DateTimeOffset.UtcNow
            };
            isNew = true;
        }

        existingUser.FirstName = model.FirstName;
        existingUser.LastName = model.LastName;
        existingUser.Email = model.Email;
        existingUser.UserName = model.Email;
        existingUser.Role = model.Role;
        existingUser.LastUpdateOn = DateTimeOffset.UtcNow;

        if (isNew)
        {
            var result = await _userManager.CreateAsync(existingUser, model.Password);
            if (!result.Succeeded)
            {
                return Result.Failure(string.Join(",", result.Errors.Select(r => r.Description)));
            }
        }
        else
        {
            var result = await _userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                return Result.Failure(string.Join(",", result.Errors));
            }
        }

        var roleUpdate = await UpdateUserRole(existingUser.Id, model.Role);

        return Result.Success();
    }

    /// <summary>
    /// Retrieves the currently logged-in user id.
    /// </summary>
    public string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
    }

    /// <summary>
    /// Retrieves the currently logged-in user.
    /// </summary>
    public async Task<ApplicationUser?> GetCurrentUser()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        return await GetUserById(userId);
    }

    public async Task<(List<ApplicationUser> Users, int TotalCount)> GetUsers(string searchQuery, int page, int pageSize)
    {
        var query = _userManager.Users;

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(u => u.FirstName.Contains(searchQuery) ||
                                     u.LastName.Contains(searchQuery) ||
                                     u.Email.Contains(searchQuery));
        }

        var totalCount = await query.CountAsync();
        var users = await query
            .OrderBy(u => u.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task<(List<ApplicationUser> Users, int TotalCount)> GetCompanyUsers(string searchQuery, int page, int pageSize, Guid companyId)
    {
        var query = _userManager.Users.Where(r => r.CompanyId == companyId);

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(u => u.FirstName.Contains(searchQuery) ||
                                     u.LastName.Contains(searchQuery) ||
                                     u.Email.Contains(searchQuery));
        }

        var totalCount = await query.CountAsync();
        var users = await query
            .OrderBy(u => u.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task<bool> UpdateUserRole(string userId, string newRole)
    {
        // Fetch the user by their ID
        var user = await GetUserById(userId);
        if (user == null)
        {
            return false;
        }

        // Fetch all roles assigned to the user
        var currentRoles = await _userManager.GetRolesAsync(user);

        // Remove the user from all current roles
        var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeRolesResult.Succeeded)
        {
            return false;
        }

        // Add the user to the new role
        var addRoleResult = await _userManager.AddToRoleAsync(user, newRole.ToString()!);
        if (!addRoleResult.Succeeded)
        {
            return false;
        }

        return true;
    }
}