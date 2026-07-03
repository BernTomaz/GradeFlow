using System.ComponentModel.DataAnnotations;
using GradeFlow.Application.DTOs.Submissions;
using GradeFlow.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GradeFlow.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class SubmissionsController(
    ISubmissionService submissionService,
    ICorrectionService correctionService) : ControllerBase
{
    [HttpGet("assignments/{assignmentId:guid}/submissions")]
    public async Task<ActionResult<IReadOnlyCollection<SubmissionResponse>>> GetByAssignmentId(
        Guid assignmentId,
        CancellationToken cancellationToken)
        => await submissionService.GetByAssignmentIdAsync(assignmentId, cancellationToken) is { } submissions
            ? Ok(submissions)
            : NotFound();

    [HttpGet("submissions/{id:guid}")]
    public async Task<ActionResult<SubmissionResponse>> GetById(Guid id, CancellationToken cancellationToken)
        => await submissionService.GetByIdAsync(id, cancellationToken) is { } submission ? Ok(submission) : NotFound();

    [HttpPost("assignments/{assignmentId:guid}/submissions")]
    public async Task<ActionResult<SubmissionResponse>> Create(
        Guid assignmentId,
        CreateSubmissionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await submissionService.CreateAsync(assignmentId, request, cancellationToken);
            return created is null
                ? NotFound()
                : CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("submissions/{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        CreateSubmissionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await submissionService.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("submissions/{submissionId:guid}/questions/{questionId:guid}/answer")]
    public async Task<IActionResult> UpdateAnswer(
        Guid submissionId,
        Guid questionId,
        UpdateStudentAnswerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await submissionService.UpdateAnswerAsync(submissionId, questionId, request, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("submissions/{id:guid}/student")]
    public async Task<IActionResult> UpdateStudentInfo(
        Guid id,
        UpdateStudentInfoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await submissionService.UpdateStudentInfoAsync(id, request, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpDelete("submissions/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await submissionService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("submissions/{submissionId:guid}/correct")]
    public async Task<ActionResult<CorrectionResponse>> Correct(Guid submissionId, CancellationToken cancellationToken)
    {
        try
        {
            return await correctionService.CorrectAsync(submissionId, cancellationToken) is { } correction
                ? Ok(correction)
                : NotFound();
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost("submissions/{submissionId:guid}/questions/{questionId:guid}/correct")]
    public async Task<ActionResult<CorrectionResponse>> CorrectQuestion(
        Guid submissionId,
        Guid questionId,
        CancellationToken cancellationToken)
    {
        try
        {
            return await correctionService.CorrectQuestionAsync(submissionId, questionId, cancellationToken) is { } correction
                ? Ok(correction)
                : NotFound();
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}
