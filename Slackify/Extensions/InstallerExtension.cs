using Slackify.Installers;

namespace Slackify.Extensions;

public static class InstallerExtension
{
    public static void InstallServicesInAssembly( this IServiceCollection services, IConfiguration configuration )
    {
        List<IInstaller> installers = typeof( Program ).Assembly.ExportedTypes
            .Where( x => typeof( IInstaller ).IsAssignableFrom( x ) && !x.IsInterface && !x.IsAbstract )
            .Select( Activator.CreateInstance )
            .Cast<IInstaller>()
            .ToList();

        installers.ForEach( installer => installer.InstallService( services, configuration ) );
    }
}
