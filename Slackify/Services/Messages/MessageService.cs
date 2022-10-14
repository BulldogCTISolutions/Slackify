namespace Slackify.Services.Messages;

public sealed class MessageService : IMessageService
{
    private readonly SlackifyDatabaseContext _databaseContext;

    public MessageService( SlackifyDatabaseContext databaseContext )
    {
        this._databaseContext = databaseContext;
    }

    ValueTask<Message> IMessageService.RegisterMessage( Message message )
    {
        throw new NotImplementedException();
    }

    public ValueTask<ICollection<Message>> GetAllMessages()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<Message> GetMessageByEmail( string email )
    {
        Message? messageInDatabase = await this._databaseContext.Messages.SingleOrDefaultAsync( m =>
                                                string.Equals( m.Chat, email, StringComparison.OrdinalIgnoreCase ) )
                                                                         .ConfigureAwait( false );
        return messageInDatabase!;   //TODO: Null needs to be handled by caller.
    }

    public async ValueTask<Message> GetMessageById( int id )
    {
        Message? messageInDatabase = await this._databaseContext.Messages.SingleOrDefaultAsync( m => m.Id == id )
                                                                         .ConfigureAwait( false );
        return messageInDatabase!;   //TODO: Null needs to be handled by caller.
    }

    public async ValueTask<Message> RegisterMessage( Message message )
    {
        this._databaseContext.Messages.Add( message );
        await this._databaseContext.SaveChangesAsync().ConfigureAwait( false );

        return message;
    }

    async ValueTask<int> IMessageService.SaveMessage( Message chatMessage )
    {
        this._databaseContext.Messages.Add( chatMessage );
        return await this._databaseContext.SaveChangesAsync().ConfigureAwait( false );
    }

    public async ValueTask<ICollection<Message>> GetConversations( int fromId, int toId )
    {
        return await this._databaseContext.Messages
                                          .Where( message => ( message.FromUserId == fromId && message.ToUserId == toId ) ||
                                                                            ( message.FromUserId == toId && message.ToUserId == fromId ) )
                                          .Include( message => message.FromUser )
                                          .OrderByDescending( message => message.CreatedDate )
                                          .AsNoTracking()
                                          .ToListAsync()
                                          .ConfigureAwait( false );
    }

    async ValueTask<ICollection<Message>> IMessageService.GetRecentConversations( int id )
    {
        return await this._databaseContext.Messages
                                         .Where( message => ( message.FromUserId == id ) ||
                                                                           ( message.ToUserId == id ) )
                                         .Include( message => message.FromUser )
                                         .OrderByDescending( message => message.CreatedDate )
                                         .Take( 20 )
                                         .AsNoTracking()
                                         .ToListAsync()
                                         .ConfigureAwait( false );
    }
}
