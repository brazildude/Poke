using System.Security.Claims;

namespace Poke.Server.Infrastructure.Auth;

public interface ICurrentUser
{
    public string UserID { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
}

public class CurrentUser : ICurrentUser
{
    public string UserID { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }

    public CurrentUser(IHttpContextAccessor accessor)
    {
        UserID = "";

        var principal = accessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated ?? false)
        {
            UserID = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            Email = principal.FindFirst(ClaimTypes.Email)?.Value;
            Name = principal.FindFirst(ClaimTypes.Name)?.Value;
            Role = principal.FindFirst("role")?.Value;
        }
    }
}
