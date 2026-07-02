using GradeFlow.Domain.Entities;

namespace GradeFlow.Application.Repositories;

public interface ISubmissionRepository
{
    Task<bool> AssignmentExistsAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Question>> GetAssignmentQuestionsAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Submission>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Submission?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Submission?> GetForCorrectionAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Submission submission);
    void Remove(Submission submission);
    void RemoveAnswers(IEnumerable<StudentAnswer> answers);
    void AddCorrectionResult(CorrectionResult correctionResult);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
