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

    public void Add(Submission submission) => dbContext.Submissions.Add(submission);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
