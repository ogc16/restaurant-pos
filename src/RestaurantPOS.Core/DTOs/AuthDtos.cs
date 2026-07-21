namespace RestaurantPOS.Core.DTOs;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FullName, string Role);
public record AuthResponse(string Token, string Email, string FullName, string Role, DateTime Expiration);
public record UserResponse(int Id, string Email, string FullName, string Role, bool IsActive);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
