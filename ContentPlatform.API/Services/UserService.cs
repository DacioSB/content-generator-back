using ContentPlatform.API.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ContentPlatform.API.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetUserByClerkIdAsync(string clerkUserId)
    {
        return await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == clerkUserId);
    }

    public async Task<User> CreateOrUpdateUserAsync(string clerkUserId, string email)
    {
        var user = await GetUserByClerkIdAsync(clerkUserId);

        if (user != null)
        {
            // User already exists, update email if it's different.
            if (user.Email != email)
            {
                user.Email = email;
                _context.Update(user);
                _logger.LogInformation("Updated user email for Clerk ID: {ClerkUserId}", clerkUserId);
                await _context.SaveChangesAsync();
            }
            return user;
        }

        // User does not exist, so let's create them.
        var newUser = new User
        {
            Id = clerkUserId,
            Email = email
        };
        _context.Add(newUser);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created new user with Clerk ID: {ClerkUserId}", clerkUserId);
            return newUser;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            // This specific error code (23505) means a unique constraint was violated.
            // This handles the race condition where another request created the user
            // between our check and our SaveChanges call.
            _logger.LogWarning(
                "Race condition detected: User with Clerk ID {ClerkUserId} was created by another request. Fetching existing user.",
                clerkUserId);
            
            // The user now exists, so we can detach our failed entity and fetch the existing one.
            _context.ChangeTracker.Clear();
            return await GetUserByClerkIdAsync(clerkUserId);
        }
    }

    public async Task<bool> ValidateUserOwnershipAsync(string clerkUserId, Guid projectId)
    {
        return await _context.Projects.AnyAsync(p => p.Id == projectId && p.UserId == clerkUserId);
    }
}
