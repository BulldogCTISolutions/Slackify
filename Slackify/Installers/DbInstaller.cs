namespace Slackify.Installers;

public class DbInstaller : IInstaller
{
    public void InstallService( IServiceCollection services, IConfiguration configuration )
    {
        _ = services.AddDbContext<SlackifyDbContext>( option =>
                option.UseSqlServer( configuration.GetConnectionString( "DefaultConnection" ) ) );
    }
}
