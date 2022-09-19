namespace Slackify.Services.Users;

public class UserService : IUserService
{
    private readonly SlackifyDbContext _dbContext;

    public UserService( SlackifyDbContext dbContext )
    {
        this._dbContext = dbContext;
    }
    public ValueTask<ICollection<User>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<User> GetUserByEmail( string email )
    {
        User userInDb = await this._dbContext.Users.Where( u => u.Email == email )
                                                  .SingleOrDefaultAsync()
                                                  .ConfigureAwait( false );
        return userInDb;
    }

    public async ValueTask<User> GetUserById( int id )
    {
        User userInDb = await this._dbContext.Users.Where( u => u.Id == id )
                                                  .SingleOrDefaultAsync()
                                                  .ConfigureAwait( false );
        return userInDb;
    }

    public async ValueTask<User> RegisterUser( User user )
    {
        _ = this._dbContext.Users.Add( user );
        _ = await this._dbContext.SaveChangesAsync().ConfigureAwait( false );

        return user;
    }
}
