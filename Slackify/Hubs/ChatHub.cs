using System.Security.Claims;

using Microsoft.AspNetCore.SignalR;

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
        Dictionary<string, HashSet<string>> list = this._connectionManager.GetUsers();

        await Clients.All.SendAsync( "ReceiveInitializeUsersList", list );
    }

    public void AddUserToRoom( string userEmail )
    {
        string currentUser = Context.User.Claims.Where( claim => claim.Type == ClaimTypes.Email ).FirstOrDefault().Value;

        string connectionId = GetConnectionId();
        this._connectionManager.Add( currentUser, connectionId );
    }

    public override async Task OnConnectedAsync()
    {
        string userEmail = Context.User.Claims.Where( claim => claim.Type == ClaimTypes.Email ).FirstOrDefault().Value;
        string userName = Context.User.Claims.Where( claim => claim.Type == ClaimTypes.Name ).FirstOrDefault().Value;

        string key = $"{userEmail}-{userName}";

        this._connectionManager.Add( key, GetConnectionId() );
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync( Exception exception )
    {
        string userEmail = Context.User.Claims.Where( claim => claim.Type == ClaimTypes.Email ).FirstOrDefault().Value;
        string userName = Context.User.Claims.Where( claim => claim.Type == ClaimTypes.Name ).FirstOrDefault().Value;

        string key = $"{userEmail}-{userName}";

        this._connectionManager.Remove( key, GetConnectionId() );

        await Clients.All.SendAsync( "UserDisconnected", userEmail );
        await base.OnDisconnectedAsync( exception );
    }

    private string GetConnectionId() => Context.ConnectionId;
}
