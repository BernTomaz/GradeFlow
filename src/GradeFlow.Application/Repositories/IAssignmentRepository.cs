using GradeFlow.Domain.Entities;

namespace GradeFlow.Application.Repositories;

public interface IAssignmentRepository
{
    Task<IReadOnlyCollection<Assignment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Assignment?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> TitleExistsAsync(string title, Guid? exceptId = null, CancellationToken cancellationToken = default);
    void Add(Assignment assignment);
    void Remove(Assignment assignment);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
