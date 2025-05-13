using System.Security.Claims;

namespace Poke.Server.Infrastructure.Auth;

public interface ICurrentUser
{
    public string UserID { get; }
    public string? Email { get; }
    public string? Name { get; }
    public string? Role { get; }
}


public class CurrentUser : ICurrentUser
{
    public string UserID { get; private set; }
    public string? Email { get; private set; }
    public string? Name { get; private set; }
    public string? Role { get; private set; }

    internal CurrentUser(string userID, string? email, string? name, string? role)
    {
        UserID = userID;
        Email = email; 
        Name = name;
        Role = role;
    }

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
