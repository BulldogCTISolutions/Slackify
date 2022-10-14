using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace Slackify.Controllers;

[ApiController]
[Route( "[controller]" )]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IPublicClientApplicationWrapper _publicClientApplicationWrapper;

    public AuthController( IUserService service, PublicClientApplicationWrapper publicClientApplicationWrapper )
    {
        this._userService = service;
        this._publicClientApplicationWrapper = publicClientApplicationWrapper;
    }

    [HttpGet( "microsoft-login" )]
    public async Task LoginMicrosoftAsync()
    {
        try
        {
            //  Attempt silent login and obtain access token.
            AuthenticationResult? result = await this._publicClientApplicationWrapper.AcquireTokenSilentAsync( this._publicClientApplicationWrapper.Scopes )
                                                                                     .ConfigureAwait( false );

            Globals.AccessToken = result?.AccessToken;
        }
        //  A MsalUiRequiredException will be thrown, if this is the first attempt to login,
        //  or after logging out.
        catch( MsalUiRequiredException )
        {
            try
            {
                //  Perform interactive login and obtain access token.
                AuthenticationResult? result = await this._publicClientApplicationWrapper.AcquireTokenInteractiveAsync( this._publicClientApplicationWrapper.Scopes )
                                                                                         .ConfigureAwait( false );

                Globals.AccessToken = result?.AccessToken;
            }
            catch
            {
                //  Ignore
            }
        }
        catch
        {
            Globals.AccessToken = null;
        }
    }

    [HttpGet( "google-login" )]
    public async Task LoginAsync()
    {
        await this.HttpContext.ChallengeAsync( GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
        {
            RedirectUri = this.Url.Action( nameof( LoginCallBack ) )
        } ).ConfigureAwait( false );
    }

    [HttpGet]
    public async Task<IActionResult> LoginCallBack()
    {
        AuthenticateResult result = await this.HttpContext.AuthenticateAsync( CookieAuthenticationDefaults.AuthenticationScheme )
                                                          .ConfigureAwait( false );

        string? email = result.Principal?.FindFirst( ClaimTypes.Email )?.Value;

        if( string.IsNullOrEmpty( email ) == false )
        {
            await this.SaveUserDetails( result ).ConfigureAwait( false );
        }

        return this.Redirect( "https://localhost:44374" );  //  TODO:  where?
    }

    [HttpGet( "signout" )]
    public async Task<IActionResult> LogoutAsync()
    {
        await this.HttpContext.SignOutAsync().ConfigureAwait( false );
        await this._publicClientApplicationWrapper.SignOutAsync().ConfigureAwait( false );
        Globals.AccessToken = null;
        return this.Redirect( "~/" );
    }

    //  TODO: should not be in a controller, should be in a service.
    //  might need to pass httpContext to service.
    private async Task SaveUserDetails( AuthenticateResult result )
    {
        string? email = result.Principal?.FindFirst( ClaimTypes.Email )?.Value;
        string? userName = result.Principal?.FindFirst( ClaimTypes.Name )?.Value;
        string? picture = this.User?.Claims.FirstOrDefault( c =>
                                string.Equals( c.Type.ToLower( System.Globalization.CultureInfo.CurrentCulture ), "picture", StringComparison.OrdinalIgnoreCase ) )?.Value;

        if( ( string.IsNullOrEmpty( email ) == false ) &&
            ( string.IsNullOrEmpty( userName ) == false ) &&
            ( string.IsNullOrEmpty( picture ) == false ) )
        {
            User user = new User()
            {
                UserName = userName,
                Email = email,
                Picture = picture,
                DateJoined = DateTime.Now
            };

            await this._userService.RegisterUser( user ).ConfigureAwait( false );
        }
    }
}
