using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slackify.Models;

[Table( "Slackify_Users" )]
public record User
{
    public User()
    {
        this.ChatMessagesFromUsers = new HashSet<Message>();
        this.ChatMessagesToUsers = new HashSet<Message>();
    }

    [Key]
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Picture { get; set; }
    public DateTime DateJoined { get; set; }
    public virtual ICollection<Message> ChatMessagesFromUsers { get; }
    public virtual ICollection<Message> ChatMessagesToUsers { get; }
}
