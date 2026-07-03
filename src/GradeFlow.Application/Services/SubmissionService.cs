using System.ComponentModel.DataAnnotations;
using GradeFlow.Application.Corrections;
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

    public async Task<bool> UpdateAsync(
        Guid id,
        CreateSubmissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetForUpdateAsync(id, cancellationToken);
        if (submission is null) return false;

        var questions = await submissionRepository.GetAssignmentQuestionsAsync(submission.AssignmentId, cancellationToken);
        Validate(request, questions);

        submission.StudentName = request.StudentName.Trim();
        submission.StudentEmail = string.IsNullOrWhiteSpace(request.StudentEmail) ? null : request.StudentEmail.Trim();
        submission.Status = SubmissionStatus.Pending;
        submission.CorrectedAt = null;
        submission.ReviewedAt = null;
        submission.FinalScore = 0;

        await submissionRepository.ReplaceAnswersAsync(
            submission.Id,
            request.Answers.Select(answer => new StudentAnswer
            {
                Id = Guid.NewGuid(),
                SubmissionId = submission.Id,
                QuestionId = answer.QuestionId,
                Answer = answer.Answer.Trim()
            }),
            cancellationToken);

        await submissionRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateAnswerAsync(
        Guid submissionId,
        Guid questionId,
        UpdateStudentAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdAsync(submissionId, cancellationToken);
        if (submission is null) return false;

        var questions = await submissionRepository.GetAssignmentQuestionsAsync(submission.AssignmentId, cancellationToken);
        var question = questions.FirstOrDefault(x => x.Id == questionId);
        if (question is null) return false;

        ValidateAnswer(request.Answer, question);

        var answerText = request.Answer.Trim();
        var existingAnswer = await submissionRepository.GetAnswerAsync(submissionId, questionId, cancellationToken);
        if (existingAnswer is null)
        {
            submissionRepository.AddAnswer(new StudentAnswer
            {
                Id = Guid.NewGuid(),
                SubmissionId = submissionId,
                QuestionId = questionId,
                Answer = answerText
            });
            await submissionRepository.SaveChangesAsync(cancellationToken);
            await submissionRepository.RefreshSubmissionAfterAnswerUpdateAsync(submissionId, cancellationToken);
            return true;
        }

        if (existingAnswer.Answer != answerText)
        {
            await submissionRepository.UpdateAnswerAsync(existingAnswer.Id, answerText, cancellationToken);
            await submissionRepository.RefreshSubmissionAfterAnswerUpdateAsync(submissionId, cancellationToken);
        }

        return true;
    }

    public async Task<bool> UpdateStudentInfoAsync(
        Guid id,
        UpdateStudentInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.StudentName))
        {
            throw new ValidationException("StudentName is required.");
        }

        var submission = await submissionRepository.GetForUpdateAsync(id, cancellationToken);
        if (submission is null) return false;

        submission.StudentName = request.StudentName.Trim();
        submission.StudentEmail = string.IsNullOrWhiteSpace(request.StudentEmail) ? null : request.StudentEmail.Trim();

        await submissionRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetForUpdateAsync(id, cancellationToken);
        if (submission is null) return false;

        submissionRepository.Remove(submission);
        await submissionRepository.SaveChangesAsync(cancellationToken);
        return true;
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
        if (!questionsById.Keys.Order().SequenceEqual(request.Answers.Select(x => x.QuestionId).Order()))
        {
            throw new ValidationException("Every assignment question must be answered.");
        }

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

            ValidateAnswer(answer.Answer, question);
        }
    }

    private static void ValidateAnswer(string answer, Question question)
    {
        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new ValidationException("Answer is required.");
        }

        if (question.Type == QuestionType.Numeric
            && !NumberParser.TryParse(answer, out _))
        {
            throw new ValidationException("Numeric answers must be valid numbers.");
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
