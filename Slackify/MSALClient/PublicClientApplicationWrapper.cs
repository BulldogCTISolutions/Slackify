
namespace Slackify.MSALClient;

public class PublicClientApplicationWrapper : IPublicClientApplicationWrapper
{
    private readonly IConfiguration _configuration;
    private readonly Settings? _settings;

    internal IPublicClientApplication? PCA { get; }

    internal bool UseEmbedded { get; set; }
    public string[]? Scopes { get; set; }

    public PublicClientApplicationWrapper( IConfiguration configuration )
    {
        this._configuration = configuration;
        this._settings = this._configuration.GetRequiredSection( "Settings" ).Get<Settings>();

        if( this._settings?.Scopes is null )
        {
            return;
        }

        this.Scopes = this._settings.Scopes.ToStringArray();

        //  Create PCA once.  Make sure that all the config parameters below are passed
        this.PCA = PublicClientApplicationBuilder.Create( this._settings?.ClientId )
                                                 .WithB2CAuthority( this._settings?.Authority )
                                                 .WithRedirectUri( "http://localhost" )
                                                 .Build();
    }

    /// <summary>
    ///  Acquire the token silently
    /// </summary>
    /// <param name="scopes">Desired scopes</param>
    /// <returns>Authentication result</returns>
    public async Task<AuthenticationResult?> AcquireTokenSilentAsync( string[]? scopes )
    {
        if( this.PCA is null )
        {
            return null;
        }

        IEnumerable<IAccount>? accounts = await this.PCA.GetAccountsAsync( this._settings?.PolicySignUpSignIn ).ConfigureAwait( false );
        IAccount? account = accounts.FirstOrDefault();

        AuthenticationResult? authResult = await this.PCA.AcquireTokenSilent( scopes, account )
                                                         .ExecuteAsync()
                                                         .ConfigureAwait( false );

        return authResult;
    }

    /// <summary>
    ///  Perform the interactive acquisition of the token for the given scope
    /// </summary>
    /// <param name="scopes">Desired scopes</param>
    /// <returns>Authentication result</returns>
    public async Task<AuthenticationResult?> AcquireTokenInteractiveAsync( string[]? scopes )
    {
        if( this.PCA is null )
        {
            return null;
        }

        IEnumerable<IAccount>? accounts = await this.PCA.GetAccountsAsync( this._settings?.PolicySignUpSignIn )
                                                        .ConfigureAwait( false );
        IAccount? account = accounts.FirstOrDefault();

        AuthenticationResult? authResult = await this.PCA.AcquireTokenInteractive( scopes )
                                                         .WithB2CAuthority( this._settings?.Authority )
                                                         .WithAccount( account )
                                                         .WithParentActivityOrWindow( PlatformConfig.Instance.ParentWindow )
                                                         .WithUseEmbeddedWebView( false )
                                                         .ExecuteAsync()
                                                         .ConfigureAwait( false );

        return authResult;
    }

    /// <summary>
    ///  It will sign out the user.
    /// </summary>
    /// <remarks>
    ///  Sign out may not perform
    /// </remarks>
    /// <returns></returns>
    public async Task SignOutAsync()
    {
        if( this.PCA is null )
        {
            return;
        }

        IEnumerable<IAccount>? accounts = await this.PCA.GetAccountsAsync().ConfigureAwait( false );
        foreach( IAccount account in accounts )
        {
            await this.PCA.RemoveAsync( account ).ConfigureAwait( false );
        }
    }
}
