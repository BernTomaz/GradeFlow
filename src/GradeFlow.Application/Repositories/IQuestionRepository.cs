using GradeFlow.Domain.Entities;

namespace GradeFlow.Application.Repositories;

public interface IQuestionRepository
{
    Task<bool> AssignmentExistsAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Question>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Question?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Question question);
    void Remove(Question question);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
