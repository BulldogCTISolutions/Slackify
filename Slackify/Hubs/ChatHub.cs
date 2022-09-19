namespace Slackify.Hubs;

public class ChatHub : Hub
{
    private readonly ConnectionManager _connectionManager;
    private readonly IUserService _userService;

    public ChatHub( ConnectionManager connectionManager, IUserService userService )
    {
        this._connectionManager = connectionManager;
        this._userService = userService;
    }

    public async Task InitializeUserList()
    {
        IEnumerable<string> list = this._connectionManager.GetUsers();

        await this.Clients.All.SendAsync( "ReceiveInitializeUsersList", list ).ConfigureAwait( false );
    }

    public void AddUserToRoom( string userEmail )
    {
        string currentUser = this.Context.User.Claims.FirstOrDefault( claim => claim.Type == ClaimTypes.Email ).Value;

        string connectionId = this.GetConnectionId();
        this._connectionManager.Add( currentUser, connectionId );
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            string userEmail = this.Context.User.Claims.FirstOrDefault( claim => claim.Type == ClaimTypes.Email ).Value;
            string userName = this.Context.User.Claims.FirstOrDefault( claim => claim.Type == ClaimTypes.Name ).Value;
            User userInDatabase = await this._userService.GetUserByEmail( userEmail ).ConfigureAwait( false );

            string key = $"{userEmail}-{userName}-{userInDatabase.Id}";

            this._connectionManager.Add( key, this.GetConnectionId() );
        }
        catch( NullReferenceException )
        {
            //  TODO: Handle exceptions
        }
        await base.OnConnectedAsync().ConfigureAwait( false );
    }

    public override async Task OnDisconnectedAsync( Exception exception )
    {
        try
        {
            string userEmail = this.Context.User.Claims.FirstOrDefault( claim => claim.Type == ClaimTypes.Email ).Value;
            string userName = this.Context.User.Claims.FirstOrDefault( claim => claim.Type == ClaimTypes.Name ).Value;
            User userInDatabase = await this._userService.GetUserByEmail( userEmail ).ConfigureAwait( false );

            string key = $"{userEmail}-{userName}-{userInDatabase.Id}";

            this._connectionManager.Remove( key, this.GetConnectionId() );

            await this.Clients.All.SendAsync( "UserDisconnected", userEmail ).ConfigureAwait( false );
        }
        catch( NullReferenceException )
        {
            //  TODO: Handle exceptions
        }
        await base.OnDisconnectedAsync( exception ).ConfigureAwait( false );
    }

    private string GetConnectionId()
    {
        return this.Context.ConnectionId;
    }
}
