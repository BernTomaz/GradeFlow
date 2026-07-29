using GradeFlow.Domain.Enums;

namespace GradeFlow.Application.DTOs.Auth;

public sealed record RegisterRequest(string Name, string Email, string Password, UserRole Role);

public sealed record SetupAdminRequest(string Name, string Email, string Password);

public sealed record SetupStatusResponse(bool Available);

public sealed record LoginRequest(string Email, string Password);

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public sealed record ChangeNameRequest(string Name);

public sealed record AuthResponse(string Token, DateTime ExpiresAt, UserResponse User);

public sealed record UserResponse(Guid Id, string Name, string Email, UserRole Role);
