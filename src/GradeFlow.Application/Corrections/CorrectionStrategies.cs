using System.Globalization;
using System.Text;
using System.Text.Json;
using GradeFlow.Domain.Corrections;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Application.Corrections;

public sealed class MultipleChoiceCorrectionStrategy : ICorrectionStrategy
{
    public QuestionType QuestionType => QuestionType.MultipleChoice;

    public CorrectionOutcome Correct(Question question, AnswerKey answerKey, StudentAnswer studentAnswer)
        => CorrectionStrategyResult.Result(
            question,
            answerKey,
            string.Equals(studentAnswer.Answer.Trim(), answerKey.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase));
}

public sealed class TrueFalseCorrectionStrategy : ICorrectionStrategy
{
    public QuestionType QuestionType => QuestionType.TrueFalse;

    public CorrectionOutcome Correct(Question question, AnswerKey answerKey, StudentAnswer studentAnswer)
    {
        if (!TryParse(studentAnswer.Answer, out var studentValue) || !TryParse(answerKey.CorrectAnswer, out var correctValue))
        {
            return new CorrectionOutcome(false, 0, answerKey.FeedbackIncorrect ?? "Resposta invalida.", true);
        }

        return CorrectionStrategyResult.Result(question, answerKey, studentValue == correctValue);
    }

    private static bool TryParse(string value, out bool result)
    {
        var normalized = value.Trim().ToLowerInvariant();

        if (normalized is "true" or "verdadeiro" or "v")
        {
            result = true;
            return true;
        }

        if (normalized is "false" or "falso" or "f")
        {
            result = false;
            return true;
        }

        return bool.TryParse(value, out result);
    }
}

public sealed class NumericCorrectionStrategy : ICorrectionStrategy
{
    public QuestionType QuestionType => QuestionType.Numeric;

    public CorrectionOutcome Correct(Question question, AnswerKey answerKey, StudentAnswer studentAnswer)
    {
        if (!decimal.TryParse(studentAnswer.Answer, NumberStyles.Number, CultureInfo.InvariantCulture, out var studentValue)
            || !decimal.TryParse(answerKey.CorrectAnswer, NumberStyles.Number, CultureInfo.InvariantCulture, out var correctValue))
        {
            return new CorrectionOutcome(false, 0, answerKey.FeedbackIncorrect ?? "Resposta numerica invalida.", true);
        }

        var isCorrect = Math.Abs(studentValue - correctValue) <= (answerKey.Tolerance ?? 0);
        return CorrectionStrategyResult.Result(question, answerKey, isCorrect);
    }
}

public sealed class ShortTextCorrectionStrategy : ICorrectionStrategy
{
    public QuestionType QuestionType => QuestionType.ShortText;

    public CorrectionOutcome Correct(Question question, AnswerKey answerKey, StudentAnswer studentAnswer)
    {
        var answer = Normalize(studentAnswer.Answer);
        var acceptedAnswers = new[] { answerKey.CorrectAnswer }
            .Concat(ReadAcceptedAnswers(answerKey.AcceptedAnswersJson))
            .Select(Normalize);

        return CorrectionStrategyResult.Result(question, answerKey, acceptedAnswers.Contains(answer));
    }

    private static IReadOnlyCollection<string> ReadAcceptedAnswers(string? json)
        => string.IsNullOrWhiteSpace(json)
            ? []
            : JsonSerializer.Deserialize<string[]>(json) ?? [];

    private static string Normalize(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        var previousWasSpace = false;

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category == UnicodeCategory.NonSpacingMark || char.IsPunctuation(character))
            {
                continue;
            }

            if (char.IsWhiteSpace(character))
            {
                if (!previousWasSpace) builder.Append(' ');
                previousWasSpace = true;
                continue;
            }

            builder.Append(character);
            previousWasSpace = false;
        }

        return builder.ToString().Trim().Normalize(NormalizationForm.FormC);
    }
}

internal static class CorrectionStrategyResult
{
    public static CorrectionOutcome Result(Question question, AnswerKey answerKey, bool isCorrect)
        => new(
            isCorrect,
            isCorrect ? question.Points : 0,
            isCorrect ? answerKey.FeedbackCorrect ?? "Resposta correta." : answerKey.FeedbackIncorrect ?? "Resposta incorreta.",
            false);
}
