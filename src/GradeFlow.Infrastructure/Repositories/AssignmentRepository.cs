using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;
using GradeFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GradeFlow.Infrastructure.Repositories;

public sealed class AssignmentRepository(GradeFlowDbContext dbContext) : IAssignmentRepository
{
    public async Task<IReadOnlyCollection<Assignment>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Assignments
            .AsNoTracking()
            .Include(x => x.Questions)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Assignments
            .AsNoTracking()
            .Include(x => x.Questions)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Assignment?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Assignments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public void Add(Assignment assignment) => dbContext.Assignments.Add(assignment);

    public void Remove(Assignment assignment) => dbContext.Assignments.Remove(assignment);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
