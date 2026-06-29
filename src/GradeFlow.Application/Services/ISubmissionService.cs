using GradeFlow.Application.DTOs.Submissions;

namespace GradeFlow.Application.Services;

public interface ISubmissionService
{
    Task<IReadOnlyCollection<SubmissionResponse>?> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<SubmissionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SubmissionResponse?> CreateAsync(Guid assignmentId, CreateSubmissionRequest request, CancellationToken cancellationToken = default);
}
