using Microsoft.AspNetCore.ResponseCompression;

namespace Slackify.Installers;

public class ServiceInstaller : IInstaller
{
    public void InstallService( IServiceCollection services, IConfiguration configuration )
    {
        services.AddScoped<CookiesProvider>();
        services.AddAutoMapper( typeof( SlackifyProfile ) );

        services.AddSignalR( config => config.EnableDetailedErrors = true );
        services.AddResponseCompression( opt =>
        {
            opt.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat( new[] { "application/octet-stream" } );
        } );
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddSingleton<ConnectionManager>();
        services.AddSingleton<IPublicClientApplicationWrapper, PublicClientApplicationWrapper>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMessageService, MessageService>();
    }
}
