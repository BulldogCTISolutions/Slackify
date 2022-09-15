using Microsoft.AspNetCore.ResponseCompression;

namespace Slackify.Installers;

public class ServiceInstaller : IInstaller
{
    public void InstallService( IServiceCollection services, IConfiguration configuration )
    {
        services.AddScoped<CookiesProvider>();

        services.AddSignalR( config => config.EnableDetailedErrors = true );
        services.AddResponseCompression( opt =>
        {
            opt.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat( new[] { "application/octet-stream" } );
        } );
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddSingleton<ConnectionManager>();
        services.AddScoped<IUserService, UserService>();
    }
}
