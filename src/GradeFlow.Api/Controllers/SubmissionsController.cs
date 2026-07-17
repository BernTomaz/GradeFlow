using System.ComponentModel.DataAnnotations;
using System.Text;
using GradeFlow.Application.DTOs.Submissions;
using GradeFlow.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GradeFlow.Api.Controllers;

[ApiController]
[Authorize]
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

    [HttpGet("submissions/{id:guid}/correction-logs")]
    public async Task<ActionResult<IReadOnlyCollection<CorrectionLogResponse>>> GetCorrectionLogs(
        Guid id,
        CancellationToken cancellationToken)
        => await submissionService.GetCorrectionLogsAsync(id, cancellationToken) is { } logs ? Ok(logs) : NotFound();

    [HttpGet("assignments/{assignmentId:guid}/report")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<AssignmentReportResponse>> GetReport(
        Guid assignmentId,
        CancellationToken cancellationToken)
        => await submissionService.GetReportAsync(assignmentId, cancellationToken) is { } report ? Ok(report) : NotFound();

    [HttpGet("assignments/{assignmentId:guid}/export/csv")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> ExportCsv(Guid assignmentId, CancellationToken cancellationToken)
        => await submissionService.ExportCsvAsync(assignmentId, cancellationToken) is { } csv
            ? File(Encoding.UTF8.GetBytes(csv), "text/csv", $"gradeflow-{assignmentId}-notas.csv")
            : NotFound();

    [HttpGet("assignments/{assignmentId:guid}/export/excel")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> ExportExcel(Guid assignmentId, CancellationToken cancellationToken)
        => await submissionService.ExportExcelAsync(assignmentId, cancellationToken) is { } file
            ? File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"gradeflow-{assignmentId}-notas.xlsx")
            : NotFound();

    [HttpGet("assignments/{assignmentId:guid}/export/pdf")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> ExportPdf(Guid assignmentId, CancellationToken cancellationToken)
        => await submissionService.ExportPdfAsync(assignmentId, cancellationToken) is { } file
            ? File(file, "application/pdf", $"gradeflow-{assignmentId}-relatorio.pdf")
            : NotFound();

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

    [HttpPost("assignments/{assignmentId:guid}/submissions/import")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<ImportSubmissionsResponse>> Import(
        Guid assignmentId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var csv = await reader.ReadToEndAsync(cancellationToken);
            return await submissionService.ImportCsvAsync(assignmentId, csv, cancellationToken) is { } result
                ? Ok(result)
                : NotFound();
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("submissions/{id:guid}")]
    [Authorize(Roles = "Admin,Teacher")]
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
    [Authorize(Roles = "Admin,Teacher,Student")]
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

    [HttpPut("student-answers/{answerId:guid}/review")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<ReviewStudentAnswerResponse>> ReviewAnswer(
        Guid answerId,
        ReviewStudentAnswerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await submissionService.ReviewAnswerAsync(answerId, request, cancellationToken) is { } review
                ? Ok(review)
                : NotFound();
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("submissions/{id:guid}/student")]
    [Authorize(Roles = "Admin,Teacher,Student")]
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
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await submissionService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("submissions/{submissionId:guid}/correct")]
    [Authorize(Roles = "Admin,Teacher")]
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
    [Authorize(Roles = "Admin,Teacher")]
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
