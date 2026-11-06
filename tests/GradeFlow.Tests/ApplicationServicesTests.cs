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
    [Theory]
    [InlineData(UserRole.Admin, false, true)]
    [InlineData(UserRole.Teacher, false, false)]
    [InlineData(UserRole.Teacher, true, true)]
    [InlineData(UserRole.Student, false, false)]
    public async Task Assignment_access_should_require_teacher_ownership(UserRole role, bool owned, bool allowed)
    {
        var userId = Guid.NewGuid();
        var assignment = new Assignment { Id = Guid.NewGuid(), Title = "Preservada", Subject = "QA", TeacherUserId = owned ? userId : null };
        var repository = new FakeAssignmentRepository(assignment);
        var user = new FakeCurrentUser(userId, role);
        var service = new AssignmentService(repository, user);
        var question = new Question { Id = Guid.NewGuid(), AssignmentId = assignment.Id };
        var questions = new FakeQuestionRepository();
        questions.Questions.Add(question);
        var questionService = new QuestionService(questions, repository, user);
        var submission = new Submission { Id = Guid.NewGuid(), AssignmentId = assignment.Id, Assignment = assignment };
        var submissions = new FakeSubmissionRepositoryForService { Submissions = [submission] };
        var submissionService = new SubmissionService(submissions, repository, user);
        var correctionService = new CorrectionService(submissions, [], user);

        (await service.GetByIdAsync(assignment.Id) is not null).Should().Be(allowed || role == UserRole.Student);
        (await service.GetAllAsync()).Count.Should().Be(allowed || role == UserRole.Student ? 1 : 0);
        (await service.UpdateAsync(assignment.Id, new UpdateAssignmentRequest("Preservada", null, "QA"))).Should().Be(allowed);
        (await questionService.GetByIdAsync(question.Id) is not null).Should().Be(allowed || role == UserRole.Student);
        if (role != UserRole.Student)
        {
            (await submissionService.GetByIdAsync(submission.Id) is not null).Should().Be(allowed);
            (await correctionService.CorrectAsync(submission.Id) is not null).Should().Be(allowed);
        }
        if (!allowed)
        {
            (await service.DeleteAsync(assignment.Id)).Should().BeFalse();
            (await questionService.DeleteAsync(question.Id)).Should().BeFalse();
            (await submissionService.DeleteAsync(submission.Id)).Should().BeFalse();
        }
        repository.Assignments.Should().ContainSingle();
        assignment.TeacherUserId.Should().Be(owned ? userId : null);
    }

    [Fact]
    public async Task Student_should_read_questions_without_keys_but_not_modify_them()
    {
        var assignment = new Assignment { Id = Guid.NewGuid() };
        var question = new Question { Id = Guid.NewGuid(), AssignmentId = assignment.Id,
            AnswerKey = new AnswerKey { CorrectAnswer = "secret" } };
        var repository = new FakeQuestionRepository();
        repository.Questions.Add(question);
        var service = new QuestionService(repository, new FakeAssignmentRepository(assignment),
            new FakeCurrentUser(Guid.NewGuid(), UserRole.Student));
        (await service.GetByIdAsync(question.Id))!.AnswerKey.Should().BeNull();
        (await service.GetByAssignmentIdAsync(assignment.Id)).Should().ContainSingle(x => x.AnswerKey == null);
        (await service.CreateAsync(assignment.Id, QuestionRequest(1, 1))).Should().BeNull();
        (await service.UpdateAsync(question.Id, UpdateQuestionRequest(1, 1))).Should().BeFalse();
        (await service.DeleteAsync(question.Id)).Should().BeFalse();
        question.AnswerKey.CorrectAnswer.Should().Be("secret");
    }

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
        var assignment = new Assignment { Id = Guid.NewGuid(), Title = "Old", Subject = "Old" };
        var repository = new FakeAssignmentRepository(assignment);
        var service = new AssignmentService(repository);

        var updated = await service.UpdateAsync(assignment.Id, new UpdateAssignmentRequest(" New ", null, " Mat "));
        var deleted = await service.DeleteAsync(assignment.Id);

        updated.Should().BeTrue();
        deleted.Should().BeTrue();
        assignment.Title.Should().Be("New");
        assignment.Subject.Should().Be("Mat");
        repository.Assignments.Should().BeEmpty();
        repository.SaveChangesCount.Should().Be(2);
    }

    [Fact]
    public async Task Assignment_service_should_require_title_and_subject()
    {
        var service = new AssignmentService(new FakeAssignmentRepository());

        var missingTitle = () => service.CreateAsync(new CreateAssignmentRequest(" ", null, "Mat"));
        var missingSubject = () => service.CreateAsync(new CreateAssignmentRequest("Prova", null, " "));

        await missingTitle.Should().ThrowAsync<ValidationException>()
            .WithMessage("Titulo e obrigatorio.");
        await missingSubject.Should().ThrowAsync<ValidationException>()
            .WithMessage("Disciplina e obrigatoria.");
    }

    [Fact]
    public async Task Assignment_service_should_reject_duplicate_title()
    {
        var existing = new Assignment { Id = Guid.NewGuid(), Title = "Prova", Subject = "Mat" };
        var other = new Assignment { Id = Guid.NewGuid(), Title = "Outra", Subject = "Mat" };
        var service = new AssignmentService(new FakeAssignmentRepository(existing, other));

        var create = () => service.CreateAsync(new CreateAssignmentRequest(" prova ", null, "Mat"));
        var update = () => service.UpdateAsync(other.Id, new UpdateAssignmentRequest(" PROVA ", null, "Mat"));

        await create.Should().ThrowAsync<ValidationException>()
            .WithMessage("Esse nome já está sendo usado.");
        await update.Should().ThrowAsync<ValidationException>()
            .WithMessage("Esse nome já está sendo usado.");
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
        var updated = await service.UpdateAsync(Guid.NewGuid(), new UpdateAssignmentRequest("X", null, "Mat"));
        var deleted = await service.DeleteAsync(Guid.NewGuid());

        all.Should().ContainSingle(x => x.TotalPoints == 5);
        found!.TotalPoints.Should().Be(5);
        missing.Should().BeNull();
        updated.Should().BeFalse();
        deleted.Should().BeFalse();
    }

    [Fact]
    public async Task Assignment_service_should_hide_other_teachers_assignments()
    {
        var teacherId = Guid.NewGuid();
        var otherTeacherId = Guid.NewGuid();
        var own = new Assignment { Id = Guid.NewGuid(), Title = "Own", TeacherUserId = teacherId };
        var other = new Assignment { Id = Guid.NewGuid(), Title = "Other", TeacherUserId = otherTeacherId };
        var service = new AssignmentService(new FakeAssignmentRepository(own, other), new FakeCurrentUser(teacherId, UserRole.Teacher));

        var all = await service.GetAllAsync();
        var blocked = await service.GetByIdAsync(other.Id);

        all.Should().ContainSingle(x => x.Id == own.Id);
        blocked.Should().BeNull();
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
        repository.CorrectionLogs.Add(new CorrectionLog
        {
            Id = Guid.NewGuid(),
            SubmissionId = submission.Id,
            QuestionId = question.Id,
            CorrectionType = "Manual",
            OriginalAnswer = "B",
            Score = 0,
            CreatedAt = DateTime.UtcNow
        });
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
        submission.DeletedAt.Should().NotBeNull();
        (await service.GetByIdAsync(submission.Id)).Should().BeNull();
        (await service.GetCorrectionLogsAsync(submission.Id)).Should().ContainSingle();
    }

    [Fact]
    public async Task Submission_service_should_hide_other_students_submissions()
    {
        var assignmentId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var own = new Submission { Id = Guid.NewGuid(), AssignmentId = assignmentId, StudentUserId = studentId, StudentName = "Ana" };
        var other = new Submission { Id = Guid.NewGuid(), AssignmentId = assignmentId, StudentUserId = Guid.NewGuid(), StudentName = "Bia" };
        var service = new SubmissionService(
            new FakeSubmissionRepositoryForService
            {
                ExistingAssignmentId = assignmentId,
                Submissions = [own, other]
            },
            currentUser: new FakeCurrentUser(studentId, UserRole.Student));

        var all = await service.GetByAssignmentIdAsync(assignmentId);
        var blocked = await service.GetByIdAsync(other.Id);

        all.Should().ContainSingle(x => x.Id == own.Id);
        blocked.Should().BeNull();
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
        var reviewerId = Guid.NewGuid();
        var service = new SubmissionService(repository, currentUser: new FakeCurrentUser(reviewerId, UserRole.Admin));

        var response = await service.ReviewAnswerAsync(
            answer.Id,
            new ReviewStudentAnswerRequest(4, "Revisado", true));

        response.Should().NotBeNull();
        response!.FinalScore.Should().Be(4);
        response.NeedsReview.Should().BeFalse();
        submission.Status.Should().Be(SubmissionStatus.Reviewed);
        repository.CorrectionLogs.Should().ContainSingle(x => x.CorrectionType == "ManualReview" && x.ReviewedByUserId == reviewerId.ToString());
    }

    [Fact]
    public async Task Student_should_submit_and_read_own_answers_without_exposing_log_keys()
    {
        var studentId = Guid.NewGuid();
        var assignment = new Assignment { Id = Guid.NewGuid() };
        var question = new Question { Id = Guid.NewGuid(), AssignmentId = assignment.Id };
        var repository = new FakeSubmissionRepositoryForService
        { ExistingAssignmentId = assignment.Id, Questions = [question] };
        var service = new SubmissionService(repository, new FakeAssignmentRepository(assignment),
            new FakeCurrentUser(studentId, UserRole.Student));
        var response = await service.CreateAsync(assignment.Id, new CreateSubmissionRequest("Aluno", null,
            [new CreateStudentAnswerRequest(question.Id, "A")]));
        response.Should().NotBeNull();
        repository.Submissions.Single().StudentUserId.Should().Be(studentId);
        (await service.GetByIdAsync(response!.Id)).Should().NotBeNull();
        (await service.UpdateAnswerAsync(response.Id, question.Id, new UpdateStudentAnswerRequest("B"))).Should().BeTrue();
        repository.CorrectionLogs.Add(new CorrectionLog { SubmissionId = response.Id, ExpectedAnswer = "secret" });
        (await service.GetCorrectionLogsAsync(response.Id)).Should().ContainSingle(x => x.ExpectedAnswer == null);
        (await service.DeleteAsync(response.Id)).Should().BeFalse();
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

    [Fact]
    public async Task Submission_service_should_import_csv_and_build_report()
    {
        var assignmentId = Guid.NewGuid();
        var firstQuestion = new Question
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            Text = "Q1",
            Type = QuestionType.MultipleChoice,
            Order = 1
        };
        var secondQuestion = new Question
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            Text = "Q2",
            Type = QuestionType.Numeric,
            Order = 2
        };
        var repository = new FakeSubmissionRepositoryForService
        {
            ExistingAssignmentId = assignmentId,
            Questions = [firstQuestion, secondQuestion]
        };
        var service = new SubmissionService(repository);

        var import = await service.ImportCsvAsync(
            assignmentId,
            """
            student_name,student_email,q1,q2
            Ana,ana@email.com,A,10
            Bia,bia@email.com,B,8
            """);
        repository.Submissions[0].FinalScore = 10;
        repository.Submissions[0].StudentAnswers.First(x => x.QuestionId == firstQuestion.Id).IsCorrect = true;
        repository.Submissions[1].FinalScore = 6;
        repository.Submissions[1].StudentAnswers.First(x => x.QuestionId == secondQuestion.Id).IsCorrect = true;

        var report = await service.GetReportAsync(assignmentId);
        var csv = await service.ExportCsvAsync(assignmentId);
        var excel = await service.ExportExcelAsync(assignmentId);
        var pdf = await service.ExportPdfAsync(assignmentId);

        import!.ImportedCount.Should().Be(2);
        repository.Submissions.Should().HaveCount(2);
        report!.AverageScore.Should().Be(8);
        report.HighestScore.Should().Be(10);
        report.LowestScore.Should().Be(6);
        report.Questions.Should().Contain(x => x.QuestionId == firstQuestion.Id && x.CorrectCount == 1 && x.IncorrectCount == 1);
        report.Questions.Should().Contain(x => x.QuestionId == secondQuestion.Id && x.CorrectCount == 1 && x.IncorrectCount == 1);
        csv.Should().Contain("student_name,student_email,final_score,status");
        csv.Should().Contain("Ana,ana@email.com,10,Pending");
        excel.Should().NotBeNullOrEmpty();
        pdf.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(",")]
    [InlineData(";")]
    [InlineData("|")]
    [InlineData("\t")]
    public async Task Submission_service_should_import_quoted_csv_fields(string separator)
    {
        var assignmentId = Guid.NewGuid();
        var question = new Question
        {
            Id = Guid.NewGuid(), AssignmentId = assignmentId,
            Text = "Q1", Type = QuestionType.ShortText, Order = 1
        };
        var repository = new FakeSubmissionRepositoryForService
        {
            ExistingAssignmentId = assignmentId, Questions = [question]
        };
        var service = new SubmissionService(repository);
        var csv = $"student_name{separator}student_email{separator}q1\r\n"
            + $"\"Silva{separator} Ana\"{separator}ana@email.com{separator}"
            + $"\"Primeira linha\r\nDisse \"\"olá\"\"{separator} sim\"\r\n";

        var result = await service.ImportCsvAsync(assignmentId, csv);

        result!.ImportedCount.Should().Be(1);
        var submission = repository.Submissions.Should().ContainSingle().Subject;
        submission.StudentName.Should().Be($"Silva{separator} Ana");
        submission.StudentEmail.Should().Be("ana@email.com");
        submission.StudentAnswers.Should().ContainSingle()
            .Which.Answer.Should().Be($"Primeira linha\r\nDisse \"olá\"{separator} sim");
        repository.SaveChangesCount.Should().Be(1);
    }

    [Theory]
    [InlineData("\"Bia,bia@email.com,B")]
    [InlineData("\"Bia\"extra,bia@email.com,B")]
    [InlineData("Bia,bia@email.com,B,coluna extra")]
    [InlineData("Bia,bia@email.com")]
    public async Task Submission_service_should_reject_malformed_csv_before_adding_submissions(string invalidRow)
    {
        var assignmentId = Guid.NewGuid();
        var repository = new FakeSubmissionRepositoryForService
        {
            ExistingAssignmentId = assignmentId,
            Questions = [new Question
            {
                Id = Guid.NewGuid(), AssignmentId = assignmentId,
                Text = "Q1", Type = QuestionType.ShortText, Order = 1
            }]
        };
        var service = new SubmissionService(repository);
        var csv = $"student_name,student_email,q1\nAna,ana@email.com,A\n{invalidRow}";

        var action = () => service.ImportCsvAsync(assignmentId, csv);

        await action.Should().ThrowAsync<ValidationException>();
        repository.Submissions.Should().BeEmpty();
        repository.SaveChangesCount.Should().Be(0);
    }

    [Theory]
    [InlineData("=1+1")]
    [InlineData("+1+1")]
    [InlineData("-1+1")]
    [InlineData("@SUM(1)")]
    [InlineData("  =1+1")]
    [InlineData("\t=1+1")]
    [InlineData("＝1+1")]
    [InlineData("＋1+1")]
    [InlineData("－1+1")]
    [InlineData("＠SUM(1)")]
    [InlineData("=HYPERLINK(\"https://example.test\",\"QA\")")]
    [InlineData("\r\n=1+1")]
    public async Task Csv_export_should_neutralize_spreadsheet_formulas(string name)
    {
        var assignmentId = Guid.NewGuid();
        var repository = new FakeSubmissionRepositoryForService
        {
            ExistingAssignmentId = assignmentId,
            Submissions = [new Submission
            {
                AssignmentId = assignmentId, StudentName = name,
                StudentEmail = "=1+1", Status = SubmissionStatus.Corrected, FinalScore = 1
            }]
        };

        var csv = await new SubmissionService(repository).ExportCsvAsync(assignmentId);

        csv.Should().Contain($"\"\t{name.Replace("\"", "\"\"")}\",\"\t=1+1\",1,Corrected");
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

        public Task<bool> TitleExistsAsync(string title, Guid? exceptId = null, CancellationToken cancellationToken = default)
            => Task.FromResult(Assignments.Any(x =>
                x.Id != exceptId
                && string.Equals(x.Title.Trim(), title.Trim(), StringComparison.OrdinalIgnoreCase)));

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
            => Task.FromResult<IReadOnlyCollection<Submission>>(Submissions.Where(x => x.AssignmentId == assignmentId && x.DeletedAt is null).ToList());

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
            => submission.DeletedAt = DateTime.UtcNow;

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

    private sealed class FakeCurrentUser(Guid id, UserRole role) : ICurrentUser
    {
        public Guid? Id { get; } = id;
        public UserRole? Role { get; } = role;
    }
}
