namespace GradeFlow.Domain.Entities;

public sealed class CorrectionResult
{
    public Guid Id { get; set; }
    public Guid StudentAnswerId { get; set; }
    public bool IsCorrect { get; set; }
    public decimal ScoreAwarded { get; set; }
    public string? Feedback { get; set; }
    public string? CorrectionType { get; set; }
    public DateTime CreatedAt { get; set; }

    public StudentAnswer? StudentAnswer { get; set; }
}
