namespace BeneficiosPlataforma.Infrastructure.Auth;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BeneficiosPlataforma.Infrastructure.Persistence;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly AppDbContext _dbContext;

    public PermissionAuthorizationHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            context.Fail();
            return;
        }

        var permissionsClaim = context.User.FindFirst("permissions");
        if (permissionsClaim != null)
        {
            var permissions = permissionsClaim.Value.Split(',');
            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
                return;
            }
        }

        var userPermissions = await GetUserPermissionsAsync(userId);
        if (userPermissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }

    private async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId)
    {
        return await Task.FromResult(_dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(_dbContext.RolePermissions,
                ur => ur.RoleId,
                rp => rp.RoleId,
                (ur, rp) => rp.PermissionId)
            .Join(_dbContext.Permissions,
                permId => permId,
                perm => perm.Id,
                (permId, perm) => perm.Code)
            .Distinct()
            .ToList());
    }
}
