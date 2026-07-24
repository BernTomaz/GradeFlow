using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using GradeFlow.Application.DTOs.Assignments;
using GradeFlow.Application.DTOs.Auth;
using GradeFlow.Application.Repositories;
using GradeFlow.Application.Services;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GradeFlow.Tests;

public sealed class JwtAuthenticationIntegrationTests
{
    private const string JwtKey = "test-only-jwt-key-with-more-than-32-chars";
    private const string Issuer = "GradeFlow.Tests";
    private const string Audience = "GradeFlow.Tests";

    [Fact]
    public async Task Login_valid_returns_token_with_expected_claims()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("teacher@gradeflow.test", "secret1"));
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        var token = new JsonWebTokenHandler().ReadJsonWebToken(auth!.Token);

        auth.Token.Should().NotBeNullOrWhiteSpace();
        auth.User.Role.Should().Be(UserRole.Teacher);
        token.Claims.Should().Contain(x => x.Type == ClaimTypes.NameIdentifier && x.Value == TestUsers.Teacher.Id.ToString());
        token.Claims.Should().Contain(x => x.Type == ClaimTypes.Role && x.Value == UserRole.Teacher.ToString());
    }

    [Fact]
    public async Task Login_invalid_returns_unauthorized_without_token()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("teacher@gradeflow.test", "wrong"));
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        body.Should().Contain("Restam 4 tentativa(s).");
    }

    [Fact]
    public async Task Login_is_rate_limited()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        for (var i = 0; i < 5; i++)
        {
            var allowed = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("teacher@gradeflow.test", "wrong"));
            allowed.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        var blocked = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("teacher@gradeflow.test", "wrong"));

        blocked.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task Protected_route_requires_valid_token()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var withoutToken = await client.GetAsync("/api/assignments");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await LoginToken(client));
        var withToken = await client.GetAsync("/api/assignments");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid.token.value");
        var tampered = await client.GetAsync("/api/assignments");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(TestUsers.Teacher, DateTime.UtcNow.AddMinutes(-5)));
        var expired = await client.GetAsync("/api/assignments");

        withoutToken.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        withToken.StatusCode.Should().Be(HttpStatusCode.OK);
        tampered.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        expired.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Role_authorization_returns_forbidden_or_success()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(TestUsers.Student, DateTime.UtcNow.AddMinutes(10)));
        var forbidden = await client.PostAsJsonAsync("/api/assignments", new CreateAssignmentRequest("Prova", null, null));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(TestUsers.Teacher, DateTime.UtcNow.AddMinutes(10)));
        var allowed = await client.PostAsJsonAsync("/api/assignments", new CreateAssignmentRequest("Prova", null, null));

        forbidden.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        allowed.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Change_password_requires_authentication_and_current_password()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var withoutToken = await client.PostAsJsonAsync("/api/auth/change-password", new ChangePasswordRequest("secret1", "Senha1995@"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await LoginToken(client));
        var invalid = await client.PostAsJsonAsync("/api/auth/change-password", new ChangePasswordRequest("wrong", "Senha1995@"));
        var valid = await client.PostAsJsonAsync("/api/auth/change-password", new ChangePasswordRequest("secret1", "Senha1995@"));

        withoutToken.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        invalid.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        valid.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Health_returns_ok()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void Invalid_jwt_configuration_fails_on_start()
    {
        using var factory = CreateFactory(("Jwt:Key", "short"));

        var act = () => factory.CreateClient();

        act.Should().Throw<OptionsValidationException>();
    }

    private static async Task<string> LoginToken(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("teacher@gradeflow.test", "secret1"));
        return (await response.Content.ReadFromJsonAsync<AuthResponse>())!.Token;
    }

    private static string CreateToken(User user, DateTime expires)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]),
            Expires = expires,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
                SecurityAlgorithms.HmacSha256)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    private static WebApplicationFactory<Program> CreateFactory(params (string Key, string Value)[] overrides)
        => new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureLogging(logging => logging.ClearProviders());

            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                var values = new Dictionary<string, string?>
                {
                    ["Jwt:Issuer"] = Issuer,
                    ["Jwt:Audience"] = Audience,
                    ["Jwt:Key"] = JwtKey,
                    ["Jwt:ExpirationMinutes"] = "480"
                };

                foreach (var (key, value) in overrides)
                {
                    values[key] = value;
                }

                configuration.AddInMemoryCollection(values);
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IUserRepository>();
                services.RemoveAll<IAssignmentService>();
                services.AddSingleton<IUserRepository>(new FakeUserRepository(TestUsers.Teacher, TestUsers.Student));
                services.AddScoped<IAssignmentService, FakeAssignmentService>();
            });
        });

    private static class TestUsers
    {
        private static readonly PasswordHasher Hasher = new();

        public static readonly User Teacher = new()
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Name = "Teacher",
            Email = "teacher@gradeflow.test",
            PasswordHash = Hasher.Hash("secret1"),
            Role = UserRole.Teacher
        };

        public static readonly User Student = new()
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Name = "Student",
            Email = "student@gradeflow.test",
            PasswordHash = Hasher.Hash("secret1"),
            Role = UserRole.Student
        };
    }

    private sealed class FakeUserRepository(params User[] users) : IUserRepository
    {
        private readonly List<User> users = users.Select(user => new User
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            Role = user.Role
        }).ToList();

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(users.FirstOrDefault(x => x.Id == id));

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(users.FirstOrDefault(x => x.Email == email));

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(users.Any(x => x.Email == email));

        public void Add(User user)
        {
            users.Add(user);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeAssignmentService : IAssignmentService
    {
        private static readonly AssignmentResponse Assignment = new(
            Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            "Prova",
            null,
            null,
            0,
            AssignmentStatus.Draft,
            DateTime.UtcNow,
            null);

        public Task<IReadOnlyCollection<AssignmentResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<AssignmentResponse>>([Assignment]);

        public Task<AssignmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<AssignmentResponse?>(Assignment);

        public Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(Assignment);

        public Task<bool> UpdateAsync(Guid id, UpdateAssignmentRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(true);
    }
}
