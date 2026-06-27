using GradeFlow.Application.DTOs.Questions;

namespace GradeFlow.Application.Services;

public interface IQuestionService
{
    Task<IReadOnlyCollection<QuestionResponse>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<QuestionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<QuestionResponse?> CreateAsync(Guid assignmentId, CreateQuestionRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateQuestionRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
