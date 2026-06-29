using System.ComponentModel.DataAnnotations;
using GradeFlow.Application.DTOs.Submissions;
using GradeFlow.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GradeFlow.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class SubmissionsController(ISubmissionService submissionService) : ControllerBase
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
}
