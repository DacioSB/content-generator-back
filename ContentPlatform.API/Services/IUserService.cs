using ContentPlatform.API.Models;

namespace ContentPlatform.API.Services;

public interface IUserService
{
    Task<User?> GetUserByClerkIdAsync(string clerkUserId);
    Task<User> CreateOrUpdateUserAsync(string clerkUserId, string email);
    Task<bool> ValidateUserOwnershipAsync(string clerkUserId, Guid projectId);
}
