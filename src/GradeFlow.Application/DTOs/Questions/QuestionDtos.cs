using System.ComponentModel.DataAnnotations;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Application.DTOs.Questions;

public sealed record CreateAnswerKeyRequest(
    [Required]
    [MaxLength(2000)]
    string CorrectAnswer,
    IReadOnlyCollection<string>? AcceptedAnswers,
    IReadOnlyCollection<string>? Keywords,
    [Range(0, double.MaxValue)]
    decimal? Tolerance,
    [MaxLength(2000)]
    string? FeedbackCorrect,
    [MaxLength(2000)]
    string? FeedbackIncorrect);

public sealed record CreateQuestionRequest(
    [Required]
    [MaxLength(4000)]
    string Text,
    QuestionType Type,
    [Range(0.01, 10)]
    decimal Points,
    [Range(1, int.MaxValue)]
    int Order,
    [Required]
    CreateAnswerKeyRequest AnswerKey);

public sealed record UpdateQuestionRequest(
    [Required]
    [MaxLength(4000)]
    string Text,
    QuestionType Type,
    [Range(0.01, 10)]
    decimal Points,
    [Range(1, int.MaxValue)]
    int Order,
    [Required]
    CreateAnswerKeyRequest AnswerKey);

public sealed record QuestionResponse(
    Guid Id,
    Guid AssignmentId,
    string Text,
    QuestionType Type,
    decimal Points,
    int Order,
    AnswerKeyResponse? AnswerKey);

public sealed record AnswerKeyResponse(
    Guid Id,
    Guid QuestionId,
    string CorrectAnswer,
    IReadOnlyCollection<string>? AcceptedAnswers,
    IReadOnlyCollection<string>? Keywords,
    decimal? Tolerance,
    string? FeedbackCorrect,
    string? FeedbackIncorrect);
