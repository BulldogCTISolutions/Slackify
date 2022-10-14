using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

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
                    string? pictureUri = context.User.GetProperty( "picture" ).GetString();
                    if( string.IsNullOrEmpty( pictureUri ) == false )
                    {
                        context.Identity?.AddClaim( new Claim( "picture", pictureUri ) );
                    }
                    return Task.CompletedTask;
                };
            } );

        // Add Microsoft MSAL
        if( configuration is null )
        {
            throw new ArgumentNullException( nameof( configuration ), "configuration cannot be null" );
        }
        services.AddAuthentication( JwtBearerDefaults.AuthenticationScheme )
                .AddMicrosoftIdentityWebApi( configuration.GetSection( "AzureAd" ) )
                    .EnableTokenAcquisitionToCallDownstreamApi()
                        .AddMicrosoftGraph( configuration.GetSection( "MicrosoftGraph" ) )
                            .AddInMemoryTokenCaches()
                        .AddDownstreamWebApi( "DownstreamApi", configuration.GetSection( "DownstreamApi" ) )
                            .AddInMemoryTokenCaches();

        services.AddAuthorization();
    }
}
