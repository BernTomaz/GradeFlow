using GradeFlow.Application.DTOs.Questions;
using GradeFlow.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GradeFlow.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class QuestionsController(IQuestionService questionService) : ControllerBase
{
    [HttpGet("assignments/{assignmentId:guid}/questions")]
    public async Task<ActionResult<IReadOnlyCollection<QuestionResponse>>> GetByAssignmentId(Guid assignmentId, CancellationToken cancellationToken)
        => Ok(await questionService.GetByAssignmentIdAsync(assignmentId, cancellationToken));

    [HttpPost("assignments/{assignmentId:guid}/questions")]
    public async Task<ActionResult<QuestionResponse>> Create(Guid assignmentId, CreateQuestionRequest request, CancellationToken cancellationToken)
        => await questionService.CreateAsync(assignmentId, request, cancellationToken) is { } question
            ? CreatedAtAction(nameof(GetById), new { id = question.Id }, question)
            : NotFound();

    [HttpGet("questions/{id:guid}")]
    public async Task<ActionResult<QuestionResponse>> GetById(Guid id, CancellationToken cancellationToken)
        => await questionService.GetByIdAsync(id, cancellationToken) is { } question ? Ok(question) : NotFound();

    [HttpPut("questions/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateQuestionRequest request, CancellationToken cancellationToken)
        => await questionService.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("questions/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await questionService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
