using System.ComponentModel.DataAnnotations;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Application.DTOs.Submissions;

public sealed record CreateSubmissionRequest(
    [Required]
    [MaxLength(200)]
    string StudentName,
    [EmailAddress]
    [MaxLength(320)]
    string? StudentEmail,
    [Required]
    IReadOnlyCollection<CreateStudentAnswerRequest> Answers);

public sealed record CreateStudentAnswerRequest(
    Guid QuestionId,
    [Required]
    [MaxLength(4000)]
    string Answer);

public sealed record UpdateStudentAnswerRequest(
    [Required]
    [MaxLength(4000)]
    string Answer);

public sealed record UpdateStudentInfoRequest(
    [Required]
    [MaxLength(200)]
    string StudentName,
    [EmailAddress]
    [MaxLength(320)]
    string? StudentEmail);

public sealed record ReviewStudentAnswerRequest(
    decimal ScoreAwarded,
    string? Feedback,
    bool IsCorrect);

public sealed record ReviewStudentAnswerResponse(
    Guid AnswerId,
    Guid SubmissionId,
    decimal ScoreAwarded,
    string? Feedback,
    bool IsCorrect,
    bool NeedsReview,
    decimal FinalScore);

public sealed record CorrectionLogResponse(
    Guid Id,
    Guid SubmissionId,
    Guid QuestionId,
    string CorrectionType,
    string OriginalAnswer,
    string? ExpectedAnswer,
    decimal Score,
    string? Message,
    DateTime CreatedAt);

public sealed record SubmissionResponse(
    Guid Id,
    Guid AssignmentId,
    string StudentName,
    string? StudentEmail,
    SubmissionStatus Status,
    decimal FinalScore,
    DateTime SubmittedAt,
    IReadOnlyCollection<StudentAnswerResponse> Answers);

public sealed record StudentAnswerResponse(
    Guid Id,
    Guid QuestionId,
    string Answer,
    decimal ScoreAwarded,
    bool IsCorrect,
    string? Feedback,
    bool NeedsReview);

public sealed record CorrectionResponse(
    Guid SubmissionId,
    decimal FinalScore,
    decimal MaxScore,
    IReadOnlyCollection<StudentAnswerCorrectionResponse> Results);

public sealed record StudentAnswerCorrectionResponse(
    Guid QuestionId,
    string Answer,
    bool IsCorrect,
    decimal ScoreAwarded,
    string? Feedback,
    bool NeedsReview);
