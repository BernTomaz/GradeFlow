using GradeFlow.Application.Repositories;
using GradeFlow.Infrastructure.Data;
using GradeFlow.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GradeFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<GradeFlowDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();

        return services;
    }
}
