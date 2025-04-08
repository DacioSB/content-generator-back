using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using ContentPlatform.API.Services;
using ContentPlatform.API.Models.DTO;
using ContentPlatform.API.Models;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    
    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Clerk user ID
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var project = await _projectService.CreateProjectAsync(userId, dto);
        return CreatedAtAction(nameof(GetProject), new { projectId = project.Id }, project);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserProjects()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var projects = await _projectService.GetUserProjectsAsync(userId);
        return Ok(projects);
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicProjects()
    {
        var projects = await _projectService.GetPublicProjectsAsync();
        return Ok(projects);
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProject(Guid projectId)
    {
        var project = await _projectService.GetProjectByIdAsync(projectId);
        if (project == null) return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!project.IsPublic && project.UserId != userId) return Forbid();

        return Ok(project);
    }

    [HttpGet("{projectId}/content")]
    public async Task<IActionResult> GetProjectContent(Guid projectId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        
        var content = await _projectService.GetProjectContentAsync(userId, projectId);
        return Ok(content);
    }
}