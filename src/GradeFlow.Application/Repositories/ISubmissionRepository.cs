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
    Task<StudentAnswer?> GetAnswerAsync(Guid submissionId, Guid questionId, CancellationToken cancellationToken = default);
    Task ReplaceAnswersAsync(Guid submissionId, IEnumerable<StudentAnswer> answers, CancellationToken cancellationToken = default);
    Task<int> UpdateAnswerAsync(Guid answerId, string answer, CancellationToken cancellationToken = default);
    Task RefreshSubmissionAfterAnswerUpdateAsync(Guid submissionId, CancellationToken cancellationToken = default);
    void Add(Submission submission);
    void AddAnswer(StudentAnswer answer);
    void Remove(Submission submission);
    void AddCorrectionResult(CorrectionResult correctionResult);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
