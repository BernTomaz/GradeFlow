using GradeFlow.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GradeFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        return services;
    }
}
