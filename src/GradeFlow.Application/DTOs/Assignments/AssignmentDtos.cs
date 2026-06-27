using System.ComponentModel.DataAnnotations;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Application.DTOs.Assignments;

public sealed record CreateAssignmentRequest(
    [Required]
    [MaxLength(200)]
    string Title,
    [MaxLength(2000)]
    string? Description,
    [MaxLength(200)]
    string? Subject);

public sealed record UpdateAssignmentRequest(
    [Required]
    [MaxLength(200)]
    string Title,
    [MaxLength(2000)]
    string? Description,
    [MaxLength(200)]
    string? Subject);

public sealed record AssignmentResponse(
    Guid Id,
    string Title,
    string? Description,
    string? Subject,
    decimal TotalPoints,
    AssignmentStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
