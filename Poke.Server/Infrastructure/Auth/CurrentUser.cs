using System.Security.Claims;

namespace Poke.Server.Infrastructure.Auth;

public class CurrentUser : ICurrentUser
{
    public int UserID { get; set; }
    public string ExternalUserID { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }

    public CurrentUser(IHttpContextAccessor accessor)
    {
        ExternalUserID = "";

        var principal = accessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated ?? false)
        {
            ExternalUserID = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            Email = principal.FindFirst(ClaimTypes.Email)?.Value;
            Name = principal.FindFirst(ClaimTypes.Name)?.Value;
            Role = principal.FindFirst("role")?.Value;
        }
    }
}

public interface ICurrentUser
{
    public int UserID { get; set; }
    public string ExternalUserID { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
}
