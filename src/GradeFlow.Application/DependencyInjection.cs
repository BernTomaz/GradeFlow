using GradeFlow.Application.Corrections;
using GradeFlow.Application.Services;
using GradeFlow.Domain.Corrections;
using Microsoft.Extensions.DependencyInjection;

namespace GradeFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<ICorrectionService, CorrectionService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICorrectionStrategy, MultipleChoiceCorrectionStrategy>();
        services.AddScoped<ICorrectionStrategy, TrueFalseCorrectionStrategy>();
        services.AddScoped<ICorrectionStrategy, NumericCorrectionStrategy>();
        services.AddScoped<ICorrectionStrategy, ShortTextCorrectionStrategy>();
        return services;
    }
}
