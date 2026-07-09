using GradeFlow.Domain.Enums;

namespace GradeFlow.Application.Services;

public interface ICurrentUser
{
    Guid? Id { get; }
    UserRole? Role { get; }
    bool IsAdmin => Role == UserRole.Admin;
    bool IsTeacher => Role == UserRole.Teacher;
    bool IsStudent => Role == UserRole.Student;
}

public sealed class SystemCurrentUser : ICurrentUser
{
    public static readonly SystemCurrentUser Instance = new();

    private SystemCurrentUser()
    {
    }

    public Guid? Id => null;
    public UserRole? Role => UserRole.Admin;
}
