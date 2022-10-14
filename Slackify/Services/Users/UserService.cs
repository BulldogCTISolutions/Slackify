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
        User? userInDatabase = await this._databaseContext.Users.SingleOrDefaultAsync( u =>
                                            string.Equals( u.Email, email, StringComparison.OrdinalIgnoreCase ) )
                                                                .ConfigureAwait( false );
        return userInDatabase!; // TODO:  Null needs to be handled by caller.
    }

    public async ValueTask<User> GetUserById( int id )
    {
        User? userInDatabase = await this._databaseContext.Users.SingleOrDefaultAsync( u => u.Id == id )
                                                                .ConfigureAwait( false );
        return userInDatabase!; // TODO:  Null needs to be handled by caller
    }

    public async ValueTask<User> RegisterUser( User user )
    {
        this._databaseContext.Users.Add( user );
        await this._databaseContext.SaveChangesAsync().ConfigureAwait( false );

        return user;
    }
}
