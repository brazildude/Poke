using FirebaseAdmin.Auth;

namespace Poke.Server.Infrastructure.Auth;

public interface IAuthService
{
    Task<string?> VerifyIdTokenAsync(string idToken);
}

public class AuthService : IAuthService
{
    public async Task<string?> VerifyIdTokenAsync(string idToken)
    {
        var payload = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
        
        return payload?.Uid;
    }
}
