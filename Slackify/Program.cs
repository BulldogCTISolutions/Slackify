WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

//  Add keys and secrets to builder configuration
builder.Configuration.SetBasePath( AppDomain.CurrentDomain.BaseDirectory )
                     .AddEnvironmentVariables()
                     .AddJsonFile( "appsettings.json" )
//  Passing “false” as the second variable for UserSecrets. That’s because in .NET 6, User Secrets were made “required” by default and by passing true, we make them optional. 
                     .AddUserSecrets<Program>( false )
                     .Build();

//  Add services to the container.
Slackify.Extensions.InstallerExtension.InstallServicesInAssembly( builder.Services, builder.Configuration );

WebApplication app = builder.Build();

//  Configure the HTTP request pipeline.
if( app.Environment.IsDevelopment() == false )
{
    app.UseExceptionHandler( "/Error" );
    //  The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseEndpoints( endpoints =>
{
    app.MapControllers();
    app.MapBlazorHub();
    app.MapFallbackToPage( "/_Host" );
    app.MapHub<ChatHub>( "/chat" );
} );

app.Run();
