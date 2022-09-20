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
        User userInDatabase = await this._databaseContext.Users.Where( u => u.Email == email )
                                                               .SingleOrDefaultAsync()
                                                               .ConfigureAwait( false );
        return userInDatabase;
    }

    public async ValueTask<User> GetUserById( int id )
    {
        User userInDatabase = await this._databaseContext.Users.Where( u => u.Id == id )
                                                               .SingleOrDefaultAsync()
                                                               .ConfigureAwait( false );
        return userInDatabase;
    }

    public async ValueTask<User> RegisterUser( User user )
    {
        this._databaseContext.Users.Add( user );
        await this._databaseContext.SaveChangesAsync().ConfigureAwait( false );

        return user;
    }
}
