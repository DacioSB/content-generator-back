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
        // Get user info from JWT claims
        var clerkUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);

        //print clerkUserId and email for debugging
        Console.WriteLine($"Clerk User ID: {clerkUserId}, Email: {email}");

        if (string.IsNullOrEmpty(clerkUserId) || string.IsNullOrEmpty(email))
        {
            return BadRequest("Invalid user information");
        }
        var user = await _userService.CreateOrUpdateUserAsync(clerkUserId, email);
           
        if (user == null)
        {
            return StatusCode(500, "Failed to sync user");
        }
        return Ok(new
        {
            message = "User synced successfully",
            userEmail = user.Email,
        });
    }
}