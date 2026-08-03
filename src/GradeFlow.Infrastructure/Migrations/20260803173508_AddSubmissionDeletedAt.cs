using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradeFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionDeletedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Submissions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Submissions");
        }
    }
}
