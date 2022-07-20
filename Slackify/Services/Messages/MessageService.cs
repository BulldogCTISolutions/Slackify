namespace Slackify.Services.Messages;

public class MessageService : IMessageService
{
    private SlackifyDbContext dbContext;

    public MessageService( SlackifyDbContext dbContext )
    {
        this.dbContext = dbContext;
    }
    public ValueTask<ICollection<Message>> GetAllMessages()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<Message> GetMessageByEmail( string email )
    {
        Message messageInDb = await this.dbContext.Messages.Where( m => m.Email == email ).SingleOrDefaultAsync();
        return messageInDb;
    }

    public async ValueTask<Message> GetMessageById( int id )
    {
        Message messageInDb = await this.dbContext.Messages.Where( m => m.Id == id ).SingleOrDefaultAsync();
        return messageInDb;
    }

    public async ValueTask<Message> RegisterMessage( Message message )
    {
        this.dbContext.Messages.Add( message );
        await this.dbContext.SaveChangesAsync();

        return message;
    }
}
