using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GradeFlow.Application.DTOs.Submissions;
using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Application.Services;

public sealed class SubmissionService(ISubmissionRepository submissionRepository) : ISubmissionService
{
    public async Task<IReadOnlyCollection<SubmissionResponse>?> GetByAssignmentIdAsync(
        Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        if (!await submissionRepository.AssignmentExistsAsync(assignmentId, cancellationToken)) return null;

        return (await submissionRepository.GetByAssignmentIdAsync(assignmentId, cancellationToken)).Select(Map).ToList();
    }

    public async Task<SubmissionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdAsync(id, cancellationToken);
        return submission is null ? null : Map(submission);
    }

    public async Task<SubmissionResponse?> CreateAsync(
        Guid assignmentId,
        CreateSubmissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var questions = await submissionRepository.GetAssignmentQuestionsAsync(assignmentId, cancellationToken);
        if (questions.Count == 0 && !await submissionRepository.AssignmentExistsAsync(assignmentId, cancellationToken))
        {
            return null;
        }

        Validate(request, questions);

        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentName = request.StudentName.Trim(),
            StudentEmail = string.IsNullOrWhiteSpace(request.StudentEmail) ? null : request.StudentEmail.Trim(),
            Status = SubmissionStatus.Pending,
            SubmittedAt = DateTime.UtcNow,
            StudentAnswers = request.Answers.Select(answer => new StudentAnswer
            {
                Id = Guid.NewGuid(),
                QuestionId = answer.QuestionId,
                Answer = answer.Answer.Trim()
            }).ToList()
        };

        submissionRepository.Add(submission);
        await submissionRepository.SaveChangesAsync(cancellationToken);
        return Map(submission);
    }

    private static void Validate(CreateSubmissionRequest request, IReadOnlyCollection<Question> questions)
    {
        if (string.IsNullOrWhiteSpace(request.StudentName))
        {
            throw new ValidationException("StudentName is required.");
        }

        if (request.Answers.Count == 0)
        {
            throw new ValidationException("At least one answer is required.");
        }

        if (request.Answers.Select(x => x.QuestionId).Distinct().Count() != request.Answers.Count)
        {
            throw new ValidationException("Duplicate QuestionId is not allowed.");
        }

        var questionsById = questions.ToDictionary(x => x.Id);
        foreach (var answer in request.Answers)
        {
            if (!questionsById.TryGetValue(answer.QuestionId, out var question))
            {
                throw new ValidationException("QuestionId must belong to the assignment.");
            }

            if (string.IsNullOrWhiteSpace(answer.Answer))
            {
                throw new ValidationException("Answer is required.");
            }

            if (question.Type == QuestionType.Numeric
                && !decimal.TryParse(answer.Answer, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
            {
                throw new ValidationException("Numeric answers must be valid numbers.");
            }
        }
    }

    private static SubmissionResponse Map(Submission submission)
        => new(
            submission.Id,
            submission.AssignmentId,
            submission.StudentName,
            submission.StudentEmail,
            submission.Status,
            submission.FinalScore,
            submission.SubmittedAt,
            submission.StudentAnswers.Select(answer => new StudentAnswerResponse(
                answer.Id,
                answer.QuestionId,
                answer.Answer,
                answer.ScoreAwarded,
                answer.IsCorrect,
                answer.Feedback,
                answer.NeedsReview)).ToList());
}
