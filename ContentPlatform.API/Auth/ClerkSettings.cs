namespace ContentPlatform.API.Auth;

public class ClerkSettings
{
    public const string SectionName = "Clerk";
    
    public string JwtKey { get; set; } = string.Empty;
    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
}
