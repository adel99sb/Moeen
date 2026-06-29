using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moeen.Api.infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyLessonStudentAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaturdayLessonStudents",
                columns: table => new
                {
                    SaturdayLessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaturdayLessonStudents", x => new { x.SaturdayLessonId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_SaturdayLessonStudents_SaturdayLesson_SaturdayLessonId",
                        column: x => x.SaturdayLessonId,
                        principalTable: "SaturdayLesson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaturdayLessonStudents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaturdayLessonStudents_StudentId",
                table: "SaturdayLessonStudents",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaturdayLessonStudents");
        }
    }
}
