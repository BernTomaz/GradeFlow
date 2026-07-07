using FluentAssertions;
using GradeFlow.Application;
using GradeFlow.Application.Services;
using GradeFlow.Domain.Corrections;
using Microsoft.Extensions.DependencyInjection;

namespace GradeFlow.Tests;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void Add_application_services_should_register_application_dependencies()
    {
        var services = new ServiceCollection();

        services.AddApplicationServices();

        services.Should().Contain(descriptor => descriptor.ServiceType == typeof(IAssignmentService));
        services.Should().Contain(descriptor => descriptor.ServiceType == typeof(IQuestionService));
        services.Should().Contain(descriptor => descriptor.ServiceType == typeof(ISubmissionService));
        services.Should().Contain(descriptor => descriptor.ServiceType == typeof(ICorrectionService));
        services.Should().Contain(descriptor => descriptor.ServiceType == typeof(ICorrectionStrategy));
    }
}
