namespace BeneficiosPlataforma.Application.Common;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Guid TenantId { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
    public string[] Permissions { get; set; } = Array.Empty<string>();
}

public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Status { get; set; } = null!;
}
