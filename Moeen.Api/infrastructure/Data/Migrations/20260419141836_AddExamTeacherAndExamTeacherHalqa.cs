using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moeen.Api.infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExamTeacherAndExamTeacherHalqa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TeacherExamId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TeacherExams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MosquId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MosqueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherExams_Mosques_MosqueId",
                        column: x => x.MosqueId,
                        principalTable: "Mosques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherExams_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamTeacherHalqa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamTeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HalqaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherExamsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoujId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTeacherHalqa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamTeacherHalqa_Foujs_FoujId",
                        column: x => x.FoujId,
                        principalTable: "Foujs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamTeacherHalqa_Halqas_HalqaId",
                        column: x => x.HalqaId,
                        principalTable: "Halqas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamTeacherHalqa_TeacherExams_TeacherExamsId",
                        column: x => x.TeacherExamsId,
                        principalTable: "TeacherExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exams_TeacherExamId",
                table: "Exams",
                column: "TeacherExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTeacherHalqa_FoujId",
                table: "ExamTeacherHalqa",
                column: "FoujId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTeacherHalqa_HalqaId",
                table: "ExamTeacherHalqa",
                column: "HalqaId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTeacherHalqa_TeacherExamsId",
                table: "ExamTeacherHalqa",
                column: "TeacherExamsId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherExams_MosqueId",
                table: "TeacherExams",
                column: "MosqueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_TeacherExams_TeacherExamId",
                table: "Exams",
                column: "TeacherExamId",
                principalTable: "TeacherExams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_TeacherExams_TeacherExamId",
                table: "Exams");

            migrationBuilder.DropTable(
                name: "ExamTeacherHalqa");

            migrationBuilder.DropTable(
                name: "TeacherExams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_TeacherExamId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "TeacherExamId",
                table: "Exams");
        }
    }
}
