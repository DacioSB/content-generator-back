using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ContentPlatform.API.Services;

namespace ContentPlatform.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthTestController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthTestController> _logger;

    public AuthTestController(IUserService userService, ILogger<AuthTestController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult PublicEndpoint()
    {
        return Ok(new { message = "This is a public endpoint that anyone can access" });
    }

    [HttpGet("protected")]
    [Authorize]
    public async Task<IActionResult> ProtectedEndpoint()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID claim not found" });
        }

        var user = await _userService.GetUserByClerkIdAsync(userId);

        return Ok(new
        {
            message = "You have successfully accessed a protected endpoint",
            clerkUserId = userId,
            email = email,
            userExists = user != null,
            userData = user != null ? new { user.Id, user.Email } : null
        });
    }

    [HttpGet("user-info")]
    [Authorize]
    public IActionResult GetUserInfo()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        
        return Ok(new
        {
            isAuthenticated = User.Identity?.IsAuthenticated ?? false,
            claims = claims
        });
    }
}
