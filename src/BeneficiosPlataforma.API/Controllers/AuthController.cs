namespace BeneficiosPlataforma.API.Controllers;

using Application.Auth.Commands;
using Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BeneficiosPlataforma.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;
    private readonly AppDbContext _dbContext;

    public AuthController(IMediator mediator, ILogger<AuthController> logger, AppDbContext dbContext)
    {
        _mediator = mediator;
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new LoginCommand(request.Email, request.Password, request.TenantSlug);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error");
            return BadRequest(new { error = "Invalid credentials" });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new RefreshTokenCommand(request.RefreshToken);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh error");
            return Unauthorized();
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var command = new LogoutCommand(parsedUserId);
        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new RegisterCommand(request.Name, request.Email, request.Password, request.TenantSlug);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Register error");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Register error");
            return BadRequest(new { error = "Registration failed" });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tenantId = User.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(userId, out var parsedUserId) || !Guid.TryParse(tenantId, out var parsedTenantId))
            return Unauthorized();

        var user = await _dbContext.Users
            .Where(u => u.Id == parsedUserId && u.TenantId == parsedTenantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return NotFound();

        var roles = await GetUserRolesAsync(parsedUserId, cancellationToken);
        var permissions = await GetUserPermissionsAsync(parsedUserId, cancellationToken);

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            TenantId = user.TenantId,
            Roles = roles,
            Permissions = permissions
        };

        return Ok(userDto);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new ForgotPasswordCommand(request.Email, request.TenantSlug);
            await _mediator.Send(command, cancellationToken);

            return Ok(new { message = "Password reset email sent" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Forgot password error");
            return BadRequest(new { error = "Failed to process password reset request" });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new ResetPasswordCommand(request.Token, request.NewPassword);
            await _mediator.Send(command, cancellationToken);

            return Ok(new { message = "Password reset successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Reset password error");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reset password error");
            return BadRequest(new { error = "Password reset failed" });
        }
    }

    private async Task<string[]> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(_dbContext.Roles,
                ur => ur.RoleId,
                role => role.Id,
                (ur, role) => role.Name)
            .ToArrayAsync(cancellationToken);
    }

    private async Task<string[]> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserRoles
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
            .ToArrayAsync(cancellationToken);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string TenantSlug { get; set; } = "default";
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class RegisterRequest
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string TenantSlug { get; set; } = "default";
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = null!;
    public string TenantSlug { get; set; } = "default";
}

public class ResetPasswordRequest
{
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
