namespace Slackify.Properties;

#pragma warning disable CA1056 // Change the type of property 'Settings.InstanceUrl' from 'string' to 'System.Uri'

public class Settings
{
    public string ClientId { get; set; }
    public string Tenant { get; set; }
    public string TenantId { get; set; }
    public string InstanceUrl { get; set; }
    public string PolicySignUpSignIn { get; set; }
    public string Authority { get; set; }
    public NestedSettings[] Scopes { get; set; }
    public string ApiUrl { get; set; }
}

#pragma warning restore CA1056 // Change the type of property 'Settings.ApiUrl' from 'string' to 'System.Uri'

public class NestedSettings
{
    public string? Value { get; set; }
}

public static class SettingsExtension
{
    public static string[] ToStringArray( this NestedSettings[] nestedSettings )
    {
        if( nestedSettings is null )
        {
            throw new ArgumentNullException( nameof( nestedSettings ) );
        }

        string[]? result = new string[nestedSettings.Length];

        for( int i = 0; i < nestedSettings.Length; ++i )
        {
            result[i] = nestedSettings[i].Value!;
        }

        return result!;
    }
}
