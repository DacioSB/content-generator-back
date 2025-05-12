using ContentPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

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

        if (user == null)
        {
            // Create new user
            user = new User
            {
                Id = clerkUserId,
                Email = email
            };

            _context.Add(user);
            _logger.LogInformation("Created new user with Clerk ID: {ClerkUserId}", clerkUserId);
        }
        else
        {
            // Update existing user if email changed
            if (user.Email != email)
            {
                user.Email = email;
                _context.Update(user);
                _logger.LogInformation("Updated user email for Clerk ID: {ClerkUserId}", clerkUserId);
            }
        }

        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ValidateUserOwnershipAsync(string clerkUserId, Guid projectId)
    {
        return await _context.Projects.AnyAsync(p => p.Id == projectId && p.UserId == clerkUserId);
    }
}
