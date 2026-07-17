using GradeFlow.Application.DTOs.Submissions;

namespace GradeFlow.Application.Services;

public interface ISubmissionService
{
    Task<IReadOnlyCollection<SubmissionResponse>?> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<SubmissionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CorrectionLogResponse>?> GetCorrectionLogsAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<AssignmentReportResponse?> GetReportAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<string?> ExportCsvAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<byte[]?> ExportExcelAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<byte[]?> ExportPdfAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<ImportSubmissionsResponse?> ImportCsvAsync(Guid assignmentId, string csv, CancellationToken cancellationToken = default);
    Task<SubmissionResponse?> CreateAsync(Guid assignmentId, CreateSubmissionRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, CreateSubmissionRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateStudentInfoAsync(Guid id, UpdateStudentInfoRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAnswerAsync(Guid submissionId, Guid questionId, UpdateStudentAnswerRequest request, CancellationToken cancellationToken = default);
    Task<ReviewStudentAnswerResponse?> ReviewAnswerAsync(Guid answerId, ReviewStudentAnswerRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
