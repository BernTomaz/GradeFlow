using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;
using GradeFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GradeFlow.Infrastructure.Repositories;

public sealed class QuestionRepository(GradeFlowDbContext dbContext) : IQuestionRepository
{
    public async Task<bool> AssignmentExistsAsync(Guid assignmentId, CancellationToken cancellationToken = default)
        => await dbContext.Assignments.AnyAsync(x => x.Id == assignmentId, cancellationToken);

    public async Task<IReadOnlyCollection<Question>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
        => await dbContext.Questions
            .AsNoTracking()
            .Include(x => x.AnswerKey)
            .Where(x => x.AssignmentId == assignmentId)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

    public async Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Questions
            .AsNoTracking()
            .Include(x => x.AnswerKey)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Question?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Questions
            .Include(x => x.AnswerKey)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public void Add(Question question) => dbContext.Questions.Add(question);

    public void Remove(Question question) => dbContext.Questions.Remove(question);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
