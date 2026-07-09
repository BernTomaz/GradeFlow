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
    public async Task Register_should_hash_password_and_reject_public_admin()
    {
        var repository = new FakeUserRepository();
        var service = new AuthService(repository, new PasswordHasher(), new FakeTokenService());

        var response = await service.RegisterAsync(new RegisterRequest(" Ana ", " ANA@EMAIL.COM ", "secret1", UserRole.Teacher));
        var admin = () => service.RegisterAsync(new RegisterRequest("Root", "root@email.com", "secret1", UserRole.Admin));

        response.User.Email.Should().Be("ana@email.com");
        repository.Users.Should().ContainSingle();
        repository.Users[0].PasswordHash.Should().NotBe("secret1");
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
            PasswordHash = hasher.Hash("secret1"),
            Role = UserRole.Student
        });
        var service = new AuthService(repository, hasher, new FakeTokenService());

        var invalid = await service.LoginAsync(new LoginRequest("ana@email.com", "wrong"));
        var valid = await service.LoginAsync(new LoginRequest(" ANA@EMAIL.COM ", "secret1"));

        invalid.Should().BeNull();
        valid!.User.Role.Should().Be(UserRole.Student);
    }

    [Fact]
    public async Task Register_should_reject_duplicate_email_ignoring_case()
    {
        var repository = new FakeUserRepository(new User { Email = "ana@email.com" });
        var service = new AuthService(repository, new PasswordHasher(), new FakeTokenService());

        var act = () => service.RegisterAsync(new RegisterRequest("Ana", " ANA@EMAIL.COM ", "secret1", UserRole.Student));

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Email ja cadastrado.");
    }

    private sealed class FakeUserRepository(params User[] users) : IUserRepository
    {
        public List<User> Users { get; } = users.ToList();

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
