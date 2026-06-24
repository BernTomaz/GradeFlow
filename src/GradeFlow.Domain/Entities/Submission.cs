using GradeFlow.Domain.Enums;

namespace GradeFlow.Domain.Entities;

public sealed class Submission
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? StudentEmail { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;
    public decimal FinalScore { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? CorrectedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public Assignment? Assignment { get; set; }
    public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
}
