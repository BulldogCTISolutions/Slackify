using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

//  Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//  Add Google OAuth
builder.Services.AddAuthentication(authenticationOptions =>
    {
        authenticationOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        authenticationOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        authenticationOptions.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(cookieOptions =>
    {
        cookieOptions.Cookie.Name = "SlackifyAuthenticationCookie";
        cookieOptions.LoginPath = "/auth/google-login";
    })
    .AddGoogle( googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

        googleOptions.Scope.Add("Profile");
        googleOptions.Events.OnCreatingTicket = context =>
        {
            //  Should handle null references.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string pictureUri = context.User.GetProperty("picture").GetString();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
            context.Identity.AddClaim(new Claim("picture", pictureUri));
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            return Task.CompletedTask;
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
