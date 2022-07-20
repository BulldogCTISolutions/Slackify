namespace Slackify.Services.Users;

public class UserService : IUserService
{
    private SlackifyDbContext dbContext;

    public UserService( SlackifyDbContext dbContext )
    {
        this.dbContext = dbContext;
    }
    public ValueTask<ICollection<User>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<User> GetUserByEmail( string email )
    {
        User userInDb = await this.dbContext.Users.Where( u => u.Email == email ).SingleOrDefaultAsync();
        return userInDb;
    }

    public async ValueTask<User> GetUserById( int id )
    {
        User userInDb = await this.dbContext.Users.Where( u => u.Id == id ).SingleOrDefaultAsync();
        return userInDb;
    }

    public async ValueTask<User> RegisterUser( User user )
    {
        this.dbContext.Users.Add( user );
        await this.dbContext.SaveChangesAsync();

        return user;
    }
}
