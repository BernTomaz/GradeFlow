using GradeFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GradeFlow.Infrastructure.Data;

public sealed class GradeFlowDbContext(DbContextOptions<GradeFlowDbContext> options) : DbContext(options)
{
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerKey> AnswerKeys => Set<AnswerKey>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<StudentAnswer> StudentAnswers => Set<StudentAnswer>();
    public DbSet<CorrectionResult> CorrectionResults => Set<CorrectionResult>();
    public DbSet<CorrectionLog> CorrectionLogs => Set<CorrectionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("Assignments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.Property(x => x.Subject).HasMaxLength(200);
            entity.Property(x => x.TotalPoints).HasPrecision(18, 2);
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Questions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Text).IsRequired().HasMaxLength(4000);
            entity.Property(x => x.Type).IsRequired();
            entity.Property(x => x.Points).HasPrecision(18, 2);
            entity.Property(x => x.Order).IsRequired();
            entity.Property(x => x.CorrectionConfigJson).HasMaxLength(4000);
            entity.HasOne(x => x.Assignment)
                .WithMany(x => x.Questions)
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.AnswerKey)
                .WithOne(x => x.Question)
                .HasForeignKey<AnswerKey>(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AnswerKey>(entity =>
        {
            entity.ToTable("AnswerKeys");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CorrectAnswer).IsRequired().HasMaxLength(2000);
            entity.Property(x => x.AcceptedAnswersJson).HasMaxLength(4000);
            entity.Property(x => x.KeywordsJson).HasMaxLength(4000);
            entity.Property(x => x.Tolerance).HasPrecision(18, 4);
            entity.Property(x => x.FeedbackCorrect).HasMaxLength(2000);
            entity.Property(x => x.FeedbackIncorrect).HasMaxLength(2000);
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.ToTable("Submissions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.StudentName).IsRequired().HasMaxLength(200);
            entity.Property(x => x.StudentEmail).HasMaxLength(320);
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.FinalScore).HasPrecision(18, 2);
            entity.Property(x => x.SubmittedAt).IsRequired();
            entity.Property(x => x.CorrectedAt);
            entity.Property(x => x.ReviewedAt);
            entity.HasOne(x => x.Assignment)
                .WithMany(x => x.Submissions)
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StudentAnswer>(entity =>
        {
            entity.ToTable("StudentAnswers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Answer).IsRequired().HasMaxLength(4000);
            entity.Property(x => x.ScoreAwarded).HasPrecision(18, 2);
            entity.Property(x => x.IsCorrect).IsRequired();
            entity.Property(x => x.Feedback).HasMaxLength(2000);
            entity.Property(x => x.NeedsReview).IsRequired();
            entity.HasOne(x => x.Submission)
                .WithMany(x => x.StudentAnswers)
                .HasForeignKey(x => x.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Question)
                .WithMany(x => x.StudentAnswers)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.CorrectionResult)
                .WithOne(x => x.StudentAnswer)
                .HasForeignKey<CorrectionResult>(x => x.StudentAnswerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CorrectionResult>(entity =>
        {
            entity.ToTable("CorrectionResults");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ScoreAwarded).HasPrecision(18, 2);
            entity.Property(x => x.Feedback).HasMaxLength(2000);
            entity.Property(x => x.CorrectionType).HasMaxLength(100);
            entity.Property(x => x.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<CorrectionLog>(entity =>
        {
            entity.ToTable("CorrectionLogs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CorrectionType).IsRequired().HasMaxLength(100);
            entity.Property(x => x.OriginalAnswer).IsRequired().HasMaxLength(4000);
            entity.Property(x => x.NormalizedAnswer).HasMaxLength(4000);
            entity.Property(x => x.ExpectedAnswer).HasMaxLength(2000);
            entity.Property(x => x.Score).HasPrecision(18, 2);
            entity.Property(x => x.Message).HasMaxLength(2000);
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.ReviewedByUserId).HasMaxLength(200);
        });
    }
}
