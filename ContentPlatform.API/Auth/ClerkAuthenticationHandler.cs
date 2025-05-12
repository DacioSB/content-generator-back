using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ContentPlatform.API.Auth;

public class ClerkAuthenticationHandler : JwtBearerHandler
{
    private readonly ClerkSettings _clerkSettings;

    public ClerkAuthenticationHandler(
        IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptionsMonitor<ClerkSettings> clerkSettings)
        : base(options, logger, encoder)
    {
        _clerkSettings = clerkSettings.CurrentValue;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Let the base JWT handler do its validation
        var result = await base.HandleAuthenticateAsync();
        
        if (!result.Succeeded)
        {
            return result;
        }

        var clerkUserId = result.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(clerkUserId))
        {
            return AuthenticateResult.Fail("Invalid token: missing user ID claim");
        }

        // You can add additional validation or claims transformation here if needed
        // For example, you might want to check if the user exists in your database
        
        return result;
    }
}

// Extension method to make it easier to add Clerk authentication
public static class ClerkAuthenticationExtensions
{
    public static AuthenticationBuilder AddClerkAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Clerk settings
        var clerkSettingsSection = configuration.GetSection(ClerkSettings.SectionName);
        services.Configure<ClerkSettings>(clerkSettingsSection);
        var clerkSettings = clerkSettingsSection.Get<ClerkSettings>();

        if (clerkSettings == null)
        {
            throw new InvalidOperationException("Clerk settings are not configured properly");
        }

        return services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = clerkSettings.Authority;
            options.Audience = clerkSettings.Audience;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = clerkSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = clerkSettings.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };
            
            // Add event handlers for additional validation or logging
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    // You can add additional validation logic here
                    var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                    {
                        context.Fail("Invalid token: missing user ID claim");
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    // Log authentication failures
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ClerkAuthenticationHandler>>();
                    logger.LogError(context.Exception, "Authentication failed");
                    return Task.CompletedTask;
                }
            };
        });
    }
}
