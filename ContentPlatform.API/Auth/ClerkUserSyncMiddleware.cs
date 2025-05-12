using System.Security.Claims;
using ContentPlatform.API.Services;

namespace ContentPlatform.API.Auth;

public class ClerkUserSyncMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ClerkUserSyncMiddleware> _logger;

    public ClerkUserSyncMiddleware(RequestDelegate next, ILogger<ClerkUserSyncMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUserService userService)
    {
        // Only process if the user is authenticated
        if (context.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                var clerkUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = context.User.FindFirstValue(ClaimTypes.Email);

                if (!string.IsNullOrEmpty(clerkUserId) && !string.IsNullOrEmpty(email))
                {
                    // Create or update the user in our database
                    await userService.CreateOrUpdateUserAsync(clerkUserId, email);
                }
                else
                {
                    _logger.LogWarning("Authenticated user missing required claims. UserId: {UserId}, Email: {Email}",
                        clerkUserId ?? "null", email ?? "null");
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't block the request
                _logger.LogError(ex, "Error synchronizing Clerk user");
            }
        }

        // Continue processing the request
        await _next(context);
    }
}

// Extension method to make it easier to add the middleware
public static class ClerkUserSyncMiddlewareExtensions
{
    public static IApplicationBuilder UseClerkUserSync(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ClerkUserSyncMiddleware>();
    }
}
