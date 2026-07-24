using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using GradeFlow.Application.DTOs.Auth;
using GradeFlow.Application.Repositories;
using GradeFlow.Application.Services;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task Register_should_hash_password_accept_teacher_and_reject_public_admin()
    {
        var repository = new FakeUserRepository();
        var service = new AuthService(repository, new PasswordHasher(), new FakeTokenService());

        var response = await service.RegisterAsync(new RegisterRequest(" Ana ", " ANA@EMAIL.COM ", "Nado1994@", UserRole.Teacher));
        var admin = () => service.RegisterAsync(new RegisterRequest("Root", "root@email.com", "Nado1994@", UserRole.Admin));

        response.User.Email.Should().Be("ana@email.com");
        response.User.Role.Should().Be(UserRole.Teacher);
        repository.Users.Should().ContainSingle();
        repository.Users[0].PasswordHash.Should().NotBe("Nado1994@");
        await admin.Should().ThrowAsync<ValidationException>()
            .WithMessage("Perfil Admin nao pode ser criado pelo registro publico.");
    }

    [Fact]
    public async Task Login_should_validate_password()
    {
        var hasher = new PasswordHasher();
        var repository = new FakeUserRepository(new User
        {
            Name = "Ana",
            Email = "ana@email.com",
            PasswordHash = hasher.Hash("Nado1994@"),
            Role = UserRole.Student
        });
        var service = new AuthService(repository, hasher, new FakeTokenService());

        var invalid = await service.LoginAsync(new LoginRequest("ana@email.com", "wrong"));
        var valid = await service.LoginAsync(new LoginRequest(" ANA@EMAIL.COM ", "Nado1994@"));

        invalid.Should().BeNull();
        valid!.User.Role.Should().Be(UserRole.Student);
    }

    [Fact]
    public async Task Login_should_accept_existing_six_character_password()
    {
        var hasher = new PasswordHasher();
        var repository = new FakeUserRepository(new User
        {
            Name = "Ana",
            Email = "ana@email.com",
            PasswordHash = hasher.Hash("nado94"),
            Role = UserRole.Student
        });
        var service = new AuthService(repository, hasher, new FakeTokenService());

        var result = await service.LoginAsync(new LoginRequest("ana@email.com", "nado94"));

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_should_reject_duplicate_email_ignoring_case()
    {
        var repository = new FakeUserRepository(new User { Email = "ana@email.com" });
        var service = new AuthService(repository, new PasswordHasher(), new FakeTokenService());

        var act = () => service.RegisterAsync(new RegisterRequest("Ana", " ANA@EMAIL.COM ", "Nado1994@", UserRole.Student));

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Email ja cadastrado.");
    }

    [Fact]
    public async Task Register_should_require_strong_password()
    {
        var service = new AuthService(new FakeUserRepository(), new PasswordHasher(), new FakeTokenService());

        var shortPassword = () => service.RegisterAsync(new RegisterRequest("Ana", "ana1@email.com", "Nado1@", UserRole.Student));
        var missingUpper = () => service.RegisterAsync(new RegisterRequest("Ana", "ana2@email.com", "nado1994@", UserRole.Student));
        var missingNumber = () => service.RegisterAsync(new RegisterRequest("Ana", "ana3@email.com", "Nadosenha@", UserRole.Student));
        var missingSpecial = () => service.RegisterAsync(new RegisterRequest("Ana", "ana4@email.com", "Nado1994", UserRole.Student));

        await shortPassword.Should().ThrowAsync<ValidationException>()
            .WithMessage("Senha deve ter pelo menos 8 caracteres.");
        await missingUpper.Should().ThrowAsync<ValidationException>()
            .WithMessage("Senha deve ter pelo menos uma letra maiuscula.");
        await missingNumber.Should().ThrowAsync<ValidationException>()
            .WithMessage("Senha deve ter pelo menos um numero.");
        await missingSpecial.Should().ThrowAsync<ValidationException>()
            .WithMessage("Senha deve ter pelo menos um caractere especial.");
    }

    [Fact]
    public async Task ChangePassword_should_validate_current_password_and_update_hash()
    {
        var hasher = new PasswordHasher();
        var user = new User { Id = Guid.NewGuid(), Email = "ana@email.com", PasswordHash = hasher.Hash("Nado1994@") };
        var repository = new FakeUserRepository(user);
        var service = new AuthService(repository, hasher, new FakeTokenService());

        var invalid = await service.ChangePasswordAsync(user.Id, new ChangePasswordRequest("wrong", "Senha1995@"));
        var valid = await service.ChangePasswordAsync(user.Id, new ChangePasswordRequest("Nado1994@", "Senha1995@"));

        invalid.Should().BeFalse();
        valid.Should().BeTrue();
        hasher.Verify(user.PasswordHash, "Nado1994@").Should().BeFalse();
        hasher.Verify(user.PasswordHash, "Senha1995@").Should().BeTrue();
    }

    private sealed class FakeUserRepository(params User[] users) : IUserRepository
    {
        public List<User> Users { get; } = users.ToList();

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Users.FirstOrDefault(x => x.Id == id));

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(Users.FirstOrDefault(x => x.Email == email));

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(Users.Any(x => x.Email == email));

        public void Add(User user) => Users.Add(user);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeTokenService : ITokenService
    {
        public AuthResponse Create(User user)
            => new("token", DateTime.UtcNow.AddHours(1), new UserResponse(user.Id, user.Name, user.Email, user.Role));
    }
}
