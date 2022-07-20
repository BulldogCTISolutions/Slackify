using System.ComponentModel.DataAnnotations;

namespace Slackify.Models;

public class User
{
    public User()
    {
        ChatMessagesFromUsers = new HashSet<Message>();
        ChatMessagesToUsers = new HashSet<Message>();
    }

    [Key]
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Picture { get; set; }
    public DateTime DateJoined { get; set; }
    public virtual ICollection<Message> ChatMessagesFromUsers { get; set; }
    public virtual ICollection<Message> ChatMessagesToUsers { get; set; }
}
