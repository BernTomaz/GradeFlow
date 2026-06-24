namespace GradeFlow.Domain.Entities;

public sealed class StudentAnswer
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public Guid QuestionId { get; set; }
    public string? Answer { get; set; }
    public decimal ScoreAwarded { get; set; }
    public bool IsCorrect { get; set; }
    public string? Feedback { get; set; }
    public bool NeedsReview { get; set; }

    public Submission? Submission { get; set; }
    public Question? Question { get; set; }
    public CorrectionResult? CorrectionResult { get; set; }
}
