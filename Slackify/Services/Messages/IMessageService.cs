namespace Slackify.Services.Messages;

public interface IMessageService
{
    ValueTask<Message> RegisterMessage( Message user );
    ValueTask<Message> GetMessageById( int id );
    ValueTask<Message> GetMessageByEmail( string email );
    ValueTask<ICollection<Message>> GetAllMessages();
}
