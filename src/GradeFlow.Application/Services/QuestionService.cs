using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using GradeFlow.Application.DTOs.Questions;
using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;

namespace GradeFlow.Application.Services;

public sealed class QuestionService(
    IQuestionRepository questionRepository,
    IAssignmentRepository? assignmentRepository = null,
    ICurrentUser? currentUser = null) : IQuestionService
{
    private readonly ICurrentUser currentUser = currentUser ?? SystemCurrentUser.Instance;

    public async Task<IReadOnlyCollection<QuestionResponse>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
        => await CanReadAssignmentAsync(assignmentId, cancellationToken)
            ? (await questionRepository.GetByAssignmentIdAsync(assignmentId, cancellationToken)).Select(Map).ToList()
            : [];

    public async Task<QuestionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var question = await questionRepository.GetByIdAsync(id, cancellationToken);
        return question is null || !await CanReadAssignmentAsync(question.AssignmentId, cancellationToken) ? null : Map(question);
    }

    public async Task<QuestionResponse?> CreateAsync(Guid assignmentId, CreateQuestionRequest request, CancellationToken cancellationToken = default)
    {
        if (!await CanManageAssignmentAsync(assignmentId, cancellationToken)) return null;
        await ValidateAsync(assignmentId, request.Points, request.Order, null, cancellationToken);

        var question = new Question
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            Text = request.Text.Trim(),
            Type = request.Type,
            Points = request.Points,
            Order = request.Order,
            AnswerKey = new AnswerKey
            {
                Id = Guid.NewGuid(),
                CorrectAnswer = request.AnswerKey.CorrectAnswer.Trim(),
                AcceptedAnswersJson = ToJson(request.AnswerKey.AcceptedAnswers),
                KeywordsJson = ToJson(request.AnswerKey.Keywords),
                Tolerance = request.AnswerKey.Tolerance,
                FeedbackCorrect = request.AnswerKey.FeedbackCorrect?.Trim(),
                FeedbackIncorrect = request.AnswerKey.FeedbackIncorrect?.Trim()
            }
        };

        questionRepository.Add(question);
        await questionRepository.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(question.Id, cancellationToken);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateQuestionRequest request, CancellationToken cancellationToken = default)
    {
        var question = await questionRepository.GetForUpdateAsync(id, cancellationToken);
        if (question is null || !await CanManageAssignmentAsync(question.AssignmentId, cancellationToken)) return false;
        await ValidateAsync(question.AssignmentId, request.Points, request.Order, question.Id, cancellationToken);

        question.Text = request.Text.Trim();
        question.Type = request.Type;
        question.Points = request.Points;
        question.Order = request.Order;

        if (question.AnswerKey is null)
        {
            question.AnswerKey = new AnswerKey { Id = Guid.NewGuid(), QuestionId = question.Id };
        }

        question.AnswerKey.CorrectAnswer = request.AnswerKey.CorrectAnswer.Trim();
        question.AnswerKey.AcceptedAnswersJson = ToJson(request.AnswerKey.AcceptedAnswers);
        question.AnswerKey.KeywordsJson = ToJson(request.AnswerKey.Keywords);
        question.AnswerKey.Tolerance = request.AnswerKey.Tolerance;
        question.AnswerKey.FeedbackCorrect = request.AnswerKey.FeedbackCorrect?.Trim();
        question.AnswerKey.FeedbackIncorrect = request.AnswerKey.FeedbackIncorrect?.Trim();

        await questionRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var question = await questionRepository.GetForUpdateAsync(id, cancellationToken);
        if (question is null || !await CanManageAssignmentAsync(question.AssignmentId, cancellationToken)) return false;
        if (await questionRepository.HasAnswersAsync(id, cancellationToken))
        {
            throw new ValidationException("Não é possível excluir uma questão que já possui respostas.");
        }

        questionRepository.Remove(question);
        await questionRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static QuestionResponse Map(Question question)
        => new(
            question.Id,
            question.AssignmentId,
            question.Text,
            question.Type,
            question.Points,
            question.Order,
            question.AnswerKey is null
                ? null
                : new AnswerKeyResponse(
                    question.AnswerKey.Id,
                    question.AnswerKey.QuestionId,
                    question.AnswerKey.CorrectAnswer,
                    FromJson(question.AnswerKey.AcceptedAnswersJson),
                    FromJson(question.AnswerKey.KeywordsJson),
                    question.AnswerKey.Tolerance,
                    question.AnswerKey.FeedbackCorrect,
                    question.AnswerKey.FeedbackIncorrect));

    private static string? ToJson(IReadOnlyCollection<string>? values)
        => values is null || values.Count == 0 ? null : JsonSerializer.Serialize(values);

    private static IReadOnlyCollection<string>? FromJson(string? json)
        => string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<string[]>(json);

    private async Task ValidateAsync(
        Guid assignmentId,
        decimal points,
        int order,
        Guid? questionId,
        CancellationToken cancellationToken)
    {
        if (points > 10) throw new ValidationException("O máximo de pontos é 10.");

        if (await questionRepository.OrderExistsAsync(assignmentId, order, questionId, cancellationToken))
        {
            throw new ValidationException("Ordem já está sendo usada.");
        }
    }

    private async Task<bool> CanReadAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken)
    {
        if (assignmentRepository is null)
            return currentUser.IsAdmin && await questionRepository.AssignmentExistsAsync(assignmentId, cancellationToken);

        var assignment = await assignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
        return assignment is not null
            && (this.currentUser.IsAdmin || (this.currentUser.IsTeacher && (assignment.TeacherUserId is null || assignment.TeacherUserId == this.currentUser.Id)));
    }

    private Task<bool> CanManageAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken)
        => CanReadAssignmentAsync(assignmentId, cancellationToken);
}
