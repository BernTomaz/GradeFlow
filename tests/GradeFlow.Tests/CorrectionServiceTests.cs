using System.ComponentModel.DataAnnotations;
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

    [Fact]
    public async Task Correct_question_async_should_correct_one_answer_and_keep_submission_pending_when_others_are_open()
    {
        var assignmentId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();
        var firstQuestion = Question(assignmentId, QuestionType.MultipleChoice, "A", points: 2, order: 1);
        var secondQuestion = Question(assignmentId, QuestionType.TrueFalse, "false", points: 3, order: 2);
        var submission = Submission(submissionId, assignmentId, firstQuestion, secondQuestion);
        var repository = new FakeSubmissionRepository(submission);
        var service = new CorrectionService(
            repository,
            [new MultipleChoiceCorrectionStrategy(), new TrueFalseCorrectionStrategy()]);

        var response = await service.CorrectQuestionAsync(submissionId, firstQuestion.Id);

        response.Should().NotBeNull();
        response!.Results.Should().ContainSingle();
        response.Results.Single().QuestionId.Should().Be(firstQuestion.Id);
        response.FinalScore.Should().Be(2);
        submission.Status.Should().Be(SubmissionStatus.Pending);
        submission.StudentAnswers.Single(x => x.QuestionId == secondQuestion.Id).Feedback.Should().BeNull();
    }

    [Fact]
    public async Task Correct_async_should_return_null_when_submission_does_not_exist()
    {
        var submission = Submission(Guid.NewGuid(), Guid.NewGuid(), Question(Guid.NewGuid(), QuestionType.MultipleChoice, "A", 1, 1));
        var service = new CorrectionService(new FakeSubmissionRepository(submission), [new MultipleChoiceCorrectionStrategy()]);

        var response = await service.CorrectAsync(Guid.NewGuid());

        response.Should().BeNull();
    }

    [Fact]
    public async Task Correct_async_should_fail_when_answer_has_no_answer_key()
    {
        var assignmentId = Guid.NewGuid();
        var question = Question(assignmentId, QuestionType.MultipleChoice, "A", 1, 1);
        question.AnswerKey = null;
        var submission = Submission(Guid.NewGuid(), assignmentId, question);
        var service = new CorrectionService(new FakeSubmissionRepository(submission), [new MultipleChoiceCorrectionStrategy()]);

        var act = () => service.CorrectAsync(submission.Id);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Every answered question must have an answer key.");
    }

    [Fact]
    public async Task Correct_async_should_fail_when_strategy_is_missing()
    {
        var assignmentId = Guid.NewGuid();
        var question = Question(assignmentId, QuestionType.Numeric, "10", 1, 1);
        var submission = Submission(Guid.NewGuid(), assignmentId, question);
        var service = new CorrectionService(new FakeSubmissionRepository(submission), []);

        var act = () => service.CorrectAsync(submission.Id);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("No correction strategy for question type Numeric.");
    }

    [Fact]
    public async Task Correct_question_async_should_fail_when_question_was_not_answered()
    {
        var assignmentId = Guid.NewGuid();
        var question = Question(assignmentId, QuestionType.MultipleChoice, "A", 1, 1);
        var submission = Submission(Guid.NewGuid(), assignmentId, question);
        var service = new CorrectionService(new FakeSubmissionRepository(submission), [new MultipleChoiceCorrectionStrategy()]);

        var act = () => service.CorrectQuestionAsync(submission.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Question must be answered before correction.");
    }

    [Fact]
    public async Task Correct_question_async_should_fail_when_answer_has_no_answer_key()
    {
        var assignmentId = Guid.NewGuid();
        var question = Question(assignmentId, QuestionType.MultipleChoice, "A", 1, 1);
        var submission = Submission(Guid.NewGuid(), assignmentId, question);
        submission.StudentAnswers.Single().Question!.AnswerKey = null;
        var service = new CorrectionService(new FakeSubmissionRepository(submission), [new MultipleChoiceCorrectionStrategy()]);

        var act = () => service.CorrectQuestionAsync(submission.Id, question.Id);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Every answered question must have an answer key.");
    }

    [Fact]
    public async Task Correct_question_async_should_return_null_when_submission_does_not_exist()
    {
        var submission = Submission(Guid.NewGuid(), Guid.NewGuid(), Question(Guid.NewGuid(), QuestionType.MultipleChoice, "A", 1, 1));
        var service = new CorrectionService(new FakeSubmissionRepository(submission), [new MultipleChoiceCorrectionStrategy()]);

        var response = await service.CorrectQuestionAsync(Guid.NewGuid(), Guid.NewGuid());

        response.Should().BeNull();
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

    private static Submission Submission(Guid submissionId, Guid assignmentId, params Question[] questions)
    {
        var assignment = new Assignment
        {
            Id = assignmentId,
            Questions = questions.ToList()
        };

        return new Submission
        {
            Id = submissionId,
            AssignmentId = assignmentId,
            Assignment = assignment,
            Status = SubmissionStatus.Pending,
            StudentAnswers = questions
                .Select(question => StudentAnswer(submissionId, question, question.AnswerKey?.CorrectAnswer ?? "A"))
                .ToList()
        };
    }

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
