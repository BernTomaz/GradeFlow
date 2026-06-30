using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Domain.Corrections;

public interface ICorrectionStrategy
{
    QuestionType QuestionType { get; }
    CorrectionOutcome Correct(Question question, AnswerKey answerKey, StudentAnswer studentAnswer);
}

public sealed record CorrectionOutcome(bool IsCorrect, decimal ScoreAwarded, string? Feedback, bool NeedsReview);
