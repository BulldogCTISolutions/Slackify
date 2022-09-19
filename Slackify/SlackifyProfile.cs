using AutoMapper;

namespace Slackify;

public class SlackifyProfile : Profile
{
    public SlackifyProfile()
    {
        this.CreateMap<Message, Models.MessagePack>()
            .ForMember( messagePack => messagePack.Message,
            opt => opt.MapFrom( message => message.Chat ) )
            .ForMember( messagePack => messagePack.UserName,
            opt => opt.MapFrom( message => message.FromUser.UserName ) )
            .ForMember( messagePack => messagePack.Picture,
            opt => opt.MapFrom( message => message.FromUser.Picture ) );
    }
}
