using GradeFlow.Domain.Enums;

namespace GradeFlow.Domain.Entities;

public sealed class Assignment
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Subject { get; set; }
    public decimal TotalPoints { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Question> Questions { get; set; } = [];
    public ICollection<Submission> Submissions { get; set; } = [];
}
