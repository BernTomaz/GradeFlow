using GradeFlow.Application.DTOs.Assignments;
using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Application.Services;

public sealed class AssignmentService(IAssignmentRepository assignmentRepository) : IAssignmentService
{
    public async Task<IReadOnlyCollection<AssignmentResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await assignmentRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();

    public async Task<AssignmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await assignmentRepository.GetByIdAsync(id, cancellationToken);
        return assignment is null ? null : Map(assignment);
    }

    public async Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Subject = request.Subject?.Trim(),
            Status = AssignmentStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        assignmentRepository.Add(assignment);
        await assignmentRepository.SaveChangesAsync(cancellationToken);
        return Map(assignment);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var assignment = await assignmentRepository.GetForUpdateAsync(id, cancellationToken);
        if (assignment is null) return false;

        assignment.Title = request.Title.Trim();
        assignment.Description = request.Description?.Trim();
        assignment.Subject = request.Subject?.Trim();
        assignment.UpdatedAt = DateTime.UtcNow;
        await assignmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await assignmentRepository.GetForUpdateAsync(id, cancellationToken);
        if (assignment is null) return false;

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
}
