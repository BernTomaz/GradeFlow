using FluentAssertions;
using GradeFlow.Application.Corrections;
using GradeFlow.Application.Repositories;
using GradeFlow.Application.Services;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Tests;

public sealed class CorrectionServiceTests
{
    [Fact]
    public async Task Correct_async_should_correct_answers_sum_score_save_result_and_update_status()
    {
        var assignmentId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();
        var firstQuestion = Question(assignmentId, QuestionType.MultipleChoice, "A", points: 2, order: 1);
        var secondQuestion = Question(assignmentId, QuestionType.TrueFalse, "false", points: 3, order: 2);
        var assignment = new Assignment
        {
            Id = assignmentId,
            Questions = [firstQuestion, secondQuestion]
        };
        var submission = new Submission
        {
            Id = submissionId,
            AssignmentId = assignmentId,
            Assignment = assignment,
            Status = SubmissionStatus.Pending,
            StudentAnswers =
            [
                StudentAnswer(submissionId, firstQuestion, "a"),
                StudentAnswer(submissionId, secondQuestion, "true")
            ]
        };
        var repository = new FakeSubmissionRepository(submission);
        var service = new CorrectionService(
            repository,
            [new MultipleChoiceCorrectionStrategy(), new TrueFalseCorrectionStrategy()]);

        var response = await service.CorrectAsync(submissionId);

        response.Should().NotBeNull();
        response!.FinalScore.Should().Be(2);
        submission.FinalScore.Should().Be(2);
        submission.Status.Should().Be(SubmissionStatus.Corrected);
        submission.StudentAnswers.Should().OnlyContain(x => x.CorrectionResult != null);
        repository.AddedCorrectionResults.Should().HaveCount(2);
        repository.SaveChangesCount.Should().Be(1);
    }

    private static Question Question(
        Guid assignmentId,
        QuestionType type,
        string correctAnswer,
        decimal points,
        int order)
    {
        var question = new Question
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            Type = type,
            Points = points,
            Order = order
        };
        question.AnswerKey = new AnswerKey
        {
            Id = Guid.NewGuid(),
            QuestionId = question.Id,
            Question = question,
            CorrectAnswer = correctAnswer
        };
        return question;
    }

    private static StudentAnswer StudentAnswer(Guid submissionId, Question question, string answer)
        => new()
        {
            Id = Guid.NewGuid(),
            SubmissionId = submissionId,
            QuestionId = question.Id,
            Question = question,
            Answer = answer
        };

    private sealed class FakeSubmissionRepository(Submission submission) : ISubmissionRepository
    {
        public List<CorrectionResult> AddedCorrectionResults { get; } = [];
        public int SaveChangesCount { get; private set; }

        public Task<bool> AssignmentExistsAsync(Guid assignmentId, CancellationToken cancellationToken = default)
            => Task.FromResult(submission.AssignmentId == assignmentId);

        public Task<IReadOnlyCollection<Question>> GetAssignmentQuestionsAsync(Guid assignmentId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Question>>(submission.Assignment?.Questions.ToList() ?? []);

        public Task<IReadOnlyCollection<Submission>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Submission>>([submission]);

        public Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(id == submission.Id ? submission : null);

        public Task<Submission?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
            => GetByIdAsync(id, cancellationToken);

        public Task<Submission?> GetForCorrectionAsync(Guid id, CancellationToken cancellationToken = default)
            => GetByIdAsync(id, cancellationToken);

        public Task<StudentAnswer?> GetAnswerAsync(Guid submissionId, Guid questionId, CancellationToken cancellationToken = default)
            => Task.FromResult(submission.StudentAnswers.FirstOrDefault(x => x.SubmissionId == submissionId && x.QuestionId == questionId));

        public Task ReplaceAnswersAsync(Guid submissionId, IEnumerable<StudentAnswer> answers, CancellationToken cancellationToken = default)
        {
            submission.StudentAnswers = answers.ToList();
            return Task.CompletedTask;
        }

        public Task<int> UpdateAnswerAsync(Guid answerId, string answer, CancellationToken cancellationToken = default)
        {
            var studentAnswer = submission.StudentAnswers.FirstOrDefault(x => x.Id == answerId);
            if (studentAnswer is null) return Task.FromResult(0);

            studentAnswer.Answer = answer;
            return Task.FromResult(1);
        }

        public Task RefreshSubmissionAfterAnswerUpdateAsync(Guid submissionId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public void Add(Submission submission)
        {
        }

        public void AddAnswer(StudentAnswer answer)
            => submission.StudentAnswers.Add(answer);

        public void Remove(Submission submission)
        {
        }

        public void AddCorrectionResult(CorrectionResult correctionResult)
            => AddedCorrectionResults.Add(correctionResult);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCount++;
            return Task.CompletedTask;
        }
    }
}
