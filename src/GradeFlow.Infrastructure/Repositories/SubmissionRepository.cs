using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;
using GradeFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GradeFlow.Infrastructure.Repositories;

public sealed class SubmissionRepository(GradeFlowDbContext dbContext) : ISubmissionRepository
{
    public async Task<bool> AssignmentExistsAsync(Guid assignmentId, CancellationToken cancellationToken = default)
        => await dbContext.Assignments.AnyAsync(x => x.Id == assignmentId, cancellationToken);

    public async Task<IReadOnlyCollection<Question>> GetAssignmentQuestionsAsync(
        Guid assignmentId,
        CancellationToken cancellationToken = default)
        => await dbContext.Questions
            .AsNoTracking()
            .Where(x => x.AssignmentId == assignmentId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Submission>> GetByAssignmentIdAsync(
        Guid assignmentId,
        CancellationToken cancellationToken = default)
        => await dbContext.Submissions
            .AsNoTracking()
            .Include(x => x.StudentAnswers)
            .Where(x => x.AssignmentId == assignmentId)
            .OrderByDescending(x => x.SubmittedAt)
            .ToListAsync(cancellationToken);

    public async Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Submissions
            .AsNoTracking()
            .Include(x => x.StudentAnswers)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Submission?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Submissions
            .Include(x => x.StudentAnswers)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Submission?> GetForCorrectionAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Submissions
            .Include(x => x.Assignment)
                .ThenInclude(x => x!.Questions)
            .Include(x => x.StudentAnswers)
                .ThenInclude(x => x.Question)
                    .ThenInclude(x => x!.AnswerKey)
            .Include(x => x.StudentAnswers)
                .ThenInclude(x => x.CorrectionResult)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<StudentAnswer?> GetAnswerAsync(
        Guid submissionId,
        Guid questionId,
        CancellationToken cancellationToken = default)
        => await dbContext.StudentAnswers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubmissionId == submissionId && x.QuestionId == questionId, cancellationToken);

    public async Task<int> UpdateAnswerAsync(Guid answerId, string answer, CancellationToken cancellationToken = default)
        => await dbContext.StudentAnswers
            .Where(x => x.Id == answerId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Answer, answer)
                .SetProperty(x => x.ScoreAwarded, 0)
                .SetProperty(x => x.IsCorrect, false)
                .SetProperty(x => x.Feedback, (string?)null)
                .SetProperty(x => x.NeedsReview, false),
                cancellationToken);

    public async Task RefreshSubmissionAfterAnswerUpdateAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        var finalScore = await dbContext.StudentAnswers
            .Where(x => x.SubmissionId == submissionId)
            .SumAsync(x => x.ScoreAwarded, cancellationToken);

        await dbContext.Submissions
            .Where(x => x.Id == submissionId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Status, GradeFlow.Domain.Enums.SubmissionStatus.Pending)
                .SetProperty(x => x.FinalScore, finalScore)
                .SetProperty(x => x.CorrectedAt, (DateTime?)null)
                .SetProperty(x => x.ReviewedAt, (DateTime?)null),
                cancellationToken);
    }

    public void Add(Submission submission) => dbContext.Submissions.Add(submission);

    public void AddAnswer(StudentAnswer answer) => dbContext.StudentAnswers.Add(answer);

    public void Remove(Submission submission) => dbContext.Submissions.Remove(submission);

    public void AddCorrectionResult(CorrectionResult correctionResult)
        => dbContext.CorrectionResults.Add(correctionResult);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
