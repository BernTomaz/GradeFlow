using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradeFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TeacherUserId",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentUserId",
                table: "Submissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TeacherUserId",
                table: "Assignments",
                column: "TeacherUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_StudentUserId",
                table: "Submissions",
                column: "StudentUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_TeacherUserId",
                table: "Assignments",
                column: "TeacherUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_StudentUserId",
                table: "Submissions",
                column: "StudentUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_TeacherUserId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_StudentUserId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_TeacherUserId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_StudentUserId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "TeacherUserId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "StudentUserId",
                table: "Submissions");
        }
    }
}
