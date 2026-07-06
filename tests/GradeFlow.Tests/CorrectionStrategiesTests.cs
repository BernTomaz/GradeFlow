using System.Text.Json;
using FluentAssertions;
using GradeFlow.Application.Corrections;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Tests;

public sealed class CorrectionStrategiesTests
{
    [Fact]
    public void Multiple_choice_should_accept_matching_option()
    {
        var outcome = new MultipleChoiceCorrectionStrategy().Correct(
            Question(QuestionType.MultipleChoice, points: 2),
            AnswerKey("A"),
            StudentAnswer("a"));

        outcome.IsCorrect.Should().BeTrue();
        outcome.ScoreAwarded.Should().Be(2);
    }

    [Fact]
    public void Multiple_choice_should_reject_different_option()
    {
        var outcome = new MultipleChoiceCorrectionStrategy().Correct(
            Question(QuestionType.MultipleChoice),
            AnswerKey("A"),
            StudentAnswer("B"));

        outcome.IsCorrect.Should().BeFalse();
        outcome.ScoreAwarded.Should().Be(0);
    }

    [Fact]
    public void True_false_should_compare_boolean_answers()
    {
        var outcome = new TrueFalseCorrectionStrategy().Correct(
            Question(QuestionType.TrueFalse),
            AnswerKey("verdadeiro"),
            StudentAnswer("true"));

        outcome.IsCorrect.Should().BeTrue();
    }

    [Fact]
    public void Numeric_should_accept_answer_inside_tolerance()
    {
        var outcome = new NumericCorrectionStrategy().Correct(
            Question(QuestionType.Numeric),
            AnswerKey("10", tolerance: 0.5m),
            StudentAnswer("10,4"));

        outcome.IsCorrect.Should().BeTrue();
    }

    [Fact]
    public void Numeric_should_reject_answer_outside_tolerance()
    {
        var outcome = new NumericCorrectionStrategy().Correct(
            Question(QuestionType.Numeric),
            AnswerKey("10", tolerance: 0.5m),
            StudentAnswer("10.6"));

        outcome.IsCorrect.Should().BeFalse();
    }

    [Fact]
    public void Numeric_should_mark_invalid_answer_for_review()
    {
        var outcome = new NumericCorrectionStrategy().Correct(
            Question(QuestionType.Numeric),
            AnswerKey("10"),
            StudentAnswer("dez"));

        outcome.IsCorrect.Should().BeFalse();
        outcome.NeedsReview.Should().BeTrue();
    }

    [Theory]
    [InlineData("correcao automatica de avaliacoes")]
    [InlineData("Correção automática de avaliações!")]
    [InlineData("AUTOMATIZAR CORREÇÕES")]
    public void Short_text_should_normalize_and_accept_alternatives(string answer)
    {
        var outcome = new ShortTextCorrectionStrategy().Correct(
            Question(QuestionType.ShortText),
            AnswerKey(
                "Correção automática de avaliações",
                acceptedAnswers: ["Automatizar correções", "Corrigir provas automaticamente"]),
            StudentAnswer(answer));

        outcome.IsCorrect.Should().BeTrue();
    }

    private static Question Question(QuestionType type, decimal points = 1)
        => new()
        {
            Id = Guid.NewGuid(),
            Type = type,
            Points = points
        };

    private static AnswerKey AnswerKey(
        string correctAnswer,
        decimal? tolerance = null,
        string[]? acceptedAnswers = null)
        => new()
        {
            Id = Guid.NewGuid(),
            CorrectAnswer = correctAnswer,
            Tolerance = tolerance,
            AcceptedAnswersJson = acceptedAnswers is null ? null : JsonSerializer.Serialize(acceptedAnswers)
        };

    private static StudentAnswer StudentAnswer(string answer)
        => new()
        {
            Id = Guid.NewGuid(),
            Answer = answer
        };
}
