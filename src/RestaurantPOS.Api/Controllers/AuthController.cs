using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Infrastructure.Identity;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IdentityService _identityService;

    public AuthController(IdentityService identityService) => _identityService = identityService;

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _identityService.LoginAsync(request);
        if (!result.IsSuccess) return Unauthorized(result.Error);
        return Ok(result.Data);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await _identityService.RegisterAsync(request);
        if (!result.IsSuccess) return BadRequest(result.Errors);
        return Ok(result.Data);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();
        var result = await _identityService.GetUserAsync(int.Parse(userId));
        if (!result.IsSuccess) return NotFound(result.Error);
        return Ok(result.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers()
    {
        var result = await _identityService.GetAllUsersAsync();
        return Ok(result.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromQuery] string role)
    {
        var result = await _identityService.UpdateUserRoleAsync(id, role);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("users/{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var result = await _identityService.DeactivateUserAsync(id);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [Authorize]
    [HttpPut("users/{id}/password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        var result = await _identityService.ChangePasswordAsync(id, request.CurrentPassword, request.NewPassword);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
