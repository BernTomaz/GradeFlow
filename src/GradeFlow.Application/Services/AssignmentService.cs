using System.ComponentModel.DataAnnotations;
using GradeFlow.Application.DTOs.Assignments;
using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Application.Services;

public sealed class AssignmentService(
    IAssignmentRepository assignmentRepository,
    ICurrentUser? currentUser = null) : IAssignmentService
{
    private readonly ICurrentUser currentUser = currentUser ?? SystemCurrentUser.Instance;

    public async Task<IReadOnlyCollection<AssignmentResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await assignmentRepository.GetAllAsync(cancellationToken))
            .Where(CanRead)
            .Select(Map)
            .ToList();

    public async Task<AssignmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await assignmentRepository.GetByIdAsync(id, cancellationToken);
        return assignment is null || !CanRead(assignment) ? null : Map(assignment);
    }

    public async Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Title, request.Subject);
        var title = request.Title.Trim();
        if (await assignmentRepository.TitleExistsAsync(title, cancellationToken: cancellationToken))
            throw new ValidationException("Esse nome já está sendo usado.");

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = TrimOrNull(request.Description),
            Subject = request.Subject.Trim(),
            Status = AssignmentStatus.Draft,
            TeacherUserId = currentUser.IsTeacher ? currentUser.Id : null,
            CreatedAt = DateTime.UtcNow
        };

        assignmentRepository.Add(assignment);
        await assignmentRepository.SaveChangesAsync(cancellationToken);
        return Map(assignment);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Title, request.Subject);
        var title = request.Title.Trim();
        if (await assignmentRepository.TitleExistsAsync(title, id, cancellationToken))
            throw new ValidationException("Esse nome já está sendo usado.");

        var assignment = await assignmentRepository.GetForUpdateAsync(id, cancellationToken);
        if (assignment is null || !CanManage(assignment)) return false;

        assignment.Title = title;
        assignment.Description = TrimOrNull(request.Description);
        assignment.Subject = request.Subject.Trim();
        assignment.UpdatedAt = DateTime.UtcNow;
        await assignmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await assignmentRepository.GetForUpdateAsync(id, cancellationToken);
        if (assignment is null || !CanManage(assignment)) return false;

        assignmentRepository.Remove(assignment);
        await assignmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static AssignmentResponse Map(Assignment assignment)
        => new(
            assignment.Id,
            assignment.Title,
            assignment.Description,
            assignment.Subject,
            assignment.Questions.Sum(question => question.Points),
            assignment.Status,
            assignment.CreatedAt,
            assignment.UpdatedAt);

    private static void Validate(string title, string subject)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ValidationException("Titulo e obrigatorio.");
        if (string.IsNullOrWhiteSpace(subject))
            throw new ValidationException("Disciplina e obrigatoria.");
    }

    private static string? TrimOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private bool CanRead(Assignment assignment)
        => currentUser.IsAdmin
            || (currentUser.IsTeacher && (assignment.TeacherUserId is null || assignment.TeacherUserId == currentUser.Id));

    private bool CanManage(Assignment assignment) => CanRead(assignment);
}
