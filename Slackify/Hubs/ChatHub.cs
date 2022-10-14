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

    public void AddUserToRoom( /*string userEmail*/ )
    {
        string? currentUser = this.Context.User?.Claims.FirstOrDefault( claim => claim.Type == ClaimTypes.Email )?.Value;

        if( string.IsNullOrEmpty( currentUser ) == false )
        {
            string connectionId = this.GetConnectionId();
            this._connectionManager.Add( currentUser, connectionId );
        }
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            string? userEmail = this.Context.User?.Claims.FirstOrDefault( claim =>
                                    string.Equals( claim.Type, ClaimTypes.Email, StringComparison.OrdinalIgnoreCase ) )?.Value;
            string? userName = this.Context.User?.Claims.FirstOrDefault( claim =>
                                    string.Equals( claim.Type, ClaimTypes.Name, StringComparison.OrdinalIgnoreCase ) )?.Value;

            if( string.IsNullOrEmpty( userEmail ) == false )
            {
                User userInDatabase = await this._userService.GetUserByEmail( userEmail ).ConfigureAwait( false );

                string key = $"{userEmail}-{userName}-{userInDatabase.Id}";

                this._connectionManager.Add( key, this.GetConnectionId() );
            }
        }
        catch( NullReferenceException )
        {
            //  TODO: Handle exceptions
        }
        await base.OnConnectedAsync().ConfigureAwait( false );
    }

    public override async Task OnDisconnectedAsync( Exception? exception )
    {
        try
        {
            string? userEmail = this.Context.User?.Claims.FirstOrDefault( claim =>
                                    string.Equals( claim.Type, ClaimTypes.Email, StringComparison.OrdinalIgnoreCase ) )?.Value;
            string? userName = this.Context.User?.Claims.FirstOrDefault( claim =>
                                    string.Equals( claim.Type, ClaimTypes.Name, StringComparison.OrdinalIgnoreCase ) )?.Value;

            if( string.IsNullOrEmpty( userEmail ) == false )
            {
                User userInDatabase = await this._userService.GetUserByEmail( userEmail ).ConfigureAwait( false );

                string key = $"{userEmail}-{userName}-{userInDatabase.Id}";

                this._connectionManager.Remove( key, this.GetConnectionId() );

                await this.Clients.All.SendAsync( "UserDisconnected", userEmail ).ConfigureAwait( false );
            }
        }
        catch( NullReferenceException )
        {
            //  TODO: Handle exceptions
        }
        await base.OnDisconnectedAsync( exception ).ConfigureAwait( false );
    }

    public async Task SendMessageAsync( string receiverEmail, string chatMessage )
    {
        string? userEmail = this.Context.User?.Claims.FirstOrDefault( claim =>
                                string.Equals( claim.Type, ClaimTypes.Email, StringComparison.OrdinalIgnoreCase ) )?.Value;
        string? userName = this.Context.User?.Claims.FirstOrDefault( claim =>
                                string.Equals( claim.Type, ClaimTypes.Name, StringComparison.OrdinalIgnoreCase ) )?.Value;
        string? userPicture = this.Context.User?.Claims.FirstOrDefault( claim =>
                                string.Equals( claim.Type, "picture", StringComparison.OrdinalIgnoreCase ) )?.Value;

        if( ( string.IsNullOrEmpty( userEmail ) == false ) &&
            ( string.IsNullOrEmpty( userName ) == false ) )
        {
            User messageSender = await this._userService.GetUserByEmail( userEmail ).ConfigureAwait( false );
            string senderKey = $"{userEmail}-{userName}-{messageSender.Id}";
            User messageReceiver = await this._userService.GetUserByEmail( receiverEmail ).ConfigureAwait( false );
            string receiverKey = $"{messageReceiver.Email}-{messageReceiver.UserName}-{messageReceiver.Id}";

            Models.MessagePack messagePack = new Models.MessagePack()
            {
                UserName = userName,
                Message = chatMessage,
                Picture = ( userPicture is null ) ? string.Empty : userPicture,
                CreatedAt = DateTime.Now
            };

            IEnumerable<string> receiverConnectionIds = this._connectionManager.GetConnections( receiverKey );
            IEnumerable<string> senderConnectionIds = this._connectionManager.GetConnections( senderKey );

            if( string.Equals( receiverKey, senderKey, StringComparison.OrdinalIgnoreCase ) )
            {
                foreach( string connectionId in receiverConnectionIds )
                {
                    await this.Clients.Client( connectionId ).SendAsync( "ReceivePrivateMessage", messagePack ).ConfigureAwait( false );
                }
            }
            else
            {
                foreach( string connectionId in receiverConnectionIds )
                {
                    await this.Clients.Client( connectionId ).SendAsync( "ReceivePrivateMessage", messagePack ).ConfigureAwait( false );
                }
                foreach( string connectionId in senderConnectionIds )
                {
                    await this.Clients.Client( connectionId ).SendAsync( "ReceivePrivateMessage", messagePack ).ConfigureAwait( false );
                }
            }
        }
    }

    private string GetConnectionId()
    {
        return this.Context.ConnectionId;
    }
}
