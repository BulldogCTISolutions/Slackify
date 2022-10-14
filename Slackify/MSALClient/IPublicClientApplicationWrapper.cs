
namespace Slackify.MSALClient;

public interface IPublicClientApplicationWrapper
{
    string[]? Scopes { get; set; }
    Task<AuthenticationResult?> AcquireTokenInteractiveAsync( string[]? scopes );
    Task<AuthenticationResult?> AcquireTokenSilentAsync( string[]? scopes );
    Task SignOutAsync();
}
