using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using GradeFlow.Application.DTOs.Auth;
using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Application.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
}

public interface ITokenService
{
    AuthResponse Create(User user);
}

public sealed class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Nome e obrigatorio.");
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email e obrigatorio.");
        ValidatePassword(request.Password, "Senha");
        if (request.Role == UserRole.Admin)
            throw new ValidationException("Perfil Admin nao pode ser criado pelo registro publico.");

        var email = request.Email.Trim().ToLowerInvariant();
        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new ValidationException("Email ja cadastrado.");

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Role = request.Role
        };
        user.PasswordHash = passwordHasher.Hash(request.Password);

        userRepository.Add(user);
        await userRepository.SaveChangesAsync(cancellationToken);
        return tokenService.Create(user);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null)
            return null;

        return !passwordHasher.Verify(user.PasswordHash, request.Password)
            ? null
            : tokenService.Create(user);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        ValidatePassword(request.NewPassword, "Nova senha");

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.CurrentPassword))
            return false;

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        await userRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void ValidatePassword(string password, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            throw new ValidationException($"{fieldName} deve ter pelo menos 8 caracteres.");
        if (!password.Any(char.IsUpper))
            throw new ValidationException($"{fieldName} deve ter pelo menos uma letra maiuscula.");
        if (!password.Any(char.IsDigit))
            throw new ValidationException($"{fieldName} deve ter pelo menos um numero.");
        if (!password.Any(x => !char.IsLetterOrDigit(x)))
            throw new ValidationException($"{fieldName} deve ter pelo menos um caractere especial.");
    }
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hash, string password);
}

public sealed class PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string hash, string password)
    {
        var parts = hash.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var expected = Convert.FromBase64String(parts[2]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
