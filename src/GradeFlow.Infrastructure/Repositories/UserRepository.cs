using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;
using GradeFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GradeFlow.Infrastructure.Repositories;

public sealed class UserRepository(GradeFlowDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken);

    public void Add(User user) => dbContext.Users.Add(user);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
