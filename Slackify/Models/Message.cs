using System.ComponentModel.DataAnnotations;

namespace Slackify.Models;

public record Message
{
    [Key]
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public string Chat { get; set; }
    public DateTime CreatedDate { get; set; }
    public virtual User FromUser { get; set; }
    public virtual User ToUser { get; set; }
}
