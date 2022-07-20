namespace Slackify.Services.Users;

public interface IUserService
{
    ValueTask<User> RegisterUser(User user);
    ValueTask<User> GetUserById(int id);
    ValueTask<User> GetUserByEmail(string email);
    ValueTask<ICollection<User>> GetAllUsers();
}
