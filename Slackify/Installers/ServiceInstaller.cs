namespace Slackify.Installers;

public class ServiceInstaller : IInstaller
{
    public void InstallService( IServiceCollection services, IConfiguration configuration )
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();

        services.AddScoped<IUserService, UserService>();
    }
}
