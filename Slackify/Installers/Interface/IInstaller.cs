﻿namespace Slackify.Installers.Interface;

public interface IInstaller
{
    void InstallService( IServiceCollection services, IConfiguration configuration );
}
