using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using GradeFlow.Application.DTOs.Assignments;
using GradeFlow.Application.DTOs.Questions;
using GradeFlow.Application.DTOs.Submissions;
using GradeFlow.Application.Repositories;
using GradeFlow.Application.Services;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Tests;

public sealed class ApplicationServicesTests
{
    [Fact]
    public async Task Assignment_service_should_create_trim_and_save_assignment()
    {
        var repository = new FakeAssignmentRepository();
        var service = new AssignmentService(repository);

        var response = await service.CreateAsync(new CreateAssignmentRequest(" Prova ", " Desc ", " Mat "));

        response.Title.Should().Be("Prova");
        response.Description.Should().Be("Desc");
        response.Subject.Should().Be("Mat");
        response.Status.Should().Be(AssignmentStatus.Draft);
        repository.Assignments.Should().ContainSingle();
        repository.SaveChangesCount.Should().Be(1);
    }

    [Fact]
    public async Task Assignment_service_should_update_and_delete_existing_assignment()
    {
        var assignment = new Assignment { Id = Guid.NewGuid(), Title = "Old" };
        var repository = new FakeAssignmentRepository(assignment);
        var service = new AssignmentService(repository);

        var updated = await service.UpdateAsync(assignment.Id, new UpdateAssignmentRequest(" New ", null, null));
        var deleted = await service.DeleteAsync(assignment.Id);

        updated.Should().BeTrue();
        deleted.Should().BeTrue();
        assignment.Title.Should().Be("New");
        repository.Assignments.Should().BeEmpty();
        repository.SaveChangesCount.Should().Be(2);
    }

    [Fact]
    public async Task Assignment_service_should_list_get_and_return_false_for_missing_items()
    {
        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Title = "Prova",
            Questions = [new Question { Points = 2 }, new Question { Points = 3 }]
        };
        var repository = new FakeAssignmentRepository(assignment);
        var service = new AssignmentService(repository);

        var all = await service.GetAllAsync();
        var found = await service.GetByIdAsync(assignment.Id);
        var missing = await service.GetByIdAsync(Guid.NewGuid());
        var updated = await service.UpdateAsync(Guid.NewGuid(), new UpdateAssignmentRequest("X", null, null));
        var deleted = await service.DeleteAsync(Guid.NewGuid());

        all.Should().ContainSingle(x => x.TotalPoints == 5);
        found!.TotalPoints.Should().Be(5);
        missing.Should().BeNull();
        updated.Should().BeFalse();
        deleted.Should().BeFalse();
    }

    [Fact]
    public async Task Question_service_should_create_question_with_answer_key()
    {
        var assignmentId = Guid.NewGuid();
        var repository = new FakeQuestionRepository { ExistingAssignmentId = assignmentId };
        var service = new QuestionService(repository);

        var response = await service.CreateAsync(assignmentId, QuestionRequest(order: 1, points: 2));

        response.Should().NotBeNull();
        response!.Text.Should().Be("Quanto e 2 + 2?");
        response.AnswerKey!.CorrectAnswer.Should().Be("4");
        response.AnswerKey.AcceptedAnswers.Should().Contain("quatro");
        repository.Questions.Should().ContainSingle();
        repository.SaveChangesCount.Should().Be(1);
    }

    [Fact]
    public async Task Question_service_should_reject_duplicate_order()
    {
        var assignmentId = Guid.NewGuid();
        var repository = new FakeQuestionRepository
        {
            ExistingAssignmentId = assignmentId,
            ExistingOrder = 1
        };
        var service = new QuestionService(repository);

        var act = () => service.CreateAsync(assignmentId, QuestionRequest(order: 1, points: 2));

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Ordem já está sendo usada.");
    }

    [Fact]
    public async Task Question_service_should_update_delete_list_and_get_questions()
    {
        var assignmentId = Guid.NewGuid();
        var question = new Question
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            Text = "Old",
            Type = QuestionType.MultipleChoice,
            Points = 1,
            Order = 1
        };
        var repository = new FakeQuestionRepository { ExistingAssignmentId = assignmentId };
        repository.Questions.Add(question);
        var service = new QuestionService(repository);

        var listed = await service.GetByAssignmentIdAsync(assignmentId);
        var found = await service.GetByIdAsync(question.Id);
        var missing = await service.GetByIdAsync(Guid.NewGuid());
        var updated = await service.UpdateAsync(question.Id, UpdateQuestionRequest(order: 2, points: 3));
        var deleted = await service.DeleteAsync(question.Id);

        listed.Should().ContainSingle();
        found!.AnswerKey.Should().BeNull();
        missing.Should().BeNull();
        updated.Should().BeTrue();
        question.Text.Should().Be("Quanto e 2 + 2?");
        question.AnswerKey!.CorrectAnswer.Should().Be("4");
        deleted.Should().BeTrue();
        repository.Questions.Should().BeEmpty();
    }

    [Fact]
    public async Task Question_service_should_return_false_or_null_for_missing_assignment_or_question()
    {
        var service = new QuestionService(new FakeQuestionRepository());

        var created = await service.CreateAsync(Guid.NewGuid(), QuestionRequest(order: 1, points: 1));
        var updated = await service.UpdateAsync(Guid.NewGuid(), UpdateQuestionRequest(order: 1, points: 1));
        var deleted = await service.DeleteAsync(Guid.NewGuid());

        created.Should().BeNull();
        updated.Should().BeFalse();
        deleted.Should().BeFalse();
    }

    [Fact]
    public async Task Question_service_should_reject_points_over_limit()
    {
        var assignmentId = Guid.NewGuid();
        var service = new QuestionService(new FakeQuestionRepository { ExistingAssignmentId = assignmentId });

        var act = () => service.CreateAsync(assignmentId, QuestionRequest(order: 1, points: 11));

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("O máximo de pontos é 10.");
    }

    [Fact]
    public async Task Submission_service_should_create_submission_for_all_assignment_questions()
    {
        var assignmentId = Guid.NewGuid();
        var question = new Question { Id = Guid.NewGuid(), AssignmentId = assignmentId, Type = QuestionType.Numeric };
        var repository = new FakeSubmissionRepositoryForService
        {
            ExistingAssignmentId = assignmentId,
            Questions = [question]
        };
        var service = new SubmissionService(repository);

        var response = await service.CreateAsync(
            assignmentId,
            new CreateSubmissionRequest(" Ana ", " ana@email.com ", [new CreateStudentAnswerRequest(question.Id, " 10 ")]));

        response.Should().NotBeNull();
        response!.StudentName.Should().Be("Ana");
        response.Answers.Should().ContainSingle(x => x.Answer == "10");
        repository.Submissions.Should().ContainSingle();
        repository.SaveChangesCount.Should().Be(1);
    }

    [Fact]
    public async Task Submission_service_should_reject_missing_answer()
    {
        var assignmentId = Guid.NewGuid();
        var question = new Question { Id = Guid.NewGuid(), AssignmentId = assignmentId, Type = QuestionType.MultipleChoice };
        var repository = new FakeSubmissionRepositoryForService
        {
            ExistingAssignmentId = assignmentId,
            Questions = [question]
        };
        var service = new SubmissionService(repository);

        var act = () => service.CreateAsync(assignmentId, new CreateSubmissionRequest("Ana", null, []));

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("At least one answer is required.");
    }

    [Fact]
    public async Task Submission_service_should_add_or_update_single_answer()
    {
        var assignmentId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();
        var question = new Question { Id = Guid.NewGuid(), AssignmentId = assignmentId, Type = QuestionType.MultipleChoice };
        var submission = new Submission { Id = submissionId, AssignmentId = assignmentId, StudentName = "Ana" };
        var repository = new FakeSubmissionRepositoryForService
        {
            ExistingAssignmentId = assignmentId,
            Questions = [question],
            Submissions = [submission]
        };
        var service = new SubmissionService(repository);

        var added = await service.UpdateAnswerAsync(submissionId, question.Id, new UpdateStudentAnswerRequest(" A "));
        var updated = await service.UpdateAnswerAsync(submissionId, question.Id, new UpdateStudentAnswerRequest(" B "));

        added.Should().BeTrue();
        updated.Should().BeTrue();
        submission.StudentAnswers.Should().ContainSingle(x => x.Answer == "B");
        repository.RefreshCount.Should().Be(2);
    }

    [Fact]
    public async Task Submission_service_should_list_get_update_student_info_and_delete()
    {
        var assignmentId = Guid.NewGuid();
        var question = new Question { Id = Guid.NewGuid(), AssignmentId = assignmentId, Type = QuestionType.MultipleChoice };
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentName = "Ana",
            StudentEmail = "old@email.com",
            Status = SubmissionStatus.Corrected,
            FinalScore = 10,
            CorrectedAt = DateTime.UtcNow,
            ReviewedAt = DateTime.UtcNow,
            StudentAnswers = [new StudentAnswer { Id = Guid.NewGuid(), QuestionId = question.Id, Answer = "A" }]
        };
        var repository = new FakeSubmissionRepositoryForService
        {
            ExistingAssignmentId = assignmentId,
            Questions = [question],
            Submissions = [submission]
        };
        var service = new SubmissionService(repository);

        var listed = await service.GetByAssignmentIdAsync(assignmentId);
        var missingList = await service.GetByAssignmentIdAsync(Guid.NewGuid());
        var found = await service.GetByIdAsync(submission.Id);
        var missing = await service.GetByIdAsync(Guid.NewGuid());
        var updated = await service.UpdateAsync(
            submission.Id,
            new CreateSubmissionRequest(" Bia ", null, [new CreateStudentAnswerRequest(question.Id, " B ")]));
        var infoUpdated = await service.UpdateStudentInfoAsync(submission.Id, new UpdateStudentInfoRequest(" Carla ", " carla@email.com "));
        var deleted = await service.DeleteAsync(submission.Id);

        listed.Should().ContainSingle();
        missingList.Should().BeNull();
        found!.StudentName.Should().Be("Ana");
        missing.Should().BeNull();
        updated.Should().BeTrue();
        submission.Status.Should().Be(SubmissionStatus.Pending);
        submission.FinalScore.Should().Be(0);
        infoUpdated.Should().BeTrue();
        submission.StudentName.Should().Be("Carla");
        deleted.Should().BeTrue();
        repository.Submissions.Should().BeEmpty();
    }

    [Fact]
    public async Task Submission_service_should_return_null_or_false_for_missing_entities()
    {
        var repository = new FakeSubmissionRepositoryForService();
        var service = new SubmissionService(repository);

        var created = await service.CreateAsync(Guid.NewGuid(), new CreateSubmissionRequest("Ana", null, []));
        var updated = await service.UpdateAsync(Guid.NewGuid(), new CreateSubmissionRequest("Ana", null, []));
        var answerUpdated = await service.UpdateAnswerAsync(Guid.NewGuid(), Guid.NewGuid(), new UpdateStudentAnswerRequest("A"));
        var infoUpdated = await service.UpdateStudentInfoAsync(Guid.NewGuid(), new UpdateStudentInfoRequest("Ana", null));
        var deleted = await service.DeleteAsync(Guid.NewGuid());

        created.Should().BeNull();
        updated.Should().BeFalse();
        answerUpdated.Should().BeFalse();
        infoUpdated.Should().BeFalse();
        deleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Submission_service_should_reject_invalid_student_name(string studentName)
    {
        var assignmentId = Guid.NewGuid();
        var question = new Question { Id = Guid.NewGuid(), AssignmentId = assignmentId, Type = QuestionType.MultipleChoice };
        var service = new SubmissionService(new FakeSubmissionRepositoryForService
        {
            ExistingAssignmentId = assignmentId,
            Questions = [question]
        });

        var act = () => service.CreateAsync(
            assignmentId,
            new CreateSubmissionRequest(studentName, null, [new CreateStudentAnswerRequest(question.Id, "A")]));

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("StudentName is required.");
    }

    [Fact]
    public async Task Submission_service_should_reject_duplicate_unmatched_blank_and_invalid_numeric_answers()
    {
        var assignmentId = Guid.NewGuid();
        var firstQuestion = new Question { Id = Guid.NewGuid(), AssignmentId = assignmentId, Type = QuestionType.MultipleChoice };
        var secondQuestion = new Question { Id = Guid.NewGuid(), AssignmentId = assignmentId, Type = QuestionType.Numeric };
        var service = new SubmissionService(new FakeSubmissionRepositoryForService
        {
            ExistingAssignmentId = assignmentId,
            Questions = [firstQuestion, secondQuestion]
        });

        var duplicate = () => service.CreateAsync(assignmentId, new CreateSubmissionRequest("Ana", null,
            [new CreateStudentAnswerRequest(firstQuestion.Id, "A"), new CreateStudentAnswerRequest(firstQuestion.Id, "B")]));
        var missingQuestion = () => service.CreateAsync(assignmentId, new CreateSubmissionRequest("Ana", null,
            [new CreateStudentAnswerRequest(firstQuestion.Id, "A")]));
        var blankAnswer = () => service.CreateAsync(assignmentId, new CreateSubmissionRequest("Ana", null,
            [new CreateStudentAnswerRequest(firstQuestion.Id, " "), new CreateStudentAnswerRequest(secondQuestion.Id, "1")]));
        var invalidNumber = () => service.CreateAsync(assignmentId, new CreateSubmissionRequest("Ana", null,
            [new CreateStudentAnswerRequest(firstQuestion.Id, "A"), new CreateStudentAnswerRequest(secondQuestion.Id, "dez")]));

        await duplicate.Should().ThrowAsync<ValidationException>().WithMessage("Duplicate QuestionId is not allowed.");
        await missingQuestion.Should().ThrowAsync<ValidationException>().WithMessage("Every assignment question must be answered.");
        await blankAnswer.Should().ThrowAsync<ValidationException>().WithMessage("Answer is required.");
        await invalidNumber.Should().ThrowAsync<ValidationException>().WithMessage("Numeric answers must be valid numbers.");
    }

    [Fact]
    public async Task Submission_service_should_review_answer_recalculate_score_and_log_change()
    {
        var assignmentId = Guid.NewGuid();
        var question = new Question
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            Type = QuestionType.ShortText,
            Points = 5,
            AnswerKey = new AnswerKey { CorrectAnswer = "GradeFlow" }
        };
        var answer = new StudentAnswer
        {
            Id = Guid.NewGuid(),
            QuestionId = question.Id,
            Question = question,
            Answer = "grade flow",
            ScoreAwarded = 0,
            NeedsReview = true
        };
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentName = "Ana",
            StudentAnswers = [answer]
        };
        answer.SubmissionId = submission.Id;
        answer.Submission = submission;
        var repository = new FakeSubmissionRepositoryForService { Submissions = [submission] };
        var service = new SubmissionService(repository);

        var response = await service.ReviewAnswerAsync(
            answer.Id,
            new ReviewStudentAnswerRequest(4, "Revisado", true));

        response.Should().NotBeNull();
        response!.FinalScore.Should().Be(4);
        response.NeedsReview.Should().BeFalse();
        submission.Status.Should().Be(SubmissionStatus.Reviewed);
        repository.CorrectionLogs.Should().ContainSingle(x => x.CorrectionType == "ManualReview");
    }

    [Fact]
    public async Task Submission_service_should_list_correction_logs()
    {
        var submission = new Submission { Id = Guid.NewGuid(), AssignmentId = Guid.NewGuid(), StudentName = "Ana" };
        var repository = new FakeSubmissionRepositoryForService
        {
            Submissions = [submission]
        };
        repository.CorrectionLogs.Add(new CorrectionLog
        {
            Id = Guid.NewGuid(),
            SubmissionId = submission.Id,
            QuestionId = Guid.NewGuid(),
            CorrectionType = "ManualReview",
            OriginalAnswer = "A",
            ExpectedAnswer = "B",
            Score = 1,
            Message = "Ajuste",
            CreatedAt = DateTime.UtcNow
        });
        var service = new SubmissionService(repository);

        var logs = await service.GetCorrectionLogsAsync(submission.Id);
        var missing = await service.GetCorrectionLogsAsync(Guid.NewGuid());

        logs.Should().ContainSingle(x => x.Message == "Ajuste");
        missing.Should().BeNull();
    }

    private static CreateQuestionRequest QuestionRequest(int order, decimal points)
        => new(
            " Quanto e 2 + 2? ",
            QuestionType.Numeric,
            points,
            order,
            new CreateAnswerKeyRequest(" 4 ", ["quatro"], null, 0.1m, " Certo ", " Errado "));

    private static UpdateQuestionRequest UpdateQuestionRequest(int order, decimal points)
        => new(
            " Quanto e 2 + 2? ",
            QuestionType.Numeric,
            points,
            order,
            new CreateAnswerKeyRequest(" 4 ", ["quatro"], null, 0.1m, " Certo ", " Errado "));

    private sealed class FakeAssignmentRepository(params Assignment[] assignments) : IAssignmentRepository
    {
        public List<Assignment> Assignments { get; } = assignments.ToList();
        public int SaveChangesCount { get; private set; }

        public Task<IReadOnlyCollection<Assignment>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Assignment>>(Assignments);

        public Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Assignments.FirstOrDefault(x => x.Id == id));

        public Task<Assignment?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
            => GetByIdAsync(id, cancellationToken);

        public void Add(Assignment assignment)
            => Assignments.Add(assignment);

        public void Remove(Assignment assignment)
            => Assignments.Remove(assignment);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCount++;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeQuestionRepository : IQuestionRepository
    {
        public Guid ExistingAssignmentId { get; init; }
        public int? ExistingOrder { get; init; }
        public List<Question> Questions { get; } = [];
        public int SaveChangesCount { get; private set; }

        public Task<bool> AssignmentExistsAsync(Guid assignmentId, CancellationToken cancellationToken = default)
            => Task.FromResult(assignmentId == ExistingAssignmentId);

        public Task<IReadOnlyCollection<Question>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Question>>(Questions.Where(x => x.AssignmentId == assignmentId).ToList());

        public Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Questions.FirstOrDefault(x => x.Id == id));

        public Task<Question?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
            => GetByIdAsync(id, cancellationToken);

        public Task<bool> HasAnswersAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<bool> OrderExistsAsync(Guid assignmentId, int order, Guid? exceptQuestionId = null, CancellationToken cancellationToken = default)
            => Task.FromResult(ExistingOrder == order || Questions.Any(x => x.AssignmentId == assignmentId && x.Order == order && x.Id != exceptQuestionId));

        public void Add(Question question)
        {
            question.AnswerKey!.QuestionId = question.Id;
            Questions.Add(question);
        }

        public void Remove(Question question)
            => Questions.Remove(question);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCount++;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeSubmissionRepositoryForService : ISubmissionRepository
    {
        public Guid ExistingAssignmentId { get; init; }
        public List<Question> Questions { get; init; } = [];
        public List<Submission> Submissions { get; init; } = [];
        public int SaveChangesCount { get; private set; }
        public int RefreshCount { get; private set; }
        public List<CorrectionLog> CorrectionLogs { get; } = [];

        public Task<bool> AssignmentExistsAsync(Guid assignmentId, CancellationToken cancellationToken = default)
            => Task.FromResult(assignmentId == ExistingAssignmentId);

        public Task<IReadOnlyCollection<Question>> GetAssignmentQuestionsAsync(Guid assignmentId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Question>>(Questions.Where(x => x.AssignmentId == assignmentId).ToList());

        public Task<IReadOnlyCollection<Submission>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Submission>>(Submissions.Where(x => x.AssignmentId == assignmentId).ToList());

        public Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Submissions.FirstOrDefault(x => x.Id == id));

        public Task<Submission?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
            => GetByIdAsync(id, cancellationToken);

        public Task<Submission?> GetForCorrectionAsync(Guid id, CancellationToken cancellationToken = default)
            => GetByIdAsync(id, cancellationToken);

        public Task<StudentAnswer?> GetAnswerAsync(Guid submissionId, Guid questionId, CancellationToken cancellationToken = default)
            => Task.FromResult(Submissions.FirstOrDefault(x => x.Id == submissionId)?.StudentAnswers.FirstOrDefault(x => x.QuestionId == questionId));

        public Task<StudentAnswer?> GetAnswerForReviewAsync(Guid answerId, CancellationToken cancellationToken = default)
            => Task.FromResult(Submissions.SelectMany(x => x.StudentAnswers).FirstOrDefault(x => x.Id == answerId));

        public Task<IReadOnlyCollection<CorrectionLog>> GetCorrectionLogsAsync(Guid submissionId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<CorrectionLog>>(CorrectionLogs.Where(x => x.SubmissionId == submissionId).ToList());

        public Task ReplaceAnswersAsync(Guid submissionId, IEnumerable<StudentAnswer> answers, CancellationToken cancellationToken = default)
        {
            var submission = Submissions.Single(x => x.Id == submissionId);
            submission.StudentAnswers = answers.ToList();
            return Task.CompletedTask;
        }

        public Task<int> UpdateAnswerAsync(Guid answerId, string answer, CancellationToken cancellationToken = default)
        {
            var studentAnswer = Submissions.SelectMany(x => x.StudentAnswers).FirstOrDefault(x => x.Id == answerId);
            if (studentAnswer is null) return Task.FromResult(0);

            studentAnswer.Answer = answer;
            return Task.FromResult(1);
        }

        public Task RefreshSubmissionAfterAnswerUpdateAsync(Guid submissionId, CancellationToken cancellationToken = default)
        {
            RefreshCount++;
            return Task.CompletedTask;
        }

        public void Add(Submission submission)
            => Submissions.Add(submission);

        public void AddAnswer(StudentAnswer answer)
            => Submissions.Single(x => x.Id == answer.SubmissionId).StudentAnswers.Add(answer);

        public void Remove(Submission submission)
            => Submissions.Remove(submission);

        public void AddCorrectionResult(CorrectionResult correctionResult)
        {
        }

        public void AddCorrectionLog(CorrectionLog correctionLog)
            => CorrectionLogs.Add(correctionLog);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCount++;
            return Task.CompletedTask;
        }
    }
}
