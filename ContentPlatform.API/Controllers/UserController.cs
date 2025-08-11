using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContentPlatform.API.Services;

namespace ContentPlatform.API.Controllers;

    [ApiController]
    [Route("api/users")]
    [Authorize] // Requires valid Clerk JWT
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncUser()
    {
        // Using the standard ClaimTypes, which match the mapped claims from the JWT handler.
        var clerkUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(clerkUserId) || string.IsNullOrEmpty(email))
        {
            Console.WriteLine($"SyncUser failed: clerkUserId is '{clerkUserId}', email is '{email}'.");
            return BadRequest("Invalid user information. Clerk User ID or Email not found in token claims.");
        }

        var user = await _userService.CreateOrUpdateUserAsync(clerkUserId, email);
           
        if (user == null)
        {
            return StatusCode(500, "Failed to sync user with the database.");
        }

        return Ok(new
        {
            message = "User synced successfully",
            userEmail = user.Email,
        });
    }
}
