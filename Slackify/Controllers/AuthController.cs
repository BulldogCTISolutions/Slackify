using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace Slackify.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;

        public AuthController( IUserService service )
        {
            this.userService = service;
        }

        [HttpGet("google-login")]
        public async Task LoginAsync()
        {
            await HttpContext.ChallengeAsync( GoogleDefaults.AuthenticationScheme,
                                              new AuthenticationProperties()
                {
                    RedirectUri = Url.Action( nameof( LoginCallBack ) )
                } );
        }

        [HttpGet]
        public async Task<IActionResult> LoginCallBack()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync( CookieAuthenticationDefaults.AuthenticationScheme );

            string email = result.Principal.FindFirst( ClaimTypes.Email ).Value;

            User userInDb = await this.userService.GetUserByEmail( email );

            if( userInDb is null )
            {
                await SaveUserDetails( result );
            }

            return Redirect( "https://localhost:44374" );
        }

        [HttpGet("signout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return Redirect("~/");
        }

        //  TODO: should not be in a controller, should be in a service.
        //  might need to pass httpContext to service.
        private async Task SaveUserDetails( AuthenticateResult result )
        {
            string email = result.Principal.FindFirst( ClaimTypes.Email ).Value;
            string userName = result.Principal.FindFirst( ClaimTypes.Name ).Value;
            string picture = User.Claims.Where( c => c.Type == "picture" ).FirstOrDefault().Value;

            User user = new User
            {
                UserName = userName,
                Email = email,
                Picture = picture,
                DateJoined = DateTime.Now
            };

            await this.userService.RegisterUser( user );
        }
    }
}
