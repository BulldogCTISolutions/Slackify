namespace Slackify.Installers;

public class DatabaseInstaller : IInstaller
{
    public void InstallService( IServiceCollection services, IConfiguration configuration )
    {
        services.AddDbContext<SlackifyDatabaseContext>( option =>
                option.UseSqlServer( configuration.GetConnectionString( "DefaultConnection" ) ) );
    }
}
