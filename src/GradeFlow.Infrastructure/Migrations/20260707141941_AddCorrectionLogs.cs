using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradeFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCorrectionLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorrectionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CorrectionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OriginalAnswer = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    NormalizedAnswer = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExpectedAnswer = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByUserId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrectionLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorrectionLogs");
        }
    }
}
