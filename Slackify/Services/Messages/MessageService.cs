namespace Slackify.Services.Messages;

public sealed class MessageService : IMessageService
{
    private readonly SlackifyDbContext _dbContext;

    public MessageService( SlackifyDbContext dbContext )
    {
        this._dbContext = dbContext;
    }

    ValueTask<Message> IMessageService.RegisterMessage( Message user )
    {
        throw new NotImplementedException();
    }

    public ValueTask<ICollection<Message>> GetAllMessages()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<Message> GetMessageByEmail( string email )
    {
        Message messageInDb = await this._dbContext.Messages.Where( m => m.Chat == email )
                                                            .SingleOrDefaultAsync()
                                                            .ConfigureAwait( false );
        return messageInDb;
    }

    public async ValueTask<Message> GetMessageById( int id )
    {
        Message messageInDb = await this._dbContext.Messages.Where( m => m.Id == id )
                                                            .SingleOrDefaultAsync()
                                                            .ConfigureAwait( false );
        return messageInDb;
    }

    public async ValueTask<Message> RegisterMessage( Message message )
    {
        _ = this._dbContext.Messages.Add( message );
        _ = await this._dbContext.SaveChangesAsync().ConfigureAwait( false );

        return message;
    }

    async ValueTask<int> IMessageService.SaveMessage( Message chatMessage )
    {
        _ = this._dbContext.Messages.Add( chatMessage );
        return await this._dbContext.SaveChangesAsync().ConfigureAwait( false );
    }

    public async ValueTask<ICollection<Message>> GetConversations( int fromId, int toId )
    {
        return await this._dbContext.Messages
                                    .Where( message => ( message.FromUserId == fromId && message.ToUserId == toId ) || ( message.FromUserId == toId && message.ToUserId == fromId ) )
                                    .Include( message => message.FromUser )
                                    .OrderByDescending( message => message.CreatedDate )
                                    .AsNoTracking()
                                    .ToListAsync()
                                    .ConfigureAwait( false );
    }

    async ValueTask<ICollection<Message>> IMessageService.GetRecentConversations( int id )
    {
        return await this._dbContext.Messages
                                    .Where( message => message.FromUserId == id || message.ToUserId == id )
                                    .Include( message => message.FromUser )
                                    .OrderByDescending( message => message.CreatedDate )
                                    .Take( 20 )
                                    .AsNoTracking()
                                    .ToListAsync()
                                    .ConfigureAwait( false );
    }
}
