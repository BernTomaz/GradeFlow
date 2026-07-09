using System.ComponentModel.DataAnnotations;
using GradeFlow.Application.DTOs.Questions;
using GradeFlow.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GradeFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public sealed class QuestionsController(IQuestionService questionService) : ControllerBase
{
    [HttpGet("assignments/{assignmentId:guid}/questions")]
    public async Task<ActionResult<IReadOnlyCollection<QuestionResponse>>> GetByAssignmentId(Guid assignmentId, CancellationToken cancellationToken)
        => Ok(await questionService.GetByAssignmentIdAsync(assignmentId, cancellationToken));

    [HttpPost("assignments/{assignmentId:guid}/questions")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<QuestionResponse>> Create(Guid assignmentId, CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await questionService.CreateAsync(assignmentId, request, cancellationToken) is { } question
                ? CreatedAtAction(nameof(GetById), new { id = question.Id }, question)
                : NotFound();
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpGet("questions/{id:guid}")]
    public async Task<ActionResult<QuestionResponse>> GetById(Guid id, CancellationToken cancellationToken)
        => await questionService.GetByIdAsync(id, cancellationToken) is { } question ? Ok(question) : NotFound();

    [HttpPut("questions/{id:guid}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Update(Guid id, UpdateQuestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await questionService.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpDelete("questions/{id:guid}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await questionService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}
