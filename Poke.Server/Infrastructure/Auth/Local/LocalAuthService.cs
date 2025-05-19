namespace Poke.Server.Infrastructure.Auth.Local;

public class LocalAuthService : IAuthService
{
    public async Task<string?> VerifyIdTokenAsync(string idToken)
    {
        return await Task.FromResult(idToken);
    }
}
