using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

namespace Slackify.Installers;

public class AuthSchemeInstaller : IInstaller
{
    public void InstallService( IServiceCollection services, IConfiguration configuration )
    {
        //  Add Google OAuth
        services.AddAuthentication( authenticationOptions =>
        {
            authenticationOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            authenticationOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            authenticationOptions.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        } )
            .AddCookie( cookieOptions =>
            {
                cookieOptions.Cookie.Name = "SlackifyAuthenticationCookie";
                cookieOptions.LoginPath = "/auth/google-login";
            } )
            .AddGoogle( googleOptions =>
            {
                googleOptions.ClientId = configuration.GetSection( "Authentication:Google:ClientId" ).Value;
                googleOptions.ClientSecret = configuration.GetSection( "Authentication:Google:ClientSecret" ).Value;

                googleOptions.Scope.Add( "Profile" );
                googleOptions.Events.OnCreatingTicket = context =>
                {
                    //  TODO: Handle null references.
                    string pictureUri = context.User.GetProperty( "picture" ).GetString();
                    context.Identity.AddClaim( new Claim( "picture", pictureUri ) );
                    return Task.CompletedTask;
                };
            } );
    }
}
