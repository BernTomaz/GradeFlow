namespace GradeFlow.Domain.Entities;

public sealed class AnswerKey
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? AcceptedAnswersJson { get; set; }
    public string? KeywordsJson { get; set; }
    public decimal? Tolerance { get; set; }
    public string? FeedbackCorrect { get; set; }
    public string? FeedbackIncorrect { get; set; }

    public Question? Question { get; set; }
}
