using GradeFlow.Application.DTOs.Assignments;
using GradeFlow.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GradeFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/assignments")]
public sealed class AssignmentsController(IAssignmentService assignmentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AssignmentResponse>>> GetAll(CancellationToken cancellationToken)
        => Ok(await assignmentService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssignmentResponse>> GetById(Guid id, CancellationToken cancellationToken)
        => await assignmentService.GetByIdAsync(id, cancellationToken) is { } assignment ? Ok(assignment) : NotFound();

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<AssignmentResponse>> Create(CreateAssignmentRequest request, CancellationToken cancellationToken)
    {
        var created = await assignmentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Update(Guid id, UpdateAssignmentRequest request, CancellationToken cancellationToken)
        => await assignmentService.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await assignmentService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
