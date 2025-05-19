using FirebaseAdmin.Auth;

namespace Poke.Server.Infrastructure.Auth.Firebase;

public class FireAuthService : IAuthService
{
    public async Task<string?> VerifyIdTokenAsync(string idToken)
    {
        var payload = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
        
        return payload?.Uid;
    }
}
