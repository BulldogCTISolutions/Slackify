using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace Slackify.Controllers;

[Route( "[controller]" )]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController( IUserService service )
    {
        this._userService = service;
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
        return this.Redirect( "~/" );
    }

    //  TODO: should not be in a controller, should be in a service.
    //  might need to pass httpContext to service.
    private async Task SaveUserDetails( AuthenticateResult result )
    {
        string? email = result.Principal?.FindFirst( ClaimTypes.Email )?.Value;
        string? userName = result.Principal?.FindFirst( ClaimTypes.Name )?.Value;
        string? picture = this.User?.Claims.FirstOrDefault( c => c.Type == "picture" )?.Value;

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
