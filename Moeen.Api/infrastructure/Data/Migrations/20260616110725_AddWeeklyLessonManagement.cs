using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moeen.Api.infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyLessonManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HalqaId",
                table: "SaturdayLesson",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "SaturdayLesson",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WeeklyLessonId",
                table: "SaturdayLesson",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WeeklyLessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyLessons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaturdayLesson_HalqaId",
                table: "SaturdayLesson",
                column: "HalqaId");

            migrationBuilder.CreateIndex(
                name: "IX_SaturdayLesson_TeacherId",
                table: "SaturdayLesson",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_SaturdayLesson_WeeklyLessonId",
                table: "SaturdayLesson",
                column: "WeeklyLessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaturdayLesson_Halqas_HalqaId",
                table: "SaturdayLesson",
                column: "HalqaId",
                principalTable: "Halqas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaturdayLesson_Teachers_TeacherId",
                table: "SaturdayLesson",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaturdayLesson_WeeklyLessons_WeeklyLessonId",
                table: "SaturdayLesson",
                column: "WeeklyLessonId",
                principalTable: "WeeklyLessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaturdayLesson_Halqas_HalqaId",
                table: "SaturdayLesson");

            migrationBuilder.DropForeignKey(
                name: "FK_SaturdayLesson_Teachers_TeacherId",
                table: "SaturdayLesson");

            migrationBuilder.DropForeignKey(
                name: "FK_SaturdayLesson_WeeklyLessons_WeeklyLessonId",
                table: "SaturdayLesson");

            migrationBuilder.DropTable(
                name: "WeeklyLessons");

            migrationBuilder.DropIndex(
                name: "IX_SaturdayLesson_HalqaId",
                table: "SaturdayLesson");

            migrationBuilder.DropIndex(
                name: "IX_SaturdayLesson_TeacherId",
                table: "SaturdayLesson");

            migrationBuilder.DropIndex(
                name: "IX_SaturdayLesson_WeeklyLessonId",
                table: "SaturdayLesson");

            migrationBuilder.DropColumn(
                name: "HalqaId",
                table: "SaturdayLesson");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "SaturdayLesson");

            migrationBuilder.DropColumn(
                name: "WeeklyLessonId",
                table: "SaturdayLesson");
        }
    }
}
