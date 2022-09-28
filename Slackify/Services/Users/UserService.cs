namespace Slackify.Services.Users;

public class UserService : IUserService
{
    private readonly SlackifyDatabaseContext _databaseContext;

    public UserService( SlackifyDatabaseContext databaseContext )
    {
        this._databaseContext = databaseContext;
    }
    public ValueTask<ICollection<User>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<User> GetUserByEmail( string email )
    {
        User? userInDatabase = await this._databaseContext.Users.SingleOrDefaultAsync( u => u.Email == email )
                                                                .ConfigureAwait( false );
#pragma warning disable CS8603 // Possible null reference return.
        return userInDatabase; // Null handled by caller.
#pragma warning restore CS8603 // Possible null reference return.
    }

    public async ValueTask<User> GetUserById( int id )
    {
        User? userInDatabase = await this._databaseContext.Users.SingleOrDefaultAsync( u => u.Id == id )
                                                                .ConfigureAwait( false );
#pragma warning disable CS8603 // Possible null reference return.
        return userInDatabase; // Null handled by caller
#pragma warning restore CS8603 // Possible null reference return.
    }

    public async ValueTask<User> RegisterUser( User user )
    {
        this._databaseContext.Users.Add( user );
        await this._databaseContext.SaveChangesAsync().ConfigureAwait( false );

        return user;
    }
}
