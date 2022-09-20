namespace Slackify.Data;

public class SlackifyDatabaseContext : DbContext
{
    public SlackifyDatabaseContext( DbContextOptions options )
        : base( options )
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        if( modelBuilder is null )
        {
            throw new ArgumentNullException( nameof( modelBuilder ),
                                             "Program goes boom! if there is no modelBuilder" );
        }
        base.OnModelCreating( modelBuilder );

        modelBuilder.Entity<Message>( entity =>
        {
            entity.HasOne( message => message.FromUser )
                  .WithMany( message => message.ChatMessagesFromUsers )
                  .HasForeignKey( message => message.FromUserId )
                  .OnDelete( DeleteBehavior.ClientSetNull );

            entity.HasOne( message => message.ToUser )
                  .WithMany( message => message.ChatMessagesToUsers )
                  .HasForeignKey( message => message.ToUserId )
                  .OnDelete( DeleteBehavior.ClientSetNull );
        } );
    }
}
