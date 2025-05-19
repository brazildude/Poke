namespace Poke.Server.Infrastructure.Auth;

public interface IAuthService
{
    Task<string?> VerifyIdTokenAsync(string idToken);
}
