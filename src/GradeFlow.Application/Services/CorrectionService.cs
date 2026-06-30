using System.ComponentModel.DataAnnotations;
using GradeFlow.Application.DTOs.Submissions;
using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Corrections;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Application.Services;

public interface ICorrectionService
{
    Task<CorrectionResponse?> CorrectAsync(Guid submissionId, CancellationToken cancellationToken = default);
}

public sealed class CorrectionService(
    ISubmissionRepository submissionRepository,
    IEnumerable<ICorrectionStrategy> strategies) : ICorrectionService
{
    public async Task<CorrectionResponse?> CorrectAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetForCorrectionAsync(submissionId, cancellationToken);
        if (submission is null) return null;

        foreach (var answer in submission.StudentAnswers)
        {
            if (answer.Question?.AnswerKey is null)
            {
                throw new ValidationException("Every answered question must have an answer key.");
            }

            var strategy = strategies.FirstOrDefault(x => x.QuestionType == answer.Question.Type)
                ?? throw new ValidationException($"No correction strategy for question type {answer.Question.Type}.");

            Apply(answer, strategy.Correct(answer.Question, answer.Question.AnswerKey, answer));
        }

        submission.FinalScore = submission.StudentAnswers.Sum(x => x.ScoreAwarded);
        submission.Status = SubmissionStatus.Corrected;
        submission.CorrectedAt = DateTime.UtcNow;

        await submissionRepository.SaveChangesAsync(cancellationToken);
        return Map(submission);
    }

    private static void Apply(StudentAnswer answer, CorrectionOutcome outcome)
    {
        answer.IsCorrect = outcome.IsCorrect;
        answer.ScoreAwarded = outcome.ScoreAwarded;
        answer.Feedback = outcome.Feedback;
        answer.NeedsReview = outcome.NeedsReview;

        answer.CorrectionResult ??= new CorrectionResult
        {
            Id = Guid.NewGuid(),
            StudentAnswerId = answer.Id
        };

        answer.CorrectionResult.IsCorrect = outcome.IsCorrect;
        answer.CorrectionResult.ScoreAwarded = outcome.ScoreAwarded;
        answer.CorrectionResult.Feedback = outcome.Feedback;
        answer.CorrectionResult.CorrectionType = "Automatic";
        answer.CorrectionResult.CreatedAt = DateTime.UtcNow;
    }

    private static CorrectionResponse Map(Submission submission)
        => new(
            submission.Id,
            submission.FinalScore,
            submission.Assignment?.Questions.Sum(x => x.Points) ?? submission.StudentAnswers.Sum(x => x.Question?.Points ?? 0),
            submission.StudentAnswers
                .OrderBy(x => x.Question?.Order)
                .Select(x => new StudentAnswerCorrectionResponse(
                    x.QuestionId,
                    x.Answer,
                    x.IsCorrect,
                    x.ScoreAwarded,
                    x.Feedback,
                    x.NeedsReview))
                .ToList());
}
