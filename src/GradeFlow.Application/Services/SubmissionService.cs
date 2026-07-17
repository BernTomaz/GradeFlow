using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using GradeFlow.Application.Corrections;
using GradeFlow.Application.DTOs.Submissions;
using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GradeFlow.Application.Services;

public sealed class SubmissionService(
    ISubmissionRepository submissionRepository,
    IAssignmentRepository? assignmentRepository = null,
    ICurrentUser? currentUser = null) : ISubmissionService
{
    private readonly ICurrentUser currentUser = currentUser ?? SystemCurrentUser.Instance;

    public async Task<IReadOnlyCollection<SubmissionResponse>?> GetByAssignmentIdAsync(
        Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        var assignment = assignmentRepository is null
            ? null
            : await assignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
        if (assignmentRepository is null)
        {
            if (!await submissionRepository.AssignmentExistsAsync(assignmentId, cancellationToken)) return null;
        }
        else if (assignment is null || !CanReadAssignment(assignment)) return null;

        return (await submissionRepository.GetByAssignmentIdAsync(assignmentId, cancellationToken))
            .Where(CanRead)
            .Select(Map)
            .ToList();
    }

    public async Task<SubmissionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdAsync(id, cancellationToken);
        return submission is null || !CanRead(submission) ? null : Map(submission);
    }

    public async Task<IReadOnlyCollection<CorrectionLogResponse>?> GetCorrectionLogsAsync(
        Guid submissionId,
        CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdAsync(submissionId, cancellationToken);
        if (submission is null || !CanRead(submission)) return null;

        return (await submissionRepository.GetCorrectionLogsAsync(submissionId, cancellationToken))
            .Select(log => new CorrectionLogResponse(
                log.Id,
                log.SubmissionId,
                log.QuestionId,
                log.CorrectionType,
                log.OriginalAnswer,
                log.ExpectedAnswer,
                log.Score,
                log.Message,
                log.CreatedAt))
            .ToList();
    }

    public async Task<AssignmentReportResponse?> GetReportAsync(
        Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        var questions = await submissionRepository.GetAssignmentQuestionsAsync(assignmentId, cancellationToken);
        if (questions.Count == 0 && !await submissionRepository.AssignmentExistsAsync(assignmentId, cancellationToken)) return null;

        var assignment = assignmentRepository is null
            ? null
            : await assignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
        if (assignmentRepository is not null && (assignment is null || !CanReadAssignment(assignment))) return null;

        var submissions = (await submissionRepository.GetByAssignmentIdAsync(assignmentId, cancellationToken))
            .Where(CanRead)
            .ToList();
        var scores = submissions.Select(x => x.FinalScore).ToList();

        return new AssignmentReportResponse(
            assignmentId,
            submissions.Count,
            scores.Count == 0 ? 0 : scores.Average(),
            scores.Count == 0 ? null : scores.Max(),
            scores.Count == 0 ? null : scores.Min(),
            submissions
                .OrderByDescending(x => x.FinalScore)
                .Select(x => new StudentScoreResponse(x.Id, x.StudentName, x.StudentEmail, x.FinalScore, x.Status))
                .ToList(),
            questions
                .OrderBy(x => x.Order)
                .Select(question => new QuestionReportResponse(
                    question.Id,
                    question.Order,
                    question.Text,
                    submissions.SelectMany(x => x.StudentAnswers).Count(x => x.QuestionId == question.Id && x.IsCorrect),
                    submissions.SelectMany(x => x.StudentAnswers).Count(x => x.QuestionId == question.Id && !x.IsCorrect)))
                .ToList());
    }

    public async Task<string?> ExportCsvAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var report = await GetReportAsync(assignmentId, cancellationToken);
        if (report is null) return null;

        var builder = new StringBuilder();
        builder.AppendLine("student_name,student_email,final_score,status");
        foreach (var student in report.Students)
        {
            builder.Append(EscapeCsv(student.StudentName)).Append(',')
                .Append(EscapeCsv(student.StudentEmail ?? string.Empty)).Append(',')
                .Append(student.FinalScore.ToString(CultureInfo.InvariantCulture)).Append(',')
                .Append(student.Status)
                .AppendLine();
        }

        return builder.ToString();
    }

    public async Task<byte[]?> ExportExcelAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var report = await GetReportAsync(assignmentId, cancellationToken);
        if (report is null) return null;

        using var workbook = new XLWorkbook();
        var students = workbook.Worksheets.Add("Notas");
        students.Cell(1, 1).Value = "Relatório GradeFlow";
        students.Range(1, 1, 1, 4).Merge().Style
            .Font.SetBold()
            .Font.SetFontSize(16)
            .Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.FromHtml("#1565C0"));
        students.Cell(2, 1).Value = "Submissões";
        students.Cell(2, 2).Value = report.SubmissionCount;
        students.Cell(2, 3).Value = "Média";
        students.Cell(2, 4).Value = report.AverageScore;
        students.Cell(3, 1).Value = "Maior nota";
        students.Cell(3, 2).Value = report.HighestScore;
        students.Cell(3, 3).Value = "Menor nota";
        students.Cell(3, 4).Value = report.LowestScore;
        students.Cell(5, 1).Value = "Aluno";
        students.Cell(5, 2).Value = "Email";
        students.Cell(5, 3).Value = "Nota";
        students.Cell(5, 4).Value = "Status";
        for (var i = 0; i < report.Students.Count; i++)
        {
            var student = report.Students.ElementAt(i);
            var row = i + 6;
            students.Cell(row, 1).Value = student.StudentName;
            students.Cell(row, 2).Value = student.StudentEmail;
            students.Cell(row, 3).Value = student.FinalScore;
            students.Cell(row, 4).Value = student.Status.ToString();
        }
        FormatWorksheet(students, 5, 4);

        var questions = workbook.Worksheets.Add("Questões");
        questions.Cell(1, 1).Value = "Questões";
        questions.Range(1, 1, 1, 4).Merge().Style
            .Font.SetBold()
            .Font.SetFontSize(16)
            .Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.FromHtml("#1565C0"));
        questions.Cell(3, 1).Value = "Ordem";
        questions.Cell(3, 2).Value = "Questão";
        questions.Cell(3, 3).Value = "Acertos";
        questions.Cell(3, 4).Value = "Erros";
        for (var i = 0; i < report.Questions.Count; i++)
        {
            var question = report.Questions.ElementAt(i);
            var row = i + 4;
            questions.Cell(row, 1).Value = question.Order;
            questions.Cell(row, 2).Value = QuestionTitle(question.Text);
            questions.Cell(row, 3).Value = question.CorrectCount;
            questions.Cell(row, 4).Value = question.IncorrectCount;
        }
        FormatWorksheet(questions, 3, 4);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void FormatWorksheet(IXLWorksheet worksheet, int headerRow, int columnCount)
    {
        var lastRow = Math.Max(headerRow, worksheet.LastRowUsed()?.RowNumber() ?? headerRow);
        var range = worksheet.Range(headerRow, 1, lastRow, columnCount);
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.OutsideBorderColor = XLColor.FromHtml("#D9E2EC");
        range.Style.Border.InsideBorderColor = XLColor.FromHtml("#D9E2EC");
        worksheet.Range(headerRow, 1, headerRow, columnCount).Style
            .Font.SetBold()
            .Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.FromHtml("#1565C0"));
        range.SetAutoFilter();
        worksheet.SheetView.FreezeRows(headerRow);
        worksheet.Columns().AdjustToContents();
        worksheet.Column(2).Width = Math.Min(Math.Max(worksheet.Column(2).Width, 28), 60);
        worksheet.Rows().AdjustToContents();
    }

    public async Task<byte[]?> ExportPdfAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var report = await GetReportAsync(assignmentId, cancellationToken);
        if (report is null) return null;

        QuestPDF.Settings.License = LicenseType.Community;
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken3));
                page.Header().Column(header =>
                {
                    header.Item()
                        .Background(Colors.Blue.Darken3)
                        .Padding(16)
                        .Column(column =>
                        {
                            column.Item().Text("GradeFlow").FontSize(20).Bold().FontColor(Colors.White);
                            column.Item().Text("Relatório de desempenho da avaliação").FontSize(10).FontColor(Colors.Blue.Lighten4);
                        });
                });
                page.Content().Column(column =>
                {
                    column.Spacing(18);
                    column.Item().Height(8);
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(cell => SummaryCard(cell, "Submissões", report.SubmissionCount.ToString(CultureInfo.InvariantCulture)));
                        row.RelativeItem().Element(cell => SummaryCard(cell, "Média", report.AverageScore.ToString("0.##", CultureInfo.InvariantCulture)));
                        row.RelativeItem().Element(cell => SummaryCard(cell, "Maior nota", report.HighestScore?.ToString("0.##", CultureInfo.InvariantCulture) ?? "-"));
                        row.RelativeItem().Element(cell => SummaryCard(cell, "Menor nota", report.LowestScore?.ToString("0.##", CultureInfo.InvariantCulture) ?? "-"));
                    });
                    column.Item().Text("Notas por aluno").FontSize(13).Bold().FontColor(Colors.Grey.Darken4);
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.4f);
                            columns.RelativeColumn(1.6f);
                            columns.ConstantColumn(70);
                        });
                        table.Header(header =>
                        {
                            HeaderCell(header.Cell(), "Aluno");
                            HeaderCell(header.Cell(), "Email");
                            HeaderCell(header.Cell(), "Nota");
                        });
                        foreach (var student in report.Students)
                        {
                            BodyCell(table.Cell(), student.StudentName);
                            BodyCell(table.Cell(), student.StudentEmail ?? "-");
                            BodyCell(table.Cell(), student.FinalScore.ToString("0.##", CultureInfo.InvariantCulture));
                        }
                    });
                    column.Item().Text("Questões").FontSize(13).Bold().FontColor(Colors.Grey.Darken4);
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(58);
                            columns.RelativeColumn();
                            columns.ConstantColumn(58);
                            columns.ConstantColumn(48);
                        });
                        table.Header(header =>
                        {
                            HeaderCell(header.Cell(), "Ordem");
                            HeaderCell(header.Cell(), "Questão");
                            HeaderCell(header.Cell(), "Acertos");
                            HeaderCell(header.Cell(), "Erros");
                        });
                        foreach (var question in report.Questions)
                        {
                            BodyCell(table.Cell(), question.Order.ToString(CultureInfo.InvariantCulture));
                            BodyCell(table.Cell(), QuestionTitle(question.Text));
                            BodyCell(table.Cell(), question.CorrectCount.ToString(CultureInfo.InvariantCulture));
                            BodyCell(table.Cell(), question.IncorrectCount.ToString(CultureInfo.InvariantCulture));
                        }
                    });
                });
                page.Footer().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(8).AlignRight().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                    text.Span("Pagina ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private static void SummaryCard(IContainer container, string label, string value)
        => container
            .PaddingRight(8)
            .Background(Colors.Grey.Lighten4)
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(10)
            .Column(column =>
            {
                column.Item().Text(label).FontSize(8).FontColor(Colors.Grey.Darken1);
                column.Item().Text(value).FontSize(16).Bold().FontColor(Colors.Blue.Darken3);
            });

    private static void HeaderCell(IContainer container, string text)
        => container
            .Background(Colors.Blue.Darken3)
            .Border(1)
            .BorderColor(Colors.White)
            .PaddingVertical(6)
            .PaddingHorizontal(8)
            .Text(text)
            .Bold()
            .FontColor(Colors.White);

    private static void BodyCell(IContainer container, string text)
        => container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(6)
            .PaddingHorizontal(8)
            .Text(text);

    private static string QuestionTitle(string text)
        => text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim() ?? text.Trim();

    public async Task<ImportSubmissionsResponse?> ImportCsvAsync(
        Guid assignmentId,
        string csv,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(csv))
        {
            throw new ValidationException("File is empty.");
        }

        var questions = (await submissionRepository.GetAssignmentQuestionsAsync(assignmentId, cancellationToken))
            .OrderBy(x => x.Order)
            .ToList();
        if (questions.Count == 0 && !await submissionRepository.AssignmentExistsAsync(assignmentId, cancellationToken)) return null;

        var assignment = assignmentRepository is null
            ? null
            : await assignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
        if (assignmentRepository is not null && (assignment is null || !CanCreateSubmission(assignment))) return null;

        var rows = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.TrimEnd('\r'))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
        if (rows.Count < 2)
        {
            throw new ValidationException("File must contain a header and at least one student row.");
        }

        var separator = DetectSeparator(rows[0]);
        var header = SplitCsvLine(rows[0], separator).Select(NormalizeHeader).ToList();
        var nameIndex = header.IndexOf("student_name");
        var emailIndex = header.IndexOf("student_email");
        if (nameIndex < 0)
        {
            throw new ValidationException("Missing required column: student_name.");
        }

        var questionColumns = questions
            .Select((question, index) => new { Question = question, Header = $"q{index + 1}", Index = header.IndexOf($"q{index + 1}") })
            .ToList();
        var missingQuestion = questionColumns.FirstOrDefault(x => x.Index < 0);
        if (missingQuestion is not null)
        {
            throw new ValidationException($"Missing required column: {missingQuestion.Header}.");
        }

        var seenStudents = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var imported = new List<SubmissionResponse>();
        foreach (var row in rows.Skip(1))
        {
            var values = SplitCsvLine(row, separator);
            var studentName = GetValue(values, nameIndex);
            var studentEmail = emailIndex < 0 ? null : GetValue(values, emailIndex);
            var duplicateKey = string.IsNullOrWhiteSpace(studentEmail) ? studentName : studentEmail;
            if (!seenStudents.Add(duplicateKey.Trim()))
            {
                throw new ValidationException($"Duplicate student row: {duplicateKey}.");
            }

            var request = new CreateSubmissionRequest(
                studentName,
                string.IsNullOrWhiteSpace(studentEmail) ? null : studentEmail,
                questionColumns
                    .Select(x => new CreateStudentAnswerRequest(x.Question.Id, GetValue(values, x.Index)))
                    .ToList());
            Validate(request, questions);

            var submission = new Submission
            {
                Id = Guid.NewGuid(),
                AssignmentId = assignmentId,
                StudentUserId = currentUser.IsStudent ? currentUser.Id : null,
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
            imported.Add(Map(submission));
        }

        await submissionRepository.SaveChangesAsync(cancellationToken);
        return new ImportSubmissionsResponse(imported.Count, imported);
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

        var assignment = assignmentRepository is null
            ? null
            : await assignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
        if (assignment is not null && !CanCreateSubmission(assignment)) return null;
        if (assignmentRepository is not null && assignment is null) return null;

        Validate(request, questions);

        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentUserId = currentUser.IsStudent ? currentUser.Id : null,
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
        if (submission is null || !CanManage(submission)) return false;

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
        if (submission is null || !CanAnswer(submission)) return false;

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
        if (submission is null || !CanAnswer(submission)) return false;

        submission.StudentName = request.StudentName.Trim();
        submission.StudentEmail = string.IsNullOrWhiteSpace(request.StudentEmail) ? null : request.StudentEmail.Trim();

        await submissionRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ReviewStudentAnswerResponse?> ReviewAnswerAsync(
        Guid answerId,
        ReviewStudentAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.ScoreAwarded < 0)
        {
            throw new ValidationException("ScoreAwarded must be greater than or equal to zero.");
        }

        var answer = await submissionRepository.GetAnswerForReviewAsync(answerId, cancellationToken);
        if (answer?.Submission is null || answer.Question is null) return null;
        if (!CanTeacherManage(answer.Submission)) return null;

        if (request.ScoreAwarded > answer.Question.Points)
        {
            throw new ValidationException("ScoreAwarded cannot be greater than question points.");
        }

        answer.ScoreAwarded = request.ScoreAwarded;
        answer.Feedback = request.Feedback?.Trim();
        answer.IsCorrect = request.IsCorrect;
        answer.NeedsReview = false;

        var submission = answer.Submission;
        submission.FinalScore = submission.StudentAnswers.Sum(x => x.Id == answer.Id ? answer.ScoreAwarded : x.ScoreAwarded);
        submission.Status = SubmissionStatus.Reviewed;
        submission.ReviewedAt = DateTime.UtcNow;

        submissionRepository.AddCorrectionLog(new CorrectionLog
        {
            Id = Guid.NewGuid(),
            SubmissionId = answer.SubmissionId,
            QuestionId = answer.QuestionId,
            CorrectionType = "ManualReview",
            OriginalAnswer = answer.Answer,
            NormalizedAnswer = answer.Answer.Trim(),
            ExpectedAnswer = answer.Question.AnswerKey?.CorrectAnswer,
            Score = answer.ScoreAwarded,
            Message = answer.Feedback,
            CreatedAt = DateTime.UtcNow
        });

        await submissionRepository.SaveChangesAsync(cancellationToken);
        return new ReviewStudentAnswerResponse(
            answer.Id,
            answer.SubmissionId,
            answer.ScoreAwarded,
            answer.Feedback,
            answer.IsCorrect,
            answer.NeedsReview,
            submission.FinalScore);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetForUpdateAsync(id, cancellationToken);
        if (submission is null || !CanTeacherManage(submission)) return false;

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

    private static char DetectSeparator(string header)
        => new[] { ';', ',', '|', '\t' }.OrderByDescending(separator => header.Count(x => x == separator)).First();

    // ponytail: CSV simples sem escape de separador entre aspas; trocar por CsvHelper se aparecer CSV real de planilha.
    private static IReadOnlyList<string> SplitCsvLine(string line, char separator)
        => line.Split(separator).Select(x => x.Trim().Trim('"')).ToList();

    private static string NormalizeHeader(string value)
        => value.Trim().ToLower(CultureInfo.InvariantCulture);

    private static string GetValue(IReadOnlyList<string> values, int index)
        => index >= 0 && index < values.Count ? values[index] : string.Empty;

    private static string EscapeCsv(string value)
        => value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;

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

    private bool CanReadAssignment(Assignment assignment)
        => currentUser.IsAdmin
            || (currentUser.IsTeacher && CanTeacherUseAssignment(assignment))
            || currentUser.IsStudent;

    private bool CanCreateSubmission(Assignment assignment)
        => currentUser.IsAdmin
            || (currentUser.IsTeacher && CanTeacherUseAssignment(assignment))
            || currentUser.IsStudent;

    private bool CanRead(Submission submission)
        => currentUser.IsAdmin
            || (currentUser.IsTeacher && CanTeacherUseSubmission(submission))
            || (currentUser.IsStudent && submission.StudentUserId == currentUser.Id);

    private bool CanAnswer(Submission submission)
        => currentUser.IsAdmin
            || (currentUser.IsTeacher && CanTeacherUseSubmission(submission))
            || (currentUser.IsStudent && submission.StudentUserId == currentUser.Id);

    private bool CanManage(Submission submission) => CanTeacherManage(submission);

    private bool CanTeacherManage(Submission submission)
        => currentUser.IsAdmin
            || (currentUser.IsTeacher && CanTeacherUseSubmission(submission));

    private bool CanTeacherUseAssignment(Assignment assignment)
        => assignment.TeacherUserId is null || assignment.TeacherUserId == currentUser.Id;

    private bool CanTeacherUseSubmission(Submission submission)
        => submission.Assignment is not null && CanTeacherUseAssignment(submission.Assignment);
}
