namespace Slackify.Models;

public record UsersForListing
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
}
