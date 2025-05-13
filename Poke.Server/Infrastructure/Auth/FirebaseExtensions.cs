using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace Poke.Server.Infrastructure.Auth;

public static class FirebaseExtensions
{
    public static void UseFirebase(this WebApplication app)
    {
        var firebaseSetting = app.Configuration["Firebase:Settings"];

        if (firebaseSetting == null)
        {
            throw new Exception("Firebase:Settings is not set.");
        }
        
        var blob = Convert.FromBase64String(firebaseSetting);
        var jsonString = Encoding.UTF8.GetString(blob);

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(jsonString)
        });
    }
}
