namespace GradeFlow.Domain.Entities;

public sealed class CorrectionLog
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public Guid QuestionId { get; set; }
    public string CorrectionType { get; set; } = string.Empty;
    public string OriginalAnswer { get; set; } = string.Empty;
    public string? NormalizedAnswer { get; set; }
    public string? ExpectedAnswer { get; set; }
    public decimal Score { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ReviewedByUserId { get; set; }
}
