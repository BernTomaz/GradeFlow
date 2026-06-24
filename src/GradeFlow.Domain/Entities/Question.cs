using GradeFlow.Domain.Enums;

namespace GradeFlow.Domain.Entities;

public sealed class Question
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public decimal Points { get; set; }
    public int Order { get; set; }
    public string? CorrectionConfigJson { get; set; }

    public Assignment? Assignment { get; set; }
    public AnswerKey? AnswerKey { get; set; }
    public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
}
