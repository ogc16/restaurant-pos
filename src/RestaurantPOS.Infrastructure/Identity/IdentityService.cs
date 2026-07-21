using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantPOS.Infrastructure.Identity;

public class IdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _configuration;

    public IdentityService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
            return Result<AuthResponse>.Failure("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Result<AuthResponse>.Failure("Invalid credentials");

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        var expiration = DateTime.UtcNow.AddHours(12);

        return Result<AuthResponse>.Success(new AuthResponse(token, user.Email!, user.FullName, user.Role, expiration));
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return Result<AuthResponse>.Failure("Email already registered");

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return Result<AuthResponse>.Failure(result.Errors.Select(e => e.Description).ToList());

        await _userManager.AddToRoleAsync(user, request.Role);

        var token = GenerateJwtToken(user);
        var expiration = DateTime.UtcNow.AddHours(12);

        return Result<AuthResponse>.Success(new AuthResponse(token, user.Email!, user.FullName, user.Role, expiration));
    }

    public async Task<Result<UserResponse>> GetUserAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result<UserResponse>.Failure("User not found");

        return Result<UserResponse>.Success(new UserResponse(user.Id, user.Email!, user.FullName, user.Role, user.IsActive));
    }

    public async Task<Result<List<UserResponse>>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();
        return Result<List<UserResponse>>.Success(
            users.Select(u => new UserResponse(u.Id, u.Email!, u.FullName, u.Role, u.IsActive)).ToList()
        );
    }

    public async Task<Result<UserResponse>> UpdateUserRoleAsync(int userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result<UserResponse>.Failure("User not found");

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, newRole);
        user.Role = newRole;
        await _userManager.UpdateAsync(user);

        return Result<UserResponse>.Success(new UserResponse(user.Id, user.Email!, user.FullName, user.Role, user.IsActive));
    }

    public async Task<Result<UserResponse>> DeactivateUserAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result<UserResponse>.Failure("User not found");

        user.IsActive = false;
        await _userManager.UpdateAsync(user);

        return Result<UserResponse>.Success(new UserResponse(user.Id, user.Email!, user.FullName, user.Role, user.IsActive));
    }

    public async Task<Result<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result<bool>.Failure("User not found");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
            return Result<bool>.Failure(result.Errors.Select(e => e.Description).ToList());

        return Result<bool>.Success(true);
    }

    private string GenerateJwtToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured")));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new("fullName", user.FullName),
            new(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
