using GradeFlow.Application.DTOs.Assignments;

namespace GradeFlow.Application.Services;

public interface IAssignmentService
{
    Task<IReadOnlyCollection<AssignmentResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AssignmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateAssignmentRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
